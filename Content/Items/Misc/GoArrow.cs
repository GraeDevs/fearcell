using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace fearcell.Content.Items.Misc
{
    public class GoArrow : ModItem
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
