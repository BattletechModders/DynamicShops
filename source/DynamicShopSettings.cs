using System;
using System.Linq;
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using HBS.Logging;
using Newtonsoft.Json;
using UnityEngine;

namespace DynamicShops
{
    [Serializable]
    public class FactionList
    {
        public string Name;
        public string[] Members;
    }

    public class DynamicShopSettings
    {
        public LogLevel LogLevel = LogLevel.Debug;
        public bool DEBUG_ShowLoad = true;

        public bool ReplaceSystemShop = true;
        public bool ReplaceFactionShop = true;
        public bool ReplaceBlackMarket = true;

        public bool FactionShopOnEveryPlanet = true;
        public bool OverrideFactionShopOwner = true;

        public string BlackMarketSystemTag = "planet_other_blackmarket";
        
  
        public FactionList[] GenericFactions;
    }
}
