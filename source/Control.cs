#undef CCDEBUG

using Harmony;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using BattleTech.UI;
using HBS.Logging;
using Newtonsoft.Json;


namespace DynamicShops
{
    public static class Control
    {
        public static DynamicShopSettings Settings = new DynamicShopSettings();

        internal static ILog Logger;
        private static FileLogAppender logAppender;
        public static void Init(string directory, string settingsJSON)
        {
            Logger = HBS.Logging.Logger.GetLogger("DynamicShops", LogLevel.Debug);
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

                Settings.Complete();


                SetupLogging(directory);
#if CCDEBUG
                var str = JsonConvert.SerializeObject(Settings, Formatting.Indented);
                Logger.LogDebug(str);

#endif  
                var harmony = HarmonyInstance.Create("io.github.denadan.DynamicShops");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                Logger.Log("Loaded DinamicShops v0.1.0.0 for bt 1.3.2");
#if CCDEBUG
                Logger.LogDebug("done");
#endif
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }

#region LOGGING

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
                Logger.Log("DynamicShops: can't create log file", e);
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
                HBS.Logging.Logger.ClearAppender("DynamicShops");
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

                HBS.Logging.Logger.AddAppender("DynamicShops", logAppender);

            }
            catch (Exception e)
            {
                Logger.Log("DynamicShops: can't create log file", e);
            }
        }

#endregion
    }
}
