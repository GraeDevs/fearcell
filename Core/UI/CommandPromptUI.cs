using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace fearcell.Core.UI
{
    /*
     * 
     * Note to self:
     * UI sucks, dont do it. I'd rather eat sawdust and popeyes biscuits with no water.
     * 
     */

    public class CommandPromptUI : UIState
    {
        private UIImage backgroundPanel;
        private UIImageButton powerButton;
        private UIPanel commandHistoryPanel;
        private UIPanel inputPanel;
        private UIText inputText;

        private CommandProcessor commandProcessor;
        private List<string> commandHistory;
        private string currentInput;
        private bool isActive;
        private float timeSinceLastBlink;
        private bool showCursor;
        private bool justActivated;
        private float scrollPosition;

        private const int UI_WIDTH = 784;
        private const int UI_HEIGHT = 520;
        private const int BUTTON_SIZE = 30;

        private const int SCREEN_LEFT = 155;
        private const int SCREEN_TOP = 100;
        private const int SCREEN_WIDTH = 440;
        private const int SCREEN_HEIGHT = 250;


        public override void OnInitialize()
        {
            commandProcessor = new CommandProcessor();
            commandHistory = new List<string>();
            currentInput = "";
            scrollPosition = 0f;

            var backdropTexture = ModContent.Request<Texture2D>("fearcell/Assets/UI/ComputerDesktop", ReLogic.Content.AssetRequestMode.ImmediateLoad);
            backgroundPanel = new UIImage(backdropTexture);
            backgroundPanel.Width.Set(UI_WIDTH, 0f);
            backgroundPanel.Height.Set(UI_HEIGHT, 0f);
            backgroundPanel.HAlign = 0.5f;
            backgroundPanel.VAlign = 0.5f;
            Append(backgroundPanel);

            powerButton = new UIImageButton(ModContent.Request<Texture2D>("fearcell/Assets/UI/ComputerUIExit", ReLogic.Content.AssetRequestMode.ImmediateLoad));
            powerButton.Width.Set(BUTTON_SIZE, 0f);
            powerButton.Height.Set(BUTTON_SIZE, 0f);
            powerButton.Top.Set(132, 0f);
            powerButton.Left.Set(658, 0f);
            powerButton.OnLeftClick += PowerOffUI;
            backgroundPanel.Append(powerButton);

            commandHistoryPanel = new UIPanel();
            commandHistoryPanel.Width.Set(SCREEN_WIDTH - 20, 0f);
            commandHistoryPanel.Height.Set(SCREEN_HEIGHT - 60, 0f);
            commandHistoryPanel.Top.Set(SCREEN_TOP - 5, 0f);
            commandHistoryPanel.Left.Set(SCREEN_LEFT + 10, 0f);
            commandHistoryPanel.BackgroundColor = Color.Transparent;
            commandHistoryPanel.BorderColor = Color.Transparent;
            backgroundPanel.Append(commandHistoryPanel);

            inputPanel = new UIPanel();
            inputPanel.Width.Set(SCREEN_WIDTH - 20, 0f);
            inputPanel.Height.Set(25, 0f);
            inputPanel.Top.Set(SCREEN_TOP + SCREEN_HEIGHT - 80, 0f);
            inputPanel.Left.Set(SCREEN_LEFT + 10, 0f);
            inputPanel.BackgroundColor = Color.Black * 0.7f;
            inputPanel.BorderColor = Color.Green;
            inputPanel.OnLeftClick += OnInputPanelClick;
            backgroundPanel.Append(inputPanel);

            inputText = new UIText("", 0.8f);
            inputText.Top.Set(3, 0f);
            inputText.Left.Set(5, 0f);
            inputText.TextColor = Color.Transparent;
            inputPanel.Append(inputText);

            ShowWelcomeMessage();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            timeSinceLastBlink += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timeSinceLastBlink >= 0.5f)
            {
                showCursor = !showCursor;
                timeSinceLastBlink = 0f;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            DrawCommandHistory(spriteBatch);

            DrawInputText(spriteBatch);

            if (isActive)
            {
                HandleKeyboardInputInDraw();
                Main.blockInput = true;
            }
        }
        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            base.ScrollWheel(evt);

            if (commandHistoryPanel.ContainsPoint(evt.MousePosition))
            {
                float scrollAmount = evt.ScrollWheelValue / 120f * 40f; // 40 pixels per scroll
                scrollPosition -= scrollAmount;

                float totalHeight = commandHistory.Count * 18f;
                float viewHeight = commandHistoryPanel.Height.Pixels;
                float maxScroll = Math.Max(0, totalHeight - viewHeight);

                scrollPosition = MathHelper.Clamp(scrollPosition, 0, maxScroll);
            }
        }

        private void DrawCommandHistory(SpriteBatch spriteBatch)
        {
            if (commandHistory.Count == 0) return;

            Vector2 screenPos = new Vector2(Main.screenWidth * 0.5f - UI_WIDTH * 0.5f, Main.screenHeight * 0.5f - UI_HEIGHT * 0.5f);
            float panelX = SCREEN_LEFT + 15 + screenPos.X;
            float panelY = SCREEN_TOP + screenPos.Y;
            float panelWidth = SCREEN_WIDTH - 30;
            float panelHeight = SCREEN_HEIGHT - 120;

            Rectangle clipRect = new Rectangle((int)panelX, (int)panelY, (int)panelWidth, (int)panelHeight);
            Rectangle originalRect = spriteBatch.GraphicsDevice.ScissorRectangle;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
                DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
            spriteBatch.GraphicsDevice.ScissorRectangle = clipRect;

            float yOffset = panelY - scrollPosition;
            float lineHeight = 18f;

            for (int i = 0; i < commandHistory.Count; i++)
            {
                string line = commandHistory[i];
                Vector2 textPos = new Vector2(panelX, yOffset + (i * lineHeight));

                // Only draw if within visible area
                if (textPos.Y >= panelY - lineHeight && textPos.Y <= panelY + panelHeight)
                {
                    Utils.DrawBorderString(spriteBatch, line, textPos, Color.Green, 0.7f);
                }
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
                DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
            spriteBatch.GraphicsDevice.ScissorRectangle = originalRect;
        }

        private void DrawInputText(SpriteBatch spriteBatch)
        {
            Vector2 screenPos = new Vector2(Main.screenWidth * 0.5f - UI_WIDTH * 0.5f, Main.screenHeight * 0.5f - UI_HEIGHT * 0.5f);
            float inputX = SCREEN_LEFT + 15 + screenPos.X;
            float inputY = SCREEN_TOP + SCREEN_HEIGHT - 77 + screenPos.Y;

            string displayText = "C:\\> " + currentInput;
            if (isActive && showCursor)
            {
                displayText += "|";
            }

            Color textColor = isActive ? Color.LimeGreen : Color.Green;
            Utils.DrawBorderString(spriteBatch, displayText, new Vector2(inputX, inputY), textColor, 0.8f);
        }

        private void HandleKeyboardInputInDraw()
        {
            // This is the hack from DragonLens - handle input in Draw to avoid chat
            if (justActivated)
            {
                justActivated = false;
                return;
            }

            PlayerInput.WritingText = true;
            Main.instance.HandleIME();

            string inputText = Main.GetInputText(currentInput);

            if (inputText != currentInput)
            {
                currentInput = inputText;
            }

            if (Main.inputText.IsKeyDown(Keys.RightShift) && !Main.oldInputText.IsKeyDown(Keys.RightShift))
            {
                ExecuteCommand(currentInput.Trim());
                currentInput = "";
                Main.clrInput();
            }
        }

        private void ExecuteCommand(string command)
        {
            if (string.IsNullOrEmpty(command)) return;

            // Special handling for clear command
            if (command.ToLower() == "clear")
            {
                ShowWelcomeMessage();
                return;
            }

            commandHistory.Add("C:\\> " + command);

            var result = commandProcessor.ProcessCommand(command);

            foreach (string line in result)
            {
                commandHistory.Add(line);
            }

            commandHistory.Add("");

            ScrollToBottom();
        }

        private void ShowWelcomeMessage()
        {
            commandHistory.Clear();
            commandHistory.Add("Click input area below and start typing");
            commandHistory.Add("Press Right-Shift to execute commands");
            commandHistory.Add("");
            commandHistory.Add("To exit the interface, click the power button on the right.");
            commandHistory.Add("");
            commandHistory.Add("Type 'help' for available commands");
            commandHistory.Add("");
            scrollPosition = 0f;
        }

        private void ScrollToBottom()
        {
            float totalHeight = commandHistory.Count * 18f;
            float viewHeight = SCREEN_HEIGHT - 70;

            if (totalHeight > viewHeight)
            {
                scrollPosition = totalHeight - viewHeight;
            }
            else
            {
                scrollPosition = 0f;
            }
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            if (!inputPanel.ContainsPoint(evt.MousePosition))
            {
                isActive = false;
                PlayerInput.WritingText = false;
            }
        }

        private void OnInputPanelClick(UIMouseEvent evt, UIElement listeningElement)
        {
            isActive = true;
            justActivated = true;
            Main.clrInput();
            PlayerInput.WritingText = true;
        }

        private void CloseUI(UIMouseEvent evt, UIElement listeningElement)
        {
            CloseTerminal();
        }

        private void PowerOffUI(UIMouseEvent evt, UIElement listeningElement)
        {
            CloseTerminal();
        }

        private void CloseTerminal()
        {
            isActive = false;
            Main.blockInput = false;
            PlayerInput.WritingText = false;
            ModContent.GetInstance<CommandPromptSystem>().HideUI();
        }

        public void ResetUI()
        {
            currentInput = "";
            isActive = false;
            justActivated = false;
            showCursor = false;
            Main.blockInput = false;
            timeSinceLastBlink = 0f;
            PlayerInput.WritingText = false;
        }

        public bool IsTyping()
        {
            return isActive;
        }
    }
}