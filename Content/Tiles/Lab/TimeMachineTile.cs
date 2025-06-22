using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using fearcell.Content.Dusts;
using fearcell.Core;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using fearcell.Content.NPCs;

namespace fearcell.Content.Tiles.Lab
{

    public class TimeMachineTile : ModTile
    {


        public override void SetStaticDefaults()
        {
            FearcellUtilities.EasyFurniture(this, 1, 10, 8, DustType<SparkBouncy>(), SoundID.Tink, false, new Color(56, 56, 56), false, false, true);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
        }

        public override void MouseOver(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            if (tile.TileFrameX >= 90)
            {
                Player Player = Main.LocalPlayer;
                Player.cursorItemIconID = ItemType<Items.Misc.LoreQuestion>();
                Player.noThrow = 2;
                Player.cursorItemIconEnabled = true;
            }
        }

        public override bool RightClick(int i, int j)
        {
            var tile = (Tile)Framing.GetTileSafely(i, j).Clone();
            Player player = Main.LocalPlayer;

            int x = i - (tile.TileFrameX - 90) / 18;
            int y = j - tile.TileFrameY / 18;

            SpawnPortal(x, y, player);

            return true;
        }

        public void SpawnPortal(int i, int j, Player player)
        {
            if(Main.netMode == NetmodeID.MultiplayerClient)
            {         //add multiplayer support eventually
                return;
            }

            int portal = Terraria.NPC.NewNPC(new EntitySource_SpawnNPC(), i * 16, j * 16 + 112, NPCType<TimePortalNPC>());
            NPC NPC = Main.npc[portal];
        }
    }
}
