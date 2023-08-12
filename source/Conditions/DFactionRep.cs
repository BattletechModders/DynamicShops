using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;

namespace DynamicShops;

[DCondition("factionrep")]
public class DFactionRep : DCondition
{
    private string Faction;

    private FactionValue faction;

    private List<SimGameReputation> less;
    private List<SimGameReputation> equal;
    private List<SimGameReputation> more;
    private List<SimGameReputation> equalMore;
    private List<SimGameReputation> equalLess;


    private bool alwaysTrue = false;

    public override bool Init(object json)
    {
        static void addToList(List<SimGameReputation> list, string value)
        {
            if (Enum.TryParse<SimGameReputation>(value, out var res))
                list.Add(res);
        }

        if (json == null || json is not IDictionary<string, object> dictionary)
            return false;

        if (!dictionary.ContainsKey("faction") || !dictionary.ContainsKey("rep"))
            return false;

        var str = dictionary["rep"].ToString();
        Faction = dictionary["faction"].ToString();

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
            var tag = item.Trim();
            if (tag.StartsWith("<"))
                addToList(less, tag.Substring(1));
            else if (tag.StartsWith(">"))
                addToList(more, tag.Substring(1));
            else if (tag.StartsWith("+"))
                addToList(equalMore, tag.Substring(1));
            else if (tag.StartsWith("-"))
                addToList(equalLess, tag.Substring(1));
            else
                addToList(equal, tag);
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

        faction ??= FactionEnumeration.GetFactionByName(Faction);

        if (faction.IsInvalidUnset)
        {
            Control.LogDebug(DInfo.Conditions, $"- Reputation check for {faction.FactionDef.ShortName.ToLower()} failed - invalid faction {Faction}");
            return false;
        }

        var reputation = sim.GetReputation(faction);
        Control.LogDebug(DInfo.Conditions, $"- Reputation check for {faction.FactionDef.ShortName.ToLower()}  rep:{reputation}");

        if (alwaysTrue)
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