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
        public static bool rep_changed = false;

        [HarmonyPrefix]
        public static void ReplaceShops(StarSystem __instance)
        {
            try
            {
                if (__instance.Def.Tags.Contains(Control.Settings.EmptyShopSystemTag))
                {
#if CCDEBUG
                    Control.Logger.LogDebug($"{__instance.Name} is Empty. Skipping shop popultaion, Clearing shops");
#endif
                    if (__instance.SystemShop.ItemCollections != null && __instance.SystemShop.ItemCollections.Count > 0)
                        __instance.SystemShop.ItemCollections.Clear();
                    if (__instance.FactionShop.ItemCollections != null && __instance.FactionShop.ItemCollections.Count > 0)
                        __instance.FactionShop.ItemCollections.Clear();
                    if (__instance.BlackMarketShop.ItemCollections != null && __instance.BlackMarketShop.ItemCollections.Count > 0)
                        __instance.BlackMarketShop.ItemCollections.Clear();

                    return;
                }

                if (!rep_changed && Control.Settings.DEBUG_AddFactionRep)
                {
                    rep_changed = true;
                    Control.Logger.LogDebug("REPUTATION CHANGED!!!");

                    __instance.Sim.AddReputation(Faction.ClanJadeFalcon, 96, false);
                    __instance.Sim.AddReputation(Faction.TaurianConcordat, 50, false);
                }


                DoSystemShop(__instance, __instance.SystemShop);
                DoFactionShop(__instance, __instance.FactionShop);
                DoBlackMarket(__instance, __instance.BlackMarketShop);
            }
            catch (Exception e)
            {
                Control.Logger.LogError(e);
            }

        }

        private static void DoBlackMarket(StarSystem starSystem, Shop blackMarketShop)
        {
#if CCDEBUG
            Control.Logger.LogDebug($"BlackMarket for {starSystem.Name} ({starSystem.Def.Description.Id}), HasBlackMarket: {starSystem.Def.Tags.Contains(Control.Settings.BlackMarketSystemTag)}");
            ShowItemCollections("Before", blackMarketShop.ItemCollections);
#endif

            blackMarketShop.ItemCollections.Clear();
            // add items from system def
            if (!Control.Settings.ClearDefaultPirateShop)
                if (starSystem.Def.BlackMarketShopItems != null && starSystem.Def.BlackMarketShopItems.Count > 0)
                    foreach (var item in starSystem.Def.BlackMarketShopItems)
                        AddItemCollection(blackMarketShop, item);

            foreach (var itemCollection in Control.Settings.BlackMarket.GetItemCollections(starSystem))
            {
                AddItemCollection(blackMarketShop, itemCollection);
            }


#if CCDEBUG
            ShowItemCollections("After", blackMarketShop.ItemCollections);
            Control.Logger.LogDebug("======================================================");
#endif
        }

        private static void DoFactionShop(StarSystem starSystem, Shop factionShop)
        {
#if CCDEBUG
            Control.Logger.LogDebug($"FactionShop for {starSystem.Name} ({starSystem.Def.Description.Id}) owner:{starSystem.GetFactionShowOwner()}/{starSystem.Owner}");
            ShowItemCollections("Before", factionShop.ItemCollections);
#endif

            factionShop.ItemCollections.Clear();
            // add items from system def
            if (!Control.Settings.ClearDefaultFactionShop)
                if (starSystem.Def.FactionShopItems != null && starSystem.Def.FactionShopItems.Count > 0)
                    foreach (var item in starSystem.Def.FactionShopItems)
                        AddItemCollection(factionShop, item);

            var faction = starSystem.GetFactionShowOwner();
            if (Control.Settings.FactionInfos.TryGetValue(faction, out var Info))
            {
                foreach (var item in Info.FactionShops.GetItemCollections(starSystem))
                    AddItemCollection(factionShop, item);
            }

            foreach (var item in Control.Settings.GetGenereicFactionFactionShopItems(faction, starSystem))
            {
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
            var reputation = starSystem.Sim.GetReputation(faction);
            if (Control.Settings.FactionInfos.TryGetValue(faction, out var Info))
            {
                foreach (var item in Info.SystemShops.GetItemCollections(starSystem))
                    AddItemCollection(systemShop, item);


                foreach (var repitem in Info.RepShops)
                    if (repitem.Reputation <= reputation)
                        foreach (var item in repitem.Items.GetItemCollections(starSystem))
                            AddItemCollection(systemShop, item);
            }

            foreach (var item in Control.Settings.GetGenereicFactionSystemShopItems(faction, starSystem, reputation))
            {
                AddItemCollection(systemShop, item);
            }

#if CCDEBUG
            ShowItemCollections("After", systemShop.ItemCollections);
            Control.Logger.LogDebug("======================================================");
#endif
        }

        private static void AddItemCollection(Shop systemShop, string item)
        {
            Control.Logger.LogError($"+ {item} ");
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