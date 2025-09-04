using CollisionLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace fearcell.Content.NPCs.Collision
{
    public class Crate : ModNPC
    {

        public CollisionSurface[] colliders = null;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.ActsLikeTownNPC[Type] = true; // Prevents random despawning
            NPCID.Sets.NoMultiplayerSmoothingByType[Type] = true; // Better multiplayer sync
        }

        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 24;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 1;
            NPC.HitSound = SoundID.Dig;
            NPC.DeathSound = SoundID.Dig;
            NPC.value = 0f;
            NPC.knockBackResist = 0f; // We'll handle knockback manually
            NPC.aiStyle = -1; // Custom AI
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.dontCountMe = true; // Don't count toward NPC limit
            NPC.friendly = true;
        }

        public override bool PreAI()
        {
            if (colliders == null || colliders.Length != 4)
            {
                colliders = new CollisionSurface[] {
                    new CollisionSurface(NPC.TopLeft, NPC.TopRight, new int[] { 1, 0, 0, 0 }, true) };
        }
            return true;
        }

        public override void AI()
        {
            // Slow down horizontal movement (friction)
            NPC.velocity.X *= 0.95f;
            if (System.Math.Abs(NPC.velocity.X) < 0.1f)
                NPC.velocity.X = 0f;

            // Handle player collision and pushing
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (!player.active || player.dead)
                    continue;

                Rectangle playerRect = player.getRect();
                Rectangle npcRect = NPC.getRect();

                // Check if player is colliding with crate
                if (playerRect.Intersects(npcRect))
                {
                    // Calculate overlap
                    int overlapX = System.Math.Min(playerRect.Right, npcRect.Right) - System.Math.Max(playerRect.Left, npcRect.Left);
                    int overlapY = System.Math.Min(playerRect.Bottom, npcRect.Bottom) - System.Math.Max(playerRect.Top, npcRect.Top);

                    if (overlapX < overlapY)
                    {
                        // Side collision - push the crate
                        if (player.Center.X < NPC.Center.X)
                        {
                            // Player is to the left, push crate right
                            Main.NewText("push right");
                            NPC.velocity.X = 2f;
                            player.position.X = NPC.position.X - player.width - 1;
                        }
                        else
                        {
                            // Player is to the right, push crate left
                            Main.NewText("push left");
                            NPC.velocity.X = -2f;
                            player.position.X = NPC.position.X + NPC.width + 1;
                        }
                    }
                }
            }

            if (colliders != null && colliders.Length == 1)
            {
                colliders[0].Update();
                colliders[0].endPoints[0] = NPC.Center + (NPC.TopLeft - NPC.Center).RotatedBy(NPC.rotation);
                colliders[0].endPoints[1] = NPC.Center + (NPC.TopRight - NPC.Center).RotatedBy(NPC.rotation);
            }

        }

           public override void PostAI()
        {
            if (colliders != null)
            {
                foreach (CollisionSurface collider in colliders)
                {
                    collider.PostUpdate(); // calls postUpdate for every collision surface
                }
            }
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            return false; // Make it immune to projectiles
        }

        public override bool? CanBeHitByItem(Player player, Item item)
        {
            return false; // Make it immune to items
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            // Apply knockback from projectiles
            Vector2 knockback = projectile.velocity * 0.3f;
            NPC.velocity += knockback;
        }

        public override bool CheckActive()
        {
            return false; // Never despawn
        }

        // Optional: Custom drawing for temporary sprite
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // You can add custom drawing here if needed
            // For now, it will use the default sprite drawing
            return true;
        }
    }
}