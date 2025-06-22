using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace fearcell.Core
{
    public class FearcellLocationSaving : ModPlayer
    {
        public Vector2 originalLocation;
        public Vector2 subworldLocation;

        public override void PreUpdate()
        {
            //Main.NewText("Position: " + Player.Center + ", Saved: " + originalLocation);
        }

        private float originalLocationX;
        private float originalLocationY;

        private float subworldLocationX;
        private float subworldLocationY;

        public override void SaveData(TagCompound tag)
        {
            tag["originalLocationX"] = originalLocationX;
            tag["originalLocationY"] = originalLocationY;

            tag["subworldLocationX"] = subworldLocationX;
            tag["subworldLocationY"] = subworldLocationY;
        }

        public override void LoadData(TagCompound tag)
        {
            originalLocationX = tag.GetFloat("originalLocationX");
            originalLocationY = tag.GetFloat("originalLocationY");

            subworldLocationX = tag.GetFloat("subworldLocationX");
            subworldLocationY = tag.GetFloat("subworldLocationY");

            originalLocation = new Vector2(originalLocationX, originalLocationY);
            subworldLocation = new Vector2(subworldLocationX, subworldLocationY);
        }

        public bool worldSavedFlag = false;
        public bool subSavedFlag = false;
        public override void OnEnterWorld()
        {
            if (worldSavedFlag && !SubworldSystem.AnyActive<fearcell>())
            {
                Player.Center = originalLocation;
                worldSavedFlag = false;
            }
            //checking if subSavedFlag is true and if any subworld is active, if so, places the player at saved coords.
            if (subSavedFlag && SubworldSystem.AnyActive<fearcell>())
            {
                Player.Center = subworldLocation;
                subSavedFlag = false;
            }
            base.OnEnterWorld();
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {

        }
    }
}
