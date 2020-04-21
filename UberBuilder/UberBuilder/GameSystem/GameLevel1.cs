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
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace UberBuilder.GameSystem
{
    public class GameLevel1 : PhysicsGameScreen//, IDemoScreen
    {
        public float _width;
        public float _height;
        public float _halfWidth;
        public float _halfHeight;

        public Border _border;
        //public WoodBlock _woodBlock;
        public List<BreakableObj1> _blocks;
        public BreakableObj1 _targetBlock;
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

            //_woodBlock = new WoodBlock(
            //    World,
            //    ScreenManager,
            //    new Vector2(0f, 0f),
            //    Camera,
            //    "wood-plank2"/*"Samples/object"*/,
            //    TriangulationAlgorithm.Bayazit, 
            //    new Vector2(0.3f, 0.05f),
            //    50f);
            //_woodBlock.SetTrajectoriesObjects();

            _blocks = new List<BreakableObj1>();

            _uberBuilder = new UBuilder(
                World,
                ScreenManager,
                new Vector2(-halfWidth + 10f, -halfHeight + 10f),
                Camera);
            _uberBuilder._body.BodyType = BodyType.Dynamic;

            _throwTrajectory = new ThrowTrajectory(
                World,
                ScreenManager);
            //_throwTrajectory.SetBodyToThrow(_woodBlock);


            _silhouette = new BuildingSilhouette(
                World,
                ScreenManager,
                new Vector2(this._halfHeight / 2f, 0f)/*new Vector2(0f, 0f)*/,
                Camera,
                new Vector2(20f, 30f)
                /*"silhouettePath",*/);

            TEST();
        }

        public bool ThrowIsCalculated = false;
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            //_woodBlock.Update(this._fixedMouseJoint);
            foreach (var b in _blocks)
            {
                b.Update(this._fixedMouseJoint);
            }
            _uberBuilder.Update();
            _throwTrajectory.Update(this);
            _silhouette.Update();

            if (IsGameEnd) ScreenOfFinalBuilding();
            if (IsCameCanMove)
            {
                if (Camera.Position.X < _silhouette._body.Position.X)
                    Camera.MoveCamera(new Vector2(0.05f, 0f));
                else IsCameCanMove = false;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.BatchEffect.View = Camera.View;
            ScreenManager.BatchEffect.Projection = Camera.Projection;
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, RasterizerState.CullNone, ScreenManager.BatchEffect);

            //_woodBlock.Draw();
            foreach (var b in _blocks)
            {
                b.Draw();
            }
            _uberBuilder.Draw();
            _throwTrajectory.Draw(isMouseLeftButtonPressed);
            _silhouette.Draw(ScreenManager.SpriteBatch);

            TESTDraw();

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
        public BreakableObj1 tBlock = null;
        protected override void HandleCursor(InputHelper input)
        {
            #region "Захват тела и создание объектов-траектории для него"
            Vector2 position = Camera.ConvertScreenToWorld(input.Cursor);//пиксели -> местные координаты???

            if ((input.IsNewButtonPress(Buttons.A) || input.IsNewMouseButtonPress(MouseButtons.LeftButton)) && _fixedMouseJoint == null)
            {
                tBlock = null;
                Fixture savedFixture = World.TestPoint(position);
                if (savedFixture != null)
                {
                    bodyToThrow = savedFixture.Body;
                    if ((tBlock = _blocks.FirstOrDefault(a => a.body == bodyToThrow)) != null)
                    {
                        _throwTrajectory.SetBodyToThrow(tBlock);
                        //bodyToThrow.BodyType = BodyType.Dynamic;
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
                if (bodyToThrow == tBlock.body)
                {
                    throwForce = (_uberBuilder._originPosition - bodyToThrow.Position) * 500f;
                    bodyToThrow.ApplyForce(throwForce);

                    foreach (var e in tBlock.throwableBodies)
                    {
                        if(World.BodyList.Contains(e.Value.body)) 
                            World.Remove(e.Value.body);
                    }
                    isMouseLeftButtonPressed = IsMouseLeftButtonPressed.No;
                }
            }

            if (_fixedMouseJoint != null)
                _fixedMouseJoint.WorldAnchorB = position;
            #endregion

            #region "Смещение камеры к силуэту, его снимок и обработка"
            //if (input.IsNewMouseButtonPress(MouseButtons.RightButton))
            //{
            //    IsGameEnd = true;
            //    HasCursor = false;   
            //}
            #endregion

            if (input.IsNewMouseButtonPress(MouseButtons.RightButton))
            {
                _blocks.Add(new WoodBlock(
                    World,
                    ScreenManager,
                    new Vector2(-10f, 0f),
                    Camera,
                    "wood-plank2"/*"Samples/object"*/,
                    TriangulationAlgorithm.Bayazit,
                    new Vector2(0.3f, 0.05f),
                    50f));
                _blocks.Last().SetTrajectoriesObjects();
            }
        }

        public bool IsGameEnd = false;
        public bool IsColorArrayProcessed = false;
        public bool IsCameCanMove = false;
        public async void ScreenOfFinalBuilding()
        {
            await Task.Run(() =>
            {
                if (!IsColorArrayProcessed)
                {
                    IsColorArrayProcessed = true;

                    #region "save texture segment to png file"
                    int w = ScreenManager.GraphicsDevice.PresentationParameters.BackBufferWidth;
                    int h = ScreenManager.GraphicsDevice.PresentationParameters.BackBufferHeight;

                    //pull the picture from the buffer 
                    int[] backBuffer = new int[w * h];
                    ScreenManager.GraphicsDevice.GetBackBufferData(backBuffer);

                    //copy into a texture 
                    Texture2D texture = new Texture2D(
                        ScreenManager.GraphicsDevice,
                        w, h,
                        false,
                        ScreenManager.GraphicsDevice.PresentationParameters.BackBufferFormat);
                    texture.SetData(backBuffer);

                    Vector2 positionInPixels = (_silhouette._body.Position + new Vector2(this._halfWidth, this._halfHeight)) * 24f;
                    int w1 = (int)(_silhouette._sprite.Size.X);
                    int h1 = (int)(_silhouette._sprite.Size.Y) - 2;
                    Vector2 rectBorderOfObjCoords = new Vector2();
                    rectBorderOfObjCoords.X = positionInPixels.X - (w1 / 2f);
                    rectBorderOfObjCoords.Y = positionInPixels.Y + (h1 / 2f);
                    //Rectangle boundRectangle = ;
                    Color[] colors = new Color[w1 * h1];
                    texture.GetData<Color>(
                        0,
                        new Rectangle(
                            (int)(rectBorderOfObjCoords.X) + 20,
                            /*(int)(rectBorderOfObjCoords.Y)*/0,
                            w1,
                            h1),
                        colors,
                        0,
                        w1 * h1);


                    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(w1, h1);
                    Color[,] rawDataAsGrid = new Color[h1, w1];
                    for (int row = 0; row < h1; row++)
                    {
                        for (int column = 0; column < w1; column++)
                        {
                            // Assumes row major ordering of the array.
                            rawDataAsGrid[row, column] = colors[row * w1 + column];

                            if (rawDataAsGrid[row, column].R >= 40 && rawDataAsGrid[row, column].R <= 70 &&
                                rawDataAsGrid[row, column].G >= 80 && rawDataAsGrid[row, column].G <= 110 &&
                                rawDataAsGrid[row, column].B >= 100 && rawDataAsGrid[row, column].B <= 135)
                            {
                                bitmap.SetPixel(
                                column,
                                row,
                                System.Drawing.Color.FromArgb(255, 255, 255, 255));
                            }
                            else
                            {
                                bitmap.SetPixel(
                                column,
                                row,
                                System.Drawing.Color.FromArgb(255, 0, 0, 0));
                            }

                            //bitmap.SetPixel(
                            //    column,
                            //    row,
                            //    System.Drawing.Color.FromArgb(
                            //    rawDataAsGrid[row, column].A,
                            //    rawDataAsGrid[row, column].R,
                            //    rawDataAsGrid[row, column].G,
                            //    rawDataAsGrid[row, column].B));
                        }
                    }

                    //System.Drawing.Bitmap bitmap2 = new System.Drawing.Bitmap(bitmap, w1 / 5, h1 / 5);
                    using (FileStream fs = new FileStream("C:\\Users\\space\\Рабочий стол\\TESTTESTTESTASSGDF\\1.png", FileMode.Create, FileAccess.ReadWrite))
                    {
                        bitmap.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    texture.Dispose();
                }
            });

            IsCameCanMove = true;//запуск движения камеры
            #endregion
        }



        #region "TEST DOTS"
        Vector2 positionInPixels;
        Vector2 rectBorderOfObjCoords;
        Sprite testDot;
        public void TEST()
        {
            positionInPixels = _silhouette._body.Position;
            rectBorderOfObjCoords = new Vector2();
            rectBorderOfObjCoords.X = positionInPixels.X - (_silhouette._sprite.Size.X * (1f / 24f) / 2f);
            rectBorderOfObjCoords.Y = positionInPixels.Y + (_silhouette._sprite.Size.Y * (1f / 24f) / 2f);


            testDot = new Sprite(ScreenManager.Assets.CircleTexture(1f, MaterialType.Blank, Color.Black, 1f, 24f));
        }
        public void TESTDraw()
        {
            ScreenManager.SpriteBatch.Draw(
                testDot.Texture,
                positionInPixels,
                null,
                Color.White,
                0f,
                testDot.Origin,
                testDot.Size * testDot.TexelSize * (1f / 24f),
                SpriteEffects.FlipVertically,
                0f);

            ScreenManager.SpriteBatch.Draw(
                testDot.Texture,
                rectBorderOfObjCoords,
                null,
                Color.White,
                0f,
                testDot.Origin,
                testDot.Size * testDot.TexelSize * (1f / 24f),
                SpriteEffects.FlipVertically,
                0f);
        }
        #endregion
    }

    public enum IsMouseLeftButtonPressed
    {
        Yes,
        No
    }
}



//47 83 105 255

//// Note that this stores the pixel's row in the first index, and the pixel's column in the second,
//// with this setup.
//Color[,] rawDataAsGrid = new Color[height, width];
//for (int row = 0; row<height; row++)
//{
//    for (int column = 0; column<width; column++)
//    {
//        // Assumes row major ordering of the array.
//        rawDataAsGrid[row, column] = rawData[row * width + column];
//    }
//}