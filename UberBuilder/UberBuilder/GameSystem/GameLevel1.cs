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
using tainicom.Aether.Physics2D.Samples.DrawingSystem;
using tainicom.Aether.Physics2D.Dynamics;
using System.IO;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System.Text.RegularExpressions;

namespace UberBuilder.GameSystem
{
    public class GameLevel1 : PhysicsGameScreen//, IDemoScreen
    {
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

        public string lvl { get; } = "lvl1";
        public string pathToOriginalSilhouette { get; set; } = @"C:\Users\space\Рабочий стол\TESTTESTTESTASSGDF\building3.png";
        public string pathToSilhouette { get; set; } = @"b4";
        public float _width { get; set; }
        public float _height { get; set; }
        public float _halfWidth { get; set; }
        public float _halfHeight { get; set; }
        public Border _border { get; set; }
        public List<BreakableObj1> _blocks { get; set; }
        public BuildingSilhouette _silhouette { get; set; }
        //public BreakableObj1 _targetBlock { get; set; }


        public override void LoadContent()
        {
            base.LoadContent();
            HasCursor = true;

            var vp = ScreenManager.GraphicsDevice.Viewport;
            _height = /*30f*/ScreenManager.GraphicsDevice.Viewport.Height * (1f / 24f); // 30 meters height
            _width = _height * vp.AspectRatio;
            _width -= 1.5f; // 1.5 meters border
            _height -= 1.5f;
            float halfWidth = _halfWidth = _width / 2f;
            float halfHeight = _halfHeight = _height / 2f;


            //DebugView.AppendFlags(DebugViewFlags.Shape);

            World.Gravity = new Vector2(0, -9.82f);

            _border = new Border(World, ScreenManager, Camera);

            _blocks = new List<BreakableObj1>();

            _silhouette = new BuildingSilhouette(
                World,
                ScreenManager,
                new Vector2(0f, 0f),
                Camera,
                new Vector2(20f, _height),
                this.pathToSilhouette);

            //TEST();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            foreach (var b in _blocks)
            {
                b.Update(this._fixedMouseJoint);
            }

            _silhouette.Update();


            if (IsGameEnd)
            {
                ScreenOfFinalBuilding();
            }

            //if (IsFirstUpd)
            //{
            //    IsFirstUpd = false;
            //    blockButton = new NewBlockButton();
            //    ScreenManager.AddScreen(blockButton);
            //}
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.BatchEffect.View = Camera.View;
            ScreenManager.BatchEffect.Projection = Camera.Projection;
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, RasterizerState.CullNone, ScreenManager.BatchEffect);

            _silhouette.Draw();
            foreach (var b in _blocks)
            {
                b.Draw();
            }

            //TESTDraw();

            ScreenManager.SpriteBatch.End();

            if(!IsGameEnd)_border.Draw();
            base.Draw(gameTime);
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



