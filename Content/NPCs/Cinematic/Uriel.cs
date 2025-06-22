using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using fearcell.Core;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader;
using Terraria.Audio;
using SubworldLibrary;
using fearcell.Core.VFX;

namespace fearcell.Content.NPCs.Cinematic
{
    public class Uriel : ModNPC
    {
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new FlavorTextBestiaryInfoElement("Something seems off about Uriel.."),
            });
        }
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
            NPC.hide = true;
            NPC.noTileCollide = true;
            NPC.alpha = 255;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return false;
        }

        public int globalTimer;
        public int chatTimer;
        /// <summary>
        /// 
        /// Basically handles all the whole pre-nightmare sequence.
        /// Get fucked if you think im going to give a shit about organization
        /// 
        /// and yes, this will be ENTIRELY if statements for every single cutscene, ill probably put the AI in a seperate file
        /// 
        /// </summary>
        public override void AI()
        {
            chatTimer++;

            Player player = Main.LocalPlayer;

            float bobOffset = (float)System.Math.Sin(NPC.ai[0] * 0.1f) * 0.2f;
            NPC.position.Y += bobOffset * 0.1f;

            float delayBeforeFadeIn = 80f;    // 1 second delay
            float fadeDuration = 300f;
            float visibleTime = 300f;  // how long to remain fully visible
            float totalLifetime = delayBeforeFadeIn + fadeDuration * 2f + visibleTime;


            NPC.localAI[0]++;


            foreach (NPC npc in Main.npc)
            {
                if(!npc.friendly)
                {
                    npc.active = false;
                }
            }

            if (vignetteAlpha < 1)
                vignetteAlpha += 0.05f;

            if (chatTimer == 1)
            {
                FadeSystem.ResetFade();
                FadeSystem.StartFadeOut(0.03f, true, true);
            }

            if (chatTimer == 160)
            {
                SoundEngine.PlaySound(FearcellSounds.UrielLine1, player.Center);
            }
            
            if (chatTimer > 170 && chatTimer < 260)
            {
                SoundEngine.PlaySound(FearcellSounds.DialogueTick, player.Center);
            }

            if (chatTimer == 170)
            {
                if (!FearcellFlags.firstNightmare)
                {
                    DialogueHandler.SetDialogue(250, "At last, I can finally speak to you.", Color.White, 740, 0.6f);

                }
            }
            if(chatTimer == 590)
            {
                SoundEngine.PlaySound(FearcellSounds.UrielLine2, player.Center);
            }

            if (chatTimer > 600 && chatTimer < 718)
            {
                SoundEngine.PlaySound(FearcellSounds.DialogueTick, player.Center);
            }
            if (chatTimer == 600)
            {
                //firstNightmare = firstNightmare completed
                if (FearcellFlags.firstNightmare)
                {
                    DialogueHandler.SetDialogue(250, "We will clear all this confusion soon, my child.", Color.White, 690, 0.6f);

                }
                else
                {
                    DialogueHandler.SetDialogue(350, "Don't be afraid. It's only temporary, my child.", Color.White, 695, 0.6f);
                }
            }

            if (chatTimer == 1100)
            {
                FadeSystem.StartFadeIn(0.02f, true);
                NPC.life = 0;
                NPC.active = false;
            }

        }

        private float vignetteAlpha;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D a = FearcellUtilities.GetAsset("Vignette");
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
            spriteBatch.Draw(a, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * vignetteAlpha);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }

    public class UrielBiome : ModBiome
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/UrielAmbience");

        public override bool IsBiomeActive(Player player)
        {
            foreach (NPC npc in Main.npc)
            {
                if (npc.type == ModContent.NPCType<Uriel>() || npc.type == ModContent.NPCType<Messenger>())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
