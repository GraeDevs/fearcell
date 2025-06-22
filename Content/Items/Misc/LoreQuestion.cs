using Terraria;
using Terraria.ModLoader;

namespace fearcell.Content.Items.Misc
{
    public class LoreQuestion : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.maxStack = 1;
        }
    }
}
