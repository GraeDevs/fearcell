using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.DataStructures;
using fearcell.Content.Dusts;
using fearcell.Core;
using fearcell.Base;
using Microsoft.Xna.Framework.Graphics;

namespace fearcell.Content.NPCs.Hostile.Lab
{
    public class Dave : ModNPC
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Velocity = 1f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 20;
            NPC.height = 40;
            NPC.damage = 14;
            NPC.defense = 3;
            NPC.lifeMax = 60;
            NPC.HitSound = FearcellSounds.DaveHurt;
            NPC.DeathSound = FearcellSounds.DaveDeath;
            NPC.value = 60f;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = NPCID.Wolf;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            new FlavorTextBestiaryInfoElement("Destructive Autonomous Versatile Entity, AKA, Dave");
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 10; i++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustType<Spark>(), -1.5f, 1, default, default, 0.25f);
        }

        public override void AI()
        {
          /*
           *Redo this 
           */
        }
    }
}
