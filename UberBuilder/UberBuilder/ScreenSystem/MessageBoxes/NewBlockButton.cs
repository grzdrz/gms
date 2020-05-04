/* Original source Farseer Physics Engine:
 * Copyright (c) 2014 Ian Qvist, http://farseerphysics.codeplex.com
 * Microsoft Permissive License (Ms-PL) v1.1
 */

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace tainicom.Aether.Physics2D.Samples.ScreenSystem
{
    /// <summary>
    /// A popup message box screen.
    /// </summary>
    public class NewBlockButton : GameScreen
    {
        private Rectangle _blockButtonRectangle;
        public Texture2D _blockButton;

        public Vector2 _blockButtonSize;

        public int _starCount;

        public NewBlockButton(/*string title, string message, int starCount*/)
        {
            //_title = title;
            //_message = message;
            //_starCount = starCount;

            IsPopup = true;
            HasCursor = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.35);
            TransitionOffTime = TimeSpan.FromSeconds(0.3);
        }

        /// <summary>
        /// Loads graphics content for this screen. This uses the shared ContentManager
        /// provided by the Game class, so the content will remain loaded forever.
        /// Whenever a subsequent MessageBoxScreen tries to load this same content,
        /// it will just get back another reference to the already loaded data.
        /// </summary>
        public override void LoadContent()
        {
            SpriteFont font = ScreenManager.Fonts.DetailsFont;
            ContentManager content = ScreenManager.Game.Content;
            //_gradientTexture = content.Load<Texture2D>("Common/popup");
            _blockButton = content.Load<Texture2D>("goldenStar1");

            // Center the message text in the viewport.
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            //_titleSize = font.MeasureString(_title);
            //_titleSize.Y *= 2f;
            //Vector2 textSize = font.MeasureString(_message);
            //textSize.X = (float)Math.Max(_titleSize.X, textSize.X);
            //textSize.Y = _titleSize.Y + textSize.Y;

            // The background includes a border somewhat larger than the text itself.
            const int hPad = 32;
            const int vPad = 64;

            //var panelSize = new Vector2(
            //    textSize.X + hPad * 2f,
            //    textSize.Y + vPad * 2f
            //    );


            float bWidth = 0;
            if (viewportSize.X > viewportSize.Y)
                _blockButtonSize = new Vector2(bWidth = (viewportSize.X / 13f), bWidth);
            else
                _blockButtonSize = new Vector2(bWidth = (viewportSize.Y / 13f), bWidth);
            _blockButtonRectangle = new Rectangle(
                        (int)(viewportSize.X / 2f - (_blockButtonSize.X / 2f)),
                        (int)(viewportSize.Y / 2f - (_blockButtonSize.Y / 2f)),
                        (int)(_blockButtonSize.X),
                        (int)(_blockButtonSize.Y)
                        );
        }

        /// <summary>
        /// Responds to user input, accepting or cancelling the message box.
        /// </summary>
        public override void HandleInput(InputHelper input, GameTime gameTime)
        {
            //if (input.IsMenuSelect() || input.IsMenuCancel() || input.IsNewMouseButtonPress(MouseButtons.LeftButton))
            //    ExitScreen();
        }

        /// <summary>
        /// Draws the message box.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.Draw(_blockButton, _blockButtonRectangle, Color.White * TransitionAlpha);

            spriteBatch.End();
        }
    }
}