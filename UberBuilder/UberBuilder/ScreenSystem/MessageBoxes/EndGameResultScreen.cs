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
    public class EndGameResultScreen : GameScreen
    {
        private Rectangle _backgroundRectangle;
        private Texture2D _gradientTexture;
        private string _title;
        private string _message;
        Vector2 _titleSize;
        private Vector2 _textPosition;

        public Vector2 _messageWindowSize;

        private List<Rectangle> _backgroundRectangleStars;
        public Texture2D _star;

        public Vector2 _starSize;

        public int _starCount;

        public EndGameResultScreen(string title, string message, int starCount)
        {
            _title = title;
            _message = message;
            _starCount = starCount;

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
            _gradientTexture = content.Load<Texture2D>("Common/popup");
            _star = content.Load<Texture2D>("goldenStar1");

            // Center the message text in the viewport.
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            _titleSize = font.MeasureString(_title);
            _titleSize.Y *= 2f;
            Vector2 textSize = font.MeasureString(_message);
            textSize.X = (float)Math.Max(_titleSize.X, textSize.X);
            textSize.Y = _titleSize.Y + textSize.Y;

            // The background includes a border somewhat larger than the text itself.
            const int hPad = 32;
            const int vPad = 64;

            var panelSize = new Vector2(
                textSize.X + hPad * 2f,
                textSize.Y + vPad * 2f
                );


            float bWidth = 0;
            if (viewportSize.X > viewportSize.Y)
                _starSize = new Vector2(bWidth = (viewportSize.X / 13f), bWidth);
            else
                _starSize = new Vector2(bWidth = (viewportSize.Y / 13f), bWidth);
            _backgroundRectangleStars = new List<Rectangle>();

            for (int i = 0; i < /*5*/_starCount; i++)
            {
                _backgroundRectangleStars.Add(new Rectangle(
                        (int)((viewportSize.X / 2f - (_starSize.X / (2f * 0.9f)) * (5f - i * 2)) + 
                        (_starSize.X - _starSize.X * 0.9f)),////////////////////////
                        (int)(viewportSize.Y / 2f - (_starSize.Y / 2f) * 3f),
                        (int)(_starSize.X),
                        (int)(_starSize.Y)
                        ));
            }

            _textPosition.X = _backgroundRectangle.X + hPad;
            _textPosition.Y = 0 + vPad;
        }

        /// <summary>
        /// Responds to user input, accepting or cancelling the message box.
        /// </summary>
        public override void HandleInput(InputHelper input, GameTime gameTime)
        {
            if (input.IsMenuSelect() || input.IsMenuCancel() || input.IsNewMouseButtonPress(MouseButtons.LeftButton))
                ExitScreen();
        }

        /// <summary>
        /// Draws the message box.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            //SpriteFont font = ScreenManager.Fonts.DetailsFont;

            //var alpha = 0.7f * TransitionAlpha;
            // Fade the popup alpha during transitions.
            //Color bgColor = new Color(0, 0, 0, 0.75f * TransitionAlpha);

            ////var colText = new Color(164, 196, 229);
            //var colText = new Color(203, 229, 164);

            spriteBatch.Begin();

            //var position = _textPosition;
            //var backgroundRectangle = _backgroundRectangle;
            //backgroundRectangle.X += (int)(_backgroundRectangle.Width * TransitionPosition);
            //position.X += _backgroundRectangle.Width * TransitionPosition;

            // Draw the background rectangle.
            //spriteBatch.Draw(_gradientTexture, backgroundRectangle, bgColor);

            for (int i = 0; i < _starCount; i++)
                spriteBatch.Draw(_star, _backgroundRectangleStars[i], Color.White * TransitionAlpha);
            
            //// Draw the title text.
            //spriteBatch.DrawString(font, _title, position + Vector2.One, Color.Black * alpha, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
            //spriteBatch.DrawString(font, _title, position, colText * TransitionAlpha, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);

            //position.Y += _titleSize.Y;

            //// Draw the message box text.
            //spriteBatch.DrawString(font, _message, position + Vector2.One, Color.Black * alpha);
            //spriteBatch.DrawString(font, _message, position, Color.White * alpha);

            spriteBatch.End();
        }
    }
}