using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Samples.DrawingSystem;
using tainicom.Aether.Physics2D.Samples.ScreenSystem;

namespace UberBuilder.GameSystem.Buttons
{
    public class PlayButton
    {
        World _world;
        ScreenManager _screenManager;
        Vector2 _position;
        Sprite _sprite;

        public PlayButton(World world, ScreenManager screenManager, Vector2 position)
        {
            _world = world;
            _screenManager = screenManager;
            _position = position;

            _sprite = new Sprite(_screenManager.Assets.CircleTexture(3f, MaterialType.Dots, Color.Blue, 1f, 24f));
        }

        public void Draw()
        {
            _screenManager.SpriteBatch.Draw(
                _sprite.Texture,
                _position,
                null, 
                Color.White,
                0f,
                _sprite.Origin,
                Vector2.One * (1f/ 24f),
                SpriteEffects.FlipVertically,
                0f);
        }

        public void Update()
        {
            
        }
    }
}
