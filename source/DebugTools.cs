using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicShops
{
    internal static class DebugTools
    {

        public static string ShowList(string prefix, List<string> list)
        {
            var o = prefix;
            foreach (var item in list)
                o += " " + item;
            return o;
        }
    }
}
