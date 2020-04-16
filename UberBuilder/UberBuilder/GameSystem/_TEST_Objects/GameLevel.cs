using System.Text;
using tainicom.Aether.Physics2D.Samples.Demos.Prefabs;
using tainicom.Aether.Physics2D.Samples.ScreenSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework.Input;
using tainicom.Aether.Physics2D.Samples.Samples.TEST;
using VirusBonker.GameSystem.GameObjects;
using System.Collections.Generic;
using tainicom.Aether.Physics2D.Dynamics;
using VirusBonker.GameSystem.UncontrollableGameObjects;
using System.Linq;

namespace tainicom.Aether.Physics2D.Samples
{//сущность конкретного игрового уровня
    public class GameLevel : PhysicsGameScreen
    {
        #region IDemoScreen Members

        public string GetTitle()
        {
            return "Stacked Objects";
        }

        public string GetDetails()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("This demo shows the stacking stability of the engine.");
            sb.AppendLine("It shows a stack of rectangular bodies stacked in the shape of a pyramid.");
            sb.AppendLine(string.Empty);
            sb.AppendLine("GamePad:");
            sb.AppendLine("  - Rotate agent: left and right triggers");
            sb.AppendLine("  - Move agent: right thumbstick");
            sb.AppendLine("  - Move cursor: left thumbstick");
            sb.AppendLine("  - Grab object (beneath cursor): A button");
            sb.AppendLine("  - Drag grabbed object: left thumbstick");
            sb.AppendLine("  - Exit to menu: Back button");
            sb.AppendLine(string.Empty);
            sb.AppendLine("Keyboard:");
            sb.AppendLine("  - Rotate agent: left and right arrows");
            sb.AppendLine("  - Move agent: A,S,D,W");
            sb.AppendLine("  - Exit to menu: Escape");
            sb.AppendLine(string.Empty);
            sb.AppendLine("Mouse / Touchscreen");
            sb.AppendLine("  - Grab object (beneath cursor): Left click");
            sb.AppendLine("  - Drag grabbed object: move mouse / finger");
            return sb.ToString();
        }

        #endregion

        private MainCharacter _mainCharacter;
        private RubberBall _ball;
        private DefenderCellsManager _background;
        private BorderTEST _border;

        private List<Virus> viruses = new List<Virus>();

        public Vector2 lvlSize;

        public override void LoadContent()
        {
            base.LoadContent();

            HasCursor = true;
            HasVirtualStick = true;
            EnableCameraControl = true;

            World.Gravity = new Vector2(0f, 0f);

            _border = new BorderTEST(World, ScreenManager, Camera);
            _mainCharacter = new MainCharacter(World, ScreenManager, new Vector2(5f, 10f));
            _ball = new RubberBall(World, ScreenManager, new Vector2(-15f, 8f));
            _background = new DefenderCellsManager(World, ScreenManager);

            //установить управляемое тело
            SetUserAgent(_mainCharacter._body, 100f, 10f);


            Camera.MinRotation = 0f;
            Camera.MaxRotation = 0f;

            Camera.TrackingBody = _mainCharacter._body;
            Camera.EnableTracking = true;
            Camera.EnableRotationTracking = false;


            var vp = ScreenManager.GraphicsDevice.Viewport;
            float height = 30f; // 30 meters height
            float width = height * vp.AspectRatio;
            width -= 1.5f; // 1.5 meters border
            height -= 1.5f;
            float halfWidth = width / 2f;
            float halfHeight = height / 2f;
            lvlSize = new Vector2(width, height);
        }

        //private bool Body_OnCollision(Dynamics.Fixture sender, Dynamics.Fixture other, Dynamics.Contacts.Contact contact)
        //{
        //    throw new NotImplementedException();
        //}


        public TimeSpan virusCreationTimeSpan = new TimeSpan(0, 0, 0);
        public Random random = new Random();
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            //новый объект вируса сохдается каждые 3 секунды в случайном месте
            virusCreationTimeSpan = virusCreationTimeSpan.Add(gameTime.ElapsedGameTime);
            if (virusCreationTimeSpan.TotalMilliseconds > 3000)
            {
                virusCreationTimeSpan = new TimeSpan(0, 0, 0);
                viruses.Add(new Virus(
                    World,
                    ScreenManager,
                    new Vector2(
                        random.Next((int)(-lvlSize.X / 2f), (int)(lvlSize.X / 2f)),
                        random.Next((int)(-lvlSize.Y / 2f), (int)(lvlSize.Y / 2f))),
                    0));
            }

