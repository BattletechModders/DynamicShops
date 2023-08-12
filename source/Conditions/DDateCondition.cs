using System;
using BattleTech;

namespace DynamicShops;

[DCondition("dateCondition")]
public class DateCondition : DCondition
{
    public bool AutoPass = false;
    public DateTime NotBefore = DateTime.MinValue;
    public DateTime NotAfter = DateTime.MaxValue;
    public override bool Init(object json)
    {
        if (json == null)
        {
            AutoPass = true;
            return false;
        }

        var jsonString = json.ToString();
        if (string.IsNullOrEmpty(jsonString))
        {
            AutoPass = true;
            return false;
        }
        AutoPass = false;
        var splitConditions = jsonString.Split(',');
        foreach (var condition in splitConditions)
        {
            var trTime = condition.Trim();
            if (trTime.StartsWith("!"))
            {
                NotAfter = DateTime.Parse(trTime.Substring(1));
            }
            else
            {
                NotBefore = DateTime.Parse(trTime);
            }
        }
        Control.LogDebug(DInfo.DateLoad, "dateCondition loaded:");
        Control.LogDebug(DInfo.DateLoad, $"- NotBefore: {NotBefore}");
        Control.LogDebug(DInfo.DateLoad, $"- NotAfter: {NotAfter}");
        return true;
    }

    public override bool IfApply(SimGameState sim, StarSystem curSystem)
    {
        Control.LogDebug(DInfo.Conditions, $"- dateCondition check for {sim.CurrentDate}");


        if (AutoPass)
        {
            Control.LogDebug(DInfo.Conditions, $"-- empty condition, passed");
            return true;
        }


        if (NotBefore != DateTime.MaxValue && sim.CurrentDate < NotBefore)
        {
            Control.LogDebug(DInfo.Conditions, $"-- failed: current date {sim.CurrentDate} is before NotBefore {NotBefore}");
            return false;
        }

        if (NotAfter != DateTime.MinValue && sim.CurrentDate > NotAfter)
        {
            Control.LogDebug(DInfo.Conditions, $"-- failed: current date {sim.CurrentDate} is after NotAfter date {NotAfter}");

            return false;
        }
        Control.LogDebug(DInfo.Conditions, $"-- passed");
        return true;
    }
}