using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using fearcell.Content.Items.Misc;
using static Terraria.ModLoader.ModContent;
using SubworldLibrary;
using Microsoft.Xna.Framework.Graphics;
using fearcell.Core;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using Terraria.Audio;
using Microsoft.CodeAnalysis;

namespace fearcell.Content.Tiles.Lab
{
    public class OfficeRoomDoor : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(1, 3);
            TileObjectData.newTile.AnchorWall = true;
            TileObjectData.addTile(Type);
            MinPick = 1000;
            MineResist = 13f;
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(53, 73, 73), name);
        }

        public override bool CanExplode(int i, int j) => false;

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;

            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ItemType<GoArrow>();
        }

        /*public override bool RightClick(int i, int j)
        {
            //checks to see if Lab is active, if so, enter the room and save player coords
            if (SubworldSystem.IsActive<LabSub>())
            {
                Player player = Main.LocalPlayer;
                player.GetModPlayer<FearcellLocationSaving>().subworldLocation = player.Center;

                SubworldSystem.Enter<OfficeRoomSub>();

            } //checks to see if the room is active, if so, sets flag as true then leaves
            else if (SubworldSystem.IsActive<OfficeRoomSub>())
            {
                Player player = Main.LocalPlayer;
                player.GetModPlayer<FearcellLocationSaving>().subSavedFlag = true;
                SubworldSystem.Enter<LabSub>();
            }

            return false;
        }*/
    }
}
