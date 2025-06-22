using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using fearcell.Content.Dusts;
using static Terraria.ModLoader.ModContent;
using Microsoft.Xna.Framework;

namespace fearcell.Content.Tiles.Lab
{
    public class LabBrickTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = false;
            HitSound = SoundID.Tink;
            MineResist = 1f;
            MinPick = 50;
            DustType = DustType<LabBrickDust>();
            AddMapEntry(new Color(252, 3, 252));
        }
    }
}
