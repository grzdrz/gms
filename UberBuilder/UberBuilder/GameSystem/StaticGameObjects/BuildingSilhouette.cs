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

namespace UberBuilder.GameSystem.StaticGameObjects
{
    public class BuildingSilhouette
    {
        public World _world;
        public ScreenManager _screenManager;
        public Camera2D _camera;

        public Body _body;
        public Sprite _sprite;
        public string _texturePath;
        public Vector2 scaleKoef;

        public BuildingSilhouette(World world, ScreenManager screenManager, Vector2 position, Camera2D camera2D, Vector2 size, string texturePath)
        {
            _world = world;
            _screenManager = screenManager;
            _camera = camera2D;
            _texturePath = texturePath;

            _body = _world.CreateRectangle(size.X, size.Y, 1f, position, 0f, BodyType.Static);
            _body.SetCollisionCategories(Category.None);
            _body.SetCollidesWith(Category.None);
            //_sprite = new Sprite(_screenManager.Assets.TextureFromShape(_body.FixtureList[0].Shape, MaterialType.Waves, Color.Red, 1f));
            _sprite = new Sprite(_screenManager.Content.Load<Texture2D>(_texturePath));
            scaleKoef = _sprite.Size / size;
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
                (_sprite.Size * _sprite.TexelSize * (1f / 24f)) * 1.3f,
                SpriteEffects.FlipVertically,
                0f);
        }
    }
}
