using Terraria.ModLoader;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;
using fearcell.Content.Tiles.Walls;

namespace fearcell.Content.Items.Tiles.Lab.Walls
{
    public class LabSheetWallItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 7;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createWall = ModContent.WallType<LabSheetWall>();
        }
    }
}