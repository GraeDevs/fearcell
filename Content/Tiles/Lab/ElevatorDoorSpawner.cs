using fearcell.Content.NPCs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace fearcell.Content.Tiles.Lab
{
    public class ElevatorDoorSpawner : ModTile
    {
        public override string Texture => "fearcell/Assets/Empty";

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = false;
            TileObjectData.newTile.Origin = new Point16(1, 1);
            Main.tileFrameImportant[Type] = false;
            DustType = ModContent.DustType<Dusts.LabDust>();
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Vector2 pos = new Vector2(2 + i * 16, 2 + j * 16);
            if (!Main.npc.Any(NPC => NPC.type == NPCType<ElevatorDoorNPC>() && (NPC.ModNPC as ElevatorDoorNPC).Parent == Main.tile[i, j] && NPC.active))
            {
                int Elevator = NPC.NewNPC(new EntitySource_WorldEvent(), (int)pos.X - 2, (int)pos.Y + 45, NPCType<ElevatorDoorNPC>());
                if (Main.npc[Elevator].ModNPC is ElevatorDoorNPC) (Main.npc[Elevator].ModNPC as ElevatorDoorNPC).Parent = Main.tile[i, j];
            }
        }
    }
}
