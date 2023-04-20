using BattleTech;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicShops
{
    [DCondition("rep")]
    public class DReputattionCondition : DCondition
    {
        private List<SimGameReputation> less;
        private List<SimGameReputation> equal;
        private List<SimGameReputation> more;
        private List<SimGameReputation> emore;
        private List<SimGameReputation> eless;


        private bool always_true = false;

        public override bool Init(object json)
        {
            void add_to_list(List<SimGameReputation> list, string value)
            {
                if (Enum.TryParse<SimGameReputation>(value, out var res))
                    list.Add(res);
            }

            if (!(json is string str))
                return false;

            if (string.IsNullOrEmpty(str))
            {
                always_true = true;
                return false;
            }
            always_true = false;
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
            FactionValue check_faction = curSystem.OwnerValue;


            check_faction = curSystem.OwnerValue;
            if (check_faction.IsInvalidUnset)
                return false;
            
            var reputation = sim.GetReputation(check_faction);
            Control.LogDebug(DInfo.Conditions, $"- Reputation check for {check_faction.FactionDef.ShortName.ToLower()}(owner)  rep:{reputation}");

            if (always_true)
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
