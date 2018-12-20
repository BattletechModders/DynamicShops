using BattleTech;
using BattleTech.UI;
using Harmony;

namespace DynamicShops.Patches
{

#if CCDEBUG
    [HarmonyPatch(typeof(StarSystem), "HasFactionStore")]
    public static class StarSystem_HasFactionStore
    {
        [HarmonyPostfix]
        public static void ShowShopInfo(StarSystem __instance)
        {
            Control.Logger.LogDebug($"HasFactionStore for {__instance.Def}");
            Control.Logger.LogDebug($"-- ItemColections: {__instance.FactionShop.ItemCollections.Count}");
            Control.Logger.LogDebug($"-- Empty {__instance.FactionShop.IsEmpty}");
            Control.Logger.LogDebug($"-- Pending {__instance.FactionShop.IsPending}");
        }
    }


    [HarmonyPatch(typeof(SG_Shop_Screen), "BeginShop")]
    public static class SG_Shop_Screen_BeginShop
    {
        [HarmonyPostfix]
        public static void ShowShopInfo(SG_Shop_Screen __instance)
        {
            void show_inventory(Shop shop)
            {
                Control.Logger.LogDebug(
                    $"{shop.ThisShopType}: IsEmpty:{shop.IsEmpty} Count:{shop.ActiveInventory.Count}");
                if(shop.ActiveInventory != null)
                foreach (var shopDefItem in shop.ActiveInventory)
                {
                    Control.Logger.LogDebug($"-- : {shopDefItem.ID} {shopDefItem.Type} {shopDefItem.Count}");
                }
            }

            var system =UnityGameInstance.BattleTechGame.Simulation.CurSystem;

            Control.Logger.LogDebug($"BeginShop for {system.Name}");
            show_inventory(system.SystemShop);
            show_inventory(system.FactionShop);
            show_inventory(system.BlackMarketShop);
        }
    }

#endif
}