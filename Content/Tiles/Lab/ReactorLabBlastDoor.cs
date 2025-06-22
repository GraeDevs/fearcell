using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ObjectData;
using fearcell.Content.Items.Misc;
using static Terraria.ModLoader.ModContent;
using SubworldLibrary;
using fearcell.Core;
using fearcell.Content.Dusts;

namespace fearcell.Content.Tiles.Lab
{
    public class ReactorLabBlastDoor : ModTile
    {
        public bool debounce = false;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            TileObjectData.newTile.Width = 5;
            TileObjectData.newTile.Height = 5;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(1, 3);
            TileObjectData.newTile.AnchorWall = true;
            TileObjectData.addTile(Type);
            MinPick = 1000;
            MineResist = 13f;
            DustType = ModContent.DustType<LabDust>();
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(53, 73, 73), name);

            AnimationFrameHeight = 90;
        }

        public override bool CanExplode(int i, int j) => false;

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;

            player.noThrow = 2;
            player.cursorItemIconEnabled = true;

            if (!debounce)
            {
                player.cursorItemIconID = ItemType<LoreQuestion>();
            }
            else
            {
                player.cursorItemIconID = ItemType<GoArrow>();
            }
        }

        //check to see if its unlocked, if so run the animation and lock it at 16, if its been unlocked, lock the frame at 16 regardless.
        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
           if(debounce)
           {
                frameCounter++;
                if (frameCounter > 4)
                {
                    frameCounter = 0;
                    frame++;
                    if (frame > 16)
                    {
                        frame = 16;
                        frameCounter--;
                    }
                        
                }
            }
        }

        //check to see if its been unlocked before, if not & if player has the keycard, unlock & if unlocked, the player can enter.
       /* public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            if(player.HasItem(ItemID.CrimtaneBar)) {
                debounce = true;
                SoundEngine.PlaySound(SourceSounds.Blastdoor, player.Center);
            }
            else if (SourceWorldFlags.labReactorDoorUnlocked) {
                debounce = true;
                SoundEngine.PlaySound(SourceSounds.Blastdoor, player.Center);
            }
            
            if(debounce)
            {
                if (SubworldSystem.IsActive<LabSub>())
                {
                    player.GetModPlayer<LocationSaving>().subworldLocation = player.Center;

                    SubworldSystem.Enter<ReactorRoomSub>();

                } //checks to see if the room is active, if so, sets flag as true then leaves
                else if (SubworldSystem.IsActive<ReactorRoomSub>())
                {
                    player.GetModPlayer<LocationSaving>().subSavedFlag = true;
                    SubworldSystem.Enter<LabSub>();
                }
            }
            return true;
        }*/
    }
}
