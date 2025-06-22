using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace fearcell.Content.Tiles
{
    public class BlackTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            MinPick = 99999;
            AddMapEntry(new Color(0, 0, 0));
        }
    }
}
