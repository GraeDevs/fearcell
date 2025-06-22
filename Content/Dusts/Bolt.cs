using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace fearcell.Content.Dusts
{
    public class BlueBolt : ModDust
    {

        public override string Texture => "fearcell/Content/Dusts/Bolt";

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = false;
            dust.color.R = 23;
            dust.color.G = 100;
            dust.color.B = 255;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) => dust.color;

        public override bool Update(Dust dust)
        {
            Lighting.AddLight(dust.position, new Vector3(0.1f, 0f, 0.5f) * 1.5f * dust.scale);
            dust.rotation += Main.rand.NextFloat(2f);
            dust.color *= 0.92f;
            if (dust.color.G > 80) dust.color.G -= 4;

            dust.scale *= 0.92f;
            if (dust.scale < 0.2f)
                dust.active = false;
            return false;
        }
    }
}