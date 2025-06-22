using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;
using fearcell.Content.Dusts;
using static Terraria.ModLoader.ModContent;
using Microsoft.Xna.Framework;
using static Terraria.ID.ContentSamples.CreativeHelper;

namespace fearcell.Content.Tiles.Lab
{
    public class LabPipeTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileBlockLight[Type] = false;
            Main.tileLighted[Type] = false;
            HitSound = SoundID.Tink;
            MineResist = 13f;
            MinPick = 50;
            DustType = DustType<Spark>();
            AddMapEntry(new Color(53, 73, 73));
        }
    }
}
