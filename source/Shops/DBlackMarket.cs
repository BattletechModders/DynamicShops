﻿using System.Collections.Generic;

namespace DynamicShops.Shops;

public class DBlackMarket : CustomShops.Shops.BlackMarketShop
{
    public override string Name => "DBlackMarket";

    public override bool Exists => CustomShops.Control.State.CurrentSystem != null &&
        CustomShops.Control.State.CurrentSystem.Def.Tags.Contains("planet_other_blackmarket");
    protected override void UpdateTags()
    {
        List<string> tags = new();
        foreach (var shop_def in Control.BlackMarketShopDefs)
        {
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
                tags.AddRange(shop_def.Items);
        }
        Tags = tags;
    }
}
