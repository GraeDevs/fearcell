using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Enums;
using Terraria.ObjectData;
using Terraria.Localization;
using fearcell.Content.Dusts;
using Microsoft.Xna.Framework.Graphics;
using fearcell.Core;
using fearcell.Content.NPCs;
using System.Linq;
using static Terraria.ModLoader.ModContent;
using Terraria.Audio;

namespace fearcell.Content.Tiles.Lab
{
    public class ReactorTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.Width = 5;
            TileObjectData.newTile.Height = 7;
            TileObjectData.newTile.Origin = new Point16(1, 3);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16, 16, 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(53, 73, 73), name);
            DustType = DustType<SparkBouncy>();
            MinPick = 1;

        }

        private const float maxDistance = 30f * 16f; // 20 tiles in pixels
        private const float maxVolume = 0.9f;
        private const int updateRate = 10; // Update every 30 ticks (0.5 seconds)

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (Main.netMode == NetmodeID.Server || Main.GameUpdateCount % updateRate != 0)
                return;

            Player player = Main.LocalPlayer;
            if (player == null || !player.active)
                return;

            Vector2 pos = new Vector2(2 + i * 16, 2 + j * 16);
            Vector2 playerCenter = player.Center;
            float distance = Vector2.Distance(playerCenter, pos);

            if (distance <= maxDistance)
            {
                // Calculate volume based on distance (closer = louder)
                float volumeMultiplier = 1f - (distance / maxDistance);
                float finalVolume = maxVolume * volumeMultiplier;

                // Ensure minimum volume threshold
                if (finalVolume < 0.09f)
                    return;

                // Play the ambient sound with calculated volume
                SoundEngine.PlaySound(new SoundStyle("fearcell/Sounds/Custom/ReactorHum")
                {
                    Volume = finalVolume,
                    Pitch = 0f,
                    PitchVariance = 0.1f,
                    MaxInstances = 1,
                    IsLooped = true,
                    SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest
                }, pos);
            }


        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            float pulse = (float)(0.8f + 0.1f * System.Math.Sin(Main.GameUpdateCount * 0.04f));

            float baseR = 0; // Red component
            float baseG = 0.6f; // Green component  
            float baseB = 0.6f;

            r = baseR * pulse * 0f; // 0.8f controls max brightness
            g = baseG * pulse * 1f;
            b = baseB * pulse * 1f;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Texture2D tex = Request<Texture2D>(Texture + "_Glow").Value;
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;
            int height = tile.TileFrameY == 70 ? 18 : 16;

            Main.spriteBatch.Draw(tex, new Vector2((i * 16) - (int)Main.screenPosition.X, (j * 16) - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

        }
    }
}
