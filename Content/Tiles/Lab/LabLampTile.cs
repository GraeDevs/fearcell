using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using static Terraria.ModLoader.ModContent;
using Microsoft.CodeAnalysis;
using System;
using fearcell.Core;

namespace fearcell.Content.Tiles.Lab
{
    public class HangingLabLampTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(1, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorBottom = default;
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
            DustType = ModContent.DustType<Dusts.Spark>();
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AddMapEntry(new Color(53, 73, 73), name);
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {

        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.9f;
            g = 0.8f;
            b = 0.6f;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            if (tile.TileFrameX != 0 || tile.TileFrameY != 0)
                return;

            Texture2D tex = Request<Texture2D>(Texture + "_Glow").Value;
            Texture2D tex1 = Request<Texture2D>("fearcell/Assets/GlowOrb").Value;
            Texture2D lightConeTexture = Request<Texture2D>("fearcell/Assets/PitGlow").Value; // unused
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);

            spriteBatch.End();
            spriteBatch.Begin(default, BlendState.Additive, SamplerState.PointClamp, default, default);

            if (Main.drawToScreen)
                zero = Vector2.Zero;

            int height = tile.TileFrameY == 38 ? 18 : 16;

            Vector2 drawPos = new Vector2((i * 16) - (int)Main.screenPosition.X, (j * 16) - (int)Main.screenPosition.Y) + zero;
            Vector2 conePosition = new Vector2((i * 16) - (int)Main.screenPosition.X, (j * 16) - (int)Main.screenPosition.Y) + zero; // unused

            drawPos += new Vector2(0, 22);
            conePosition += new Vector2(0, 10); // unused

            Main.spriteBatch.Draw(tex, new Vector2((i * 16) - (int)Main.screenPosition.X, (j * 16) - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            for (int k = 0; k < 2; k++)
            {
                spriteBatch.Draw(tex1, drawPos, null, new Color(255, 173, 76) * (0.35f + (float)Math.Sin(FearcellSystem.rottime) * 0.06f), 0, tex.Size() / 2, k * 1.2f, 0, 0);    
            }

            spriteBatch.End();
            spriteBatch.Begin(default, default, SamplerState.PointClamp, default, default);
        }
    }

    public class LabWallLampTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorWall = true;
            TileObjectData.addTile(Type);
            MinPick = 50;
            MineResist = 5f;
            LocalizedText name = CreateMapEntryName();
            DustType = ModContent.DustType<Dusts.Spark>();
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AddMapEntry(new Color(211, 199, 67), name);
        }
        public override bool CanExplode(int i, int j) => false;

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.9f;
            g = 0.8f;
            b = 0.6f;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            if (tile.TileFrameX != 0 || tile.TileFrameY != 0)
                return;

            Texture2D tex = Request<Texture2D>(Texture + "_Glow").Value;
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            Texture2D tex1 = Request<Texture2D>("fearcell/Assets/GlowOrb").Value;

            spriteBatch.End();
            spriteBatch.Begin(default, BlendState.Additive, SamplerState.PointClamp, default, default);

            if (Main.drawToScreen)
                zero = Vector2.Zero;

            int height = tile.TileFrameY == 38 ? 18 : 16;

            Vector2 drawPos = new Vector2((i * 16) - (int)Main.screenPosition.X, (j * 16) - (int)Main.screenPosition.Y) + zero;

            drawPos += new Vector2(-1, -1);

            Main.spriteBatch.Draw(tex, new Vector2((i * 16) - (int)Main.screenPosition.X, (j * 16) - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            for (int k = 0; k < 2; k++)
                spriteBatch.Draw(tex1, drawPos, null, new Color(255, 173, 76) * (0.35f + (float)Math.Sin(FearcellSystem.rottime) * 0.06f), 0, tex.Size() / 2, k * 1.2f, 0, 0);

            spriteBatch.End();
            spriteBatch.Begin(default, default, SamplerState.PointClamp, default, default);
        }
    }
}
