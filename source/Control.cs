//#undef CCDEBUG

using Harmony;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using BattleTech.UI;
using HBS.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using BattleTech;
using System.Linq;
using DynamicShops.Shops;

namespace DynamicShops
{
    public static class Control
    {
        public static DynamicShopSettings Settings = new DynamicShopSettings();
        private const string ModName = "DynamicShops";
        private const string LogPrefix = "[DShops]";

        internal static Dictionary<string, List<DCustomShopDef>> CustomShopDefs;
        internal static List<DShopDef> ShopDefs { get; private set; }
        internal static List<DFactionShopDef> FactionShopDefs { get; private set; }
        internal static List<DShopDef> BlackMarketShopDefs { get; private set; }

        internal static ILog Logger;
        private static FileLogAppender logAppender;
        public static void Init(string directory, string settingsJSON)
        {
            Logger = HBS.Logging.Logger.GetLogger(ModName, LogLevel.Debug);
            try
            {
                try
                {
                    Settings = JsonConvert.DeserializeObject<DynamicShopSettings>(settingsJSON);
                    HBS.Logging.Logger.SetLoggerLevel(Logger.Name, Settings.LogLevel);
                }
                catch (Exception)
                {
                    Settings = new DynamicShopSettings();
                }



                SetupLogging(directory);
#if CCDEBUG
                var str = JsonConvert.SerializeObject(Settings, Formatting.Indented);
                Logger.LogDebug(str);

#endif  
                var harmony = HarmonyInstance.Create("io.github.denadan.DynamicShops");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                RegisterConditions(Assembly.GetExecutingAssembly());

                if (Settings.ReplaceSystemShop)
                    CustomShops.Control.RegisterShop(new Shops.DSystemShop(), new List<string>() { "systemchange", "monthend" });
                if (Settings.ReplaceFactionShop)
                    CustomShops.Control.RegisterShop(new Shops.DFactionShop(), new List<string>() { "systemchange", "monthend" });
                if (Settings.ReplaceBlackMarket)
                    CustomShops.Control.RegisterShop(new Shops.DBlackMarket(), new List<string>() { "systemchange", "monthend" });

                ShopDefs = new List<DShopDef>();
                FactionShopDefs = new List<DFactionShopDef>();
                BlackMarketShopDefs = new List<DShopDef>();

                Logger.Log("Loaded DynamicShops v0.6 for bt 1.9.1");
#if CCDEBUG
                Logger.LogDebug("done");
#endif
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }

        public static void RegisterConditions(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                var attr = type.GetCustomAttribute<DConditionAttribute>();
                if (attr == null)
                    continue;

                LogDebug(DInfo.Main, $"Found condition {type} with name {attr.Name}");

                if (typeof(DCondition).IsAssignableFrom(type))
                    ConditionBuilder.Register(type, attr);
                else
                    LogError($"{type} not DCondition, skipped");
            }
        }

        public static void FinishedLoading(Dictionary<string, Dictionary<string, VersionManifestEntry>> customResources)
        {
            Log("Finish Loading");
            Dictionary<string, VersionManifestEntry> manifest = null;
            if (customResources.TryGetValue("DShopDef", out manifest))
            {
                LogDebug(DInfo.Loading, "- Loading DShopDef");
                LoadShopDefs(manifest, ShopDefs);
            }
            if (customResources.TryGetValue("DFactionShopDef", out manifest))
            {
                LogDebug(DInfo.Loading, "- Loading DFactionShopDef");
                LoadShopDefs(manifest, FactionShopDefs);
            }
            if (customResources.TryGetValue("DBMShopDef", out manifest))
            {
                LogDebug(DInfo.Loading, "- Loading DBMShopDef");
                LoadShopDefs(manifest, BlackMarketShopDefs);
            }
            if (customResources.TryGetValue("DCustomShopDef", out manifest))
            {
                LogDebug(DInfo.Loading, "- Loading DBMShopDef");
                List<DCustomShopDef> custom_shop_defs = new();
                LoadShopDefs(manifest, custom_shop_defs);
                if(custom_shop_defs != null)
                    CustomShopDefs = custom_shop_defs
                        .GroupBy(i => i.ShopName)
                        .ToDictionary(i => i.Key,
                            i=>i.ToList());
                else
                    CustomShopDefs = new Dictionary<string, List<DCustomShopDef>>();
            }
            if (customResources.TryGetValue("DCustomShopDescriptor", out manifest))
            {
                LogDebug(DInfo.Loading, "- Loading DBMShopDef");
                LoadCustomShops(manifest);

            }

            Log("Loaded");
            Log("- System shops: " + ShopDefs.Count.ToString());
            Log("- Faction shops: " + FactionShopDefs.Count.ToString());
            Log("- Black market shops: " + BlackMarketShopDefs.Count.ToString());
        }

