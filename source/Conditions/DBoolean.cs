using System.Collections.Generic;
using System.Linq;
using BattleTech;

namespace DynamicShops
{
 
    [DCondition("true")]
    public class DTrueCondition : DCondition
    {
        public override bool Init(object json)
        {
            return true;
        }

        public override bool IfApply(SimGameState sim, StarSystem CurSystem)
        {
            return true;
        }
    }

    [DCondition("false")]
    public class DFalseCondition :DCondition
    {
        public override bool Init(object json)
        {
            return true;
        }

        public override bool IfApply(SimGameState sim, StarSystem CurSystem)
        {
            return false;
        }
    }

    [DCondition("or")]
    public class DOrCondition : DCondition
    {
        private List<DCondition> conditions;

        public override bool Init(object json)
        {
            conditions = ConditionBuilder.FromJson(json);
            return conditions != null;
        }

        public override bool IfApply(SimGameState sim, StarSystem CurSystem)
        {
            if (conditions == null || conditions.Count == 0)
                return true;

            return conditions.Any(i => i.IfApply(sim, CurSystem));
        }
    }

    [DCondition("not")]
    public class DNotCondition : DCondition
    {
        private List<DCondition> conditions;

        public override bool Init(object json)
        {
            conditions = ConditionBuilder.FromJson(json);
            return conditions != null;
        }

        public override bool IfApply(SimGameState sim, StarSystem CurSystem)
        {
            if (conditions == null || conditions.Count == 0)
                return false;

            return !conditions.All(i => i.IfApply(sim, CurSystem));
        }
    }
}