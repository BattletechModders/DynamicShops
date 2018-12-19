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
    }

    [Serializable]
    public class RepCollection
    {
        public SimGameReputation Reputation;
        public TaggedCollections ItemCollections;
    }

    [Serializable]
    public class TaggedCollections
    {
        public TaggedCollection[] ItemCollections;
        [JsonIgnore]
        Dictionary<string, string[]> CollectionsDictionary;

        [JsonIgnore]
        public string[] this[string tag]
        {
            get
            {
                if (CollectionsDictionary == null || CollectionsDictionary.Count == 0)
                    return null;

                if (CollectionsDictionary.TryGetValue(tag, out var res))
                    return res;

                return null;
            }
        }

        public void Complete()
        {
            CollectionsDictionary = new Dictionary<string, string[]>();
            if (ItemCollections == null || ItemCollections.Length == 0)
                return;

            foreach (var collection in ItemCollections)
            {
                try
                {
                    CollectionsDictionary.Add(collection.Tag, collection.ItemCollections);
                }
                catch
                { }
            }
        }

        public IEnumerable<string> GetItemCollections(StarSystem starSystem)
        {
            foreach (var tag in starSystem.Def.Tags)
            {
                var items = this[tag];
                if (items != null && items.Length > 0)
                    foreach (var item in items)
                        yield return item;
            }
        }
    }

    [Serializable]
    public class FactionInfo
    {
        public Faction Faction;

        public TaggedCollections TaggedShops;
        public RepCollection[] RepShops;
        public TaggedCollections FactionShops;

        public void Complete()
        {
            if (TaggedShops == null)
                TaggedShops = new TaggedCollections();
            TaggedShops.Complete();

            if (RepShops == null)
                RepShops = new RepCollection[0];
            RepShops = RepShops.OrderBy(i => i.Reputation).ToArray();
            foreach (var rep in RepShops)
            {
                if (rep.ItemCollections == null)
                    rep.ItemCollections = new TaggedCollections();
                rep.ItemCollections.Complete();
            }

            if (FactionShops == null)
                FactionShops = new TaggedCollections();
            FactionShops.Complete();
        }
    }

    public class DynamicShopSettings
    {
        public LogLevel LogLevel = LogLevel.Debug;

        public bool ClearDefaultSystemShop = true;
        public bool FactionShopOnEveryPlanet = true;

        public string EmptyShopSystemTag = "planet_other_empty";
        public string BlackMarketSystemTag = "planet_other_blackmarket";

        public FactionInfo[] Factions;
        public TaggedCollections TaggedShops;

        [JsonIgnore]
        public Dictionary<Faction, FactionInfo> FactionInfos;
        [JsonIgnore]
        public Dictionary<string, string[]> TagInfos;

        public void Complete()
        {
            if (TaggedShops == null)
                TaggedShops = new TaggedCollections();
            TaggedShops.Complete();

            if (Factions == null)
                Factions = new FactionInfo[0];

            FactionInfos = new Dictionary<Faction, FactionInfo>();
            for (int i = 0; i < Factions.Length; i++)
            {
                Factions[i].Complete();
                try
                {
                    FactionInfos.Add(Factions[i].Faction, Factions[i]);
                }
                catch
                {

                }
            }
        }
    }
}
