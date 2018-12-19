using System;
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
    }

    [Serializable]
    public class FactionInfo
    {
        public Faction Faction;
        public string[] ItemCollections;

        public TaggedCollection[] TaggedCollections;

    }

    public class DynamicShopSettings
    {
        public LogLevel LogLevel = LogLevel.Debug;

        public bool ClearDefaultShop = false;
        //public FactionInfo[] FactionCollections;
        public TaggedCollection[] TaggedCollections;

        //[JsonIgnore]
        //public Dictionary<Faction, FactionInfo> FactionInfos;
        [JsonIgnore]
        public Dictionary<string, string[]> TagInfos;

        public void Complete()
        {
            //FactionInfos = new Dictionary<Faction, FactionInfo>();
            //foreach (var collection in FactionCollections)
            //{
            //    try
            //    {
            //        FactionInfos.Add(collection.Faction, collection);
            //    }
            //    catch
            //    { }
            //}

            TagInfos = new Dictionary<string, string[]>();
            if (TaggedCollections != null && TaggedCollections.Length > 0)
                foreach (var collection in TaggedCollections)
                {
                    try
                    {
                        TagInfos.Add(collection.Tag, collection.ItemCollections);
                    }
                    catch
                    { }
                }
        }
    }
}
