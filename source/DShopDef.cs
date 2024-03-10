using System.Collections.Generic;
using System.Linq;

namespace DynamicShops;

public class DShopDef
{
    public List<DCondition> Conditions;
    public List<string> Items;

    public virtual bool FromJson(Dictionary<string, object> json)
    {
        if (json.TryGetValue("conditions", out var con))
            Conditions = ConditionBuilder.FromJson(con);
        else
            Conditions = null;
        if (json.TryGetValue("items", out var items))
        {
            Items = ConditionBuilder.StringsFromJson(items);
            return Items != null && Items.Count > 0;
        }
        return false;
    }
}
public class DFactionShopDef : DShopDef
{
    public List<string> Factions;

    public override bool FromJson(Dictionary<string, object> json)
    {
        Control.LogDebug(DInfo.Loading, "---- load faction");
        if (base.FromJson(json))
        {

            if (json.TryGetValue("factions", out var items))
            {
                var factions = ConditionBuilder.StringsFromJson(items);
                if (factions == null || factions.Count == 0)
                {
                    Control.LogDebug(DInfo.Loading, "----- factions list empty");
                    return false;
                }
                Factions = new List<string>();
                foreach (var faction in factions)
                    Factions.AddRange(ConditionBuilder.ExpandGenericFaction(faction));
#if CCDEBUG
                if (Control.Settings.DebugInfo.HasFlag(DInfo.Loading))
                {
                    string list = "";
                    foreach (var f in Factions)
                        list += f + ";";
                    Control.LogDebug(DInfo.Loading, "----- Loaded: " + list);
                }
#endif
                return Factions.Count > 0;
            }
            Control.LogDebug(DInfo.Loading, "----- failed get faction");

        }
        Control.LogDebug(DInfo.Loading, "----- failed base");
        return false;
    }
}

public class DCustomShopDef : DShopDef
{
    public string ShopName;

    public override bool FromJson(Dictionary<string, object> json)
    {
        if (base.FromJson(json))
        {
            if (!json.ContainsKey("shop"))
                return false;
            ShopName = json["shop"].ToString();
            return true;
        }

        return false;
    }
}
