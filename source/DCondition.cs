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
        public DCondition(string json) { }


        public abstract bool IfApply(StarSystem curSystem);
    }

    [DCondition("tag")]
    public class DTagCondition : DCondition
    {
        private List<string> tags;
        private List<string> ntags;
        private bool allways_true = false;

        public DTagCondition(string json) : base(json)
        {
            if (string.IsNullOrEmpty(json))
            {
                allways_true = true;
                return;
            }
            allways_true = false;
            var strs = json.Split(',');
            tags = new List<string>();
            ntags = new List<string>();
            foreach (var tag in strs)
            {
                if (tag.StartsWith("!"))
                    ntags.Add(tag.Substring(1));
                else
                    tags.Add(tag);
            }
        }

        public override bool IfApply(StarSystem curSystem)
        {
            if (allways_true)
                return true;

            foreach (var item in tags)
                if (!curSystem.Def.Tags.Contains(item))
                    return false;

            foreach (var item in ntags)
                if (curSystem.Def.Tags.Contains(item))
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

        public DOwnerCondition(string json) : base(json)
        {
            if (string.IsNullOrEmpty(json))
            {
                allways_true = true;
                return;
            }
            allways_true = false;
            var strs = json.Split(',');
            owners = new List<string>();
            nowners = new List<string>();
            foreach (var tag in strs)
            {
                if (tag.StartsWith("!"))
                    nowners.Add(tag.Substring(1));
                else
                    owners.Add(tag);
            }
        }

        public override bool IfApply(StarSystem curSystem)
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

        public DReputattionCondition(string json) : base(json)
        {
            void add_to_list(List<SimGameReputation> list, string value)
            {
                if (Enum.TryParse<SimGameReputation>(value, out var res))
                    list.Add(res);
            }


            if (string.IsNullOrEmpty(json))
            {
                allways_true = true;
                return;
            }
            allways_true = false;
            var strs = json.ToUpper().Split(',');

            less = new List<SimGameReputation>();
            equal = new List<SimGameReputation>();
            more = new List<SimGameReputation>();

            foreach (var item in strs)
            {
                if (item.StartsWith("<"))
                    add_to_list(less, item.Substring(1));
                else if (item.StartsWith(">"))
                    add_to_list(more, item.Substring(1));
                else
                    add_to_list(equal, item);
            }
        }
        public override bool IfApply(StarSystem curSystem)
        {
            if (allways_true)
                return true;

            var reputation = CustomShops.Control.State.Sim.GetReputation(curSystem.OwnerValue);

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

    [DCondition("piraterep")]
    public class DPirateReputattionCondition : DCondition
    {
        private List<SimGameReputation> less;
        private List<SimGameReputation> equal;
        private List<SimGameReputation> more;
        private bool allways_true = false;

        public DPirateReputattionCondition(string json) : base(json)
        {
            void add_to_list(List<SimGameReputation> list, string value)
            {
                if (Enum.TryParse<SimGameReputation>(value, out var res))
                    list.Add(res);
            }


            if (string.IsNullOrEmpty(json))
            {
                allways_true = true;
                return;
            }
            allways_true = false;
            var strs = json.ToUpper().Split(',');

            less = new List<SimGameReputation>();
            equal = new List<SimGameReputation>();
            more = new List<SimGameReputation>();

            foreach (var item in strs)
            {
                if (item.StartsWith("<"))
                    add_to_list(less, item.Substring(1));
                else if (item.StartsWith(">"))
                    add_to_list(more, item.Substring(1));
                else
                    add_to_list(equal, item);
            }
        }
        public override bool IfApply(StarSystem curSystem)
        {
            if (allways_true)
                return true;

            var reputation = CustomShops.Control.State.Sim.GetReputation(FactionEnumeration.GetBlackMarketFactionValue());

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

    [DCondition("mrb")]
    public class DMRBCondition : DCondition
    {
        private List<int> less;
        private List<int> equal;
        private List<int> more;
        private bool allways_true = false;

        public DMRBCondition(string json) : base(json)
        {
            void add_to_list(List<int> list, string value)
            {
                if (int.TryParse(value,System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var res))
                    list.Add(res);
            }


            if (string.IsNullOrEmpty(json))
            {
                allways_true = true;
                return;
            }
            allways_true = false;
            var strs = json.ToUpper().Split(',');

            less = new List<int>();
            equal = new List<int>();
            more = new List<int>();

            foreach (var item in strs)
            {
                if (item.StartsWith("<"))
                    add_to_list(less, item.Substring(1));
                else if (item.StartsWith(">"))
                    add_to_list(more, item.Substring(1));
                else
                    add_to_list(equal, item);
            }
        }
        public override bool IfApply(StarSystem curSystem)
        {
            if (allways_true)
                return true;

            var reputation = CustomShops.Control.State.Sim.GetCurrentMRBLevel();

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
