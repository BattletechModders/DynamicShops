# DynamicShops

**v1.1.0.0 and higher require modtek v3 or higher**

**LogLevel in mod.json has been deprecated. Please set logging level using AdvancedJsonMerge, or by adding setting for `CustomShops` to your modpacks HBS debug settings.json.**

Basic idea of DynamicShops is ability to fill shop based on given conditions, that can be changed.

## How game shop work

Vanila shops have preset list of item collection in StarSystemDef. 
For example from starsystemdef_Bringdam.json
```
    "SystemShopItems": [
        "itemCollection_minor_AuriganRestoration",
        "itemCollection_shop_battlefield"
    ],
    "factionShopOwnerID": "INVALID_UNSET",
    "FactionShopItems": null,
    "BlackMarketShopItems": [
        "itemCollection_faction_AuriganPirates"
    ],
```
System Shop have 2 item collections, faction shops is empty an BlackMarket have 1 item collection.
When you enter system shop refreshing it filled from this itemcollections.
DynamicShops replace this list based on System tag/owner/reputation/mrb rating

## Basic Settings(mod.json) with default values

`"DEBUG_ShowLoad" : true` - show loading process of shopdefs

`"ReplaceSystemShop" : true` - use DynamicShops for system shops

`"ReplaceFactionShop" : true` - use DynamicShops for faction shops

`"ReplaceBlackMarket" : true` - use DynamicShops for black market shops

`"OverrideFactionShopOwner" : true` - change faction shop owner(if present) to system owner

`"FactionShopOnEveryPlanet" : true` - make faction shop avaliable on every planet of faction(require OverrideFactionShopOwner : true)

`"GenericFactions" : [ ]` - faction groups

## Generic Faction lists

Generic Factions can be used to group factions to simplify usage of faction conditions
DynamicShops use FactionDef.factionID to define factions

For example:
```
  "GenericFactions" : [
   {
				"Name" : "GreatHouses",
				"Members" : [
					"Davion",
					"Kurita",
					"Liao",
					"Marik",
					"Stainer",
					"ComStar"
				]
			}
		]
```
Will make new group GreatHouses, which can be used anywhere as faction replacement for all 6 of this factions

## ShopDefs

To fill itemcollection lists DynamicShops use ShopDefs - conditional item lists
there are 5 types of them for each type of shop

`"CustomResourceTypes": ["DShopDef", "DFactionShopDef", "DBMShopDef", "DCustomShopDef", "DCustomShopDescriptor"]`

you can reference other mod to DynamicShops and use this types in manifest to load ShopDefs with your mod, for example:

```
"Manifest": [ 
		{
			"Type": "DShopDef",
			"Path": "sshops" 			
		},
		{
			"Type": "DFactionShopDef",
			"Path": "fshops" 			
		},
		{
			"Type": "DBMShopDef",
			"Path": "bmshops" 			
		},
        {
            "Type": "DCustomShopDescriptor",
            "Path": "customshops_descr"
        },
        {
            "Type": "DCustomShopDef",
            "Path": "spares"
        },
        {
            "Type": "DCustomShopDef",
            "Path": "manufacturers"
        }
	]
```
Each file in this folder is simple json that can contain single or list of shopdefs

example of single from bm_general.json
```
{
	"items" : "ItemCollection_BlackMarket"
}
```
example of list from general.json
```
[
	{
		"conditions" : { "tag" : "planet_civ_primitive" },
		"items" : "RT_standard_Miniscule"
	},
	{
		"conditions" : { "tag" : "planet_other_gamesworld" },
		"items" : "RT_List_Solaris"
	},
	...
]
```

## ShopDefs Syntas

Full syntax
```
{
	"factions" : [] - list or single faction, used for factions shop only, ignored in system and bm shops
	"conditions " : { } list of condition to check, if empty or skipped - always true
	"items" : list of single itemcollection to add when conditions met
}
```

### Conditions

each condition consist of pair "type" : "value"
in prebuild conditions value - simple string with coma separated values. DynamicShops check each value and return true if all of them true

Current implemented conditions:

"ctag" : "ctag1,!ctag2" - true if each of ctag1 present and each of ctag2 not present in companytags

"careerLengthCondition": "30, !200" - true if career length is longer than 30 days, and less than 200 days 

"dateCondition": "3062-03-28T00:00:00Z, !3062-05-28T00:00:00Z" - true if current simgame date is *after* 3062-03-28 and *before* 3062-05-28

"tag" : "tag1,!tag2" - true if each of tag1 present and each of tag2 not present in system

"owner" : "faction1,!faction2" - check if system owner is one of faction1 and none of faction2

"rep" : "value1,>value2,<value3,+value4,-value5" - check if reputation to system owner is equal to value1, more then value2, less then value3,
more or equal then value4, less or equal then value5.

