using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Harmony;
using BattleTech;
using BattleTech.UI;

namespace DynamicShops.Patches
{
//    [HarmonyPatch(typeof(StarSystemDef))]
//    [HarmonyPatch("FactionShopOwner")]
//    [HarmonyPatch(MethodType.Getter)]
//    public static class StarSystemDef_FactionShopOwner
//    {
//        [HarmonyPostfix]
//        public static void GetOwner(ref Faction __result, StarSystemDef __instance)
//        {
//            try
//            {
//#if CCDEBUG
//                Control.Logger.LogDebug("StarSystemDef_FactionShopOwner");
//#endif
//                if (!Control.Settings.OverrideFactionShopOwner)
//                {
//#if CCDEBUG
//                    Control.Logger.LogDebug("** Dont override owner - return default");
//#endif
//                    return;
//                }

//                if (UnityGameInstance.BattleTechGame.Simulation == null ||
//                    UnityGameInstance.BattleTechGame.Simulation.CurSystem == null)
//                {
//#if CCDEBUG
//                    Control.Logger.LogDebug("** no simgame or starsystem");
//#endif
//                    return;
//                }

//                if (Control.Settings.FactionShopOnEveryPlanet || __result != Faction.INVALID_UNSET)
//                {
//#if CCDEBUG
//                    Control.Logger.LogDebug($"** Ovveriding {__result} to {UnityGameInstance.BattleTechGame.Simulation.CurSystem.Owner}");
//#endif
//                    __result = UnityGameInstance.BattleTechGame.Simulation.CurSystem.Owner;
//                }
//                else
//                {
//#if CCDEBUG
//                    Control.Logger.LogDebug("** no overriding needed");
//#endif
//                }

//            }
//            catch (Exception e)
//            {
//                Control.Logger.LogError(e);
//                return;
//            }
//        }
//    }

    public static class StarSystemExtention
    {
        public static Faction GetFactionShowOwner(this StarSystem starSystem)
        {
            try
            {

#if CCDEBUG
                Control.Logger.LogDebug("GetFactionShowOwner");
#endif
                if (!Control.Settings.OverrideFactionShopOwner)
                {
#if CCDEBUG
                    Control.Logger.LogDebug("** Dont override owner - return default");
#endif
                    return starSystem.Def.FactionShopOwner;
                }

                if (UnityGameInstance.BattleTechGame.Simulation == null ||
                    UnityGameInstance.BattleTechGame.Simulation.CurSystem == null)
                {
#if CCDEBUG
                    Control.Logger.LogDebug("** no simgame or starsystem");
#endif
                    return starSystem.Def.FactionShopOwner;
                }

                if (Control.Settings.FactionShopOnEveryPlanet ||
                    starSystem.Def.FactionShopOwner != Faction.INVALID_UNSET)
                {
#if CCDEBUG
                    Control.Logger.LogDebug(
                        $"** Ovveriding {starSystem.Def.FactionShopOwner} to {UnityGameInstance.BattleTechGame.Simulation.CurSystem.Owner}");
#endif
                    return UnityGameInstance.BattleTechGame.Simulation.CurSystem.Owner;
                }
                else
                {
#if CCDEBUG
                    Control.Logger.LogDebug("** no overriding needed");
#endif
                    return starSystem.Def.FactionShopOwner;
                }

            }
            catch (Exception e)
            {
                Control.Logger.LogError(e);
                return starSystem.Def.FactionShopOwner;
            }
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var method1 = typeof(StarSystem).GetProperty("Def").GetGetMethod();
            var method2 = typeof(StarSystemDef).GetProperty("FactionShopOwner").GetGetMethod();
            bool flag = false;
            CodeInstruction last = null;
            foreach (var instruction in instructions)
            {
                if (flag)
                {
                    if (instruction.operand is MethodInfo operand && operand == method2)
                    {
                        yield return new CodeInstruction(OpCodes.Call, ((Func<StarSystem, Faction>)StarSystemExtention.GetFactionShowOwner).Method);
                        //    yield return new CodeInstruction(OpCodes.Nop);
                    }
                    else
                    {
                        yield return last;
                        yield return instruction;
                    }

                    flag = false;
                }
                else
                {
                    if (instruction.operand is MethodInfo operand && operand == method1)
                    {
                        last = instruction;
                        flag = true;
                    }
                    else
                        yield return instruction;

                }
            }
        }
    }

    [HarmonyPatch(typeof(StarSystem), "CanUseFactionStore")]
    public static class StarSystem_CanUseFactionStore
    {
        public static bool Prefix(StarSystem __instance, ref bool __result)
        {
            if (__instance.HasFactionStore())
            {
                Faction factionShopOwner = __instance.GetFactionShowOwner();
                __result = __instance.Sim.IsFactionAlly(factionShopOwner, null);
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(SG_Shop_Screen), "RefreshStoreTypeButtons")]
    public static class SG_Shop_Screen_RefreshStoreTypeButtons
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return StarSystemExtention.Transpiler(instructions);
        }
    }

    [HarmonyPatch(typeof(SG_Shop_Screen), "FillInFactionData")]
    public static class SG_Shop_Screen_FillInFactionData
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return StarSystemExtention.Transpiler(instructions);
        }
    }

    [HarmonyPatch(typeof(SG_Shop_Screen), "RefreshColorAreas")]
    public static class SG_Shop_Screen_RefreshColorAreas
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return StarSystemExtention.Transpiler(instructions);
        }
    }

    [HarmonyPatch(typeof(SG_Shop_Screen), "UpdateHeaderArea")]
    public static class SG_Shop_Screen_UpdateHeaderArea
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return StarSystemExtention.Transpiler(instructions);
        }
    }
}
