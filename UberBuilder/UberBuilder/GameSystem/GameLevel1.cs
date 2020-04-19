using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Collision;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Common.Decomposition;
using tainicom.Aether.Physics2D.Common.PhysicsLogic;
using tainicom.Aether.Physics2D.Common.PolygonManipulation;
using tainicom.Aether.Physics2D.Diagnostics;
using tainicom.Aether.Physics2D.Dynamics.Joints;
using tainicom.Aether.Physics2D.Samples.Demos.Prefabs;
using tainicom.Aether.Physics2D.Samples.DemosTEST.Prefabs;
using tainicom.Aether.Physics2D.Samples.ScreenSystem;
using UberBuilder.GameSystem.ConstructionMaterials;
using UberBuilder.GameSystem.StaticGameObjects;
using UberBuilder.GameSystem.ControllableCharacters;
using tainicom.Aether.Physics2D.Samples.DrawingSystem;
using tainicom.Aether.Physics2D.Dynamics;
using System.IO;

namespace UberBuilder.GameSystem
{
    public class GameLevel1 : PhysicsGameScreen//, IDemoScreen
    {
        public float _width;
        public float _height;
        public float _halfWidth;
        public float _halfHeight;

        public Border _border;
        public WoodBlock _woodBlock;
        public UBuilder _uberBuilder;

        public ThrowTrajectory _throwTrajectory;

        public BuildingSilhouette _silhouette;

        #region IDemoScreen Members

        public string GetTitle()
        {
            return "Breakable bodies and explosions";
        }

        public string GetDetails()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("TODO: Add sample description!");
            sb.AppendLine(string.Empty);
            sb.AppendLine("GamePad:");
            sb.AppendLine("  - Explode (at cursor): B button");
            sb.AppendLine("  - Move cursor: left thumbstick");
            sb.AppendLine("  - Grab object (beneath cursor): A button");
            sb.AppendLine("  - Drag grabbed object: left thumbstick");
            sb.AppendLine("  - Exit to menu: Back button");
            sb.AppendLine(string.Empty);
            sb.AppendLine("Keyboard:");
            sb.AppendLine("  - Exit to menu: Escape");
            sb.AppendLine(string.Empty);
            sb.AppendLine("Mouse / Touchscreen");
            sb.AppendLine("  - Explode (at cursor): Right click");
            sb.AppendLine("  - Grab object (beneath cursor): Left click");
            sb.AppendLine("  - Drag grabbed object: move mouse / finger");
            return sb.ToString();
        }

        #endregion

        public override void LoadContent()
        {
            base.LoadContent();
            HasCursor = true;

            var vp = ScreenManager.GraphicsDevice.Viewport;
            _height = 30f; // 30 meters height
            _width = _height * vp.AspectRatio;
            _width -= 1.5f; // 1.5 meters border
            _height -= 1.5f;
            float halfWidth = _halfWidth = _width / 2f;
            float halfHeight = _halfHeight = _height / 2f;

            DebugView.AppendFlags(DebugViewFlags.Shape);

            World.Gravity = /*Vector2.Zero*/new Vector2(0, -9.82f);

            _border = new Border(World, ScreenManager, Camera);

            _woodBlock = new WoodBlock(
                World,
                ScreenManager,
                new Vector2(0f, 0f),
                Camera,
                /*"woodBlock1"*/"Samples/object",
                TriangulationAlgorithm.Seidel);

            _uberBuilder = new UBuilder(
                World,
                ScreenManager,
                new Vector2(-halfWidth + 10f, -halfHeight + 10f),
                Camera);

            _throwTrajectory = new ThrowTrajectory(
                World,
                ScreenManager);


            _silhouette = new BuildingSilhouette(
                World,
                ScreenManager,
                new Vector2(this._halfHeight / 2f, 0f),
                Camera,
                new Vector2(20f, 30f)
                /*"silhouettePath",*/);
        }

        public bool ThrowIsCalculated = false;
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            _woodBlock.Update(this._fixedMouseJoint);
            _uberBuilder.Update();
            _throwTrajectory.Update(this);
            _silhouette.Update();

