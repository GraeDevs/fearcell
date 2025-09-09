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
        private const int searchFrequency = 30;
        private const float maxDistance = 200f;
        private const float interestRange = 30f;
        private bool debounce = false;

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

            if(!debounce && hasTarget)
            {
                debounce = true;
                CombatText.NewText(NPC.getRect(), Color.White, "Buzz Buzz!");
            }

            GenerateErraticMovement();

            if (hasTarget)
            {
                MoveTowardsLight();
            }
            else
            {
                RandomMovement();
            }

            UpdateRotation();

        }

        private void SearchForLightSources()
        {
            Vector2 npcCenter = NPC.Center;
            float closestDistance = maxDistance;
            Vector2 closestLight = Vector2.Zero;
            bool foundLight = false;

            // Check for torches and other light-emitting tiles
            int searchRadius = (int)(maxDistance / 16f);
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
                   tileType == ModContent.TileType<HangingLabLampTile>() ||
                   tileType == ModContent.TileType<LabWallLampTile>() ||
                   Main.tileShine[tileType] > 0;
        }

        private void GenerateErraticMovement()
        {
            // Create jittery, erratic movement pattern
            if (timer % 5 == 0)
            {
                offset = new Vector2(
                    Main.rand.NextFloat(-1f, 1f) * 3f,
                    Main.rand.NextFloat(-1f, 1f) * 3f
                );
            }

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
                float baseOrbitSpeed = 0.05f + Main.rand.NextFloat(-0.02f, 0.03f);
                float orbitAngle = timer * baseOrbitSpeed;

                float baseRadius = interestRange * (0.3f + Main.rand.NextFloat(-0.2f, 0.4f));
                float radiusVariation = (float)Math.Sin(timer * 0.15f) * (interestRange * 0.2f);
                float currentRadius = Math.Max(20f, baseRadius + radiusVariation);

                float xDistortion = 0.5f + Main.rand.NextFloat(-0.3f, 0.5f);
                float yDistortion = 0.3f + Main.rand.NextFloat(-0.2f, 0.4f);

                Vector2 orbitOffset = new Vector2(
                    (float)Math.Cos(orbitAngle) * currentRadius * xDistortion,
                    (float)Math.Sin(orbitAngle) * currentRadius * yDistortion
                );

                // Add sudden direction changes and jitters
                if (Main.rand.NextBool(30)) 
                {
                    orbitOffset += new Vector2(
                        Main.rand.NextFloat(-30f, 30f),
                        Main.rand.NextFloat(-30f, 30f)
                    );
                }

                // Add spiral behavior occasionally
                if (Main.rand.NextBool(80)) 
                {
                    float spiralFactor = (float)Math.Sin(timer * 0.3f) * 40f;
                    orbitOffset *= (1f + spiralFactor / currentRadius);
                }

                Vector2 targetPosition = targetSource + orbitOffset + offset;
                Vector2 moveDirection = targetPosition - NPC.Center;

                if (moveDirection.Length() > 0)
                {
                    moveDirection.Normalize();
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

                    float zigzagOffset = (float)Math.Sin(timer * 0.1f) * 2f;
                    Vector2 perpendicular = new Vector2(-direction.Y, direction.X) * zigzagOffset;

                    Vector2 targetVelocity = (direction * 3f) + perpendicular + offset;

                    if (Main.rand.NextBool(60))
                    {
                        targetVelocity *= Main.rand.NextFloat(0.3f, 1.8f);
                    }

                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.05f);
                }
            }

            if (distance > maxDistance * 1.5f)
            {
                hasTarget = false;
            }
        }

        private void RandomMovement()
        {
            // Simple erratic wandering when no light source - no timer dependencies

            // Frequent direction changes
            if (Main.rand.NextBool(40)) // 1/40 chance each frame for direction change
            {
                Vector2 newDirection = new Vector2(
                    Main.rand.NextFloat(-1f, 1f),
                    Main.rand.NextFloat(-1f, 1f)
                );
                newDirection.Normalize();
                NPC.velocity = newDirection * Main.rand.NextFloat(1.2f, 2.5f);
            }

            // Constant small jitters
            if (Main.rand.NextBool(5)) // 1/5 chance each frame for jitter
            {
                Vector2 jitter = new Vector2(
                    Main.rand.NextFloat(-0.8f, 0.8f),
                    Main.rand.NextFloat(-0.8f, 0.8f)
                );
                NPC.velocity += jitter;
            }

            // Sudden direction changes
            if (Main.rand.NextBool(80)) // 1/80 chance for sudden change
            {
                Vector2 suddenChange = new Vector2(
                    Main.rand.NextFloat(-1f, 1f),
                    Main.rand.NextFloat(-1f, 1f)
                );
                suddenChange.Normalize();
                NPC.velocity = suddenChange * Main.rand.NextFloat(1.5f, 3f);
            }

            // Keep speed reasonable
            if (NPC.velocity.Length() > 3.5f)
            {
                NPC.velocity.Normalize();
                NPC.velocity *= 3.5f;
            }

            // Prevent getting stuck
            if (NPC.velocity.Length() < 0.8f)
            {
                Vector2 boost = new Vector2(
                    Main.rand.NextFloat(-1f, 1f),
                    Main.rand.NextFloat(-1f, 1f)
                );
                boost.Normalize();
                NPC.velocity = boost * 1.5f;
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
            timer = Main.rand.NextFloat(0f, 100f);

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
