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
using fearcell.Core;
using fearcell.Content.NPCs.Hostile.Lab;
using Terraria.ID;

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

        public override void RandomUpdate(int i, int j)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            int spawnX = i * 16 + 8;
            int spawnY = j * 16 + 8;

            if (!Collision.SolidTiles(i - 1, i + 1, j - 1, j + 1))
            {
                int npcIndex = NPC.NewNPC(Entity.GetSource_NaturalSpawn(), spawnX, spawnY, ModContent.NPCType<Dave>());

                if (Main.netMode != NetmodeID.Server)
                {
                    for (int k = 0; k < 15; k++)
                    {
                        Dust.NewDust(new Vector2(spawnX - 8, spawnY - 8), 6, 6, ModContent.DustType<Spark>());
                    }
                }

                // Sync for multiplayer
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npcIndex);
                }
            }
        }
    }
}
