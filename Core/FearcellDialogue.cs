using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using Terraria.Audio;

namespace fearcell.Core
{
    public class FearcellDialogue : ModPlayer
    {
        public int dialogueMax, dialogueProg;
        public float textPositionHorizontally, textPositionVertically;
        public string dialogue;
        public Color dialogueColor;

        public override void PostUpdate()
        {
            if (dialogueProg > 0)
                dialogueProg--;
            if (dialogueProg == 0)
            {
                dialogue = null;
                dialogueMax = 0;
                dialogueColor = Color.White;
            }
        }
    }

    public static class DialogueHandler
    {
        public static string GetDialogueText()
        {
            var player = Main.LocalPlayer.GetModPlayer<FearcellDialogue>();
            float progress = Utils.GetLerpValue(0, player.dialogueMax, player.dialogueProg);
            float realProg = ((MathHelper.Clamp((1f - progress) * 3, 0, 1)));
            string text = player.dialogue;
            int count = (int)(text.Length * realProg);
            string something = $"{text.Substring(0, count)}";
            return something;
        }
        public static void SetDialogue(int progress, string text, Color color, float textToScreenPos, float textToScreenPosVert)
        {
            FearcellDialogue player = Main.LocalPlayer.GetModPlayer<FearcellDialogue>();
            player.dialogueMax = progress;
            player.dialogueProg = progress;
            player.dialogue = text;
            player.dialogueColor = color;
            player.textPositionHorizontally = textToScreenPos;
            player.textPositionVertically = textToScreenPosVert;
        }
        public static void DrawDialogue()
        {
            FearcellDialogue player = Main.LocalPlayer.GetModPlayer<FearcellDialogue>();
            if (player.dialogueProg > 0)
            {
                float progress = Utils.GetLerpValue(0, player.dialogueMax, player.dialogueProg);
                float alpha = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 3, 0, 1);
                string text = GetDialogueText();
                Main.spriteBatch.Reload(BlendState.AlphaBlend);
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, text, new Vector2(player.textPositionHorizontally, Main.screenHeight * player.textPositionVertically), player.dialogueColor * alpha, 0, new Vector2(0.5f, 0.5f), new Vector2(1f, 1f), Main.screenWidth - 100);
                Main.spriteBatch.Reload(Main.DefaultSamplerState);
            }
        }
    }
}
