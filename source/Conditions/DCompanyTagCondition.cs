using BattleTech;
using System.Collections.Generic;

namespace DynamicShops;

[DCondition("ctag")]
public class DCompanyTagCondition : DCondition
{
    private List<string> tags;
    private List<string> negativeTags;
    private bool alwaysTrue = false;

    public override bool Init(object json)
    {
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
        tags = new List<string>();
        negativeTags = new List<string>();
        foreach (var tag in strs)
        {
            var ttag = tag.Trim();
            if (ttag.StartsWith("!"))
                negativeTags.Add(ttag.Substring(1));
            else
                tags.Add(ttag);
        }
        return true;
    }

    public override bool IfApply(SimGameState sim, StarSystem CurSystem)
    {

        if (alwaysTrue)
            return true;

        foreach (var item in tags)
            if (!sim.CompanyTags.Contains(item))
                return false;

        foreach (var item in negativeTags)
            if (sim.CompanyTags.Contains(item))
                return false;
        return true;
    }
}