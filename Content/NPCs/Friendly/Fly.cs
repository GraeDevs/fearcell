using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using fearcell.Content.Tiles.Lab;

namespace fearcell.Content.NPCs.Friendly
{
    public class Fly : ModNPC
    {

        private Vector2 targetSource = Vector2.Zero;
        private bool hasTarget = false;
        private float timer = 0f;
        private Vector2 offset = Vector2.Zero;
        private int lightSearchCooldown = 0;
        private const int searchFrequency = 30; // Search for light every 30 ticks
        private const float maxDistance = 200f; // Max distance to detect light
        private const float interestRange = 30f; // How close to stay to light source

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1; // Animation frames
            NPCID.Sets.CountsAsCritter[Type] = true; // Acts like a critter
        }

        public override void SetDefaults()
        {
            NPC.width = 8;
            NPC.height = 8;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 5;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 0f;
            NPC.knockBackResist = 0.8f;
            NPC.aiStyle = -1;
            NPC.noGravity = true; 
            NPC.noTileCollide = false;
        }

        public override void AI()
        {
            timer += 1f;
            lightSearchCooldown--;

            if (lightSearchCooldown <= 0)
            {
                SearchForLightSources();
                lightSearchCooldown = searchFrequency;
            }

            GenerateErraticMovement();

            if (hasTarget)
            {
                MoveTowardsLight();
            }
            else
            {
                // Random movement when no light source
                RandomMovement();
            }

            // Update rotation to face movement direction
            UpdateRotation();

        }

        private void SearchForLightSources()
        {
            Vector2 npcCenter = NPC.Center;
            float closestDistance = maxDistance;
            Vector2 closestLight = Vector2.Zero;
            bool foundLight = false;

            // Check for torches and other light-emitting tiles
            int searchRadius = (int)(maxDistance / 16f); // Convert to tile units
            int npcTileX = (int)(npcCenter.X / 16f);
            int npcTileY = (int)(npcCenter.Y / 16f);

            for (int x = npcTileX - searchRadius; x <= npcTileX + searchRadius; x++)
            {
                for (int y = npcTileY - searchRadius; y <= npcTileY + searchRadius; y++)
                {
                    if (!WorldGen.InWorld(x, y)) continue;

                    Tile tile = Main.tile[x, y];
                    if (tile == null || !tile.HasTile) continue;

                    // Check if tile emits light (torches, candles, etc.)
                    if (IsLightEmittingTile(tile.TileType))
                    {
                        Vector2 tileCenter = new Vector2(x * 16f + 8f, y * 16f + 8f);
                        float distance = Vector2.Distance(npcCenter, tileCenter);

                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestLight = tileCenter;
                            foundLight = true;
                        }
                    }
                }
            }

            // Also check for light pets and other light sources from players
            foreach (Player player in Main.player)
            {
                if (player.active && !player.dead)
                {
                    float distance = Vector2.Distance(npcCenter, player.Center);
                    if (distance < closestDistance && player.HasBuff(BuffID.Shine))
                    {
                        closestDistance = distance;
                        closestLight = player.Center;
                        foundLight = true;
                    }
                }
            }

