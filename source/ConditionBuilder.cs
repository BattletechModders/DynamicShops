//#undef CCDEBUG

using System;
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

        internal static DCondition CreateAndInit(string name, string value)
        {
            if (types.TryGetValue(name, out Type type))
            {
                try
                {
                    var item = (Activator.CreateInstance(type) as DCondition);
                    item.Init(value);
                    return item;
                    
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
            throw new NotImplementedException();
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
    }
}