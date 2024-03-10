using BattleTech;
using System;
using System.Collections.Generic;

namespace DynamicShops;

[DCondition("piraterep")]
public class DPirateReputationCondition : DCondition
{
    private List<SimGameReputation> less;
    private List<SimGameReputation> equal;
    private List<SimGameReputation> more;
    private List<SimGameReputation> equalMore;
    private List<SimGameReputation> equalLess;
    
    private bool alwaysTrue = false;

    public override bool Init(object json)
    {
        static void add_to_list(List<SimGameReputation> list, string value)
        {
            if (Enum.TryParse<SimGameReputation>(value, out var res))
                list.Add(res);
        }

        if (json == null && json is not string)
            return false;

        var str = json.ToString();

        if (string.IsNullOrEmpty(str))
        {
            alwaysTrue = true;
            return false;
        }
        alwaysTrue = false;
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
        return true;
    }
    public override bool IfApply(SimGameState sim, StarSystem curSystem)
    {
        if (alwaysTrue)
            return true;

        var reputation = sim.GetReputation(FactionEnumeration.GetBlackMarketFactionValue());

        foreach (var item in equal)
            if (reputation != item)
                return false;

        foreach (var item in equalLess)
            if (reputation > item)
                return false;

        foreach (var item in equalMore)
            if (reputation < item)
                return false;

        foreach (var item in less)
            if (reputation >= item)
                return false;

        foreach (var item in more)
            if (reputation <= item)
                return false;


        return true;
    }
}