            if (IsGameEnd) FinalMoveCameraToSilhouette();
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.BatchEffect.View = Camera.View;
            ScreenManager.BatchEffect.Projection = Camera.Projection;
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, RasterizerState.CullNone, ScreenManager.BatchEffect);

            _woodBlock.Draw(ScreenManager.SpriteBatch);
            _uberBuilder.Draw();
            _throwTrajectory.Draw(isMouseLeftButtonPressed);
            _silhouette.Draw(ScreenManager.SpriteBatch);

            ScreenManager.SpriteBatch.End();

            if(!IsGameEnd)_border.Draw();
            base.Draw(gameTime);
        }

        public override void UnloadContent()
        {
            DebugView.RemoveFlags(DebugViewFlags.Shape);

            base.UnloadContent();
        }

        public override void HandleInput(InputHelper input, GameTime gameTime)
        {
            #region "Explosions"
            //if (input.IsNewMouseButtonPress(MouseButtons.RightButton) ||
            //    input.IsNewButtonPress(Buttons.B))
            //{
            //    Vector2 cursorPos = Camera.ConvertScreenToWorld(input.Cursor);

            //    Vector2 min = cursorPos - new Vector2(10, 10);
            //    Vector2 max = cursorPos + new Vector2(10, 10);

            //    AABB aabb = new AABB(ref min, ref max);

            //    World.QueryAABB(fixture =>
            //    {
            //        Vector2 fv = fixture.Body.Position - cursorPos;
            //        fv.Normalize();
            //        fv *= 40;
            //        fixture.Body.ApplyLinearImpulse(ref fv);
            //        return true;
            //    }, ref aabb);
            //}
            #endregion

            base.HandleInput(input, gameTime);
        }





        public Body bodyToThrow;
        public Vector2 throwForce = Vector2.Zero;
        public IsMouseLeftButtonPressed isMouseLeftButtonPressed = IsMouseLeftButtonPressed.No;

        protected override void HandleCursor(InputHelper input)
        {
            #region "Захват тела и создание объектов-траектории для него"
            Vector2 position = Camera.ConvertScreenToWorld(input.Cursor);

            if ((input.IsNewButtonPress(Buttons.A) || input.IsNewMouseButtonPress(MouseButtons.LeftButton)) && _fixedMouseJoint == null)
            {
                Fixture savedFixture = World.TestPoint(position);
                if (savedFixture != null)
                {
                    bodyToThrow = savedFixture.Body;
                    if (bodyToThrow == _uberBuilder._body)
                    {
                        bodyToThrow.BodyType = BodyType.Dynamic;
                        _fixedMouseJoint = new FixedMouseJoint(bodyToThrow, position);
                        _fixedMouseJoint.MaxForce = 1000.0f * bodyToThrow.Mass;
                        World.Add(_fixedMouseJoint);
                        bodyToThrow.Awake = true;

                        isMouseLeftButtonPressed = IsMouseLeftButtonPressed.Yes;
                    }
                }
                else savedFixture = null;
            }

            if ((input.IsNewButtonRelease(Buttons.A) || input.IsNewMouseButtonRelease(MouseButtons.LeftButton)) && _fixedMouseJoint != null)
            {
                World.Remove(_fixedMouseJoint);
                _fixedMouseJoint = null;
                if (bodyToThrow == _uberBuilder._body)
                {
                    bodyToThrow.BodyType = BodyType.Dynamic;
                    throwForce = (_uberBuilder._originPosition - bodyToThrow.Position) * 500f;
                    bodyToThrow.ApplyForce(throwForce);

                    isMouseLeftButtonPressed = IsMouseLeftButtonPressed.No;
                }
            }

            if (_fixedMouseJoint != null)
                _fixedMouseJoint.WorldAnchorB = position;
            #endregion

            #region "Смещение камеры к силуэту, его снимок и обработка"
            if (input.IsNewMouseButtonPress(MouseButtons.RightButton))
            {
                IsGameEnd = true;
                HasCursor = false;


                #region "save png"
                //int w = ScreenManager.GraphicsDevice.PresentationParameters.BackBufferWidth;
                //int h = ScreenManager.GraphicsDevice.PresentationParameters.BackBufferHeight;

                //////force a frame to be drawn (otherwise back buffer is empty) 
                ////Draw(new GameTime());

                ////pull the picture from the buffer 
                //int[] backBuffer = new int[w * h];
                //ScreenManager.GraphicsDevice.GetBackBufferData(backBuffer);

                ////copy into a texture 
                //Texture2D texture = new Texture2D(
                //    ScreenManager.GraphicsDevice,
                //    w, h,
                //    false,
                //    ScreenManager.GraphicsDevice.PresentationParameters.BackBufferFormat);
                //texture.SetData(backBuffer);

                ////save to disk 
                //Stream stream = File.OpenWrite("C:\\Users\\space\\Рабочий стол\\TESTTESTTESTASSGDF\\1.png");

                //texture.SaveAsPng(stream, w, h);
                //stream.Dispose();

                //texture.Dispose();
                #endregion

            }
            #endregion
        }

        public bool IsGameEnd = false;
        public void FinalMoveCameraToSilhouette()
        {
            if (Camera.Position.X < _silhouette._body.Position.X) Camera.MoveCamera(new Vector2(0.1f, 0f));
        }
    }

    public enum IsMouseLeftButtonPressed
    {
        Yes,
        No
    }
}

