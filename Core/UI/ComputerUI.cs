using ReLogic.Graphics;
using System.Collections.Generic;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Enums;
using Terraria.ObjectData;
using Terraria.Localization;
using fearcell.Content.Dusts;
using Microsoft.Xna.Framework.Graphics;
using fearcell.Core;

namespace fearcell.Core.UI
{
    public class ComputerUI : SmartUIState
    {
        private readonly ComputerUIInner innerBox = new();
        public UIImage exitButton = new(Request<Texture2D>("fearcell/Assets/UI/ComputerUIClose", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value);

        public override int InsertionIndex(List<GameInterfaceLayer> layers)
        {
            return layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        }

        public override void OnInitialize()
        {
            innerBox.Left.Set(232, 0.5f);
            innerBox.Top.Set(0, 0.5f);
            Append(innerBox);

            exitButton.OnLeftClick += (a, b) => Visible = false;
            AddElement(exitButton, 200, 0.5f, 32, 0.5f, 38, 0f, 38, 0f);
        }

        public void Display(string title, string author, string message)
        {
            innerBox.Title = title;
            innerBox.Author = author;
            innerBox.Message = message;
            Visible = true;
        }
    }

    public class ComputerUIInner : SmartUIElement
    {
        public string Title;
        public string Author;
        public string Message;

        Texture2D backdrop = Request<Texture2D>("fearcell/Assets/UI/ComputerUI", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

        public override void Draw(SpriteBatch spriteBatch)
        {
            DynamicSpriteFont font = Terraria.GameContent.FontAssets.MouseText.Value;
            string message = FearcellUtilities.WrapString(Message, 360, font, 1);
            float height = font.MeasureString(message).Y + 96;

            Height.Set(backdrop.Height, 0);
            Width.Set(backdrop.Width, 0);
            Left.Set(-232, 0.5f);
            Top.Set(height, 0f);

            spriteBatch.Draw(backdrop, GetDimensions().ToRectangle(), Color.White);

            Utils.DrawBorderString(spriteBatch, Title, GetDimensions().ToRectangle().TopLeft() + Vector2.One * 32, Color.LimeGreen);
            Utils.DrawBorderString(spriteBatch, Author, GetDimensions().ToRectangle().TopLeft() + new Vector2(32, 56), Color.LimeGreen, 0.75f);
            Utils.DrawBorderString(spriteBatch, message, GetDimensions().ToRectangle().TopLeft() + new Vector2(32, 78), Color.LimeGreen);

            if (Parent is ComputerUI)
            {
                var parent = Parent as ComputerUI;
                parent.exitButton.Left.Set(156, 0.5f);
                parent.exitButton.Top.Set(height + 290, 0f);
                parent.exitButton.Recalculate();
                parent.Recalculate();
            }

            Recalculate();
        }
    }
}
