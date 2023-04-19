using System.Collections.Generic;

namespace DynamicShops.Shops
{
    public class DBlackMarket : CustomShops.Shops.BlackMarketShop
    {
        public override string Name => "DBlackMarket";

        protected override void UpdateTags()
        {
            List<string> tags = new List<string>();
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
}
