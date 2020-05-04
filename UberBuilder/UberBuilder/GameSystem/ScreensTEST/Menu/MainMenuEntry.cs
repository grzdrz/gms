/* Original source Farseer Physics Engine:
 * Copyright (c) 2014 Ian Qvist, http://farseerphysics.codeplex.com
 * Microsoft Permissive License (Ms-PL) v1.1
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Samples.DrawingSystem;

namespace tainicom.Aether.Physics2D.Samples.ScreenSystem
{
    /// <summary>
    /// Helper class represents a single entry in a MenuScreen. By default this
    /// just draws the entry text string, but it can be customized to display menu
    /// entries in different ways. This also provides an event that will be raised
    /// when the menu entry is selected.
    /// </summary>
    public sealed class MainMenuEntry
    {
        private float _height;
        private MainMenuScreen _menu;

        private float _scale;
        public float _buttonScale;

        /// <summary>
        /// Tracks a fading selection effect on the entry.
        /// </summary>
        /// <remarks>
        /// The entries transition out of the selection effect when they are deselected.
        /// </remarks>
        private float _selectionFade;

        public EntryType _type;
        private float _width;


        Sprite gameIcon1;
        /// <summary>
        /// Constructs a new menu entry with the specified text.
        /// </summary>
        public MainMenuEntry(MainMenuScreen menu, EntryType type, GameScreen screen)
        {
            Screen = screen;
            _type = type;
            _menu = menu;
            _scale = 0.9f;
            Alpha = 1.0f;
        }

        /// <summary>
        /// Gets or sets the position at which to draw this menu entry.
        /// </summary>
        public Vector2 Position { get; set; }

        public float Alpha { get; set; }

        public GameScreen Screen { get; private set; }

        public void Initialize()
        {
            if (_type == EntryType.Screen)
                gameIcon1 = new Sprite(_menu.ScreenManager.Content.Load<Texture2D>("gameObjs\\buttonPlay"));
            else if (_type == EntryType.ExitItem)
                gameIcon1 = new Sprite(_menu.ScreenManager.Content.Load<Texture2D>("gameObjs\\buttonExit"));

            if (gameIcon1 != null)
            {
                var viewport_height = _menu.ScreenManager.GraphicsDevice.Viewport.Height;
                var viewport_width = _menu.ScreenManager.GraphicsDevice.Viewport.Width;
                var percentage = ((float)viewport_width) / 7f;//для кнопки старта игры
                _buttonScale = percentage / gameIcon1.Size.X;

                _width = gameIcon1.Size.X * _buttonScale;
                _height = gameIcon1.Size.Y * _buttonScale;
            }
        }

        public bool IsExitItem()
        {
            return _type == EntryType.ExitItem;
        }

        public bool IsSelectable()
        {
            return _type != EntryType.Separator;
        }

        /// <summary>
        /// Updates the menu entry.
        /// </summary>
        public void Update(bool isSelected, GameTime gameTime)
        {
            // there is no such thing as a selected item on Windows Phone, so we always
            // force isSelected to be false
#if WINDOWS_PHONE
            isSelected = false;
#endif
            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            if (_type != EntryType.Separator)
            {
                float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;
                if (isSelected)
                    _selectionFade = Math.Min(_selectionFade + fadeSpeed, 1f);
                else
                    _selectionFade = Math.Max(_selectionFade - fadeSpeed, 0f);

                _scale = 0.7f + 0.1f * _selectionFade;
            }
        }

        /// <summary>
        /// Draws the menu entry. This can be overridden to customize the appearance.
        /// </summary>
        public void Draw()
        {
            SpriteBatch batch = _menu.ScreenManager.SpriteBatch;

            // Draw the selected entry   
            var col = new Color(235, 204, 255);
            var colSel = new Color(203, 164, 229);
            //var colSep = new Color(164, 190, 229);
            var colSep = new Color(164, 229, 203);


            Color color = _type == EntryType.Separator ? colSep : Color.Lerp(col, colSel, _selectionFade);
            color *= Alpha;

            if (gameIcon1 != null)
                batch.Draw(
                    gameIcon1.Texture,
                    Position,
                    null,
                    Color.White,
                    0f,
                    gameIcon1.Origin,
                    Vector2.One/* * (1f / 24f)*/ * _buttonScale,
                    SpriteEffects.None,
                    0f);
        }

        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        public int GetHeight()
        {
            return (int)_height;
        }

        /// <summary>
        /// Queries how wide the entry is, used for centering on the screen.
        /// </summary>
        public int GetWidth()
        {
            return (int)_width;
        }
    }
}