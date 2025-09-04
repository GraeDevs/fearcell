using CollisionLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace fearcell.Content.NPCs
{
    public class LabElevator : ModNPC
    {
        public Tile Parent;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public CollisionSurface[] colliders = null;

        public override void SetDefaults()
        {
            NPC.width = 80;
            NPC.height = 11;
            NPC.lifeMax = 1000;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.ShowNameOnHover = false;
        }

        public override bool CheckActive() => false;

        public override bool PreAI()
        {
            if (colliders == null || colliders.Length != 1)
            {
                colliders = new CollisionSurface[] {
                    new CollisionSurface(NPC.TopLeft, NPC.TopRight, new int[] { 1, 0, 0, 0 }, true) };
            }
            return true;
        }

        public float standing;
        public int elevatorOn;
        public bool goUp = false;

        public float downSpeed = 1f;
        public float upSpeed = -1f;
        public override void AI()
        {
            //Main.NewText(elevatorOn);
            Rectangle rect = new((int)NPC.position.X, (int)NPC.position.Y - 14, 128, 16);
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (!player.active || player.dead || elevatorOn != 0)
                    continue;

                if (!player.Hitbox.Intersects(rect))
                    continue;

                standing += 2;
                if (standing < 60)
                    continue;

                if (goUp)
                    elevatorOn = 2;
                else
                    elevatorOn = 1;
            }
            switch (elevatorOn)
            {
                case 0:
                    standing--;
                    NPC.ai[0] = 0;
                    NPC.ai[1] = 0;
                    break;
                case 1: // going down
                    if (NPC.ai[0]++ == 50)
                    {
                        //play sound
                    }
                    if (NPC.ai[0] >= 50)
                    {
                        NPC.velocity.Y = downSpeed;

                        if (NPC.ai[0] >= 499)
                        {
                            NPC.velocity.Y = 0;
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                Player player = Main.player[i];
                                if (!player.active || player.dead)
                                    continue;

                                if (player.Hitbox.Intersects(rect))
                                    continue;

                                standing--;
                                if (standing > 0)
                                    continue;
                                elevatorOn = 0;
                                standing = 0;
                                goUp = true;
                            }
                        }
                    }
                    break;
                case 2: // going up
                    if (NPC.ai[1]++ == 50)
                    {
                        //play sound
                    }
                    if (NPC.ai[1] >= 50)
                    {
                        NPC.velocity.Y = upSpeed;

                        if (NPC.ai[1] >= 499)
                        {
                            NPC.velocity.Y = 0;
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                Player player = Main.player[i];
                                if (!player.active || player.dead)
                                    continue;

                                if (player.Hitbox.Intersects(rect))
                                    continue;

                                standing--;
                                if (standing > 0)
                                    continue;
                                elevatorOn = 0;
                                goUp = false;
                            }

                        }
                    }
                    break;
            }

            standing = MathHelper.Clamp(standing, 0, 80);
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
                    collider.PostUpdate();
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>("fearcell/Content/NPCs/LabElevator_Glow").Value;
            Texture2D tex = ModContent.Request<Texture2D>("fearcell/Content/NPCs/LabElevator").Value;
            Vector2 drawOrigin = new(tex.Width / 2, tex.Height / 2);

            Main.EntitySpriteDraw(tex, NPC.Center - new Vector2(0, NPC.velocity.Y * 2) - screenPos, null, NPC.GetAlpha(drawColor), NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(glow, NPC.Center - new Vector2(0, NPC.velocity.Y * 2) - screenPos, null, NPC.GetAlpha(Color.White) * (standing / 60), NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);


            return false;
        }
    }
}
