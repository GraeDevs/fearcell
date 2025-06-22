using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using fearcell.Core;

namespace fearcell.Content.NPCs
{
    public class LabPA : ModNPC
    {
        public Tile Parent;

        public override string Texture => "fearcell/Assets/Empty";

        public override void SetDefaults()
        {
            NPC.width = 16;
            NPC.height = 16;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.lifeMax = 1;
            NPC.dontCountMe = true;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.knockBackResist = 0;
            NPC.behindTiles = true;
        }

        public int timer;

        public override void AI()
        {
            //60 ticks is 1 second
            timer++;
            if(timer > 3600) //every minute
            {
                foreach (Player Player in Main.player.Where(Player => Vector2.Distance(Player.Center, NPC.Center) <= 80))
                {
                  
                }
                timer = 0;
            }
        }
    }
}
