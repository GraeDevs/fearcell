using fearcell.Core.VFX;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.UI;

namespace fearcell.Core.UI
{
    public class CommandPromptSystem : ModSystem
    {
        internal UserInterface commandPromptInterface;
        internal CommandPromptUI commandPromptUI;

        public static bool promptActive = false;

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "fearcell: Command Prompt",
                    delegate 
                    {
                        commandPromptInterface?.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public override void PostSetupContent()
        {
            if (!Main.dedServ)
            {
                commandPromptInterface = new UserInterface();
                commandPromptUI = new CommandPromptUI();
                commandPromptUI.Activate();
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            commandPromptInterface?.Update(gameTime);
        }

        public void ShowUI()
        {
            Player player = Main.LocalPlayer;

            commandPromptInterface?.SetState(commandPromptUI);
            commandPromptUI?.ResetUI();
            SoundEngine.PlaySound(FearcellSounds.ComputerStartup, player.Center);
        }

        public void HideUI()
        {
            Player player = Main.LocalPlayer;

            commandPromptInterface?.SetState(null);
            SoundEngine.PlaySound(FearcellSounds.ComputerExit, player.Center);
        }

        public void HideUIOnEnter()
        {
            Player player = Main.LocalPlayer;

            commandPromptInterface?.SetState(null);
        }

        public void ToggleUI()
        {
            if (commandPromptInterface?.CurrentState != null)
            {
                HideUI();
            }
            else
            {
                ShowUI();
            }
        }

        public bool IsUIVisible()
        {
            return commandPromptInterface?.CurrentState != null;
        }
    }
}