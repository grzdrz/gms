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

        public Dictionary<int, Body> _throwTrajectory;
        public Sprite dots;

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

            dots = new Sprite(ScreenManager.Assets.CircleTexture(1f, MaterialType.Blank, Color.Red, 1f, 24f));

            _throwTrajectory = new Dictionary<int, Body>();
        }

        public bool ThrowIsCalculated = false;
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            _woodBlock.Update(this._fixedMouseJoint);
            _uberBuilder.Update();

            if (isMouseLeftButtonPressed == IsMouseLeftButtonPressed.Yes)
            {
                throwForce = (_uberBuilder._originPosition - bodyToThrow.Position) * 500f;
                bodyToThrow.Rotation = 0f;

                //обновления объектов траектории один раз после остановки бросаемого объекта
                if (_uberBuilder._body.LinearVelocity.Length() > new Vector2(0.5f, 0.5f).Length()) ThrowIsCalculated = false;
                if (_uberBuilder._body.LinearVelocity.Length() <= new Vector2(0.5f, 0.5f).Length() && !ThrowIsCalculated)
                {
                    ThrowIsCalculated = true;
                    for (int j = 0; j < 5; j++)
                    {
                        if (_throwTrajectory.ContainsKey(j))
                        {
                            World.Remove(_throwTrajectory[j]);
                            _throwTrajectory.Remove(j);
                        }

                        _throwTrajectory[j] = World.CreateCircle(1f, 1f, _uberBuilder._body.Position, BodyType.Dynamic);
                        Category c = (Category)((int)Math.Pow(2, (j + 2)));
                        _throwTrajectory[j].SetCollisionCategories(c);
                        _throwTrajectory[j].SetCollidesWith(c);
                        _throwTrajectory[j].ApplyForce(throwForce);
                    }
                }
            }

            //стопорит объекты траектории на определенном расстоянии от бросаемого объекта
            int i = 1;
            foreach (var e in _throwTrajectory)
            {
                if (Math.Abs((e.Value.Position - _uberBuilder._body.Position).Length()) > (float)i * 5f)/*!!!*/
                {
                    e.Value.BodyType = BodyType.Static;
                }
                i++;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.BatchEffect.View = Camera.View;
            ScreenManager.BatchEffect.Projection = Camera.Projection;
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, RasterizerState.CullNone, ScreenManager.BatchEffect);

            _woodBlock.Draw(ScreenManager.SpriteBatch);
            _uberBuilder.Draw();

            if (isMouseLeftButtonPressed == IsMouseLeftButtonPressed.Yes)
            {
                //for (int i = 0; i < _throwTrajectory.Count; i++)
                //{
                //    ScreenManager.SpriteBatch.Draw(
                //        dots.Texture,
                //        _throwTrajectory[i].Position,
                //        null,
                //        Color.White,
                //        _throwTrajectory[i].Rotation,
                //        dots.Origin,
                //        dots.Size * dots.TexelSize * (1f / 24f),
                //        SpriteEffects.FlipVertically,
                //        0f);
                //}
            }

            ScreenManager.SpriteBatch.End();

            _border.Draw();
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
        }

        public enum IsMouseLeftButtonPressed
        {
            Yes,
            No
        }
    }
}

