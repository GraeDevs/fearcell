using fearcell.Core;
using fearcell.Core.VFX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace fearcell.Content.NPCs.Cinematic
{
    public class Messenger : ModNPC
    {
        public override string Texture => "fearcell/Assets/GlowOrb";

        public override void SetDefaults()
        {
            NPC.width = 160;
            NPC.height = 358;
            NPC.damage = 0;
            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
            NPC.immortal = true;
            NPC.noGravity = true;
            NPC.friendly = true;
            NPC.lifeMax = 1;
            NPC.aiStyle = -1;
            NPC.hide = false;
            NPC.noTileCollide = true;
            NPC.alpha = 255;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return true;
        }

        public override void OnSpawn(IEntitySource source)
        {
        }


        public int timer;
        private bool hasFlickerStarted = false;
        public override void AI()
        {
            timer++;
            Player player = Main.LocalPlayer;


            if (timer >= 200 && !hasFlickerStarted)
            {
                hasFlickerStarted = true;

                FearcellSystem.StartShake(6f, 10f);
                FlickerSystem.StartGlitchFlicker();

                CameraSystem.ChangeCameraPos(player.Center, 500, 1.65f);
            }

            if(timer == 300)
            {
                DialogueHandler.SetDialogue(200, "You thought the nightmare was over..?", Color.White, 720, 0.65f);
            }
            if(timer > 310 && timer < 370)
            {
                SoundEngine.PlaySound(FearcellSounds.DialogueTick, player.Center);
            }

            if(timer == 580)
            {
                DialogueHandler.SetDialogue(200, "it's just getting started, my child.", Color.White, 730, 0.65f);
            }
            if (timer > 590 && timer < 650)
            {
                SoundEngine.PlaySound(FearcellSounds.DialogueTick, player.Center);
            }


            if (timer == 760)
            {
                FlickerSystem.StopFlicker();
                SoundEngine.StopTrackedSounds();
                NPC.active = false;              
            }
        }
    }
}