        public Random rnd = new Random();
        public Body bodyToThrow { get; set; }
        public BreakableObj1 tBlock { get; set; } = null;
        protected override void HandleCursor(InputHelper input)
        {
            #region "Манипуляции с блоками"
            Vector2 position = Camera.ConvertScreenToWorld(input.Cursor);//пиксели -> местные координаты???

            //захватить блок
            if ((/*input.IsNewButtonPress(Buttons.A) || */input.IsNewMouseButtonPress(MouseButtons.LeftButton)) && _fixedMouseJoint == null)
            {
                tBlock = null;
                Fixture savedFixture = World.TestPoint(position);
                if (savedFixture != null)
                {
                    bodyToThrow = savedFixture.Body;
                    if ((tBlock = _blocks.FirstOrDefault(a => a.body == bodyToThrow)) != null)
                    {
                        //_throwTrajectory.SetBodyToThrow(tBlock);
                        //bodyToThrow.BodyType = BodyType.Dynamic;
                        _fixedMouseJoint = new FixedMouseJoint(bodyToThrow, position);
                        _fixedMouseJoint.MaxForce = 1000.0f * bodyToThrow.Mass;
                        World.Add(_fixedMouseJoint);
                        bodyToThrow.Awake = true;

                        tBlock._blockState = BlockState.Griped;
                        tBlock.fixedRotation = tBlock.body.Rotation;
                    }
                }
                else savedFixture = null;
            }

            //перетаскивать блок
            if ((/*input.IsNewButtonRelease(Buttons.A) || */input.IsNewMouseButtonRelease(MouseButtons.LeftButton)) && _fixedMouseJoint != null)
            {
                if (bodyToThrow == tBlock.body)
                {
                    if (tBlock._blockState == BlockState.Griped)
                    {
                        tBlock.body.AngularVelocity = 0f;
                    }

                    tBlock._blockState = BlockState.Throwed;
                }

                World.Remove(_fixedMouseJoint);
                _fixedMouseJoint = null;
            }

            //заморозить блок
            if (tBlock != null)
            {
                if (input.IsNewKeyPress(Keys.Space) && (tBlock._blockState == BlockState.Throwed || tBlock._blockState == BlockState.Griped))
                {
                    tBlock.body.BodyType = BodyType.Static;
                    //tBlock.body.SetCollisionCategories(Category.None);
                    //tBlock.body.SetCollidesWith(Category.None);

                    tBlock._blockState = BlockState.Throwed;
                }
            }

            //повернуть блок
            if (tBlock != null)
            {
                if ((input.KeyboardState.IsKeyDown(Keys.Z)) && tBlock._blockState == BlockState.Griped)
                {
                        tBlock.fixedRotation += 0.03f;
                }
                if ((input.KeyboardState.IsKeyDown(Keys.C)) && tBlock._blockState == BlockState.Griped)
                {
                    tBlock.fixedRotation -= 0.03f;
                }
            }

            //разморозить блоки
            if (input.IsNewKeyPress(Keys.F))
            {
                foreach (var e in _blocks)
                {
                    e.body.BodyType = BodyType.Dynamic;
                    e.body.SetCollisionCategories(Category.Cat1);
                    e.body.SetCollidesWith(Category.Cat1);
                }
            }

            //создать блок
            if (input.IsNewMouseButtonPress(MouseButtons.RightButton))
            {
                string blockPath = "";
                Vector2 sizeScale = default;
                var blockNum = rnd.Next(1, 4);
                if (blockNum == 1)
                {
                    blockPath = "wood-plank3";
                    sizeScale = new Vector2((float)(rnd.Next(2, 7)) / 10f, 0.04f);
                }
                else if (blockNum == 2)
                {
                    blockPath = "wood-plank33";
                    sizeScale = new Vector2((float)(rnd.Next(2, 7)) / 10f, 0.04f);
                }
                else
                {
                    blockPath = "bootilka1";
                    sizeScale = new Vector2(0.1f, 0.3f);
                }

                _blocks.Add(new WoodBlock(
                    World,
                    ScreenManager,
                    new Vector2(-15f, -10f),
                    Camera,
                    blockPath,
                    TriangulationAlgorithm.Bayazit,
                    sizeScale,
                    50f));
                _blocks.Last()._blockState = BlockState.Created;
            }

            if (_fixedMouseJoint != null)
                _fixedMouseJoint.WorldAnchorB = position;
            #endregion

            #region "Смещение камеры к силуэту, его снимок и обработка"
            if (input.IsNewKeyPress(Keys.Q))
            {
                IsGameEnd = true;
                HasCursor = false;
            }

            //вывести звезды
            if (input.IsNewKeyPress(Keys.E))
            {
                resultMessage = new EndGameResultScreen(this.GetTitle(), this.GetDetails(), starCount);

                ScreenManager.AddScreen(resultMessage);
            }
            #endregion
        }




