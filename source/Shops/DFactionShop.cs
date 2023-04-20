using System.Collections.Generic;
using BattleTech;

namespace DynamicShops.Shops
{
    public class DFactionShop : CustomShops.Shops.FactionShop
    {
        public override string Name => "DFaction";
        public override bool Exists
        {
            get
            {
                if (Control.Settings.FactionShopOnEveryPlanet)
                    return RelatedFaction != null && RelatedFaction.FactionDef.ShortName != "the Locals" && !RelatedFaction.IsInvalidUnset;
                else
                    return base.Exists;
            }
        }

        public override FactionValue RelatedFaction
        {
            get
            {
                if (!Control.Settings.OverrideFactionShopOwner)
                    return base.RelatedFaction;

                if (Control.Settings.FactionShopOnEveryPlanet)
                    return CustomShops.Control.State.CurrentSystem.OwnerValue;
                else
                    if(base.RelatedFaction != null)
                        return CustomShops.Control.State.CurrentSystem.OwnerValue;
                return base.RelatedFaction;
            }
        }

        protected override void UpdateTags()
        {
            if (RelatedFaction == null)
            {
                Tags = new List<string>();
                return;
            }
            var name = RelatedFaction.FactionDef.ShortName.ToLower();
            
            List<string> tags = new List<string>();
            foreach (var shop_def in Control.FactionShopDefs)
            {
                Control.LogDebug(DInfo.Conditions, $"Start to check conditions:");

                if (shop_def.Factions == null || shop_def.Factions.Count == 0) 
                {
                    Control.LogDebug(DInfo.Conditions, $"- empty faction list, failed");
                    continue;
                }
                if (!shop_def.Factions.Contains(name))
                {
                    Control.LogDebug(DInfo.Conditions, $"- failed faction check for {name} in [{DebugTools.ShowList("", shop_def.Factions)}]");
                    continue;
                }

                var use = true;
                if (shop_def.Conditions != null)
                {
                    foreach (var c in shop_def.Conditions)
                    {
                        if (!c.IfApply(CustomShops.Control.State.Sim, CustomShops.Control.State.CurrentSystem))
                        {
                            use = false;
                            break;
                        }
                    }
                }
                if (use)
                {
                    Control.LogDebug(DInfo.Conditions, DebugTools.ShowList("- passed", shop_def.Items));
                    tags.AddRange(shop_def.Items);
                }
            }
            Tags = tags;
        }
    }
}
