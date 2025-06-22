using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using fearcell.Content.NPCs.Cinematic;
using System;


/*
 * Credits to Macrocosm for a simplified version of the FadeToBlack system I created
 * I used projectiles to handle the activation and deactivation. they didn't.
 * Won't claim this as my own.
 */
namespace fearcell.Core.VFX
{
    public class FadeSystem : ModSystem
    {
        public static bool IsFading => isFading;

        public static float CurrentFade => fadeAlpha / 255f;

        private static int fadeAlpha;
        private static float fadeSpeed;
        private static bool isFading;
        private static bool isFadingIn;
        private static bool interfaceSelfDraw;
        private static bool keepActiveUntilReset;

        public static void Draw()
        {
            if (Main.hasFocus || Main.netMode == NetmodeID.MultiplayerClient)
                UpdateFadeEffect();

            DrawBlack(1f - fadeAlpha / 255f);
        }

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            if (interfaceSelfDraw && (Main.hasFocus || Main.netMode == NetmodeID.MultiplayerClient))
            {
                if (isFading)
                {
                    Draw();
                }
                else
                {
                    if (!keepActiveUntilReset)
                    {
                        interfaceSelfDraw = false;
                        isFading = false;
                    }
                }
            }

            /*
             * Handles Uriel's appearance over the black effect
             */
            foreach (NPC npc in Main.npc)
            {
                if (!npc.active || npc.type != ModContent.NPCType<Uriel>())
                    continue;

                Texture2D texture = TextureAssets.Npc[npc.type].Value;
                Rectangle frame = npc.frame;
                SpriteEffects effects = npc.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                // Bobbing (optional visual override, matches NPC movement)
                float bobOffset = (float)System.Math.Sin(Main.GlobalTimeWrappedHourly * 4f + npc.whoAmI) * 4f;

                float delayBeforeFadeIn = 80f;
                float fadeDuration = 300f;
                float visibleTime = 300f;
                float totalLifetime = delayBeforeFadeIn + fadeDuration * 2f + visibleTime;

                float t = npc.localAI[0];
                float fade;

                if (t < delayBeforeFadeIn)
                {
                    fade = 0f; // invisible during delay
                }
                else if (t < delayBeforeFadeIn + fadeDuration)
                {
                    // Fading in
                    float progress = (t - delayBeforeFadeIn) / fadeDuration;
                    fade = MathHelper.Clamp(progress, 0f, 1f);
                }
                else if (t < delayBeforeFadeIn + fadeDuration + visibleTime)
                {
                    fade = 1f;
                }
                else
                {
                    // Fading out
                    float progress = (t - delayBeforeFadeIn - fadeDuration - visibleTime) / fadeDuration;
                    fade = MathHelper.Clamp(1f - progress, 0f, 1f);
                }

                Vector2 drawPosition = new Vector2(Main.screenWidth, Main.screenHeight) / 2f + new Vector2(0, bobOffset);

                spriteBatch.Draw(
                    texture,
                    drawPosition,
                    frame,
                    Color.White * fade,
                    npc.rotation,
                    frame.Size() / 2f,
                    npc.scale,
                    effects,
                    0f
                );
            }
        }

        public static void DrawBlack(float opacity)
        {
            Main.spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth + 1, Main.screenHeight + 1), Color.Black * opacity);
        }

        public static void ResetFade()
        {
            fadeAlpha = 0;
            isFading = false;
            isFadingIn = false;
            interfaceSelfDraw = false;
            keepActiveUntilReset = false;
        }

        public static void StartFadeIn(float speed = 0.098f, bool selfDraw = false, bool keepActive = false)
        {
            interfaceSelfDraw = selfDraw;
            keepActiveUntilReset = keepActive;
            fadeAlpha = 0;
            fadeSpeed = speed;
            isFadingIn = true;
            isFading = true;
        }

        public static void StartFadeOut(float speed = 0.098f, bool selfDraw = false, bool keepActive = false)
        {
            interfaceSelfDraw = selfDraw;
            keepActiveUntilReset = keepActive;
            fadeAlpha = 255;
            fadeSpeed = speed;
            isFadingIn = false;
            isFading = true;
        }

        private static void UpdateFadeEffect()
        {
            if (!isFading)
                return;

            if (isFadingIn)
            {
                fadeAlpha += (int)(fadeSpeed * 255f);
                if (fadeAlpha >= 255)
                {
                    fadeAlpha = 255;

                    if (!keepActiveUntilReset)
                        isFading = false;
                }
            }
            else
            {
                fadeAlpha -= (int)(fadeSpeed * 255f);
                if (fadeAlpha <= 0)
                {
                    fadeAlpha = 0;

                    if (!keepActiveUntilReset)
                        isFading = false;
                }
            }
        }
    }
}
