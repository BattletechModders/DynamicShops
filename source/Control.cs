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

namespace DynamicShops
{
    public static class Control
    {
        public static DynamicShopSettings Settings = new DynamicShopSettings();
        private const string ModName = "DynamicShops";
        private const string LogPrefix = "[DShops]";

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
                    CustomShops.Control.RegisterShop(new Shops.DSystemShop());
                if (Settings.ReplaceFactionShop)
                    CustomShops.Control.RegisterShop(new Shops.DFactionShop());
                if (Settings.ReplaceBlackMarket)
                    CustomShops.Control.RegisterShop(new Shops.DBlackMarket());

                ShopDefs = new List<DShopDef>();
                FactionShopDefs = new List<DFactionShopDef>();
                BlackMarketShopDefs = new List<DShopDef>();

                Logger.Log("Loaded DinamicShops v0.5 for bt 1.9.1");
#if CCDEBUG
                Logger.LogDebug("done");
#endif
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }
        private static void RegisterConditions(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                var attr = type.GetCustomAttribute<DConditionAttribute>();
                if (attr == null)
                    continue;

                Control.LogDebug($"Found condition {type} with name {attr.Name}");

                if (typeof(DCondition).IsAssignableFrom(type))
                    ConditionBuilder.Register(type, attr);
                else
                    Control.LogError($"{type} not DCondition, skipped");
            }
        }

        public static void FinishedLoading(Dictionary<string, Dictionary<string, VersionManifestEntry>> customResources)
        {
            Dictionary<string, VersionManifestEntry> manifest = null;
            if (customResources.TryGetValue("DShopDef", out manifest))
                LoadShopDefs(manifest, ShopDefs);
            if (customResources.TryGetValue("DFactionShopDef", out manifest))
                LoadShopDefs(manifest, FactionShopDefs);
            if (customResources.TryGetValue("DBMShopDef", out manifest))
                LoadShopDefs(manifest, BlackMarketShopDefs);
        }

        private static void LoadShopDefs<ShopType>(Dictionary<string, VersionManifestEntry> manifest, List<ShopType> shopDefs)
            where ShopType : DShopDef, new()
        {
            void load_shop(Object obj)
            {
                var dict = obj as Dictionary<string, object>;
                if (dict == null)
                    return;

                ShopType shop = new ShopType();
                if (shop.FromJson(dict))
                    shopDefs.Add(shop);
            }

            foreach (var item in manifest.Values)
            {
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
                        load_shop(obj);
                    }
                    else if (obj is IEnumerable<object> list)
                    {
                        foreach (var sub in list)
                            load_shop(obj);
                    }
                    else
                    {
                        Control.LogError($"Missformed json {item.FilePath}");
                    }
                }
                catch (Exception e)
                {
                    Control.LogError($"Error reading {item.FilePath}", e);
                }
            }
        }


        #region LOGGING
        [Conditional("CCDEBUG")]
        public static void LogDebug(string message)
        {
            Logger.LogDebug(LogPrefix + message);
        }
        [Conditional("CCDEBUG")]
        public static void LogDebug(string message, Exception e)
        {
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
