using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoMod.Utils;
using fearcell.Content.Dusts;
using fearcell.Content.Items.Tiles.Lab.Walls; 
using Terraria;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using fearcell.Core;

namespace fearcell.Content.Tiles.Walls
{
    class ElevatorShaftWall : ModWall
    {
        public override string Texture => "fearcell/Content/Tiles/Walls/ElevatorShaftWallOutline";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            DustType = DustType<Spark>();
           // AddMapEntry(new Color(106, 106, 106));
            RegisterItemDrop(ItemType<LabPlatingWallItem>());
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Texture2D tex = Request<Texture2D>("fearcell/Content/Tiles/Walls/ElevatorShaftWall").Value;
            var target = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y);
            target += new Vector2(FearcellUtilities.TileAdj.X * 16, FearcellUtilities.TileAdj.Y * 16);
            var source = new Rectangle(i % 14 * 16, j % 25 * 16, 16, 16);

            Tile tile = Framing.GetTileSafely(i, j);

            if (Lighting.NotRetro)
            {
                Lighting.GetCornerColors(i, j, out VertexColors vertices);
                Main.tileBatch.Draw(tex, target, source, vertices, Vector2.Zero, 1f, SpriteEffects.None);
            }

            if (TileID.Sets.DrawsWalls[tile.TileType])
                spriteBatch.Draw(tex, target, source, Lighting.GetColor(i, j));
        }
    }
}
