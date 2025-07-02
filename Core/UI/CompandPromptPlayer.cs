using Terraria;
using Terraria.ModLoader;

namespace fearcell.Core.UI
{
    public class CommandPromptPlayer : ModPlayer
    {
        public override void OnEnterWorld()
        {
            ModContent.GetInstance<CommandPromptSystem>().HideUIOnEnter();
        }
    }
}