            foreach (var virus in viruses)
            {
                if (virus.RotateTEST > 0)
                {
                    //1)визуальное реагирование на контакт посредством выполнения определенной анимации
                    //2)либо наложение сверху еще 1 спрайта-фильтра

                    //изменение поворота тела
                    virus.body.Rotation += virus.RotateTEST;
                    virus.RotateTEST = 0;
                }
            }


            _background.Update(_mainCharacter);
            

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.BatchEffect.View = Camera.View;
            ScreenManager.BatchEffect.Projection = Camera.Projection;
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, RasterizerState.CullNone, ScreenManager.BatchEffect);

            //_ball.Draw();

            _background.Draw();

            _mainCharacter.Draw();

            foreach (var viruse in viruses)
            {
                viruse.Draw();
            }

            ScreenManager.SpriteBatch.End();
            _border.Draw();
            base.Draw(gameTime);
        }

        public float _agentForce = 100f;
        public float _agentTorque = 10f;
        public bool InputButtonDown_W = false;

        protected override void HandleUserAgent(InputHelper input)
        {
            //Клавиатура
            float forceAmount = _agentForce * 1f;

            Vector2 force = Vector2.Zero;
            float torque = 0;

            float HalfPI = ((float)Math.PI / 2f);
            if (input.KeyboardState.IsKeyDown(Keys.W))
            {
                float angle = _mainCharacter._body.Rotation + HalfPI;
                float directingCosx = (float)Math.Cos(angle);
                float directingCosy = (float)Math.Cos(HalfPI - angle);
                Vector2 unitVector = new Vector2(directingCosx, directingCosy);
                force += unitVector * forceAmount;
                InputButtonDown_W = true;//test
            }
            if (input.KeyboardState.IsKeyDown(Keys.D))
            {
                float angle = _mainCharacter._body.Rotation + HalfPI / 2;
                float directingCosx = (float)Math.Cos(angle);
                float directingCosy = (float)Math.Cos(HalfPI - angle);
                Vector2 unitVector = new Vector2(directingCosx, directingCosy);
                force += unitVector * forceAmount;
            }
            if (input.KeyboardState.IsKeyDown(Keys.A))
            {
                float angle = _mainCharacter._body.Rotation + HalfPI + HalfPI / 2;
                float directingCosx = (float)Math.Cos(angle);
                float directingCosy = (float)Math.Cos(HalfPI - angle);
                Vector2 unitVector = new Vector2(directingCosx, directingCosy);
                force += unitVector * forceAmount;
            }
            if (input.KeyboardState.IsKeyDown(Keys.S))
            {
                float angle = _mainCharacter._body.Rotation - HalfPI;
                float directingCosx = (float)Math.Cos(angle);
                float directingCosy = (float)Math.Cos(HalfPI - angle);
                Vector2 unitVector = new Vector2(directingCosx, directingCosy);
                force += unitVector * forceAmount;
            }
            //if (input.KeyboardState.IsKeyDown(Keys.Q))
            //    torque -= _agentTorque;
            //if (input.KeyboardState.IsKeyDown(Keys.E))
            //    torque += _agentTorque;

            if (force == Vector2.Zero)//test
                InputButtonDown_W = false;

            _mainCharacter._body.ApplyForce(force);
            _mainCharacter._body.ApplyTorque(torque);
        }
    }
}

//Управление осуществляется посредством вызова виртуального метода HandleInput корневого класса GameScreen.
//При этом как происходит вызов скрыто, т.к. это встроенный в XNA метод.
//При вызове HandleInput выполняется его переопределенная часть, а затем базовая(см. виртуальные методы).

//В нашем случае в переопределенном методе HandleInput происходит вызов различных методов управления при определенных условиях.
//Внутри этого метода прописан вызов виртуального метода HandleUserAgent при условии наличия экземпляра приватного поля _userAgent.
//Собственно этот метод и задает параметры движения.

//Для настройки управления нужно модифицировать переопределенный метод HandleInput
