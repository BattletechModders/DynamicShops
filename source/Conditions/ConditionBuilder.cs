//#undef CCDEBUG

using System;
using System.Linq;
using System.Collections.Generic;

namespace DynamicShops
{
    public static class ConditionBuilder
    {
        private static Dictionary<string, Type> types = new Dictionary<string, Type>();

        internal static void Register(Type type, DConditionAttribute attr)
        {
            Control.Log($"Condition {type} registred as {attr.Name}");
            types[attr.Name] = type;
        }

        internal static DCondition CreateAndInit(string name, object value)
        {
            if (types.TryGetValue(name, out Type type))
            {
                try
                {
                    var item = (Activator.CreateInstance(type) as DCondition);
                    if(item.Init(value))
                        return item;
                    Control.LogError($"Error during initilization of {name}");
                    return null;
                    
                }
                catch (Exception e)
                {
                    Control.LogError($"Error create instance of {name}", e);
                    return null;
                }
            }
            Control.LogError($"Unknown condition type: {name}");
            return null;
        }

        internal static List<DCondition> FromJson(object con)
        {
            if (con == null)
                return null;
            var dict = con as Dictionary<string, object>;
            if (dict == null)
                return null;

            var result = new List<DCondition>();

            foreach (var pair in dict)
            {
                var condition = CreateAndInit(pair.Key, pair.Value);
                if (condition != null)
                    result.Add(condition);
            }
            
            return result;

        }

        internal static List<string> StringsFromJson(object json)
        {
            List<string> values = new List<string>();
            if (json is string str)
            {
                values.Add(str);
                return values;
            }
            else if(json is IEnumerable<object> items)
            {
                foreach (var item in items)
                    values.Add(item.ToString());
                return values;
            }
            return null;
        }

        internal static IEnumerable<string> ExpandGenericFaction(string faction)
        {
            var generic = Control.Settings.GenericFactions.FirstOrDefault(i => i.Name == faction);
            if (generic == null)
                return Enumerable.Repeat(faction, 1);
            return generic.Members;
        }
    }
}