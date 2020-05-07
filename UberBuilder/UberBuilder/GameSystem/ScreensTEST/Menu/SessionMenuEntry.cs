/* Original source Farseer Physics Engine:
 * Copyright (c) 2014 Ian Qvist, http://farseerphysics.codeplex.com
 * Microsoft Permissive License (Ms-PL) v1.1
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Samples.DrawingSystem;
using UberBuilder.GameSystem;
using UberBuilder.GameSystem.MessageBoxes;

namespace tainicom.Aether.Physics2D.Samples.ScreenSystem
{
    /// <summary>
    /// Helper class represents a single entry in a MenuScreen. By default this
    /// just draws the entry text string, but it can be customized to display menu
    /// entries in different ways. This also provides an event that will be raised
    /// when the menu entry is selected.
    /// </summary>
    public sealed class SessionMenuEntry
    {
        private float _height;
        private RestartButton _menu;

        private float _scale;
        public float _buttonScale;
        public float _sizeScaleInPercent = 10f;//%%%

        private float _selectionFade;

        public EntryType _type;
        private float _width;


        Sprite gameIcon1;
        public SessionMenuEntry(RestartButton menu, EntryType type, GameScreen screen)
        {
            Screen = screen;
            _type = type;
            _menu = menu;
            _scale = 0.9f;
            Alpha = 1.0f;


            var viewport_height = _menu.ScreenManager.GraphicsDevice.Viewport.Height;
            var viewport_width = _menu.ScreenManager.GraphicsDevice.Viewport.Width;
            gameIcon1 = new Sprite(_menu.ScreenManager.Content.Load<Texture2D>("gameObjs\\buttonRestart"));
            var percentage = ((float)viewport_width) / (100f / _sizeScaleInPercent);
            _buttonScale = percentage / gameIcon1.Size.X;

            _width = gameIcon1.Size.X * _buttonScale;
            _height = gameIcon1.Size.Y * _buttonScale;
        }

        public Vector2 Position { get; set; }

        public float Alpha { get; set; }

        public GameScreen Screen { get; private set; }

        public bool IsExitItem()
        {
            return _type == EntryType.ExitItem;
        }

        public bool IsSelectable()
        {
            return _type != EntryType.Separator;
        }

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
            //коэффициент от 0 до 1 включительно, определяющий изменение размера(в Draw()) при наведении на кнопку
            if (_type != EntryType.Separator)
            {
                float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;
                if (isSelected)
                    _selectionFade = Math.Min(_selectionFade + fadeSpeed, 1f);
                else
                    _selectionFade = Math.Max(_selectionFade - fadeSpeed, 0f);

                _scale = 1f/*0.7f*/ + 0.1f * _selectionFade;
            }
        }

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
                    Vector2.One/* * (1f / 24f)*/ * _buttonScale * _scale,
                    SpriteEffects.None,
                    0f);
        }

        public int GetHeight()
        {
            return (int)_height;
        }

        public int GetWidth()
        {
            return (int)_width;
        }
    }
}