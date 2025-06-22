using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using fearcell.Content.NPCs.Cinematic;
using System;

namespace fearcell.Core.VFX
{
    public class FlickerSystem : ModSystem
    {
        public static bool IsFlickering => isFlickering;
        public static float CurrentFlickerIntensity => flickerIntensity;

        private static bool isFlickering;
        private static float flickerIntensity;
        private static float flickerTimer;
        private static float flickerSpeed;
        private static float flickerFrequency;
        private static float minIntensity;
        private static float maxIntensity;
        private static bool interfaceSelfDraw;
        private static FlickerPattern currentPattern;
        private static int patternTimer;
        private static Random flickerRandom = new Random();

        // Fade in/out variables
        private static bool isFadingIn;
        private static bool isFadingOut;
        private static float fadeAlpha; // 0 = no effect, 1 = full effect
        private static float fadeSpeed;
        private static float targetFlickerIntensity;

        public enum FlickerPattern
        {
            Random,      // Random flickering
            Pulse,       // Smooth pulsing
            Strobe,      // Sharp on/off strobing
            Lightning,   // Lightning-like flashes
            Glitch,      // Glitchy, irregular pattern
            Heartbeat    // Double-pulse like heartbeat
        }

        public static void Draw()
        {
            if (Main.hasFocus || Main.netMode == NetmodeID.MultiplayerClient)
                UpdateFlickerEffect();

            if (isFlickering)
                DrawFlicker();
        }

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            if (interfaceSelfDraw && (Main.hasFocus || Main.netMode == NetmodeID.MultiplayerClient))
            {
                if (isFlickering)
                {
                    Draw();
                }
            }

        }

        public static void DrawFlicker()
        {
            if (targetFlickerIntensity > 0f)
            {
                // Apply fade alpha to the flicker intensity
                float finalIntensity = targetFlickerIntensity * fadeAlpha;
                if (finalIntensity > 0f)
                {
                    Color flickerColor = Color.Black * finalIntensity;
                    Main.spriteBatch.Draw(
                        TextureAssets.BlackTile.Value,
                        new Rectangle(0, 0, Main.screenWidth + 1, Main.screenHeight + 1),
                        flickerColor
                    );
                }
            }
        }

        /// <summary>
        /// Starts the flicker effect with specified parameters
        /// </summary>
        /// <param name="pattern">The flicker pattern to use</param>
        /// <param name="speed">How fast the flicker changes (higher = faster)</param>
        /// <param name="frequency">How often flickers occur (0.1 to 1.0)</param>
        /// <param name="minIntensity">Minimum flicker intensity (0.0 to 1.0)</param>
        /// <param name="maxIntensity">Maximum flicker intensity (0.0 to 1.0)</param>
        /// <param name="selfDraw">Whether to handle drawing internally</param>
        /// <param name="fadeInSpeed">Speed of fade in effect (default 0.02f)</param>
        public static void StartFlicker(
            FlickerPattern pattern = FlickerPattern.Random,
            float speed = 2f,
            float frequency = 0.5f,
            float minIntensity = 0.1f,
            float maxIntensity = 0.6f,
            bool selfDraw = false,
            float fadeInSpeed = 0.02f)
        {
            isFlickering = true;
            interfaceSelfDraw = selfDraw;
            currentPattern = pattern;
            flickerSpeed = speed;
            flickerFrequency = MathHelper.Clamp(frequency, 0.1f, 1f);
            FlickerSystem.minIntensity = MathHelper.Clamp(minIntensity, 0f, 1f);
            FlickerSystem.maxIntensity = MathHelper.Clamp(maxIntensity, 0f, 1f);
            flickerTimer = 0f;
            patternTimer = 0;
            targetFlickerIntensity = 0f;

            // Start fade in
            isFadingIn = true;
            isFadingOut = false;
            fadeAlpha = 0f;
            fadeSpeed = fadeInSpeed;
        }

        /// <summary>
        /// Stops the flicker effect with a fade out
        /// </summary>
        /// <param name="fadeOutSpeed">Speed of fade out effect (default 0.03f)</param>
        public static void StopFlicker(float fadeOutSpeed = 0.03f)
        {
            if (isFlickering && !isFadingOut)
            {
                isFadingOut = true;
                isFadingIn = false;
                fadeSpeed = fadeOutSpeed;
            }
        }

        /// <summary>
        /// Immediately stops the flicker effect without fade
        /// </summary>
        public static void StopFlickerImmediate()
        {
            isFlickering = false;
            interfaceSelfDraw = false;
            targetFlickerIntensity = 0f;
            flickerTimer = 0f;
            patternTimer = 0;
            fadeAlpha = 0f;
            isFadingIn = false;
            isFadingOut = false;
        }

        /// <summary>
        /// Quick flicker presets for easy AI activation
        /// </summary>
        public static void StartSubtleFlicker() => StartFlicker(FlickerPattern.Random, 1f, 0.3f, 0.05f, 0.2f, true, 0.015f);
        public static void StartIntenseFlicker() => StartFlicker(FlickerPattern.Strobe, 3f, 0.8f, 0.3f, 0.8f, true, 0.025f);
        public static void StartLightningFlicker() => StartFlicker(FlickerPattern.Lightning, 4f, 0.6f, 0.0f, 0.9f, true, 0.04f);
        public static void StartGlitchFlicker() => StartFlicker(FlickerPattern.Glitch, 4f, 0.8f, 0.4f, 0.85f, true, 0.02f);
        public static void StartHeartbeatFlicker() => StartFlicker(FlickerPattern.Heartbeat, 1.5f, 0.4f, 0.0f, 0.5f, true, 0.018f);