values are SimGameReputation(not case sensivity)
```
	public enum SimGameReputation
    {
        LOATHED = -3,
        HATED = -2,
        DISLIKED = -1,
        INDIFFERENT = 0,
        LIKED = 1,
        FRIENDLY = 2,
        HONORED = 3,
        ALLIED = 3
    }
```
"piraterep" : "value1,>value2,<value3,+value4,-value5" - same for pirate reputation

"factionrep" " { "faction" : "shortname", "rep" : "value1,>value2,<value3,+value4,-value5" } same for any other faction

"mrb" : "value1,>value2,<value3,+value4,-value5" - same for mrb rating. values is integer

### Simple Boolean conditions

Basic list of condition act as boolean AND - it return true if all of it members return true

"true" : {} - value ignored, allways return true

"false" : {} - value ignored allways return false

"not" : { list of condtions } - return NOT(conditions) if list empty - return false;

"or" : { list of conditions } - return  true if any of conditins is true;

## Examples
```
	{
		"conditions" : { "tag" : "planet_industry_manufacturing" },
		"items" : "itemCollection_shop_manufacturing"
	}
```
if planet have tag planet_industry_manufacturing add itemCollection_shop_manufacturing to shop
```
	{
		"conditions" : { "tag" : "planet_industry_mining" },
		"items" : ["itemCollection_shop_manufacturing", "itemCollection_shop_mining"]
	}
```
if planet have tag planet_industry_mining add itemCollection_shop_manufacturing and itemCollection_shop_mining to shop
```
	{
		"conditions" : 
			{ 
				"tag" : "planet_pop_small",
				"owner" : "Davion"
			},
		"items" : "RT_List_Davion_Minor"
	}
```
if planet owned by davions and have tag planet_pop_small add RT_List_Davion_Minor
```
	{
		"conditions" : 
			{ 
				"owner" : "GreatHouses, !ComStar"
			},
		"items" : "RT_List_GreatHouses_Minor"
	}
```
if planet owned by any of GreatHouses except ComStar - add RT_List_GreatHouses_Minor
```
	{
		"conditions" : 
			{ 
				"tag" : "planet_pop_medium, !planet_civ_innersphere",
				"owner" : "Davion"
			},
		"items" : "RT_List_Davion_Minor"
	}
```
if planet owned by Davion, have tag planet_pop_medium and dont have tag planet_civ_innersphere add RT_List_Davion_Minor
```
	{
		"conditions" : 
			{ 
				"tag" : "planet_pop_large",
				"owner" : "Davion",
				"rep" : "+FRIENDLY"
			},
		"items" : "RT_List_Davion_Major"
	},
```
if planet owned by Davion, have tag planet_pop_large and you have friendly or more reputation with Davion add RT_List_Davion_Major

## Custom Conditions(for dll modders)

If you want to add condition and dont want to wait while i add it(or this condition based on your mod changes and cannot be added by me) you can add it himself
To do this you need

1. Reference DynamicShops to your mod
2. Implement DCondition abstract class
```
    public abstract class DCondition
    {
        public abstract bool Init(object json);
        public abstract bool IfApply(SimGameState sim, StarSystem CurSystem);
    }
```  
2.1. Init - should fill your condition based on json parameter. return true if parameters correct and condition can be used

it can be 

string/int/bool - simple value as `"mycondition" : 42` for example

IEnumerable<object> - list of values `"mycondition" : [5, 12, 42]`

IDictionary<string, object> - object json value `"mycondition" : { "a" : "test", "b" : 42}`

2.2. IfApply - called when shop check condition. Gives SimGameState and Current Star System (you can get allmost any other info about game from SimGameState)

3. Add DConditionAttribute to this class whith name used to identify condition
```
    [DCondition("mycondition")]
    public class MyCondition : DCondition
    {
	}
```
4. Register your conditions to DynamicShops somewhere during mod initilization
`DynamicShops.Control.RegisterConditions(Assembly.GetExecutingAssembly());`

### Custom Shops

If you want to implement completely custom shop definition (adds additional tab in Shops interface) you can use `DCustomShopDescriptor` to make a general definition of shop and `DCustomShopDef` to define what items to include to this shop

1. Define `DCustomShopDescriptor`

Syntax

```
{
    "Name": "Ammunition_Shop",
    "TabText": "Spares",
    "Icon": "customshops_cbill",
    "SortOrder": 800,
    "ExistConditions":{
        ...
    }
}

```
where `Name` is ID for this shop definition (it should be used in `DCustomShopDef` to link shop with it description), `TabText` is title for shop, `Icon` - icon that should be used for shop, `SortOrder` - number which shows place of this shop in tabs and
`ExistConditions` contains conditions that controls will be this shop shown and when. Conditions has quite same format as given above. Custom conditions created by other mods applied too.

2. Define particular instance of custom shop by defining `DCustomShopDef`

Full syntax
```
{
	"shop" : "name" - name of custom dynamic shop from DCustomShopDescriptor above
	"factions" : [] - list or single faction, used for factions shop only, ignored in system and bm shops
	"conditions " : { } list of condition to check, if empty or skipped - always true
	"items" : list of single itemcollection to add when conditions met
}
```
