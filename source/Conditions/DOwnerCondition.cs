using BattleTech;
using System.Collections.Generic;

namespace DynamicShops;

[DCondition("owner")]
public class DOwnerCondition : DCondition
{
    private List<string> owners;
    private List<string> noOwners;
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
        owners = new List<string>();
        noOwners = new List<string>();

        foreach (var tag in strs)
        {
            var trimmedTag = tag.Trim().ToLower();
            if (trimmedTag.StartsWith("!"))
                noOwners.AddRange(ConditionBuilder.ExpandGenericFaction(trimmedTag.Substring(1)));
            else
                owners.AddRange(ConditionBuilder.ExpandGenericFaction(trimmedTag));
        }

        Control.LogDebug(DInfo.FactionLoad, "Owner loaded:");
        Control.LogDebug(DInfo.FactionLoad, $"- base: {str}");
        Control.LogDebug(DInfo.FactionLoad, DebugTools.ShowList("- owners:", owners));
        Control.LogDebug(DInfo.FactionLoad, DebugTools.ShowList("- noOwners:", noOwners));

        return true;
    }

    public override bool IfApply(SimGameState sim, StarSystem curSystem)
    {
        Control.LogDebug(DInfo.Conditions, $"- Owner check for {curSystem.OwnerValue.FactionDef.ShortName.ToLower()}");

        if (alwaysTrue)
        {
            Control.LogDebug(DInfo.Conditions, $"-- empty condition, passed");
            return true;
        }


        if (!owners.Contains(curSystem.OwnerValue.FactionDefID))
        {

            Control.LogDebug(DInfo.Conditions, DebugTools.ShowList("-- owner failed:", owners));

            return false;
        }

        if (noOwners.Contains(curSystem.OwnerValue.FactionDefID))
        {
            Control.LogDebug(DInfo.Conditions, DebugTools.ShowList("-- noOwner failed:", noOwners));

            return false;
        }
        Control.LogDebug(DInfo.Conditions, $"-- passed");
        return true;
    }
}
