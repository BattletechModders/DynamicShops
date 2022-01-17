using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace DynamicShops.Shops
{
    public class DCustomShopDescriptor
    {
        public string Name { get; set; } = "";
        public string TabText { get; set; } = "";
        public string Icon { get; set; } = "";
        public string BackColor { get; set; } = "";
        public string IconColor { get; set; } = "";

        public List<string> RefreshEvents { get; set; } = new() { "SystemChange", "MonthEnd" };
        public int SortOrder { get; set; } = 0;

        public string Faction { get; set; } = "";

        [JsonIgnore]
        public List<DCondition> ExistConditions { get; set; } = null;
        [JsonIgnore]
        public List<DCondition> UseConditions { get; set; } = null;

        public static DCustomShopDescriptor FromJson(Dictionary<string, object> obj)
        {
            string _Name = obj.GetString("Name");
            string _TabText = obj.GetString("TabText");
            string _Icon = obj.GetString("Icon");
            string _BackColor = obj.GetString("BackColor"); // Not used ATM
            string _IconColor = obj.GetString("IconColor"); // Not used ATM

            int _SortOrder = obj.GetInt("SortOrder");

            List<string> _RefreshEvents = null;

            if (obj.TryGetValue("RefreshEvents", out var refreshEvents))
            {
                _RefreshEvents = refreshEvents as List<string>;
            }

            if (string.IsNullOrEmpty(_Name) || string.IsNullOrEmpty(_TabText))
                return null;

            DCustomShopDescriptor descriptor = new()
            {
                Name = _Name,
                TabText = _TabText,
                Icon = _Icon,
                BackColor = _BackColor,
                IconColor = _IconColor,
                SortOrder = _SortOrder
            };

            if (_RefreshEvents != null && _RefreshEvents.Count > 0)
                descriptor.RefreshEvents = _RefreshEvents;

            if (obj.TryGetValue("ExistConditions", out var extConditions))
                descriptor.ExistConditions = ConditionBuilder.FromJson(extConditions);
            else
                descriptor.ExistConditions = null;

            if (obj.TryGetValue("UseConditions", out var useConditions))
                descriptor.UseConditions = ConditionBuilder.FromJson(useConditions);
            else
                descriptor.UseConditions = null;

            Control.Log($"reading descriptor {JsonConvert.SerializeObject(descriptor)}");
            return descriptor;
        }
    }

    public static class ExtenstionMethods
    {
        public static string GetString(this Dictionary<string, object> _Dict, string _Key, string _DefValue = "")
        {
            if (string.IsNullOrEmpty(_Key))
                return _DefValue;

            if (_Dict == null)
                return _DefValue;

            if (_Dict.ContainsKey(_Key))
                return _Dict[_Key].ToString();

            return _DefValue;
        }

        public static int GetInt(this Dictionary<string, object> _Dict, string _Key, int _DefValue = -1)
        {
            if (string.IsNullOrEmpty(_Key))
                return _DefValue;

            if (_Dict == null)
                return _DefValue;

            if (_Dict.ContainsKey(_Key))
            {
                return Convert.ToInt32(_Dict[_Key]);
            }

            return _DefValue;
        }
    }
}