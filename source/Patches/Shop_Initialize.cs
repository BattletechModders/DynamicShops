using System.Collections.Generic;
using BattleTech;
using Harmony;
using JetBrains.Annotations;

namespace DynamicShops.Patches
{
    [HarmonyPatch(typeof(Shop), "Initialize")]
    public static class Shop_Initialize
    {
        public static List<string> dummy = new List<string>()
        {
            "ItemCollection_DUMMY"
        };

        [HarmonyPrefix]
        public static void SetType(ref List<string> collections)
        {
            if (collections == null || collections.Count == 0)
                collections = dummy;
        }
    }
}