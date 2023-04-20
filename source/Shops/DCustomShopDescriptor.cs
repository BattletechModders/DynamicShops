using System.Collections.Generic;
using fastJSON;

namespace DynamicShops.Shops
{
    public enum ShopSubType { FactionBased, Custom }

    public class DCustomShopDescriptor
    {
        public string Name { get; set; }
        public string TabText { get; set; }
        public string Icon { get; set; }
        public string BackColor { get; set; }
        public string IconColor { get; set; }

        public string[] RefreshEvents { get; set; }
        public int SortOrder { get; set; }

        public ShopSubType Type { get; set; }

        public string Faction { get; set; }

        [JsonIgnore]
        public List<DCondition> ExistConditions { get; set; }
        [JsonIgnore]
        public List<DCondition> UseConditions { get; set; }

        public static DCustomShopDescriptor FromJson(object o)
        {
            return null;
        }
    }
}