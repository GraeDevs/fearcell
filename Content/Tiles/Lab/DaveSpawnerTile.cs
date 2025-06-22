using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.Enums;
using fearcell.Content.Dusts;

namespace fearcell.Content.Tiles.Lab
{
    public class DaveSpawnerTile : ModTile
    {
        public static int maxPick = 999;
        public bool isLore = false;

        public int timer;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(56, 56, 56), name);
            DustType = ModContent.DustType<LabDust>();
            MinPick = 999;

            AnimationFrameHeight = 54;
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frame = 1;
            //if (.labSecurity == true)
            //{
               // frame = 2;
           // }
        }
/*
        public override void NearbyEffects(int i, int j, bool closer)
        {
            Vector2 pos = new Vector2(2 + i * 16, 2 + j * 16);
            string ding = "Ding";
            if (!SourceWorldFlags.labSecurity)
            {
                timer++;
                if (timer >= 1800)
                {
                    timer = 0;
                    SoundEngine.PlaySound(SourceSounds.DaveSpawner, pos);
                    NPC.NewNPC(new EntitySource_WorldEvent(), (int)pos.X + 3, (int)pos.Y + 28, NPCType<Dave>());
                }
            }
        }*/
    }
}
