using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using SubworldLibrary;
using Microsoft.Xna.Framework.Input;

namespace fearcell
{
	public class fearcell : Mod
	{
        public static Mod SubworldLibrary;
        public static fearcell Instance { get; set; }

        public fearcell()
        {
            Instance = this;
        }
    }
}
