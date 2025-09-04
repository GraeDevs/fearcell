using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using fearcell.Content.Tiles;
using static Terraria.ModLoader.ModContent;
using fearcell.Content.Rarities;
using fearcell.Content.Tiles.Lab;

namespace fearcell.Content.Items.Tiles.Lab
{
    public class SteelBeam : ModItem
    {
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
            Item.rare = RarityType<DeltaRarity>();
            Item.consumable = true;
            Item.value = 2000;
            Item.DefaultToPlaceableTile(TileType<LabBeamTile>(), 0);
        }
    }
}

public class SolidSteelBeam : ModItem
{

    public override string Texture => "fearcell/Content/Items/Tiles/Lab/SteelBeam";
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
        Item.rare = RarityType<DeltaRarity>();
        Item.consumable = true;
        Item.value = 2000;
        Item.DefaultToPlaceableTile(TileType<LabBeamTile_Solid>(), 0);
    }
}
