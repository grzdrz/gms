using System.Collections.Generic;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Samples.DrawingSystem;
using tainicom.Aether.Physics2D.Samples.ScreenSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.CompilerServices;
using VirusBonker.GameSystem.UncontrollableGameObjects;
using System.Linq;

namespace tainicom.Aether.Physics2D.Samples.Demos.Prefabs
{//менеджер неуправляемых игровых объектов
    public class Virus : IGameObject
    {
        public Sprite _circle;
        public Sprite _circle2;
        public Body body;
        private SpriteBatch _batch;

        public float RotateTEST;
        //public Color ColorTEST;
        public Vector2 scaleKoef;

        ScreenManager _sm;

        public Virus(World world, ScreenManager screenManager, Vector2 position,/* int count,*/ float density)
        {
            _sm = screenManager;

            _batch = screenManager.SpriteBatch;

            CreateBody(world, position);
            #region "old"
            //Vertices circle = PolygonTools.CreateCircle(1f, 8);//набор вершин
            //PolygonShape shape = new PolygonShape(circle, density);//форма для вычисления коллизий

            //body = world.CreateBody();
            //body.BodyType = BodyType.Dynamic;
            //body.Position = position;//начальная позиция
            //body.CreateFixture(shape);

            //_boxes.Add(body);

            //Vector2 rowStart = position;//начальная позиция
            //rowStart.Y -= 0.5f + count * 1.1f;

            //Vector2 deltaRow = new Vector2(-0.625f, -1.1f);
            //const float spacing = 1.25f;

            //_boxes = new List<_body>();

            //for (int i = 0; i < count; ++i)
            //{
            //    Vector2 pos = rowStart;

            //    for (int j = 0; j < i + 1; ++j)
            //    {
            //        _body body = world.CreateBody();
            //        body.BodyType = BodyType.Dynamic;
            //        body.Position = pos;
            //        body.CreateFixture(shape);
            //        _boxes.Add(body);

            //        pos.X += spacing;

            //    }

            //    rowStart += deltaRow;
            //}
            #endregion

            //GFX
            CreateGFX(screenManager.Assets, screenManager);
        }

        private void CreateBody(World world, Vector2 position)
        {
            body = world.CreateCircle(1f, 10f);
            body.BodyType = BodyType.Dynamic;
            //body.AngularDamping = LimbAngularDamping;
            body.Mass = 2f;
            body.Position = position;

            body.Tag = new GameObjectOfBodyInfo("Virus", this);
        }
        private void CreateGFX(AssetCreator assets, ScreenManager screenManager)
        {
            //ColorTEST = Color.White;
            _circle = new Sprite(assets.CircleTexture(1f, MaterialType.Waves, Color.Red, 1f, 24f));
            _circle2 = new Sprite(screenManager.Game.Content.Load<Texture2D>("virus1"));
            scaleKoef = _circle2.Size / _circle.Size;
        }
        public void Draw()
        {
            //_batch.Draw(
            //    _circle.Texture,
            //    body.Position,
            //    null,
            //    Color.White,
            //    body.Rotation,
            //    _circle.Origin,
            //    new Vector2(2f, 2f) * _circle.TexelSize,
            //    SpriteEffects.FlipVertically,
            //    0f);

            _batch.Draw(
                _circle2.Texture,
                body.Position,
                null,
                Color.White,
                body.Rotation,
                _circle2.Origin,
                (new Vector2(2f, 2f) * _circle.TexelSize) / scaleKoef,
                SpriteEffects.FlipVertically,
                0f);
        }

        //private Texture2D RenderTextureTEST(int width, int height, Texture2D material/*, List<VertexPositionColorTexture[]> verticesFill, VertexPositionColor[] verticesOutline*/)
        //{
        //    Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0f);
        //    PresentationParameters pp = _sm.Assets._device.PresentationParameters;
        //    RenderTarget2D texture = new RenderTarget2D(_sm.Assets._device, width + 2, height + 2, false, SurfaceFormat.Color, DepthFormat.None, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);
        //    _sm.Assets._device.RasterizerState = RasterizerState.CullNone;
        //    _sm.Assets._device.SamplerStates[0] = SamplerState.LinearWrap;

        //    _sm.Assets._device.SetRenderTarget(texture);
        //    _sm.Assets._device.Clear(Color.Transparent);
        //    _sm.Assets._effect.Projection = Matrix.CreateOrthographic(width + 2f, height + 2f, 0f, 1f);
        //    _sm.Assets._effect.View = halfPixelOffset;
        //    // render shape;
        //    _sm.Assets._effect.TextureEnabled = true;
        //    _sm.Assets._effect.Texture = material;
        //    _sm.Assets._effect.VertexColorEnabled = true;
        //    _sm.Assets._effect.CurrentTechnique.Passes[0].Apply();
        //    //for (int i = 0; i < verticesFill.Count; ++i)
        //    //{
        //    //    _sm.Assets._device.DrawUserPrimitives(PrimitiveType.TriangleList, verticesFill[i], 0, verticesFill[i].Length / 3);
        //    //}
        //    // render outline;
        //    _sm.Assets._effect.TextureEnabled = false;
        //    _sm.Assets._effect.CurrentTechnique.Passes[0].Apply();
        //    //_sm.Assets._device.DrawUserPrimitives(PrimitiveType.LineList, verticesOutline, 0, verticesOutline.Length / 2);
        //    _sm.Assets._device.SetRenderTarget(null);
        //    return texture;
        //}
    }
}
