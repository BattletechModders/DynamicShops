using System;
using System.Collections.Generic;
using BattleTech;
using DynamicShops;
using Harmony;

namespace DynamicShops.Patches
{
    [HarmonyPatch(typeof(StarSystem), "OnSystemChange")]
    public static class StarSystem_OnSystemChange
    {
#if CCDEBUG
        public static bool rep_changed = false;
#endif
        [HarmonyPrefix]
        public static void ReplaceShops(StarSystem __instance)
        {
            if (__instance.Def.Tags.Contains(Control.Settings.EmptyShopSystemTag))
                return;
#if CCDEBUG
#warning REMOVE THIS FOR RELEASE!
            if (!rep_changed)
            {
                rep_changed = true;
                Control.Logger.LogDebug("REPUTATION CHANGED!!! REMOVE THIS!!");

                __instance.Sim.AddReputation(Faction.Liao, 100, false);
                __instance.Sim.AddReputation(Faction.TaurianConcordat, 50, false);
            }
#endif

            DoSystemShop(__instance, __instance.SystemShop);
            DoFactionShop(__instance, __instance.FactionShop);
        }

        private static void DoFactionShop(StarSystem starSystem, Shop factionShop)
        {
#if CCDEBUG
            Control.Logger.LogDebug($"FactionShop for {starSystem.Name} ({starSystem.Def.Description.Id})");
            ShowItemCollections("Before", factionShop.ItemCollections);
#endif

            factionShop.ItemCollections.Clear();
            // add items from system def
            if (!Control.Settings.ClearDefaultFactionShop)
                if (starSystem.Def.FactionShopItems != null && starSystem.Def.FactionShopItems.Count > 0)
                    foreach (var item in starSystem.Def.FactionShopItems)
                        AddItemCollection(factionShop, item);

            var faction = starSystem.Def.FactionShopOwner;
            if (Control.Settings.FactionInfos.TryGetValue(faction, out var Info))
            {
                foreach(var item in Info.FactionShops.GetItemCollections(starSystem))
                    AddItemCollection(factionShop, item);
            }

#if CCDEBUG
            ShowItemCollections("After", factionShop.ItemCollections);
            Control.Logger.LogDebug("======================================================");
#endif
        }

        private static void DoSystemShop(StarSystem starSystem, Shop systemShop)
        {
#if CCDEBUG
            Control.Logger.LogDebug($"SystemShop for {starSystem.Name} ({starSystem.Def.Description.Id})");
            ShowItemCollections("Before", systemShop.ItemCollections);
#endif


            systemShop.ItemCollections.Clear();

            // add items from system def
            if (!Control.Settings.ClearDefaultSystemShop)
                if (starSystem.Def.SystemShopItems != null && starSystem.Def.SystemShopItems.Count > 0)
                    foreach (var item in starSystem.Def.SystemShopItems)
                        AddItemCollection(systemShop, item);

            // add items from tags
            foreach (var item in Control.Settings.SystemShops.GetItemCollections(starSystem))
                AddItemCollection(systemShop, item);

            // Add faction items
            var faction = starSystem.Owner;
            if (Control.Settings.FactionInfos.TryGetValue(faction, out var Info))
            {
                foreach (var item in Info.SystemShops.GetItemCollections(starSystem))
                    AddItemCollection(systemShop, item);

                var reputation = starSystem.Sim.GetReputation(faction);

                foreach (var repitem in Info.RepShops)
                    if (repitem.Reputation <= reputation)
                        foreach (var item in repitem.Items.GetItemCollections(starSystem))
                            AddItemCollection(systemShop, item);
            }
#if CCDEBUG
            ShowItemCollections("After", systemShop.ItemCollections);
            Control.Logger.LogDebug("======================================================");
#endif
        }

        private static void AddItemCollection(Shop systemShop, string item)
        {
            try
            {
                var collection = UnityGameInstance.BattleTechGame.DataManager.ItemCollectionDefs.Get(item);
                if (collection == null)
                {
                    Control.Logger.LogError("---- No ItemCollection " + item);
                }
                else
                    systemShop.ItemCollections.Add(collection);
            }
            catch
            {
                Control.Logger.LogError("---- Cannot Load ItemCollection " + item);
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