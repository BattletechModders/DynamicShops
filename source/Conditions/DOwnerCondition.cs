using BattleTech;
using System.Collections.Generic;

namespace DynamicShops
{
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
                var ttag = tag.Trim();
                if (ttag.StartsWith("!"))
                    nowners.AddRange(ConditionBuilder.ExpandGenericFaction(ttag.Substring(1)));
                else
                    owners.AddRange(ConditionBuilder.ExpandGenericFaction(ttag));
            }
            return true;
        }

        public override bool IfApply(SimGameState sim, StarSystem curSystem)
        {
            if (allways_true)
                return true;

            if (!owners.Contains(curSystem.OwnerValue.Name))
                return false;

            if (nowners.Contains(curSystem.OwnerValue.Name))
                return false;

            return true;
        }
    }
}
