using fearcell.Content.Rarities;
using fearcell.Content.Tiles.Lab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace fearcell.Content.Items.Tiles.Lab
{
  

    public class HangingLabLamp : ModItem
    {
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
            Item.createTile = ModContent.TileType<HangingLabLampTile>();
        }
    }

    public class LabWallLamp : ModItem
    {

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
            Item.createTile = ModContent.TileType<LabWallLampTile>();
        }
    }
}
