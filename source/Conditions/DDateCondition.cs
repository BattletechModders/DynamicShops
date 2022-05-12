using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;

namespace DynamicShops
{
    [DCondition("dateCondition")]
    public class DateCondition : DCondition
    {
        public bool Autopass = false;
        public DateTime NotBefore = new(2500, 1, 1);
        public DateTime NotAfter = new(3200, 1, 1);
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


            if (Autopass)
            {
                Control.LogDebug(DInfo.Conditions, $"-- empty condition, passed");
                return true;
            }


            if (sim.CurrentDate < NotBefore)
            {
                Control.LogDebug(DInfo.Conditions, $"-- failed: current date {sim.CurrentDate} is before NotBefore {NotBefore}");
                return false;
            }

            if (sim.CurrentDate > NotAfter)
            {
                Control.LogDebug(DInfo.Conditions, $"-- failed: current date {sim.CurrentDate} is after NotAfter date {NotAfter}");

                return false;
            }
            Control.LogDebug(DInfo.Conditions, $"-- passed");
            return true;
        }
    }
}