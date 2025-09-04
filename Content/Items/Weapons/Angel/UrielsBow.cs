using System;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria;
using MonoMod.Utils;
using Microsoft.Xna.Framework;
using fearcell.Core;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.Operations;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using fearcell.Content.Dusts;

namespace fearcell.Content.Items.Weapons.Angel
{
    public class UrielsBow : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(0, 2, 50, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.knockBack = 4;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 13;
            Item.useAnimation = 13;
            Item.useAmmo = AmmoID.Arrow;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<UrielsBowProj>();
            Item.shootSpeed = 9;
            Item.UseSound = SoundID.Item5;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 1; i <= 2; i++)
                Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<UrielsBowProj>(), damage, knockback, player.whoAmI, i);
            return false;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 50f;

            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4f, -2f);
        }
    }

    public class UrielsBowProj : ModProjectile, IDrawPrimitive
    {
        private const float TRAIL_WIDTH = 0.35f;

        private List<Vector2> cache;
        private Trail trail;

        private Color color = new(255, 255, 255);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.Size = new Vector2(10, 10);
            Projectile.timeLeft = 350;
        }

        public override void AI()
        {
            ManageCaches();
            ManageTrail();

            Lighting.AddLight((int)(Projectile.position.X / 16f), (int)(Projectile.position.Y / 16f), 0.396f, 0.396f, 0.396f);
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

            Vector2 currentSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y);
            Projectile.velocity = currentSpeed.RotatedBy(Main.rand.Next(-1, 2) * (Math.PI / 60));

            Vector2 position = Projectile.Center + new Vector2(Main.rand.NextFloat(-1.0f, 1.0f) * 10f, Main.rand.NextFloat(-1.0f, 1.0f) * 10f);
            for (int i = 0; i < 2; i++)
            {
                int num = Dust.NewDust(position, 0, 0, ModContent.DustType<AngelicDust>(), 0, 0, 100, default, 0.4f);
                Main.dust[num].velocity = Vector2.Zero;
                Main.dust[num].noGravity = true;
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }
            return false;
        }

        private void ManageCaches()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();

                for (int i = 0; i < 15; i++)
                {
                    cache.Add(Projectile.Center);
                }
            }

            cache.Add(Projectile.Center);

            while (cache.Count > 15)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail ??= new Trail(Main.instance.GraphicsDevice, 15, new TriangularTip(5), factor => factor * 16 * TRAIL_WIDTH, factor =>
            {
                if (factor.X >= 0.98f)
                    return Color.White * 0;
                return new Color(255, 255, 255) * (factor.X * Projectile.timeLeft);
            });
            trail.Positions = cache.ToArray();

            trail.NextPosition = Projectile.Center + Projectile.velocity;
        }
        public void DrawPrimitives()
        {
            Effect effect = Filters.Scene["DatsuzeiTrail"].GetShader().Shader;

            var world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.02f);
            effect.Parameters["repeats"].SetValue(8f);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("fearcell/Assets/FireTrail").Value);

            trail?.Render(effect);

            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("fearcell/Assets/LightningTrail").Value);

            trail?.Render(effect);

        }
    }
}
