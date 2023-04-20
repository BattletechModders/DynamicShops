using System.Collections.Generic;

namespace DynamicShops.Shops;

public class DSystemShop : CustomShops.Shops.SystemShop
{
    public override string Name => "DSystem";

    protected override void UpdateTags()
    {
        List<string> tags = new List<string>();
        if (!string.IsNullOrWhiteSpace(Control.Settings.EmptyPlanetTag))
            if (CustomShops.Control.State.CurrentSystem.Def.Tags.Contains(Control.Settings.EmptyPlanetTag))
            {
                Tags = tags;
                return;
            }

        foreach (var shop_def in Control.ShopDefs)
        {
            Control.LogDebug(DInfo.Conditions, $"Start to check conditions:");

            var use = true;
            if (shop_def.Conditions != null)
            {
                foreach (var c in shop_def.Conditions)
                {

                    Control.LogDebug(DInfo.Conditions, $"- {c.GetType().ToString()}");
                    if (!c.IfApply(CustomShops.Control.State.Sim, CustomShops.Control.State.CurrentSystem))
                    {
                        use = false;
                        break;
                    }
                }
            }
            else
            {
                Control.LogDebug(DInfo.Conditions, $"- empty");
            }
            if (use)
            {
                Control.LogDebug(DInfo.Conditions, DebugTools.ShowList("passed", shop_def.Items));
                tags.AddRange(shop_def.Items);
            }
        }
        Tags = tags;
    }
}
