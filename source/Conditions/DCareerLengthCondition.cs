using BattleTech;

namespace DynamicShops;

[DCondition("careerLengthCondition")]
public class CareerLengthCondition : DCondition
{
    public bool AutoPass = false;
    public int MinLength = int.MinValue;
    public int MaxLength = int.MaxValue;
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
                MaxLength = int.Parse(trTime.Substring(1));
            }
            else
            {
                MinLength = int.Parse(trTime);
            }
        }
        Control.LogDebug(DInfo.CLengthLoad, "careerLengthCondition loaded:");
        Control.LogDebug(DInfo.CLengthLoad, $"- NotBefore: {MaxLength}");
        Control.LogDebug(DInfo.CLengthLoad, $"- NotAfter: {MinLength}");
        return true;
    }

    public override bool IfApply(SimGameState sim, StarSystem curSystem)
    {
        Control.LogDebug(DInfo.Conditions, $"- careerLengthCondition check for {sim.CurrentDate}");

        if (AutoPass)
        {
            Control.LogDebug(DInfo.Conditions, $"-- empty condition, passed");
            return true;
        }

        if (MinLength != int.MinValue && sim.DaysPassed < MinLength)
        {
            Control.LogDebug(DInfo.Conditions, $"-- failed: current career length {sim.DaysPassed} is less than MinLength {MinLength}");
            return false;
        }

        if (MaxLength != int.MaxValue && sim.DaysPassed > MaxLength)
        {
            Control.LogDebug(DInfo.Conditions, $"-- failed: current career length {sim.DaysPassed} is greater than MaxLength {MaxLength}");

            return false;
        }
        Control.LogDebug(DInfo.Conditions, $"-- passed");
        return true;
    }
}