        private static void LoadCustomShops(Dictionary<string, VersionManifestEntry> manifest)
        {
            foreach (var item in manifest.Values)
            {
                LogDebug(DInfo.Loading, "-- Loading " + item.FilePath);
                string json = "";

                using (var reader = new StreamReader(item.FilePath))
                {
                    json = reader.ReadToEnd();
                }
                try
                {
                    var obj = fastJSON.JSON.ToObject(json, true);
                    if (obj is Dictionary<string, object> dict)
                    {
                        DCustomShopDescriptor descriptor = DCustomShopDescriptor.FromJson(obj);
                        if (descriptor != null)
                        {
                            DCustomShop shop = null;

                            switch (descriptor.Type)
                            {
                                case ShopSubType.FactionBased:
                                    break;
                                case ShopSubType.Custom:
                                    break;
                            }

                            if(shop == null)
                                continue;

                            CustomShops.Control.RegisterShop(shop, descriptor.RefreshEvents);
                        }
                    }
                    else
                    {
                        LogError($"Missformed json {item.FilePath}");
                    }
                }
                catch (Exception e)
                {
                    LogError($"Error reading {item.FilePath}", e);
                }
            }
        }

        private static void LoadShopDefs<ShopType>(Dictionary<string, VersionManifestEntry> manifest, List<ShopType> shopDefs)
            where ShopType : DShopDef, new()
        {
            void load_shop(Object obj)
            {
                var dict = obj as Dictionary<string, object>;
                if (dict == null)
                {
                    LogDebug(DInfo.Loading, "---- cannot get dictionary - skipped");
                    return;
                }
                ShopType shop = new ShopType();
                if (shop.FromJson(dict))
                {
                    shopDefs.Add(shop);
                }
                else
                    LogDebug(DInfo.Loading, "---- bad shopdef - skipped");

            }

            foreach (var item in manifest.Values)
            {
                LogDebug(DInfo.Loading, "-- Loading " + item.FilePath);
                string json = "";

                using (var reader = new StreamReader(item.FilePath))
                {
                    json = reader.ReadToEnd();
                }
                try
                {
                    var obj = fastJSON.JSON.ToObject(json, true);
                    if (obj is Dictionary<string, object> dict)
                    {
                        LogDebug(DInfo.Loading, "--- single item");
                        load_shop(obj);
                    }
                    else if (obj is IEnumerable<object> list)
                    {
                        LogDebug(DInfo.Loading, $"--- list of {list.Count()} items");
                        foreach (var sub in list)
                            load_shop(sub);
                    }
                    else
                    {
                        LogError($"Missformed json {item.FilePath}");
                    }
                }
                catch (Exception e)
                {
                    LogError($"Error reading {item.FilePath}", e);
                }
            }
        }


        #region LOGGING
        [Conditional("CCDEBUG")]
        public static void LogDebug(DInfo level, string message)
        {
            if(Settings.DebugInfo.HasFlag(level))
            Logger.LogDebug(LogPrefix + message);
        }
        [Conditional("CCDEBUG")]
        public static void LogDebug(DInfo level, string message, Exception e)
        {
            if (Settings.DebugInfo.HasFlag(level))
                Logger.LogDebug(LogPrefix + message, e);
        }

        public static void LogError(string message)
        {
            Logger.LogError(LogPrefix + message);
        }
        public static void LogError(string message, Exception e)
        {
            Logger.LogError(LogPrefix + message, e);
        }
        public static void LogError(Exception e)
        {
            Logger.LogError(LogPrefix, e);
        }

        public static void Log(string message)
        {
            Logger.Log(LogPrefix + message);
        }



        internal static void SetupLogging(string Directory)
        {
            var logFilePath = Path.Combine(Directory, "log.txt");

            try
            {
                ShutdownLogging();
                AddLogFileForLogger(logFilePath);
            }
            catch (Exception e)
            {
                Logger.Log($"{ModName}: can't create log file", e);
            }
        }

        internal static void ShutdownLogging()
        {
            if (logAppender == null)
            {
                return;
            }

            try
            {
                HBS.Logging.Logger.ClearAppender(ModName);
                logAppender.Flush();
                logAppender.Close();
            }
            catch
            {
            }

            logAppender = null;
        }

        private static void AddLogFileForLogger(string logFilePath)
        {
            try
            {
                logAppender = new FileLogAppender(logFilePath, FileLogAppender.WriteMode.INSTANT);
                HBS.Logging.Logger.AddAppender(ModName, logAppender);

            }
            catch (Exception e)
            {
                Logger.Log($"{ModName}: can't create log file", e);
            }
        }

        #endregion
    }
}
