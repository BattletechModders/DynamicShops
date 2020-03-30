using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicShops.Shops
{
    public class DSystemShop : CustomShops.Shops.SystemShop
    {
        public override string Name => "DSystem";

        protected override void UpdateTags()
        {
            List<string> tags = new List<string>();
            foreach (var shop_def in Control.ShopDefs)
            {
                var use = true;
                if (shop_def.Conditions != null)
                {
                    foreach (var c in shop_def.Conditions)
                    {
                        if (!c.IfApply(CustomShops.Control.State.CurrentSystem))
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
