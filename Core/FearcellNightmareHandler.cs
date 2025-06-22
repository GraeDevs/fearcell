using Terraria.ModLoader;
using Terraria;
using fearcell.Content.NPCs.Cinematic;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.UI;

namespace fearcell.Core
{
    public class FearcellNightmareHandler : ModPlayer
    {
        public bool nightmareActive = false;
        public bool tauntActive = false;
        public int timer;

        public override void PreUpdate()
        {
            //Main.NewText(nightmareActive);
            Player player = Main.LocalPlayer;
            if(Main.dayTime == false)
            {
                if (player.sleeping.isSleeping)
                {
                    if (!nightmareActive)
                    {
                        if (Main.rand.NextBool(100))
                        {
                            ActivateNightmare();
                        }
                    }
                }
            }

            if(Main.dayTime == true && NPC.downedBoss1 == true)
            {
                if(Main.rand.NextBool(100))
                {
                    ActivateTaunt();
                }
            }
        }

        /// <summary>
        /// test if Main.hideUI works
        /// </summary>
        public void ActivateNightmare()
        {
            timer++;
            Player player = Main.LocalPlayer;
            nightmareActive = true;

            NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (int)player.Center.X, (int)player.Center.Y + 15, ModContent.NPCType<Uriel>());
        }

        public void ActivateTaunt()
        {
            if(!tauntActive)
            {
                tauntActive = true;
                Player player = Main.LocalPlayer;
                NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (int)player.Center.X, (int)player.Center.Y + 15, ModContent.NPCType<Messenger>());
            }
        }
    }
}
