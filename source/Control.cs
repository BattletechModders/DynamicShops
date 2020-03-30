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
        public static void FinishedLoading(Dictionary<string, Dictionary<string, VersionManifestEntry>> customResources)
        {
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
