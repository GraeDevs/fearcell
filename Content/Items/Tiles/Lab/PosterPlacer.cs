using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Enums;
using Terraria.ObjectData;
using Terraria.Localization;
using fearcell.Content.Dusts;
using Microsoft.Xna.Framework.Graphics;
using fearcell.Core;
using fearcell.Content.Rarities;
using fearcell.Content.Tiles.Lab;

namespace fearcell.Content.Items.Tiles.Lab
{

    public class Zone1Placer : ModItem
    {

        public override string Texture => "fearcell/Assets/Question";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 20;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ModContent.RarityType<DeltaRarity>();
            Item.consumable = true;
            Item.value = 2000;
            Item.createTile = ModContent.TileType<LabZone_1>();
        }

    }
    public class PosterPlacer : ModItem
    {

        public override string Texture => "fearcell/Content/Items/Tiles/Lab/WarningSign";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 20;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ModContent.RarityType<DeltaRarity>();
            Item.consumable = true;
            Item.value = 2000;
            Item.createTile = ModContent.TileType<WarningSignTile>();
        }

    }

    public class PosterPlacer1 : ModItem
    {

        public override string Texture => "fearcell/Content/Items/Tiles/Lab/WarningSign1";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 20;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ModContent.RarityType<DeltaRarity>();
            Item.consumable = true;
            Item.value = 2000;
            Item.createTile = ModContent.TileType<WarningSignTile1>();
        }

    }
}
