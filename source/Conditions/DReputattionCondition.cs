using BattleTech;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicShops;

[DCondition("rep")]
public class DReputationCondition : DCondition
{
    private List<SimGameReputation> less;
    private List<SimGameReputation> equal;
    private List<SimGameReputation> more;
    private List<SimGameReputation> equalMore;
    private List<SimGameReputation> equalLess;


    private bool always_true = false;

    public override bool Init(object json)
    {
        static void add_to_list(List<SimGameReputation> list, string value)
        {
            if (Enum.TryParse<SimGameReputation>(value, out var res))
                list.Add(res);
        }

        if (json is not string str)
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
        equalMore = new List<SimGameReputation>();
        equalLess = new List<SimGameReputation>();

        foreach (var item in strs)
        {
            var trimmedTag = item.Trim();
            if (trimmedTag.StartsWith("<"))
                add_to_list(less, trimmedTag.Substring(1));
            else if (trimmedTag.StartsWith(">"))
                add_to_list(more, trimmedTag.Substring(1));
            else if (trimmedTag.StartsWith("+"))
                add_to_list(equalMore, trimmedTag.Substring(1));
            else if (trimmedTag.StartsWith("-"))
                add_to_list(equalLess, trimmedTag.Substring(1));
            else
                add_to_list(equal, trimmedTag);
        }

        Control.LogDebug(DInfo.RepLoad, $"Reputation to owner loaded:");
        if (less.Count > 0) Control.LogDebug(DInfo.RepLoad, "- <:" + less.Aggregate("", (total, next) => total += next.ToString() + ";"));
        if (more.Count > 0) Control.LogDebug(DInfo.RepLoad, "- >:" + more.Aggregate("", (total, next) => total += next.ToString() + ";"));
        if (equalMore.Count > 0) Control.LogDebug(DInfo.RepLoad, "- +:" + equalMore.Aggregate("", (total, next) => total += next.ToString() + ";"));
        if (equalLess.Count > 0) Control.LogDebug(DInfo.RepLoad, "- -:" + equalLess.Aggregate("", (total, next) => total += next.ToString() + ";"));
        if (equal.Count > 0) Control.LogDebug(DInfo.RepLoad, "- =:" + equal.Aggregate("", (total, next) => total += next.ToString() + ";"));
        

        return true;
    }
    public override bool IfApply(SimGameState sim, StarSystem curSystem)
    {
        FactionValue check_faction = curSystem.OwnerValue;
       
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
        foreach (var item in equalLess)
            if (reputation > item)
            {
                Control.LogDebug(DInfo.Conditions, $"-- failed {reputation} > {item}");
                return false;
            }

        foreach (var item in equalMore)
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
