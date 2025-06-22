using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using fearcell.Content.Dusts;
using fearcell.Content.Rarities;
using fearcell.Content.NPCs;
using Terraria.ObjectData;
using Terraria.Enums;
using Terraria.Localization;

namespace fearcell.Content.Tiles.Lab
{
    public class LabSpeakerTile : ModTile
    {

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorLeft = AnchorData.Empty;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.AnchorRight = AnchorData.Empty;
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);
            DustType = ModContent.DustType<Spark>();
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(81, 77, 71));
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Vector2 pos = new Vector2(4 + i * 16, 4 + j * 16);
            if (!Main.npc.Any(NPC => NPC.type == NPCType<LabPA>() && (NPC.ModNPC as LabPA).Parent == Main.tile[i, j] && NPC.active))
            {
                int PA = NPC.NewNPC(new EntitySource_WorldEvent(), (int)pos.X + 4, (int)pos.Y + 21, NPCType<LabPA>());
                if (Main.npc[PA].ModNPC is LabPA) (Main.npc[PA].ModNPC as LabPA).Parent = Main.tile[i, j];
            }
        }
    }

    public class LabSpeakerItem : ModItem
    {
        public override string Texture => "fearcell/Content/Items/Tiles/Lab/LabSpeakerItem";
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.rare = RarityType<DeltaRarity>();
            Item.consumable = true;
            Item.createTile = TileType<LabSpeakerTile>();
        }
    }
}