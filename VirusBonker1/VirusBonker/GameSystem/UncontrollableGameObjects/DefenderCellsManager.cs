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
using VirusBonker.GameSystem.GameObjects;

namespace VirusBonker.GameSystem.UncontrollableGameObjects
{
    public class DefenderCellsManager
    {
        public World _world;
        public ScreenManager _screenManager;

        public SpriteBatch _batch;
        public List<DefenderCell> _objs;

        public DefenderCellsManager(World world, ScreenManager screenManager)
        {
            _batch = screenManager.SpriteBatch;
            _world = world;
            _screenManager = screenManager;

            CreateCells(screenManager.Assets, screenManager);
        }

        private void CreateCells(AssetCreator assets, ScreenManager screenManager)
        {
            _objs = new List<DefenderCell>();
            Random random = new Random();
            Vector2 size;
            float sideSize;

            //////////////////////////////////////////////////////
            var vp = screenManager.GraphicsDevice.Viewport;
            float height = 30f; // 30 meters height
            float width = height * vp.AspectRatio;
            width -= 1.5f; // 1.5 meters border
            height -= 1.5f;
            float halfWidth = width / 2f;
            float halfHeight = height / 2f;
            ///////////////////////////////////////////////////////

            for (int i = 0; i < 50; i++)
            {
                _objs.Add(new DefenderCell(
                    size = new Vector2(sideSize = random.Next(2, 4), sideSize), 
                    new Vector2(random.Next((int)-halfWidth, (int)halfWidth), random.Next((int)-halfHeight, (int)halfHeight)),
                    //random.Next(0, 2),
                    _world,
                    _screenManager
                    ));
            }
        }

        public float sphereRad = 15f;
        public float nullZoneRad = 6f;
        public float smooth = 0.1f;
        public void Update(MainCharacter _mainCharacter)
        {
            var circles = _objs;
            for (int i = 0; i < circles.Count; i++)
            {
                Vector2 acc = Vector2.Zero;

                for (int j = 0; j < circles.Count; j++)
                {
                    if (i == j) continue;
                    var objA = circles[i];
                    var objB = circles[j];

                    Vector2 delta = new Vector2(objB._body.Position.X - objA._body.Position.X, objB._body.Position.Y - objA._body.Position.Y);
                    float dist = delta.Length();
                    if (dist == 0) dist = 1;
                    float force = (dist - sphereRad) / dist * objB._body.Mass;

                    acc += delta * force;//????????
                }

                //доп. просчет для управляемого шара
                var objA2 = circles[i];
                Vector2 delta2 = new Vector2(_mainCharacter._body.Position.X - objA2._body.Position.X, _mainCharacter._body.Position.Y - objA2._body.Position.Y);
                float dist2 = delta2.Length();
                if (dist2 == 0) dist2 = 1;
                float force2;

                if (dist2 < nullZoneRad) force2 = (dist2 - nullZoneRad) * _mainCharacter._body.Mass / 10;
                else force2 = objA2._body.Mass;

                acc += delta2 * force2;//????????

                circles[i]._velocity.X = circles[i]._velocity.X * smooth + acc.X * circles[i]._body.Mass;
                circles[i]._velocity.Y = circles[i]._velocity.Y * smooth + acc.Y * circles[i]._body.Mass;

                circles[i]._body.ApplyLinearImpulse(circles[i]._body.Mass * circles[i]._velocity);
            }
        }

        public void Draw()
        {
            foreach (var e in _objs)
            {
                _batch.Draw(
                    e._sprite.Texture,
                    e._body.Position,
                    null,
                    Color.White,
                    0,
                    e._sprite.Origin,
                    e._size.X * e._sprite.TexelSize,
                    SpriteEffects.FlipVertically,
                    0f);
            }
        }
    }

    public class DefenderCell
    {
        public World _world;
        public ScreenManager _screenManager;

        public Body _body;
        public Sprite _sprite;

        public Vector2 _size;
        public Vector2 _velocity;
        

        private Category _collisionCategories;
        public Category CollisionCategories
        {
            get { return _collisionCategories; }
            set
            {
                _collisionCategories = value;
                _body.SetCollisionCategories(value);
            }
        }

        private Category _collidesWith;
        public Category CollidesWith
        {
            get { return _collidesWith; }
            set
            {
                _collidesWith = value;
                _body.SetCollidesWith(value);
            }
        }

        public DefenderCell(Vector2 size, Vector2 position, World world, ScreenManager screenManager)
        {
            _screenManager = screenManager;
            _world = world;

            _size = size;

            CreateBody(position);

            CreateGFX();
        }

        private void CreateBody(Vector2 position)
        {
            _body = _world.CreateCircle(_size.X / 2f, 10f);
            _body.BodyType = BodyType.Dynamic;
            //_body.AngularDamping = LimbAngularDamping;
            _body.Mass = _size.X / 15f;
            _body.Position = position;
        }

        private void CreateGFX()
        {
            _sprite = new Sprite(_screenManager.Assets.CircleTexture(_size.X, MaterialType.Blank, new Color(150, 10, 30, 255), 1f, 24f));
        }
    }
}
