using System;
using System.Collections.Generic;
using BattleTech;
using DynamicShops;
using Harmony;

namespace DinamicShops.Patches
{
    [HarmonyPatch(typeof(StarSystem), "OnSystemChange")]
    public static class StarSystem_OnSystemChange
    {
        [HarmonyPrefix]
        public static void ReplaceShops(StarSystem __instance)
        {
            DoSystemShop(__instance, __instance.SystemShop);
        }

        private static void DoSystemShop(StarSystem starSystem, Shop systemShop)
        {


#if CCDEBUG
            Control.Logger.LogDebug($"SystemShop for {starSystem.Name} ({starSystem.Def.Description.Id})");
            ShowItemCollections("Before", systemShop.ItemCollections);
#endif

            //            if (systemShop.IsEmpty)
            //            {
            //#if CCDEBUG
            //                Control.Logger.LogDebug("---- Empty");
            //                Control.Logger.LogDebug("======================================================");
            //#endif
            //                return;
            //            }

            systemShop.ItemCollections.Clear();
            if (Control.Settings.ClearDefaultShop)
                if(starSystem.Def.SystemShopItems != null && starSystem.Def.SystemShopItems.Count > 0)
                    foreach (var item in starSystem.Def.SystemShopItems)
                        GetItemCollection(systemShop, item);

            foreach (var defTag in starSystem.Def.Tags)
            {
                if (Control.Settings.TagInfos.TryGetValue(defTag, out var items))
                    foreach (var item in items)
                        GetItemCollection(systemShop, item);
            }

#if CCDEBUG
            ShowItemCollections("After", systemShop.ItemCollections);
            Control.Logger.LogDebug("======================================================");
#endif
        }

        private static void GetItemCollection(Shop systemShop, string item)
        {
            try
            {
                var collection = UnityGameInstance.BattleTechGame.DataManager.ItemCollectionDefs.Get(item);
                if (collection == null)
                {
                    Control.Logger.LogError("Cannot Load ItemCollection " + item);
                }
                else
                    systemShop.ItemCollections.Add(collection);
            }
            catch (Exception e)
            {
                Control.Logger.LogError("Cannot Load ItemCollection " + item);
                Control.Logger.LogError(e);
                throw;
            }
        }

#if CCDEBUG
        private static void ShowItemCollections(string message, List<ItemCollectionDef> itemCollections)
        {
            Control.Logger.LogDebug("-- " + message);
            if (itemCollections == null || itemCollections.Count == 0)
            {
                Control.Logger.LogDebug("---- Empty");
                return;
            }

            foreach (var collection in itemCollections)
            {
                Control.Logger.LogDebug($"---- {collection.ID}");
            }
        }
#endif
    }
}