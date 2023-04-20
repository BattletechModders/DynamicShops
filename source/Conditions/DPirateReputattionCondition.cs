using BattleTech;
using System;
using System.Collections.Generic;

namespace DynamicShops;

[DCondition("piraterep")]
public class DPirateReputattionCondition : DCondition
{
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

        if (json == null && !(json is string))
            return false;

        var str = json.ToString();

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
        return true;
    }
    public override bool IfApply(SimGameState sim, StarSystem curSystem)
    {
        if (allways_true)
            return true;

        var reputation = sim.GetReputation(FactionEnumeration.GetBlackMarketFactionValue());

        foreach (var item in equal)
            if (reputation != item)
                return false;

        foreach (var item in eless)
            if (reputation > item)
                return false;

        foreach (var item in emore)
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
