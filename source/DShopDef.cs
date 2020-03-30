using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicShops
{
    public class DShopDef
    {
        public List<DCondition> Conditions;
        public List<string> Items;
    }

    public class DFactionShopDef : DShopDef
    {
        public List<string> Factions;
    }
}
