using fearcell.Core.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace fearcell.Core
{
    public class FearcellFlags : ModSystem
    {
        /// <summary>
        /// Handles the sequences of nightmares.
        /// Each variable determines if the player has finished a nightmare.
        /// firstNightmare & secondNightmare are both PreHardmode variables.
        /// </summary>
        public static bool firstNightmare = false;
        public static bool secondNightmare = false;
        public static bool labSecurity = false;

        /// <summary>
        /// Cutscene Variables
        /// </summary>
        /// 
        public static bool shawnIntro = false;

        public override void ClearWorld()
        {
            firstNightmare = false;
            secondNightmare = false;
            shawnIntro = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (firstNightmare)
                tag["firstNightmare"] = true;
            if (secondNightmare)
                tag["secondNightmare"] = true;
            if (shawnIntro)
                tag["shawnIntro"] = true;  
        }

        public override void LoadWorldData(TagCompound tag)
        {
            firstNightmare = tag.ContainsKey("firstNightmare");
            secondNightmare = tag.ContainsKey("secondNightmare");
            shawnIntro = tag.ContainsKey("shawnIntro");
        }

        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            flags[0] = firstNightmare;
            flags[1] = shawnIntro;
            flags[2] = secondNightmare; 
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            firstNightmare = flags[0];
            shawnIntro = flags[1];
            secondNightmare = flags[2];
        }
    }
}
