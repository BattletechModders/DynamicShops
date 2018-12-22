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
    public class FactionList
    {
        public string Name;
        public Faction[] Members;
        public CollectionDefs Items;
        public CollectionDefs FactionShop;


        public void Complete()
        {
            if(Items == null)
                Items = new CollectionDefs();
            Items.Complete();

            if(FactionShop == null)
                FactionShop = new CollectionDefs();
            FactionShop.Complete();
        }

        public bool IsPartOf(Faction faction)
        {
            return Members.Contains(faction);
        }
    }

    [Serializable]
    public class RepCollection
    {
        public SimGameReputation Reputation;
        public CollectionDefs Items;
    }

    [Serializable]
    public class CollectionDefs
    {
        public TaggedCollection[] Tagged;
        public string[] Untagged;

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
            if (Tagged == null || Tagged.Length == 0)
                return;

            foreach (var collection in Tagged)
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
            if (Untagged != null && Untagged.Length > 0)
                for (int i = 0; i < Untagged.Length; i++)
                    yield return Untagged[i];

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

        public CollectionDefs SystemShops;
        public RepCollection[] RepShops;
        public CollectionDefs FactionShops;

        public void Complete()
        {
            if (SystemShops == null)
                SystemShops = new CollectionDefs();
            SystemShops.Complete();

            if (RepShops == null)
                RepShops = new RepCollection[0];
            RepShops = RepShops.OrderBy(i => i.Reputation).ToArray();

            foreach (var rep in RepShops)
            {
                if (rep.Items == null)
                    rep.Items = new CollectionDefs();
                rep.Items.Complete();
            }

            if (FactionShops == null)
                FactionShops = new CollectionDefs();
            FactionShops.Complete();
        }
    }

    public class DynamicShopSettings
    {
        public LogLevel LogLevel = LogLevel.Debug;

        public bool ClearDefaultSystemShop = true;
        public bool ClearDefaultFactionShop = true;
        public bool ClearDefaultPirateShop = true;
        public bool FactionShopOnEveryPlanet = true;
        public bool OverrideFactionShopOwner = true;

        public bool DEBUG_AllowBlackMarket = false;
        public bool DEBUG_AddFactionRep = false;

        public string EmptyShopSystemTag = "planet_other_empty";
        public string BlackMarketSystemTag = "planet_other_blackmarket";
        //public float BlacMarketPrice = 1.5f;
        //public int BlackMarketItemsFromFactionStore = 3;

        public FactionInfo[] Factions;
        public CollectionDefs SystemShops;
        public CollectionDefs BlackMarket;
        public FactionList[] GenericFactions;

        [JsonIgnore]
        public Dictionary<Faction, FactionInfo> FactionInfos;
        [JsonIgnore]
        public Dictionary<string, string[]> TagInfos;


        public IEnumerable<string> GetGenereicFactionSystemShopItems(Faction faction, StarSystem starSystem)
        {
            if(GenericFactions == null || GenericFactions.Length == 0)
                yield break;

            foreach (var genericFaction in GenericFactions)
            {
                if(genericFaction.IsPartOf(faction))
                    foreach (var item in genericFaction.Items.GetItemCollections(starSystem))
                        yield return item;
            }
        }

        public IEnumerable<string> GetGenereicFactionFactionShopItems(Faction faction, StarSystem starSystem)
        {
            if (GenericFactions == null || GenericFactions.Length == 0)
                yield break;

            foreach (var genericFaction in GenericFactions)
            {
                if (genericFaction.IsPartOf(faction))
                    foreach (var item in genericFaction.FactionShop.GetItemCollections(starSystem))
                        yield return item;
            }
        }

        public void Complete()
        {
            if (SystemShops == null)
                SystemShops = new CollectionDefs();
            SystemShops.Complete();

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

            if(BlackMarket == null)
                BlackMarket = new CollectionDefs();
            BlackMarket.Complete();

            if (GenericFactions != null && GenericFactions.Length > 0)
            {
                GenericFactions = GenericFactions.Where(i => i.Members != null && i.Members.Length > 0).ToArray();
                foreach(var item in GenericFactions)
                    item.Complete();
            }
        }
    }
}
