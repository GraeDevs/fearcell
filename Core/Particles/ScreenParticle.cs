﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace fearcell.Core.Particles
{
    public class ScreenParticle
    {
        public int ID; // you don't have to use this
        public int Type; // you don't have to use this
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Origin;
        public Color Color;
        public float Rotation;
        public float Scale;
        public uint TimeActive;

        public Texture2D Texture => ParticleHandler.GetTexture(Type);

        /// <summary>
        /// Set this to true to disable default particle drawing, thus calling Particle.CustomDraw() instead.
        /// </summary>
        public virtual bool UseCustomDraw => false;

        /// <summary>
        /// Set this to true to make your particle use additive blending instead of alphablend.
        /// </summary>
        public virtual bool UseAdditiveBlend => false;

        /// <summary>
        /// The chance at any given tick that this particle will spawn.
        /// Return 0f if you want the particle to not naturally spawn (if you want to spawn it yourself).
        /// This hook will not run if Particle.ActiveCondition returns false.
        /// </summary>
        public virtual float SpawnChance => 0f;

        /// <summary>
        /// Call this when you want to clear your particle and remove it from the world.
        /// </summary>
        public void Kill() => ParticleHandler.DeleteParticleAtIndex(ID);

        /// <summary>
        /// Called every tick. Update your particle in this method.
        /// Particle velocity is automatically added to the particle position for you, and TimeAlive is incremented.
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// Allows you to do custom drawing for your particle. Only called if Particle.UseCustomDrawing is true.
        /// </summary>
        public virtual void CustomDraw(SpriteBatch spriteBatch) { }

        /// <summary>
        /// Called if the particle tries to naturally spawn. This can only be called if Particle.SpawnChance returns a value greater than 0f and Particle.ActiveCondition is true.
        /// Use this to spawn your particle randomly.
        /// </summary>
        public virtual void OnSpawnAttempt() { }
    }
}
