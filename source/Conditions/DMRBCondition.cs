using BattleTech;
using System.Collections.Generic;

namespace DynamicShops;

[DCondition("mrb")]
public class DMRBCondition : DCondition
{
    private List<int> less;
    private List<int> equal;
    private List<int> more;
    private List<int> emore;
    private List<int> eless;

    private bool allways_true = false;

    public override bool Init(object json)
    {
        void add_to_list(List<int> list, string value)
        {
            if (int.TryParse(value, out var res))
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
        var strs = str.Split(',');

        less = new List<int>();
        equal = new List<int>();
        more = new List<int>();
        emore = new List<int>();
        eless = new List<int>();

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

        var reputation = sim.GetCurrentMRBLevel();

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
