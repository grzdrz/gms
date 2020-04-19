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

        public BuildingSilhouette(World world, ScreenManager screenManager, Vector2 position, Camera2D camera2D, Vector2 size)
        {
            _world = world;
            _screenManager = screenManager;
            _camera = camera2D;

            _body = _world.CreateRectangle(size.X, size.Y, 1f, position, 0f, BodyType.Static);
            _body.SetCollisionCategories(Category.Cat30);
            _body.SetCollidesWith(Category.Cat30);
            _sprite = new Sprite(_screenManager.Assets.TextureFromShape(_body.FixtureList[0].Shape, MaterialType.Waves, Color.Red, 1f));
        }

        public void Update()
        {
            
        }

        public void Draw(SpriteBatch batch)
        {
            
        }
    }
}
