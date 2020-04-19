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

namespace UberBuilder.GameSystem.ControllableCharacters
{
    public class UBuilder
    {
        public World _world;
        public ScreenManager _screenManager;
        public Vector2 _originPosition;
        public Camera2D _camera;

        public Body _body;
        public Sprite _sprite;

        public UBuilder(World world, ScreenManager screenManager, Vector2 position, Camera2D camera)
        {
            _world = world;
            _screenManager = screenManager;
            _originPosition = position;
            _camera = camera;

            _body = _world.CreateCircle(1f, 1f, position, BodyType.Static);
            _sprite = new Sprite(_screenManager.Assets.CircleTexture(1f, MaterialType.Blank, Color.Azure, 1f, 24f));
        }

        public void Update()
        { 
        }

        public void Draw()
        {
            _screenManager.SpriteBatch.Draw(
                _sprite.Texture,
                _body.Position,
                null,
                Color.White,
                _body.Rotation,
                _sprite.Origin,
                _sprite.Size * _sprite.TexelSize * (1f / 24f),
                SpriteEffects.FlipVertically,
                0f);
        }
    }

    public class TrajectoryDraw
    {
        public TrajectoryDraw(Vector2 force)
        {
            
        }
    }
}
