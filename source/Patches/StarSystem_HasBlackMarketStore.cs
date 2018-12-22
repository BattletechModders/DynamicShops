using System.Runtime.CompilerServices;
using BattleTech;
using Harmony;

namespace DynamicShops.Patches
{
    [HarmonyPatch(typeof(StarSystem),"HasBlackMarketStore")]
    public static class StarSystem_HasBlackMarketStore
    {
        [HarmonyPrefix]
        public static bool HasBlackMarketStore(StarSystem __instance, ref bool __result)
        {
            __result = __instance.Tags.Contains(Control.Settings.BlackMarketSystemTag);

            return false;
        }
    }
}