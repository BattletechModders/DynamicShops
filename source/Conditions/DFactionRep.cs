using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;

namespace DynamicShops
{
    [DCondition("factionrep")]
    public class DFactionRep : DCondition
    {
        private string Faction;

        private FactionValue faction;

        private List<SimGameReputation> less;
        private List<SimGameReputation> equal;
        private List<SimGameReputation> more;
        private List<SimGameReputation> emore;
        private List<SimGameReputation> eless;


        private bool allways_true = false;

        public override bool Init(object json)
        {
            void add_to_list(List<SimGameReputation> list, string value)
            {
                if (Enum.TryParse<SimGameReputation>(value, out var res))
                    list.Add(res);
            }

            if (json == null || !(json is IDictionary<string, object> dictionary))
                return false;

            if (!dictionary.ContainsKey("faction") || !dictionary.ContainsKey("rep"))
                return false;

            var str = dictionary["rep"].ToString();
            Faction = dictionary["Faction"].ToString().ToLower();

            if (string.IsNullOrEmpty(str))
            {
                allways_true = true;
                return false;
            }
            allways_true = false;
            var strs = str.ToUpper().Split(',');


            less = new List<SimGameReputation>();
            equal = new List<SimGameReputation>();
            more = new List<SimGameReputation>();
            emore = new List<SimGameReputation>();
            eless = new List<SimGameReputation>();

            foreach (var item in strs)
            {
                var ttag = item.Trim();
                if (ttag.StartsWith("<"))
                    add_to_list(less, ttag.Substring(1));
                else if (ttag.StartsWith(">"))
                    add_to_list(more, ttag.Substring(1));
                else if (ttag.StartsWith("+"))
                    add_to_list(emore, ttag.Substring(1));
                else if (ttag.StartsWith("-"))
                    add_to_list(eless, ttag.Substring(1));
                else
                    add_to_list(equal, ttag);
            }

            Control.LogDebug(DInfo.RepLoad, $"Reputation to owner loaded:");
            if (less.Count > 0) Control.LogDebug(DInfo.RepLoad, "- <:" + less.Aggregate("", (total, next) => total += next.ToString() + ";"));
            if (more.Count > 0) Control.LogDebug(DInfo.RepLoad, "- >:" + more.Aggregate("", (total, next) => total += next.ToString() + ";"));
            if (emore.Count > 0) Control.LogDebug(DInfo.RepLoad, "- +:" + emore.Aggregate("", (total, next) => total += next.ToString() + ";"));
            if (eless.Count > 0) Control.LogDebug(DInfo.RepLoad, "- -:" + eless.Aggregate("", (total, next) => total += next.ToString() + ";"));
            if (equal.Count > 0) Control.LogDebug(DInfo.RepLoad, "- =:" + equal.Aggregate("", (total, next) => total += next.ToString() + ";"));


            return true;
        }
        public override bool IfApply(SimGameState sim, StarSystem curSystem)
        {

            if (faction == null)
            {
                faction = FactionEnumeration.FactionList.FirstOrDefault(
                    i => i.FactionDef.ShortName.ToLower() == Faction);

                if (faction == null)
                    faction = FactionEnumeration.GetInvalidUnsetFactionValue();
            }

            if (faction.IsInvalidUnset)
            {
                Control.LogDebug(DInfo.Conditions, $"- Reputation check for {faction.FactionDef.ShortName.ToLower()} failed - invalid faction {Faction}");
                return false;
            }

            var reputation = sim.GetReputation(faction);
            Control.LogDebug(DInfo.Conditions, $"- Reputation check for {faction.FactionDef.ShortName.ToLower()}  rep:{reputation}");

            if (allways_true)
            {
                Control.LogDebug(DInfo.Conditions, $"-- empty rep lists, passed");
                return true;
            }

            foreach (var item in equal)
                if (reputation != item)
                {
                    Control.LogDebug(DInfo.Conditions, $"-- failed {reputation} != {item}");
                    return false;
                }
            foreach (var item in eless)
                if (reputation > item)
                {
                    Control.LogDebug(DInfo.Conditions, $"-- failed {reputation} > {item}");
                    return false;
                }

            foreach (var item in emore)
                if (reputation < item)
                {
                    Control.LogDebug(DInfo.Conditions, $"-- failed {reputation} < {item}");
                    return false;
                }

            foreach (var item in less)
                if (reputation >= item)
                {
                    Control.LogDebug(DInfo.Conditions, $"-- failed {reputation} >= {item}");
                    return false;
                }

            foreach (var item in more)
                if (reputation <= item)
                {
                    Control.LogDebug(DInfo.Conditions, $"-- failed {reputation} <= {item}");
                    return false;
                }

            return true;
        }
    }
}