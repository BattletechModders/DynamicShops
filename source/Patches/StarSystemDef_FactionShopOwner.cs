using System;
using System.Collections.Generic;
using System.Text;
using Harmony;
using BattleTech;

namespace DynamicShops.Patches
{
    [HarmonyPatch(typeof(StarSystemDef))]
    [HarmonyPatch("FactionShopOwner", PropertyMethod.Getter)]
    public static class StarSystemDef_FactionShopOwner
    {
        [HarmonyPrefix]
        public static bool GetOwner(ref Faction __result, StarSystemDef __instance)
        {
            if (!Control.Settings.OverrideFactionShopOwner)
                return true;

            if (UnityGameInstance.BattleTechGame.Simulation == null || UnityGameInstance.BattleTechGame.Simulation.CurSystem == null)
                return true;

            if (Control.Settings.FactionShopOnEveryPlanet || __instance.FactionShopOwner != Faction.INVALID_UNSET)
                __result = UnityGameInstance.BattleTechGame.Simulation.CurSystem.Owner;
            else
                return true;

            return false;
        }
    }
}
