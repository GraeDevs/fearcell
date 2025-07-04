﻿using Microsoft.Xna.Framework;
using fearcell.Content.Dusts;
using fearcell.Content.Items.Tiles.Walls;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ID.ContentSamples.CreativeHelper;
using static Terraria.ModLoader.ModContent;

namespace fearcell.Content.Tiles.Walls
{
    public class LabWallTile : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            DustType = DustType<Spark>();
            AddMapEntry(new Color(106, 106, 106));
            RegisterItemDrop(ItemType<LabWall>());
        }

        //public override void KillWall(int i, int j, ref bool fail) => fail = true;

    }
}