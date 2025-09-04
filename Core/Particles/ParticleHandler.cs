﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ModLoader;

namespace fearcell.Core.Particles
{
    public static class ParticleHandler
    {
        private static readonly int MaxParticlesAllowed = 500;

        private static ScreenParticle[] particles;
        private static int nextVacantIndex;
        private static int activeParticles;
        private static Dictionary<Type, int> particleTypes;
        private static Dictionary<int, Texture2D> particleTextures;
        private static List<ScreenParticle> particleInstances;
        private static List<ScreenParticle> batchedAlphaBlendParticles;
        private static List<ScreenParticle> batchedAdditiveBlendParticles;

        internal static void RegisterParticles()
        {
            particles = new ScreenParticle[MaxParticlesAllowed];
            particleTypes = new Dictionary<Type, int>();
            particleTextures = new Dictionary<int, Texture2D>();
            particleInstances = new List<ScreenParticle>();
            batchedAlphaBlendParticles = new List<ScreenParticle>(MaxParticlesAllowed);
            batchedAdditiveBlendParticles = new List<ScreenParticle>(MaxParticlesAllowed);

            Type baseParticleType = typeof(ScreenParticle);
            fearcell fearcell = ModContent.GetInstance<fearcell>();

            foreach (Type type in fearcell.Code.GetTypes())
            {
                if (type.IsSubclassOf(baseParticleType) && !type.IsAbstract && type != baseParticleType)
                {
                    int assignedType = particleTypes.Count;
                    particleTypes[type] = assignedType;

                    string texturePath = type.Namespace.Replace('.', '/') + "/" + type.Name;
                    particleTextures[assignedType] = ModContent.Request<Texture2D>(texturePath, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

                    particleInstances.Add((ScreenParticle)FormatterServices.GetUninitializedObject(type));
                }
            }
        }

        internal static void Unload()
        {
            particles = null;
            particleTypes = null;
            particleTextures = null;
            particleInstances = null;
            batchedAlphaBlendParticles = null;
            batchedAdditiveBlendParticles = null;
        }

        /// <summary>
        /// Spawns the particle instance provided into the world (if the particle limit is not reached).
        /// </summary>
        public static void SpawnParticle(ScreenParticle particle)
        {
            if (Main.netMode == Terraria.ID.NetmodeID.Server || activeParticles == MaxParticlesAllowed)
                return;

            particles[nextVacantIndex] = particle;
            particle.ID = nextVacantIndex;
            particle.Type = particleTypes[particle.GetType()];

            if (nextVacantIndex + 1 < particles.Length && particles[nextVacantIndex + 1] == null)
                nextVacantIndex++;
            else
                for (int i = 0; i < particles.Length; i++)
                    if (particles[i] == null)
                        nextVacantIndex = i;

            activeParticles++;
        }

        public static void SpawnParticle(int type, Vector2 position, Vector2 velocity, Vector2 origin = default, float rotation = 0f, float scale = 1f)
        {
            ScreenParticle particle = new ScreenParticle(); // yes i know constructors exist. yes i'm doing this so you dont have to make constructors over and over.
            particle.Position = position;
            particle.Velocity = velocity;
            particle.Color = Color.White;
            particle.Origin = origin;
            particle.Rotation = rotation;
            particle.Scale = scale;
            particle.Type = type;

            SpawnParticle(particle);
        }

        public static void SpawnParticle(int type, Vector2 position, Vector2 velocity)
        {
            ScreenParticle particle = new ScreenParticle();
            particle.Position = position;
            particle.Velocity = velocity;
            particle.Color = Color.White;
            particle.Origin = Vector2.Zero;
            particle.Rotation = 0f;
            particle.Scale = 1f;
            particle.Type = type;

            SpawnParticle(particle);
        }

        /// <summary>
        /// Deletes the particle at the given index. You typically do not have to use this; use Particle.Kill() instead.
        /// </summary>
        public static void DeleteParticleAtIndex(int index)
        {
            particles[index] = null;
            activeParticles--;
            nextVacantIndex = index;
        }

        /// <summary>
        /// Clears all the currently spawned particles.
        /// </summary>
        public static void ClearAllParticles()
        {
            for (int i = 0; i < particles.Length; i++)
                particles[i] = null;

            activeParticles = 0;
            nextVacantIndex = 0;
        }

        internal static void UpdateAllParticles()
        {
            foreach (ScreenParticle particle in particles)
            {
                if (particle == null)
                    continue;

                particle.TimeActive++;
                particle.Position += particle.Velocity;

                particle.Update();
            }
        }

        internal static void RunRandomSpawnAttempts()
        {
            foreach (ScreenParticle particle in particleInstances)
                if (Main.rand.NextFloat() < particle.SpawnChance)
                    particle.OnSpawnAttempt();
        }

        internal static void DrawAllParticles(SpriteBatch spriteBatch)
        {
            foreach (ScreenParticle particle in particles)
            {
                if (particle == null || particle is ForegroundParticle && ModContent.GetInstance<FearcellConfig>().ForegroundParticles == false)
                    continue;

                if (particle.UseAdditiveBlend)
                    batchedAdditiveBlendParticles.Add(particle);
                else
                    batchedAlphaBlendParticles.Add(particle);
            }
            spriteBatch.End();

            if (batchedAlphaBlendParticles.Count > 0)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, null, null, Main.GameViewMatrix.ZoomMatrix);

                foreach (ScreenParticle particle in batchedAlphaBlendParticles)
                    if (particle.UseCustomDraw)
                        particle.CustomDraw(spriteBatch);
                    else
                        spriteBatch.Draw(particleTextures[particle.Type], particle.Position - Main.screenPosition, null, particle.Color, particle.Rotation, particle.Origin, particle.Scale * Main.GameViewMatrix.Zoom, SpriteEffects.None, 0f);

                spriteBatch.End();
            }

            if (batchedAdditiveBlendParticles.Count > 0)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

                foreach (ScreenParticle particle in batchedAdditiveBlendParticles)
                    if (particle.UseCustomDraw)
                        particle.CustomDraw(spriteBatch);
                    else
                        spriteBatch.Draw(particleTextures[particle.Type], particle.Position - Main.screenPosition, null, particle.Color, particle.Rotation, particle.Origin, particle.Scale * Main.GameViewMatrix.Zoom, SpriteEffects.None, 0f);

                spriteBatch.End();
            }

            batchedAlphaBlendParticles.Clear();
            batchedAdditiveBlendParticles.Clear();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        }

        /// <summary>
        /// Gets the texture of the given particle type.
        /// </summary>
        public static Texture2D GetTexture(int type) => particleTextures[type];

        /// <summary>
        /// Returns the numeric type of the given particle.
        /// </summary>
        public static int ParticleType<T>() => particleTypes[typeof(T)];
    }
}
