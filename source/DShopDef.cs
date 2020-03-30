using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicShops
{
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
            if (base.FromJson(json))
            {
                if (json.TryGetValue("factions", out var items))
                {
                    var factions = ConditionBuilder.StringsFromJson(items);
                    if (Factions == null || Factions.Count == 0)
                        return false;
                    Factions = new List<string>();
                    foreach (var faction in factions)
                        factions.AddRange(ConditionBuilder.ExpandGenericFaction(faction));
                    return Factions.Count > 0;
                }
            }
            return false;
        }
    }
}
