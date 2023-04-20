using BattleTech;
using System.Collections.Generic;

namespace DynamicShops;

[DCondition("owner")]
public class DOwnerCondition : DCondition
{
    private List<string> owners;
    private List<string> nowners;
    private bool allways_true = false;
    public override bool Init(object json)
    {
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
        owners = new List<string>();
        nowners = new List<string>();

        foreach (var tag in strs)
        {
            var ttag = tag.Trim().ToLower();
            if (ttag.StartsWith("!"))
                nowners.AddRange(ConditionBuilder.ExpandGenericFaction(ttag.Substring(1)));
            else
                owners.AddRange(ConditionBuilder.ExpandGenericFaction(ttag));
        }

        Control.LogDebug(DInfo.FactionLoad, "Owner loaded:");
        Control.LogDebug(DInfo.FactionLoad, $"- base: {str}");
        Control.LogDebug(DInfo.FactionLoad, DebugTools.ShowList("- owners:", owners));
        Control.LogDebug(DInfo.FactionLoad, DebugTools.ShowList("- nowners:", nowners));

        return true;
    }

    public override bool IfApply(SimGameState sim, StarSystem curSystem)
    {
        Control.LogDebug(DInfo.Conditions, $"- Owner check for {curSystem.OwnerValue.FactionDef.ShortName.ToLower()}");


        if (allways_true)
        {
            Control.LogDebug(DInfo.Conditions, $"-- empty condition, passed");
            return true;
        }


        if (!owners.Contains(curSystem.OwnerValue.Name.ToLower()))
        {

            Control.LogDebug(DInfo.Conditions, DebugTools.ShowList("-- owner failed:", owners));

            return false;
        }

        if (nowners.Contains(curSystem.OwnerValue.Name.ToLower()))
        {
            Control.LogDebug(DInfo.Conditions, DebugTools.ShowList("-- nowner failed:", nowners));

            return false;
        }
        Control.LogDebug(DInfo.Conditions, $"-- passed");
        return true;
    }
}
