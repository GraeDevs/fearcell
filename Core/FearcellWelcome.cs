using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace fearcell.Core
{
    public class FearcellWelcome : ModPlayer
    {
        public bool ranOnce;

        public override void OnEnterWorld()
        {
            if(!ranOnce)
            {
                ranOnce = true;
                Main.NewText("");
            }
        }
    }
}