        public EndGameResultScreen resultMessage { get; set; }
        public int starCount { get; set; } = 0;
        public bool IsGameEnd { get; set; } = false;
        public bool IsColorArrayProcessed { get; set; } = false;
        public void ScreenOfFinalBuilding()
        {
            if (!IsColorArrayProcessed)
            {
                    IsColorArrayProcessed = true;

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

                int w1 = (int)((_silhouette._sprite.Size.X) / _silhouette.scaleKoef.X);
                int h1 = (int)((_silhouette._sprite.Size.Y) / _silhouette.scaleKoef.Y);
                Vector2 positionInPixels = (_silhouette._body.Position + new Vector2(this._halfWidth, this._halfHeight)) * 24f;
                Vector2 rectBorderOfObjCoords = new Vector2();
                rectBorderOfObjCoords.X = positionInPixels.X - (w1 / 2f);
                rectBorderOfObjCoords.Y = positionInPixels.Y + (h1 / 2f);
                Color[] colors = new Color[w1 * h1];
                texture.GetData<Color>(
                    0,
                    new Rectangle(
                        (int)(rectBorderOfObjCoords.X + 0.7f * (24f)), //0.7f * (24f) - ширина бордюра в пикселях
                        (int)(0.7f * (24f)),
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

                        if (rawDataAsGrid[row, column].R == 61 &&
                            rawDataAsGrid[row, column].G == 96 &&
                            rawDataAsGrid[row, column].B == 119) //61 96 119
                        {
                            bitmap.SetPixel(
                            column,
                            row,
                            System.Drawing.Color.FromArgb(0, 255, 255, 255));
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

                #region "Тестовый сейв скриншота"
                using (FileStream fs = new FileStream(@"C:\Users\space\Рабочий стол\TESTTESTTESTASSGDF\resultTEST\333.png", FileMode.Create, FileAccess.ReadWrite))
                {
                    bitmap.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
                }
                #endregion

                #region "saveAsFile"
                //System.Drawing.Bitmap bitmap2 = new System.Drawing.Bitmap(bitmap, w1 / 5, h1 / 5);
                //System.Drawing.Bitmap bitmapSource = new System.Drawing.Bitmap(
                //    new System.Drawing.Bitmap("C:\\Users\\space\\Рабочий стол\\TESTTESTTESTASSGDF\\building3.png"),
                //    w1 / 5,
                //    h1 / 5);
                //using (FileStream fs = new FileStream("C:\\Users\\space\\Рабочий стол\\TESTTESTTESTASSGDF\\11.png", FileMode.Create, FileAccess.ReadWrite))
                //{
                //    bitmap2.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
                //}
                //using (FileStream fs = new FileStream("C:\\Users\\space\\Рабочий стол\\TESTTESTTESTASSGDF\\22.png", FileMode.Create, FileAccess.ReadWrite))
                //{
                //    bitmapSource.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
                //}
                #endregion

                System.Drawing.Bitmap bitmapGame = new System.Drawing.Bitmap(bitmap, w1 / 5, h1 / 5);
                System.Drawing.Bitmap bitmapSource = new System.Drawing.Bitmap(
                    new System.Drawing.Bitmap(this.pathToOriginalSilhouette),
                    w1 / 5,
                    h1 / 5);
                float coincidencesCount = 0.001f;
                float countOfBlackPixelsInSource = 0.001f;
                for (int i = 0; i < bitmapGame.Height; i++)
                {
                    for (int j = 0; j < bitmapGame.Width; j++)
                    {
                        var c1 = bitmapGame.GetPixel(j, i);
                        var c2 = bitmapSource.GetPixel(j, i);
                        if (c1.R == c2.R && c2.R == (byte)0 &&
                           c1.G == c2.G && c2.G == (byte)0 &&
                           c1.B == c2.B && c2.B == (byte)0 &&
                           c1.A == c2.A && c2.A == (byte)255)
                            coincidencesCount++;

                        if (c2.A == 255) countOfBlackPixelsInSource++;
                    }
                }

                float result = coincidencesCount / countOfBlackPixelsInSource;
                if (result > 0.8f) starCount = 5;
                else if (result > 0.6f && result <= 0.8f) starCount = 4;
                else if (result > 0.4f && result <= 0.6f) starCount = 3;
                else if (result > 0.2f && result <= 0.4f) starCount = 2;
                else starCount = 1;

                texture.Dispose();

                resultMessage = new EndGameResultScreen(this.GetTitle(), this.GetDetails(), starCount);
                ScreenManager.AddScreen(resultMessage);

                #region "Сохранение результата"
                ////////////////добавить проценты в файл
                string resultFileContent = "";
                string resultFileContentUpd = "";
                using (FileStream fs = new FileStream(
                    @"C:\Users\space\Рабочий стол\TESTTESTTESTASSGDF\resultTEST\clientGameSaves.txt",
                    FileMode.OpenOrCreate,
                    FileAccess.Read
                    ))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        resultFileContent = sr.ReadToEnd();
                    }
                }
                using (FileStream fs = new FileStream(
                    @"C:\Users\space\Рабочий стол\TESTTESTTESTASSGDF\resultTEST\clientGameSaves.txt",
                    FileMode.Create,
                    FileAccess.Write
                    ))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        Regex regex = new Regex(this.lvl + ":[0-5]{1}");
                        Match match = regex.Match(resultFileContent);
                        if (match.Success)
                            resultFileContentUpd = regex.Replace(resultFileContent, this.lvl + ":" + starCount);
                        else
                            resultFileContentUpd = resultFileContent + (this.lvl + ":" + starCount + ";");

                        sw.WriteLine(resultFileContentUpd);
                    }
                }
                #endregion
            }
        }


        public override void UnloadContent()
        {
            ScreenManager.RemoveScreen(resultMessage);

            //DebugView.RemoveFlags(DebugViewFlags.Shape);
            IsGameEnd = false;
            IsColorArrayProcessed = false;
            starCount = 0;
            bodyToThrow = null;
            tBlock = null;

            base.UnloadContent();
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