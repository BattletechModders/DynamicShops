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
    public class TaggedCollection
    {
        public string Tag;
        public string[] ItemCollections;


        [JsonIgnore] private string[] tags;

        public bool CanApply(StarSystem starSystem)
        {

            if (tags == null || tags.Length == 0)
            {
                return false;
            }
        


        foreach (var tag in tags)
            {
                if (tag.StartsWith("!"))
                {
                    if (starSystem.Tags.Contains(tag.Substring(1)))
                        return false;
                }
                else if (!starSystem.Tags.Contains(tag))
                    return false;

            }

            return true;

        }

        public void Complete()
        {
            if (!string.IsNullOrEmpty(Tag))
                tags = Tag.Split(',').Select(i => i.Trim()).ToArray();
            else
                tags = null;
        }
    }


    [Serializable]
    public class FactionList
    {
        public string Name;
        public string[] Members;
    }

    public class DynamicShopSettings
    {
        public LogLevel LogLevel = LogLevel.Debug;
        public bool Completed { get; private set; } = false;

        public bool ReplaceSystemShop = true;
        public bool ReplaceFactionShop = true;
        public bool ReplaceBlackMarket = true;
        public bool FactionShopOnEveryPlanet = true;
        public bool OverrideFactionShopOwner = true;

        public string BlackMarketSystemTag = "planet_other_blackmarket";
  
        public FactionList[] GenericFactions;
    }
}
