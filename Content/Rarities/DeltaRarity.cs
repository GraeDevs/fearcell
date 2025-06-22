using Microsoft.Xna.Framework;
using fearcell.Core;
using Terraria.ModLoader;
using Terraria;

namespace fearcell.Content.Rarities
{
    public class DeltaRarity : ModRarity
    {
        public override Color RarityColor => FearcellUtilities.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.White, Color.Green, Color.DarkGreen);

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            return Type;
        }
    }
}
