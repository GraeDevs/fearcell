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
using System.Linq;
using static Terraria.ModLoader.ModContent;
using fearcell.Content.Items.Misc;
using fearcell.Content.Items.Tiles.Lab;
using Terraria.Audio;

namespace fearcell.Content.Tiles.Lab
{
    public class VendingMachineTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.Origin = new Point16(1, 3);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(53, 73, 73), name);
            DustType = ModContent.DustType<Spark>();
            MinPick = 1;

        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;

            player.cursorItemIconID = ItemType<VendingMachine>();
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;

            Tile tile = Main.tile[i, j];
            int left = i - (tile.TileFrameX / 18) % 2;
            int top = j - (tile.TileFrameY / 18) % 3;

            // Dispense a random item
            DispenseRandomItem(left, top, player);

            return true;
        }

        private void DispenseRandomItem(int tileX, int tileY, Player player)
        {
            int requiredPrice = Item.buyPrice(0, 0, 5, 0);

            if (!player.CanAfford(requiredPrice))
            {
                SoundEngine.PlaySound(FearcellSounds.Error, player.Center);

                if (Main.netMode != NetmodeID.Server)
                {
                    CombatText.NewText(player.getRect(), Color.White, "Go sell some ass brokie!");
                }
                return;
            }

            player.BuyItem(requiredPrice);

            // Array of possible items to dispense
            int[] vendingItems = new int[]
            {
                ItemID.Ale,
                ItemID.CreamSoda,
            };

            // Corresponding stack sizes for each item
            int[] stackSizes = new int[]
            {
                1,  // Ale
                1,  // CreamSoda
            };

            // Pick a random item
            int randomIndex = Main.rand.Next(vendingItems.Length);
            int itemType = vendingItems[randomIndex];
            int stackSize = stackSizes[randomIndex];

            // Calculate spawn position (in front of the vending machine)
            int spawnX = (tileX + 1) * 16; // Center of the 2-wide tile
            int spawnY = (tileY + 2) * 16; // Bottom of the 3-tall tile

            // Create the item in the world
            int itemIndex = Item.NewItem(new EntitySource_TileInteraction(player, tileX, tileY),
                spawnX, spawnY, 0, 0, itemType, stackSize);

            // Add some visual/sound effects
            if (Main.netMode != NetmodeID.Server)
            {
                SoundEngine.PlaySound(FearcellSounds.VendingMachine, player.Center);

            }
        }
    }
}
