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
#if CCDEBUG
            if (Control.Settings.DEBUG_ShowLoad)
                Control.LogDebug("---- load faction");
#endif
            if (base.FromJson(json))
            {

                if (json.TryGetValue("factions", out var items))
                {
                    var factions = ConditionBuilder.StringsFromJson(items);
                    if (factions == null || factions.Count == 0)
                    {
#if CCDEBUG
                        if (Control.Settings.DEBUG_ShowLoad)
                            Control.LogDebug("----- factions list empty");
#endif
                        return false;
                    }
                    Factions = new List<string>();
                    foreach (var faction in factions)
                        Factions.AddRange(ConditionBuilder.ExpandGenericFaction(faction));
#if CCDEBUG
                    if (Control.Settings.DEBUG_ShowLoad)
                    {
                        string list = "";
                        foreach (var f in Factions)
                            list += f + ";";
                        Control.LogDebug("----- Loaded: " + list);
                    }   
#endif
                    return Factions.Count > 0;
                }
#if CCDEBUG
                if (Control.Settings.DEBUG_ShowLoad)
                    Control.LogDebug("----- failed get faction");
#endif

            }
#if CCDEBUG
            if (Control.Settings.DEBUG_ShowLoad)
                Control.LogDebug("----- failed base");
#endif
            return false;
        }
    }
}
