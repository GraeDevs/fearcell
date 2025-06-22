using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using System;
using fearcell.Core;

namespace fearcell.Content.Dusts
{
    public class BlueGlow : ModDust 
    {
        public override string Texture => "fearcell/Assets/GlowSoft";
        
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = false;
            dust.frame = new Rectangle(0, 0, 64, 64);
            dust.color.R = 23;
            dust.color.G = 100;
            dust.color.B = 255;
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

            Lighting.AddLight(dust.position, dust.color.ToVector3());

            if (dust.scale < 0.05f)
                dust.active = false;

            return false;
        }
    }

    public class BlueGlowBouncy : ModDust
    {
        public override string Texture => "fearcell/Assets/GlowSoft";

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = false;
            dust.frame = new Rectangle(0, 0, 64, 64);
            dust.color.R = 169;
            dust.color.G = 92;
            dust.color.B = 255;
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
            dust.scale *= 0.95f;
            Lighting.AddLight(dust.position, dust.color.ToVector3());
            if (dust.scale < 0.05f)
            {
                dust.active = false;
            }

            return false;
        }

    }
}
