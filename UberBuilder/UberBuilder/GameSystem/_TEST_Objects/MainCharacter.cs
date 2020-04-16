using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Samples.Demos.Prefabs;
using tainicom.Aether.Physics2D.Samples.DrawingSystem;
using tainicom.Aether.Physics2D.Samples.ScreenSystem;
using VirusBonker.GameSystem.UncontrollableGameObjects;

namespace VirusBonker.GameSystem.GameObjects
{
    public class MainCharacter
    {
        public ScreenManager _screenManager;
        private SpriteBatch _batch;

        public Body _body;
        public Sprite _sprite;
        public Dictionary<int, Sprite> _mainCharPairsSpriteHP;
        public Sprite _spriteToDraw;

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


        public int HP = 100;

        public MainCharacter(World world, ScreenManager screenManager, Vector2 position)
        {
            _screenManager = screenManager;
            _batch = screenManager.SpriteBatch;

            _collidesWith = Category.All;
            _collisionCategories = Category.All;

            CreateBody(world, position);
            //CreateJoints(world);

            CreateGFX(screenManager.Assets);

            //изменение объектов при коллизии через изменение их внутренних спец. свойств и Update GameLevel'а
            this._body.OnCollision += (sender, other, contact) =>
            {
                if ((other.Body.Tag as GameObjectOfBodyInfo) != null)
                {
                    if ((other.Body.Tag as GameObjectOfBodyInfo).Name == "Virus")
                    {
                        var test = (other.Body.Tag as GameObjectOfBodyInfo).GameObject as Virus;
                        test.RotateTEST += 1f;

                        if (this.HP > 0) this.HP -= 10;
                        else this.HP = 100;////
                    }
                }
                return true;
            };
        }

        private void CreateBody(World world, Vector2 position)
        {
            //main character body
            _body = world.CreateCircle(1f, 10f);
            _body.BodyType = BodyType.Dynamic;
            //_mainCharBody.AngularDamping = LimbAngularDamping;
            _body.Mass = 10f;
            _body.Position = /*position*/new Vector2(0, 0);
        }

        private void CreateGFX(AssetCreator assets)
        {
            int counterAlpha = (int)Math.Floor(255f / 10f);
            _sprite = new Sprite(assets.CircleTexture(1f, MaterialType.Waves, Color.Gray, 1f, 24f));
            _mainCharPairsSpriteHP = new Dictionary<int, Sprite>();
            for (int i = 0; i <= 10; i += 1)
            { 
                _mainCharPairsSpriteHP[i] = new Sprite(assets.CircleTexture(1f, MaterialType.Waves, new Color(255, 0, 0, 255 - counterAlpha * i), 1f, 24f));
            }
        }

        public void Draw()
        {
            if (!(_mainCharPairsSpriteHP.ContainsKey(this.HP / 10))) 
                _spriteToDraw = _sprite;
            else 
                _spriteToDraw = _mainCharPairsSpriteHP[this.HP / 10];
            _batch.Draw(
                _sprite.Texture,
                _body.Position,
                null,
                Color.White,
                _body.Rotation,
                _sprite.Origin,
                2f * _sprite.TexelSize,
                SpriteEffects.FlipVertically,
                0f);
            _batch.Draw(
                _spriteToDraw.Texture,
                _body.Position,
                null,
                new Color(128, 128, 128, 255),
                _body.Rotation,
                _spriteToDraw.Origin,
                2f * _spriteToDraw.TexelSize,
                SpriteEffects.FlipVertically,
                0f);

        }
    }
}
