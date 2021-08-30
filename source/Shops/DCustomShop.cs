using CustomShops;
using System.Collections.Generic;
using System.Linq;
using c = CustomShops.Control;

namespace DynamicShops.Shops
{
    public abstract class DCustomShop : TaggedShop
    {
        public DCustomShopDescriptor Descriptor { get; private set; }

        public override bool CanUse => UseConditions == null ||
                                       UseConditions.All(i => i.IfApply(c.State.Sim, c.State.CurrentSystem));
        public override bool Exists => ExistsConditions == null ||
                                       ExistsConditions.All(i => i.IfApply(c.State.Sim, c.State.CurrentSystem));

        public override int SortOrder => Descriptor.SortOrder;
        public override bool RefreshOnSystemChange => false;
        public override bool RefreshOnMonthChange => false;
        public override bool RefreshOnOwnerChange => false;

        protected override void UpdateTags()
        {
            List<string> tags = new List<string>();


            foreach (var shop_def in Control.ShopDefs)
            {
                Control.LogDebug(DInfo.Conditions, $"Start to check conditions:");

                var use = true;
                if (shop_def.Conditions != null)
                {
                    foreach (var c in shop_def.Conditions)
                    {

                        Control.LogDebug(DInfo.Conditions, $"- {c.GetType().ToString()}");
                        if (!c.IfApply(CustomShops.Control.State.Sim, CustomShops.Control.State.CurrentSystem))
                        {
                            use = false;
                            break;
                        }
                    }
                }
                else
                {
                    Control.LogDebug(DInfo.Conditions, $"- empty");
                }
                if (use)
                {
                    Control.LogDebug(DInfo.Conditions, DebugTools.ShowList("passed", shop_def.Items));
                    tags.AddRange(shop_def.Items);
                }
            }
            Tags = tags;
        }

        public override string Name => Descriptor.Name;
        public override string TabText => Descriptor.TabText;

        public List<DCondition> UseConditions => Descriptor.UseConditions;
        public List<DCondition> ExistsConditions => Descriptor.ExistConditions;

        public DCustomShop(DCustomShopDescriptor decriptor)
        {
            Descriptor = decriptor;
        }
    }

}