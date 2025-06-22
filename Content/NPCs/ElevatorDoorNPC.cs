using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using fearcell.Core;

namespace fearcell.Content.NPCs
{
    public class ElevatorDoorNPC : ModNPC
    {
        public Tile Parent;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 16;
            NPC.height = 64;
            NPC.lifeMax = 1000;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.behindTiles = true;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.ShowNameOnHover = false;
        }

        public int timer;
        public int doorAction;
        public override void AI()
        {
            Player player = Main.LocalPlayer;

            bool open = false;
            foreach (Player Player in Main.player.Where(Player => Vector2.Distance(Player.Center, NPC.Center) <= 85))
            {
                open = true;
            }

            foreach (NPC Npc in Main.npc.Where(Npc => Vector2.Distance(Npc.Center, NPC.Center) <= 85 ))
            {
                if (Npc.type == NPCID.BlueSlime)
                    continue;
                if (Npc.type == ModContent.NPCType<ElevatorDoorNPC>())
                    continue;
                if (Npc.type == ModContent.NPCType<LabPA>())
                    continue;
                if (Npc.type == ModContent.NPCType<LabElevator>())
                    continue;
                open = true;
            }

            if (open)
            {
                if (timer < 60)
                {
                    timer++;
                    NPC.position.Y -= 3;
                }
            }
            else
            {
                if (timer > 0)
                {
                    timer--;
                    NPC.position.Y += 3;
                }
            }
        }
    }
}