            hasTarget = foundLight;
            if (foundLight)
            {
                targetSource = closestLight;
            }
        }

        private bool IsLightEmittingTile(int tileType)
        {
            return tileType == TileID.Torches ||
                   tileType == TileID.Candles ||
                   tileType == TileID.Lamps ||
                   tileType == TileID.HangingLanterns ||
                   tileType == TileID.Campfire ||
                   tileType == TileID.Furnaces ||
                   tileType == TileID.Hellforge ||
                   tileType == TileID.LunarOre ||
                   tileType == TileID.Crystals ||
                   tileType == ModContent.TileType<LabLampTile>() ||
                   tileType == ModContent.TileType<LabLampTile2>() ||
                   tileType == ModContent.TileType<SmallLabLampTile>() ||
                   Main.tileShine[tileType] > 0;
        }

        private void GenerateErraticMovement()
        {
            // Create jittery, erratic movement pattern
            if (timer % 5 == 0) // Change direction frequently
            {
                offset = new Vector2(
                    Main.rand.NextFloat(-1f, 1f) * 3f,
                    Main.rand.NextFloat(-1f, 1f) * 3f
                );
            }

            // Add some sine wave motion for more organic feel
            float sineOffset = (float)Math.Sin(timer * 0.2f) * 0.5f;
            offset.Y += sineOffset;
        }

        private void MoveTowardsLight()
        {
            Vector2 direction = targetSource - NPC.Center;
            float distance = direction.Length();

            // If too close to light source, buzz around it erratically
            if (distance < interestRange)
            {
                // Create highly erratic orbit behavior
                float baseOrbitSpeed = 0.05f + Main.rand.NextFloat(-0.02f, 0.03f); // Variable orbit speed
                float orbitAngle = timer * baseOrbitSpeed;

                // Add random orbit radius changes
                float baseRadius = interestRange * (0.3f + Main.rand.NextFloat(-0.2f, 0.4f));
                float radiusVariation = (float)Math.Sin(timer * 0.15f) * (interestRange * 0.2f);
                float currentRadius = Math.Max(20f, baseRadius + radiusVariation);

                // Random orbit shape distortion
                float xDistortion = 0.5f + Main.rand.NextFloat(-0.3f, 0.5f);
                float yDistortion = 0.3f + Main.rand.NextFloat(-0.2f, 0.4f);

                Vector2 orbitOffset = new Vector2(
                    (float)Math.Cos(orbitAngle) * currentRadius * xDistortion,
                    (float)Math.Sin(orbitAngle) * currentRadius * yDistortion
                );

                // Add sudden direction changes and jitters
                if (Main.rand.NextBool(30)) // 1/30 chance each frame for sudden movement
                {
                    orbitOffset += new Vector2(
                        Main.rand.NextFloat(-30f, 30f),
                        Main.rand.NextFloat(-30f, 30f)
                    );
                }

                // Add spiral behavior occasionally
                if (Main.rand.NextBool(80)) // Occasional spiral in/out
                {
                    float spiralFactor = (float)Math.Sin(timer * 0.3f) * 40f;
                    orbitOffset *= (1f + spiralFactor / currentRadius);
                }

                Vector2 targetPosition = targetSource + orbitOffset + offset;
                Vector2 moveDirection = targetPosition - NPC.Center;

                if (moveDirection.Length() > 0)
                {
                    moveDirection.Normalize();

                    // Variable movement speed for more erratic behavior
                    float moveSpeed = 1.5f + Main.rand.NextFloat(-0.5f, 1.5f);
                    float acceleration = 0.08f + Main.rand.NextFloat(-0.03f, 0.07f);

                    NPC.velocity = Vector2.Lerp(NPC.velocity, moveDirection * moveSpeed, acceleration);
                }

                // Occasionally "panic" and dart away briefly
                if (Main.rand.NextBool(120)) // Rare panic behavior
                {
                    Vector2 panicDirection = new Vector2(
                        Main.rand.NextFloat(-1f, 1f),
                        Main.rand.NextFloat(-1f, 1f)
                    );
                    panicDirection.Normalize();
                    NPC.velocity += panicDirection * 4f;
                }
            }
            else
            {
                // Move towards light source with erratic approach
                if (distance > 0)
                {
                    direction.Normalize();

                    // Add some zigzag approach behavior
                    float zigzagOffset = (float)Math.Sin(timer * 0.1f) * 2f;
                    Vector2 perpendicular = new Vector2(-direction.Y, direction.X) * zigzagOffset;

                    Vector2 targetVelocity = (direction * 3f) + perpendicular + offset;

                    // Randomly slow down or speed up approach
                    if (Main.rand.NextBool(60))
                    {
                        targetVelocity *= Main.rand.NextFloat(0.3f, 1.8f);
                    }

                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.05f);
                }
            }

            // Lose target if too far away
            if (distance > maxDistance * 1.5f)
            {
                hasTarget = false;
            }
        }

        private void RandomMovement()
        {
            // Active wandering movement when no light source
            if (timer % 120 == 0) // Change direction every 2 seconds
            {
                Vector2 randomDirection = new Vector2(
                    Main.rand.NextFloat(-1f, 1f),
                    Main.rand.NextFloat(-1f, 1f)
                );
                randomDirection.Normalize();

                NPC.velocity = Vector2.Lerp(NPC.velocity, randomDirection * 2f + offset, 0.1f);
            }
            else
            {
                // Continuous gradual movement changes
                NPC.velocity += offset * 0.05f;
                NPC.velocity *= 0.98f; // Slight drag to prevent infinite acceleration

                // Ensure minimum movement
                if (NPC.velocity.Length() < 0.5f)
                {
                    Vector2 nudge = new Vector2(
                        Main.rand.NextFloat(-0.8f, 0.8f),
                        Main.rand.NextFloat(-0.8f, 0.8f)
                    );
                    NPC.velocity += nudge;
                }
            }
        }


        private void UpdateRotation()
        {
            // Face the direction of movement
            if (NPC.velocity.Length() > 0.1f)
            {
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            // Start with random erratic values
            timer = Main.rand.NextFloat(0f, 100f);

            // Give initial random velocity to prevent straight-up movement
            Vector2 initialDirection = new Vector2(
                Main.rand.NextFloat(-1f, 1f),
                Main.rand.NextFloat(-1f, 1f)
            );
            initialDirection.Normalize();
            NPC.velocity = initialDirection * Main.rand.NextFloat(1f, 2f);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneOverworldHeight && !spawnInfo.Player.ZoneBeach)
            {
                return Main.dayTime ? 0.05f : 0.15f;
            }
            return 0f;
        }
    }
}
