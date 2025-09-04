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
    public class LabBeamTile_Solid : ModTile
    {

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = false;
            HitSound = SoundID.Tink;
            MineResist = 6f;
            MinPick = 50;
            DustType = DustType<Spark>();
            AddMapEntry(new Color(53, 73, 73));
        }
    }
}
