using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Graphics.Effects;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using System;
using Terraria.Audio;
using fearcell.Core;
using Terraria;
using static System.Net.Mime.MediaTypeNames;

namespace fearcell.Content.NPCs
{
    public class TimePortalNPC : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.friendly = true;
            NPC.width = 52;
            NPC.height = 90;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.aiStyle = -1;
            NPC.lifeMax = 2;
            NPC.knockBackResist = 0;
            NPC.noTileCollide = true;
            NPC.alpha = 255;
            NPC.npcSlots = 0;
            NPC.hide = true;
            NPC.behindTiles = true;
            NPC.ShowNameOnHover = false;
        }

        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
        }

        public override bool UsesPartyHat() => false;

        public override bool CanChat() => false;


        private float RotTime;
        public override void AI()
        {
            Lighting.AddLight(NPC.Center, new Color(0, 255, 0).ToVector3());

            for (int i = 0; i < 2; i++)
            {
                if (Main.rand.NextBool(200))
                {
                    FearcellUtilities.Bolt(NPC.BottomLeft, NPC.Right + Vector2.One.RotatedByRandom(6.28f) * Main.rand.Next(5, 15), ModContent.DustType<Dusts.GreenSparkBolt>(), 0.4f, 7);
                }
                if (Main.rand.NextBool(150))
                {
                    FearcellUtilities.Bolt(NPC.BottomRight, NPC.Left + Vector2.One.RotatedByRandom(6.28f) * Main.rand.Next(5, 15), ModContent.DustType<Dusts.GreenSparkBolt>(), 0.4f, 2);
                }

                if (Main.rand.NextBool(200))
                {
                    FearcellUtilities.Bolt(NPC.TopLeft, NPC.Right + Vector2.One.RotatedByRandom(6.28f) * Main.rand.Next(5, 15), ModContent.DustType<Dusts.GreenSparkBolt>(), 0.4f, 7);
                }
                if (Main.rand.NextBool(150))
                {
                    FearcellUtilities.Bolt(NPC.TopRight, NPC.Left + Vector2.One.RotatedByRandom(6.28f) * Main.rand.Next(5, 15), ModContent.DustType<Dusts.GreenSparkBolt>(), 0.4f, 2);
                }
            }

            if (Vector2.Distance(Main.screenPosition + new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), NPC.Center) <= Main.screenWidth / 2 + 100)
            {

                RotTime += (float)Math.PI / 120;
                RotTime *= 1.01f;
                if (RotTime >= Math.PI) RotTime = 0;
                float timer = RotTime;
                Filters.Scene.Activate("Shockwave", NPC.Center).GetShader().UseProgress(2f).UseIntensity(400).UseDirection(new Vector2(0.002f + timer * 0.09f, 1 * 0.009f - timer * 0.009f));

                if (RotTime > 0.5 && RotTime < 0.6 && !Main.dedServ)
                {
                    SoundEngine.PlaySound(FearcellSounds.PortalSound, NPC.position);
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D background = ModContent.Request<Texture2D>("fearcell/Assets/Cosmic").Value;
            Texture2D npcTex = ModContent.Request<Texture2D>("fearcell/Content/NPCs/TimePortalNPC").Value;
            Rectangle target = new Rectangle((int)(NPC.position.X - screenPos.X), (int)(NPC.position.Y - screenPos.Y), NPC.width, NPC.height);

            float offX = (screenPos.X + Main.screenWidth / 2 - NPC.Center.X) * -0.14f;
            float offY = (screenPos.Y + Main.screenHeight / 2 - NPC.Center.Y) * -0.14f;

            Rectangle source = new Rectangle((int)(NPC.position.X % background.Width) + (int)offX, (int)(NPC.position.Y % background.Height) + (int)offY, NPC.width, NPC.height);

            spriteBatch.Draw(background, target, source, Color.White);
            spriteBatch.Draw(npcTex, target, npcTex.Frame(), drawColor);
        }
    }
}
