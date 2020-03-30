using BattleTech;
using System;
using System.Collections.Generic;

namespace DynamicShops
{
    public class DConditionAttribute : Attribute
    {
        public String Name { get; private set; }

        public DConditionAttribute(string name) { Name = name; }
    }

    public abstract class DCondition
    {
        public abstract bool Init(object json);
        public abstract bool IfApply(SimGameState sim, StarSystem CurSystem);
    }

    [DCondition("tag")]
    public class DTagCondition : DCondition
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
                if (!CurSystem.Def.Tags.Contains(item))
                    return false;

            foreach (var item in ntags)
                if (CurSystem.Def.Tags.Contains(item))
                    return false;
            return true;
        }
    }
    
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

            if (!owners.Contains(curSystem.OwnerValue.FactionDef.ShortName))
                return false;

            if (nowners.Contains(curSystem.OwnerValue.FactionDef.ShortName))
                return false;

            return true;
        }
    }

    [DCondition("rep")]
    public class DReputattionCondition : DCondition
    {
        private List<SimGameReputation> less;
        private List<SimGameReputation> equal;
        private List<SimGameReputation> more;
        private bool allways_true = false;

        public override bool Init(object json)
        {
            void add_to_list(List<SimGameReputation> list, string value)
            {
                if (Enum.TryParse<SimGameReputation>(value, out var res))
                    list.Add(res);
            }

            if (json == null && !(json is string))
                return false;

            var str = json.ToString();

            if (string.IsNullOrEmpty(str))
            {
                allways_true = true;
                return false;
            }
            allways_true = false;
            var strs = str.ToUpper().Split(',');


            less = new List<SimGameReputation>();
            equal = new List<SimGameReputation>();
            more = new List<SimGameReputation>();

            foreach (var item in strs)
            {
                var ttag = item.Trim();
                if (ttag.StartsWith("<"))
                    add_to_list(less, ttag.Substring(1));
                else if (ttag.StartsWith(">"))
                    add_to_list(more, ttag.Substring(1));
                else
                    add_to_list(equal, ttag);
            }
            return true;
        }
        public override bool IfApply(SimGameState sim, StarSystem curSystem)
        {
            if (allways_true)
                return true;

            var reputation = sim.GetReputation(curSystem.OwnerValue);

            foreach (var item in equal)
                if (reputation != item)
                    return false;

            foreach (var item in less)
                if (reputation > item)
                    return false;

            foreach (var item in more)
                if (reputation < item)
                    return false;

            return true;
        }
    }

    [DCondition("piraterep")]
    public class DPirateReputattionCondition : DCondition
    {
        private List<SimGameReputation> less;
        private List<SimGameReputation> equal;
        private List<SimGameReputation> more;
        private bool allways_true = false;

        public override bool Init(object json)
        {
            void add_to_list(List<SimGameReputation> list, string value)
            {
                if (Enum.TryParse<SimGameReputation>(value, out var res))
                    list.Add(res);
            }

            if (json == null && !(json is string))
                return false;

            var str = json.ToString();

            if (string.IsNullOrEmpty(str))
            {
                allways_true = true;
                return false;
            }
            allways_true = false;
            var strs = str.ToUpper().Split(',');

            less = new List<SimGameReputation>();
            equal = new List<SimGameReputation>();
            more = new List<SimGameReputation>();

            foreach (var item in strs)
            {
                var ttag = item.Trim();
                if (ttag.StartsWith("<"))
                    add_to_list(less, ttag.Substring(1));
                else if (ttag.StartsWith(">"))
                    add_to_list(more, ttag.Substring(1));
                else
                    add_to_list(equal, ttag);
            }
            return true;
        }
        public override bool IfApply(SimGameState sim, StarSystem curSystem)
        {
            if (allways_true)
                return true;

            var reputation = sim.GetReputation(FactionEnumeration.GetBlackMarketFactionValue());

            foreach (var item in equal)
                if (reputation != item)
                    return false;

            foreach (var item in less)
                if (reputation > item)
                    return false;

            foreach (var item in more)
                if (reputation < item)
                    return false;

            return true;
        }
    }

    [DCondition("mrb")]
    public class DMRBCondition : DCondition
    {
        private List<int> less;
        private List<int> equal;
        private List<int> more;
        private bool allways_true = false;

        public override bool Init(object json)
        {
            void add_to_list(List<int> list, string value)
            {
                if (int.TryParse(value, out var res))
                    list.Add(res);
            }

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

            less = new List<int>();
            equal = new List<int>();
            more = new List<int>();

            foreach (var item in strs)
            {
                var ttag = item.Trim();
                if (ttag.StartsWith("<"))
                    add_to_list(less, ttag.Substring(1));
                else if (ttag.StartsWith(">"))
                    add_to_list(more, ttag.Substring(1));
                else
                    add_to_list(equal, ttag);
            }
            return true;
        }
        public override bool IfApply(SimGameState sim, StarSystem curSystem)
        {
            if (allways_true)
                return true;

            var reputation = sim.GetCurrentMRBLevel();

            foreach (var item in equal)
                if (reputation != item)
                    return false;

            foreach (var item in less)
                if (reputation >= item)
                    return false;

            foreach (var item in more)
                if (reputation <= item)
                    return false;

            return true;
        }
    }
}
