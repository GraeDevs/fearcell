using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using System;
using fearcell.Core;

namespace fearcell.Content.Dusts
{
    public class Spark : ModDust
    {
        public override string Texture => "fearcell/Assets/GlowSoft";

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = false;
            dust.noLight = false;
            dust.frame = new Rectangle(0, 0, 64, 64);
            dust.color.R = 255;
            dust.color.G = 152;
            dust.color.B = 56;
            dust.shader = new Terraria.Graphics.Shaders.ArmorShaderData(new Ref<Effect>(fearcell.Instance.Assets.Request<Effect>("Effects/GlowingDust", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "GlowingDustPass");
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color;
        }

        public override bool Update(Dust dust)
        {
            if (dust.customData is null)
            {
                dust.position -= Vector2.One * 15 * dust.scale;
                dust.customData = true;
            }

            if (Main.tile[(int)dust.position.X / 16, (int)dust.position.Y / 16].HasTile && Main.tile[(int)dust.position.X / 16, (int)dust.position.Y / 16].BlockType == Terraria.ID.BlockType.Solid && Main.tileSolid[Main.tile[(int)dust.position.X / 16, (int)dust.position.Y / 16].TileType])
            {
                dust.velocity *= -0.5f;
            }


            Vector2 currentCenter = dust.position + Vector2.One.RotatedBy(dust.rotation) * 32 * dust.scale;

            dust.scale *= 0.65f;
            Vector2 nextCenter = dust.position + Vector2.One.RotatedBy(dust.rotation + 0.06f) * 32 * dust.scale;

            dust.rotation += 0.06f;
            dust.position += currentCenter - nextCenter;

            dust.shader.UseColor(dust.color);

            dust.position += dust.velocity;

            if (!dust.noGravity)
                dust.velocity.Y += 0.1f;

            dust.velocity *= 0.99f;
            dust.color *= 0.95f;

           // Lighting.AddLight(dust.position, dust.color.ToVector3());

            if (dust.scale < 0.05f)
                dust.active = false;

            return false;
        }
    }
    public class SparkBouncy : ModDust
    {
        public override string Texture => "fearcell/Assets/GlowSoft";

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = false;
            dust.frame = new Rectangle(0, 0, 64, 64);
            dust.color.R = 255;
            dust.color.G = 152;
            dust.color.B = 56;
            dust.shader = new Terraria.Graphics.Shaders.ArmorShaderData(new Ref<Effect>(fearcell.Instance.Assets.Request<Effect>("Effects/GlowingDust", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "GlowingDustPass");
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color;
        }

        public override bool Update(Dust dust)
        {
            /*if (dust.customData is null)
            {
                dust.position -= Vector2.One * 32 * dust.scale;
                dust.customData = true;
            }*/
            dust.position += dust.velocity;
            dust.velocity.Y += 0.2f;
            if (Main.tile[(int)dust.position.X / 16, (int)dust.position.Y / 16].HasTile && Main.tile[(int)dust.position.X / 16, (int)dust.position.Y / 16].BlockType == Terraria.ID.BlockType.Solid && Main.tileSolid[Main.tile[(int)dust.position.X / 16, (int)dust.position.Y / 16].TileType])
            {
                dust.velocity *= -0.5f;
            }

            dust.shader.UseColor(dust.color);
            dust.rotation = dust.velocity.ToRotation();
            dust.scale *= 0.55f;
            Lighting.AddLight(dust.position, dust.color.ToVector3());
            if (dust.scale < 0.05f)
            {
                dust.active = false;
            }

            return false;
        }

    }

    public class SparkBolt : ModDust
    {
        public override string Texture => "fearcell/Assets/GlowSoft";

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = false;
            dust.frame = new Rectangle(0, 0, 64, 64);
            dust.color.R = 255;
            dust.color.G = 152;
            dust.color.B = 56;
            dust.shader = new Terraria.Graphics.Shaders.ArmorShaderData(new Ref<Effect>(fearcell.Instance.Assets.Request<Effect>("Effects/GlowingDust", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "GlowingDustPass");
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color;
        }

        public override bool Update(Dust dust)
        {
            dust.rotation += Main.rand.NextFloat(2f);
            dust.scale *= 0.92f;
            if (dust.scale < 0.05f)
                dust.active = false;


            dust.shader.UseColor(dust.color);

            Lighting.AddLight(dust.position, dust.color.ToVector3());

            return false;
        }
    }

    public class GreenSparkBolt : ModDust
    {
        public override string Texture => "fearcell/Content/Dusts/Bolt";

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = false;
            dust.color.R = 0;
            dust.color.G = 255;
            dust.color.B = 0;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color;
        }

        public override bool Update(Dust dust)
        {
            Lighting.AddLight(dust.position, new Vector3(0f, 1f, 0f) * 1.5f * dust.scale);
            dust.rotation += Main.rand.NextFloat(2f);
            dust.color *= 0.92f;
            if (dust.color.B > 80) dust.color.B -= 4;

            dust.scale *= 0.92f;
            if (dust.scale < 0.2f)
                dust.active = false;
            return false;
        }
    }

    public class GreenSpark : ModDust
    {
        public override string Texture => "fearcell/Assets/GlowSoft";

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = false;
            dust.noLight = false;
            dust.frame = new Rectangle(0, 0, 64, 64);
            dust.color.R = 0;
            dust.color.G = 255;
            dust.color.B = 0;
            dust.shader = new Terraria.Graphics.Shaders.ArmorShaderData(new Ref<Effect>(fearcell.Instance.Assets.Request<Effect>("Effects/GlowingDust", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "GlowingDustPass");
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color;
        }

        public override bool Update(Dust dust)
        {
            if (dust.customData is null)
            {
                dust.position -= Vector2.One * 32 * dust.scale;
                dust.customData = true;
            }

            if (Main.tile[(int)dust.position.X / 16, (int)dust.position.Y / 16].HasTile && Main.tile[(int)dust.position.X / 16, (int)dust.position.Y / 16].BlockType == Terraria.ID.BlockType.Solid && Main.tileSolid[Main.tile[(int)dust.position.X / 16, (int)dust.position.Y / 16].TileType])
            {
                dust.velocity *= -0.5f;
            }


            Vector2 currentCenter = dust.position + Vector2.One.RotatedBy(dust.rotation) * 32 * dust.scale;

            dust.scale *= 0.95f;
            Vector2 nextCenter = dust.position + Vector2.One.RotatedBy(dust.rotation + 0.06f) * 32 * dust.scale;

            dust.rotation += 0.06f;
            dust.position += currentCenter - nextCenter;

            dust.shader.UseColor(dust.color);

            dust.position += dust.velocity;

            if (!dust.noGravity)
                dust.velocity.Y += 0.1f;

            dust.velocity *= 0.99f;
            dust.color *= 0.95f;

            // Lighting.AddLight(dust.position, dust.color.ToVector3());

            if (dust.scale < 0.05f)
                dust.active = false;

            return false;
        }
    }
}
