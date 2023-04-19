using BattleTech;
using System.Collections.Generic;

namespace DynamicShops
{
    [DCondition("tag")]
    public class DCompanyTagCondition : DCondition
    {
        private List<string> tags;
        private List<string> ntags;
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
            tags = new List<string>();
            ntags = new List<string>();
            foreach (var tag in strs)
            {
                var ttag = tag.Trim();
                if (ttag.StartsWith("!"))
                    ntags.Add(ttag.Substring(1));
                else
                    tags.Add(ttag);
            }
            return true;
        }

        public override bool IfApply(SimGameState sim, StarSystem CurSystem)
        {

            if (allways_true)
                return true;

            foreach (var item in tags)
                if (!sim.CompanyTags.Contains(item))
                    return false;

            foreach (var item in ntags)
                if (sim.CompanyTags.Contains(item))
                    return false;
            return true;
        }
    }
}