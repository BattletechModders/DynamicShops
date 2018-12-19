using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace DynamicShops.Patches
{
//    [HarmonyPatch(typeof(Shop), "Initialize")]
    public static class Shop_Initialize
    {
        [HarmonyPrefix]
        public static void ReplaceCollections(ref List<string> collections, Shop.ShopType shopType, Shop __instance)
        {
            var system = new Traverse(__instance).Field<StarSystem>("system").Value;

#if CCDEBUG
            Control.Logger.LogDebug($"{shopType} Shop for {system.Name}({system.Def.Description.Id})");
            Control.Logger.LogDebug("-- Before:");

            if (collections == null || collections.Count == 0)
            {
                Control.Logger.LogDebug("---- Empty");
            }
            else
            {
                foreach (var item in collections)
                {
                    Control.Logger.LogDebug("---- " + item);
                }
            }

#endif

            switch (shopType)
            {
                case Shop.ShopType.System:
                    DoSystemShop(ref collections, system);
                    break;

            }

#if CCDEBUG
            Control.Logger.LogDebug("-- After:");
            if (collections == null || collections.Count == 0)
            {
                Control.Logger.LogDebug("---- Empty");
            }
            else
            {
                foreach (var item in collections)
                {
                    Control.Logger.LogDebug("---- " + item);
                }
            }

            Control.Logger.LogDebug("======================================================");
#endif
        }

        private static void DoSystemShop(ref List<string> collections, StarSystem system)
        {
            if(collections == null)
                collections = new List<string>();

            if(Control.Settings.ClearDefaultShop)
                collections.Clear();

            foreach (var defTag in system.Def.Tags)
            {
                if (Control.Settings.TagInfos.TryGetValue(defTag, out var items))
                {
                    collections.AddRange(items);
                }
            }
        }
    }
}