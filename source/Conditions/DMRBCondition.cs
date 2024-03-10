using BattleTech;
using System.Collections.Generic;

namespace DynamicShops;

[DCondition("mrb")]
public class DMRBCondition : DCondition
{
    private List<int> less;
    private List<int> equal;
    private List<int> more;
    private List<int> equalMore;
    private List<int> equalLess;

    private bool alwaysTrue = false;

    public override bool Init(object json)
    {
        void add_to_list(List<int> list, string value)
        {
            if (int.TryParse(value, out var res))
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
        var strs = str.Split(',');

        less = new List<int>();
        equal = new List<int>();
        more = new List<int>();
        equalMore = new List<int>();
        equalLess = new List<int>();

        foreach (var item in strs)
        {
            var tag = item.Trim();
            if (tag.StartsWith("<"))
                add_to_list(less, tag.Substring(1));
            else if (tag.StartsWith(">"))
                add_to_list(more, tag.Substring(1));
            else if (tag.StartsWith("+"))
                add_to_list(equalMore, tag.Substring(1));
            else if (tag.StartsWith("-"))
                add_to_list(equalLess, tag.Substring(1));
            else
                add_to_list(equal, tag);
        }
        return true;
    }
    public override bool IfApply(SimGameState sim, StarSystem curSystem)
    {
        if (alwaysTrue)
            return true;

        var reputation = sim.GetCurrentMRBLevel();

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
