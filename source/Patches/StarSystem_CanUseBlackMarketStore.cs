using System.Runtime.CompilerServices;
using BattleTech;
using Harmony;

namespace DynamicShops.Patches
{
    [HarmonyPatch(typeof(StarSystem), "CanUseBlackMarketStore")]
    public static class StarSystem_CanUseBlackMarketStore
    {
        [HarmonyPrefix]
        public static bool Override_Access(StarSystem __instance, ref bool __result)
        {
            if (Control.Settings.DEBUG_AllowBlackMarket)
            {
                __result = __instance.HasBlackMarketStore();
                return false;
            }
            return true;
        }
    }
}