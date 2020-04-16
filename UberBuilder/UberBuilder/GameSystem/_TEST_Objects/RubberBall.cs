using System.Text;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Samples.Demos.Prefabs;
using tainicom.Aether.Physics2D.Samples.DrawingSystem;
using tainicom.Aether.Physics2D.Samples.ScreenSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace tainicom.Aether.Physics2D.Samples.Samples.TEST
{
    public class RubberBall
    {
        private Body _ball;
        private Sprite _circleSprite;
        private SpriteBatch _batch;

        public RubberBall(World world, ScreenManager screenManager, Vector2 position)
        {
            _batch = screenManager.SpriteBatch;

            float restitution = 0.8f;//1f - возврат на исходную позицию

            _ball = world.CreateCircle(1.5f, 1f, position);
            _ball.BodyType = BodyType.Dynamic;
            _ball.SetRestitution(restitution);
            _ball.Mass = 1f;

            CreateGFX(screenManager.Assets);
        }

        private void CreateGFX(AssetCreator assets)
        {
            // create sprite based on body
            _circleSprite = new Sprite(assets.TextureFromShape(_ball.FixtureList[0].Shape, MaterialType.Waves, Color.Brown, 1f));
        }

        public void Draw()
        {
            _batch.Draw(
                _circleSprite.Texture, 
                _ball.Position, 
                null, 
                Color.White,
                _ball.Rotation,
                _circleSprite.Origin, 
                new Vector2(2f * 1.5f) * _circleSprite.TexelSize,
                SpriteEffects.FlipVertically,
                0f);
        }
    }
}