        private static void UpdateFlickerEffect()
        {
            if (!isFlickering && !isFadingOut)
                return;

            // Handle fade in/out
            if (isFadingIn)
            {
                fadeAlpha += fadeSpeed;
                if (fadeAlpha >= 1f)
                {
                    fadeAlpha = 1f;
                    isFadingIn = false;
                }
            }
            else if (isFadingOut)
            {
                fadeAlpha -= fadeSpeed;
                if (fadeAlpha <= 0f)
                {
                    fadeAlpha = 0f;
                    isFadingOut = false;
                    // Completely stop the effect
                    isFlickering = false;
                    interfaceSelfDraw = false;
                    targetFlickerIntensity = 0f;
                    return;
                }
            }

            // Only update flicker patterns if we're actively flickering (not just fading out)
            if (isFlickering)
            {
                flickerTimer += flickerSpeed * (1f / 60f); // Assuming 60 FPS
                patternTimer++;

                switch (currentPattern)
                {
                    case FlickerPattern.Random:
                        UpdateRandomFlicker();
                        break;
                    case FlickerPattern.Pulse:
                        UpdatePulseFlicker();
                        break;
                    case FlickerPattern.Strobe:
                        UpdateStrobeFlicker();
                        break;
                    case FlickerPattern.Lightning:
                        UpdateLightningFlicker();
                        break;
                    case FlickerPattern.Glitch:
                        UpdateGlitchFlicker();
                        break;
                    case FlickerPattern.Heartbeat:
                        UpdateHeartbeatFlicker();
                        break;
                }
            }
        }

        private static void UpdateRandomFlicker()
        {
            if (flickerRandom.NextDouble() < flickerFrequency * 0.1f)
            {
                targetFlickerIntensity = MathHelper.Lerp(minIntensity, maxIntensity, (float)flickerRandom.NextDouble());
            }
            else
            {
                targetFlickerIntensity = MathHelper.Lerp(targetFlickerIntensity, 0f, 0.1f);
            }
        }

        private static void UpdatePulseFlicker()
        {
            float wave = (float)Math.Sin(flickerTimer * Math.PI) * 0.5f + 0.5f;
            targetFlickerIntensity = MathHelper.Lerp(minIntensity, maxIntensity, wave);
        }

        private static void UpdateStrobeFlicker()
        {
            int strobeInterval = (int)(60f / flickerSpeed);
            targetFlickerIntensity = (patternTimer % strobeInterval) < (strobeInterval * flickerFrequency) ? maxIntensity : minIntensity;
        }

        private static void UpdateLightningFlicker()
        {
            if (flickerRandom.NextDouble() < 0.02f) // 2% chance per frame
            {
                targetFlickerIntensity = maxIntensity;
            }
            else if (flickerRandom.NextDouble() < 0.05f && targetFlickerIntensity > minIntensity)
            {
                targetFlickerIntensity = maxIntensity * 0.3f;
            }
            else
            {
                targetFlickerIntensity = MathHelper.Lerp(targetFlickerIntensity, minIntensity, 0.2f);
            }
        }

        private static void UpdateGlitchFlicker()
        {
            // More pronounced glitch with darker screen effect
            if (flickerRandom.NextDouble() < flickerFrequency * 0.12f) // Increased frequency slightly
            {
                // More dramatic intensity changes
                float targetIntensity = flickerRandom.NextDouble() < 0.8f ? maxIntensity : minIntensity;
                targetFlickerIntensity = MathHelper.Lerp(targetFlickerIntensity, targetIntensity, 0.4f); // Faster transition
            }
            else if (flickerRandom.NextDouble() < 0.08f)
            {
                // More frequent mid-range variations
                float randomTarget = MathHelper.Lerp(minIntensity + 0.3f, maxIntensity, (float)flickerRandom.NextDouble());
                targetFlickerIntensity = MathHelper.Lerp(targetFlickerIntensity, randomTarget, 0.25f);
            }
            else
            {
                // Drift toward a much darker baseline (75% toward max darkness)
                float baselineIntensity = minIntensity + (maxIntensity - minIntensity) * 0.75f;
                targetFlickerIntensity = MathHelper.Lerp(targetFlickerIntensity, baselineIntensity, 0.08f);
            }
        }

        private static void UpdateHeartbeatFlicker()
        {
            float heartbeatCycle = flickerTimer % 2f; // 2-second cycle
            if (heartbeatCycle < 0.1f || (heartbeatCycle > 0.2f && heartbeatCycle < 0.3f))
            {
                // Double pulse
                targetFlickerIntensity = maxIntensity;
            }
            else
            {
                targetFlickerIntensity = MathHelper.Lerp(targetFlickerIntensity, minIntensity, 0.15f);
            }
        }
        public static void ResetFlicker()
        {
            isFlickering = false;
            interfaceSelfDraw = false;
            targetFlickerIntensity = 0f;
            flickerTimer = 0f;
            flickerSpeed = 2f;
            flickerFrequency = 0.5f;
            minIntensity = 0.1f;
            maxIntensity = 0.6f;
            currentPattern = FlickerPattern.Random;
            patternTimer = 0;
            fadeAlpha = 0f;
            isFadingIn = false;
            isFadingOut = false;
            fadeSpeed = 0.02f;
        }
    }
}