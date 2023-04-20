using BattleTech;

namespace DynamicShops;

[DCondition("careerLengthCondition")]
public class CareerLengthCondition : DCondition
{
    public bool Autopass = false;
    public int MinLength = int.MinValue;
    public int MaxLength = int.MaxValue;
    public override bool Init(object json)
    {
        if (json == null)
        {
            Autopass = true;
            return false;
        }

        var jsonString = json.ToString();
        if (string.IsNullOrEmpty(jsonString))
        {
            Autopass = true;
            return false;
        }
        Autopass = false;
        var splitConditions = jsonString.Split(',');
        foreach (var cond in splitConditions)
        {
            var trTime = cond.Trim();
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

        if (Autopass)
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