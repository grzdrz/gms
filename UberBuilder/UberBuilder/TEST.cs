using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace First3DGame
{
    public class GameTEST : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Matrix projectionMatrix;
        Matrix viewMatrix;
        Matrix worldMatrix;

        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;

        BasicEffect effect;

        VertexPositionTexture[] verts;

        short[] textureIndices =
        {
                0,1,2, // передняя сторона
                2,3,0,

                6,5,4, // задняя сторона
                4,7,6,

                4,0,3, // левый бок
                3,7,4,

                1,5,6, // правый бок
                6,2,1,

                4,5,1, // вверх
                1,0,4,

                3,2,6, // низ
                6,7,3,
         };

        Texture2D texture;

        public GameTEST()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 6), Vector3.Zero, Vector3.Up);

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                (float)Window.ClientBounds.Width / (float)Window.ClientBounds.Height,
                1, 100);

            worldMatrix = Matrix.CreateWorld(new Vector3(0f, 0f, 0f), new Vector3(0, 0, -1), Vector3.Up);

            verts = new VertexPositionTexture[8];
            verts[0] = new VertexPositionTexture(new Vector3(-1, 1, 1), new Vector2(0, 0));
            verts[1] = new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(1, 0));
            verts[2] = new VertexPositionTexture(new Vector3(1, -1, 1), new Vector2(1, 1));
            verts[3] = new VertexPositionTexture(new Vector3(-1, -1, 1), new Vector2(0, 1));
            verts[4] = new VertexPositionTexture(new Vector3(-1, 1, -1), new Vector2(0, 0));
            verts[5] = new VertexPositionTexture(new Vector3(1, 1, -1), new Vector2(1, 0));
            verts[6] = new VertexPositionTexture(new Vector3(1, -1, -1), new Vector2(1, 1));
            verts[7] = new VertexPositionTexture(new Vector3(-1, -1, -1), new Vector2(0, 1));

            vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionTexture),
                 verts.Length, BufferUsage.None);
            vertexBuffer.SetData(verts);

            effect = new BasicEffect(GraphicsDevice);

            indexBuffer = new IndexBuffer(graphics.GraphicsDevice, typeof(short), 36, BufferUsage.None);
            indexBuffer.SetData<short>(textureIndices);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            texture = Content.Load<Texture2D>("wood");
            effect.TextureEnabled = true;
            effect.Texture = texture;
        }

        protected override void UnloadContent()
        { }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                worldMatrix *= Matrix.CreateRotationX(MathHelper.ToRadians(1));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                worldMatrix *= Matrix.CreateRotationX(-1 * MathHelper.ToRadians(1));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                worldMatrix *= Matrix.CreateRotationY(MathHelper.ToRadians(1));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                worldMatrix *= Matrix.CreateRotationY(-1 * MathHelper.ToRadians(1));
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            effect.World = worldMatrix;
            effect.View = viewMatrix;
            effect.Projection = projectionMatrix;

            GraphicsDevice.SetVertexBuffer(vertexBuffer);
            GraphicsDevice.Indices = indexBuffer;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, verts, 0, 8, textureIndices, 0, 12);
            }

            base.Draw(gameTime);
        }
    }
}
