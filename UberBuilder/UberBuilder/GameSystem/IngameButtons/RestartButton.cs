using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Samples.ScreenSystem;

namespace UberBuilder.GameSystem.MessageBoxes
{
    public class RestartButton
    {
        private const float NumEntries = 15;
        private List<SessionMenuEntry> _menuEntries = new List<SessionMenuEntry>();
        private int _selectedEntry;
        private float _menuBorderTop;
        private float _menuBorderBottom;
        private float _menuBorderMargin;

        public TimeSpan TransitionOnTime { get; set; }
        public TimeSpan TransitionOffTime { get; set; }
        public ScreenManager ScreenManager { get; set; }
        public GameScreen Lvl { get; set; }
        public RestartButton(GameScreen lvl, ScreenManager screenManager)
        {
            ScreenManager = screenManager;
            Lvl = lvl;

            TransitionOnTime = TimeSpan.FromSeconds(0.7);
            TransitionOffTime = TimeSpan.FromSeconds(0.7);

            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            SpriteFont font = ScreenManager.Fonts.MenuSpriteFont;

            _menuBorderMargin = font.MeasureString("M").Y * 0.8f;
            _menuBorderTop = (viewport.Height - _menuBorderMargin * (NumEntries - 1)) / 2f;
            _menuBorderBottom = (viewport.Height + _menuBorderMargin * (NumEntries - 1)) / 2f;
        }

        public void AddMenuItem(EntryType type, GameScreen screen)
        {
            SessionMenuEntry entry = new SessionMenuEntry(this, type, screen);
            _menuEntries.Add(entry);
        }

        private int GetMenuEntryAt(Vector2 position)
        {
            int index = 0;
            foreach (SessionMenuEntry entry in _menuEntries)
            {
                float width = entry.GetWidth();
                float height = entry.GetHeight();
                Rectangle rect = new Rectangle((int)(entry.Position.X - width / 2f), (int)(entry.Position.Y - height / 2f), (int)width, (int)height);

                if (rect.Contains((int)position.X, (int)position.Y) && entry.Alpha > 0.1f)
                    return index;

                ++index;
            }
            return -1;
        }

        public void HandleInput(InputHelper input, GameTime gameTime)
        {
            // Mouse or touch on a menu item
            int hoverIndex = GetMenuEntryAt(input.Cursor);
            if (hoverIndex > -1 && _menuEntries[hoverIndex].IsSelectable())
                _selectedEntry = hoverIndex;
            else
                _selectedEntry = -1;

            if (input.IsMenuSelect() && _selectedEntry != -1)
            {
                if (_menuEntries[_selectedEntry].Screen != null)
                {
                    _menuEntries[_selectedEntry].Screen.UnloadContent();
                    _menuEntries[_selectedEntry].Screen.LoadContent();
                }
            }
        }

        protected void UpdateMenuEntryLocations()
        {
            Vector2 position = Vector2.Zero;
            var viewport_height = ScreenManager.GraphicsDevice.Viewport.Height;
            var viewport_width = ScreenManager.GraphicsDevice.Viewport.Width;
            float borderWidthHeight = 30f + (((float)viewport_width) / 10f) / 2f;
            position.X = borderWidthHeight;//стартовое значение
            //(т.к. объект отрисовывается от центра, то нужно установить смещение на половину ширины вправо + ширина условной рамки,
            //где ширина кнопки == 1/10 ширины вьюпорта)
            //position.Y = _menuBorderTop - _menuOffset;

            // update each menu entry's location in turn
            for (int i = 0; i < _menuEntries.Count; ++i)
            {
                if (_menuEntries[i]._type == EntryType.ExitItem)
                {
                    position.X = viewport_width - 30f - _menuEntries[i].GetWidth() / 2f;
                    position.Y = viewport_height - 30f - _menuEntries[i].GetHeight() / 2f;
                }
                else
                {
                    position.X = _menuEntries[i].GetWidth() / 2f + 20f;/*ScreenManager.GraphicsDevice.Viewport.Width / 2f;*/
                    position.Y = _menuEntries[i].GetHeight() / 2f + 20f;/*ScreenManager.GraphicsDevice.Viewport.Height / 2f;*/
                }
                //if (ScreenState == ScreenState.TransitionOn)
                //    position.X -= transitionOffset * 256;
                //else
                //    position.X += transitionOffset * 256;

                // set the entry's position
                _menuEntries[i].Position = position;

                if (position.Y < _menuBorderTop)
                    _menuEntries[i].Alpha = 1f - Math.Min(_menuBorderTop - position.Y, _menuBorderMargin) / _menuBorderMargin;
                else if (position.Y > _menuBorderBottom)
                    _menuEntries[i].Alpha = 1f - Math.Min(position.Y - _menuBorderBottom, _menuBorderMargin) / _menuBorderMargin;
                else
                    _menuEntries[i].Alpha = 1f;
            }
        }

        public void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            // Update each nested MenuEntry object.
            for (int i = 0; i < _menuEntries.Count; ++i)
            {
                bool isSelected = /*IsActive &&*/ (i == _selectedEntry);
                _menuEntries[i].Update(isSelected, gameTime);
            }
        }

        public void Draw(GameTime gameTime)
        {
            // make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations();

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Fonts.MenuSpriteFont;

            spriteBatch.Begin();
            // Draw each menu entry in turn.
            for (int i = 0; i < _menuEntries.Count; ++i)
            {
                _menuEntries[i].Draw();
            }

            spriteBatch.End();
        }
    }
}
