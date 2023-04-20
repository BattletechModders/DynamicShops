using System;
using HBS.Logging;

namespace DynamicShops
{
    [Serializable]
    public class FactionList
    {
        public string Name;
        public string[] Members;
    }

    public enum DInfo {
        None = 0,
        Main = 1,
        Loading = 1 << 1,
        Conditions = 1 << 2,
        FactionLoad = 1 << 3,
        RepLoad = 1 << 4,
        DateLoad = 1 << 5,
        CLengthLoad = 1 << 6,
        All = 0xffff,
    }

    public class DynamicShopSettings
    {
        public LogLevel LogLevel = LogLevel.Debug;
        public DInfo DebugInfo = DInfo.Main | DInfo.Loading;
        
        public bool DEBUG_ShowLoad = true;

        public bool ReplaceSystemShop = true;
        public bool ReplaceFactionShop = true;
        public bool ReplaceBlackMarket = true;

        public bool FactionShopOnEveryPlanet = true;
        public bool OverrideFactionShopOwner = true;

        public string BlackMarketSystemTag = "planet_other_blackmarket";
        public string EmptyPlanetTag = "planet_other_empty";
  
        public FactionList[] GenericFactions;
    }
}
