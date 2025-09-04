using Microsoft.Xna.Framework;
using fearcell.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ID.ContentSamples.CreativeHelper;
using static Terraria.ModLoader.ModContent;
using fearcell.Content.Items.Tiles.Lab.Walls;

namespace fearcell.Content.Tiles.Walls
{
    public class LabYellowStripWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            DustType = DustType<Spark>();
            AddMapEntry(new Color(106, 101, 30));
            RegisterItemDrop(ItemType<LabYellowStripWallItem>());
        }

        public override void KillWall(int i, int j, ref bool fail) => fail = false;

    }
}