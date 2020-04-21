using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Joints;
using tainicom.Aether.Physics2D.Samples.DrawingSystem;
using tainicom.Aether.Physics2D.Samples.ScreenSystem;
using UberBuilder.GameSystem.ConstructionMaterials.Base;
using static UberBuilder.GameSystem.GameLevel1;

namespace UberBuilder.GameSystem.ControllableCharacters
{
    public class ThrowTrajectory
    {
        public World _world;
        public ScreenManager _screenManager;

        //public Dictionary<int, Body> _throwTrajectory;
        public Sprite dots;
        public Dictionary<int, ThrowableBody> _throwTrajectory;
        public ThrowableBody _bodyToThrow;

        public ThrowTrajectory(World world, ScreenManager screenManager)
        {
            _world = world;
            _screenManager = screenManager;

            _throwTrajectory = new Dictionary<int, ThrowableBody>();
        }

        public bool ThrowIsCalculated = false;
        public void Update(GameLevel1 lvl)
        {
            if (lvl.isMouseLeftButtonPressed == IsMouseLeftButtonPressed.Yes)
            {
                lvl.throwForce = (lvl._uberBuilder._originPosition - lvl.bodyToThrow.Position) * 500f;
                //lvl.bodyToThrow.Rotation = 0f;

                //обновления объектов траектории один раз после остановки бросаемого объекта
                if (_bodyToThrow.body.LinearVelocity.Length() > new Vector2(0.5f, 0.5f).Length() &&
                     Math.Abs(_bodyToThrow.body.AngularVelocity) > 0f) ThrowIsCalculated = false;
                if (_bodyToThrow.body.LinearVelocity.Length() <= new Vector2(0.5f, 0.5f).Length() &&
                    Math.Abs(_bodyToThrow.body.AngularVelocity) <= 0f &&
                    !ThrowIsCalculated)
                {
                    ThrowIsCalculated = true;
                    for (int j = 0; j < 5; j++)
                    {
                        if (_throwTrajectory.ContainsKey(j) && _world.BodyList.Contains(_throwTrajectory[j].body))
                        {
                            _world.Remove(_throwTrajectory[j].body);
                        }

                        _world.Add(_throwTrajectory[j].body);
                        _throwTrajectory[j].body.ResetDynamics();
                        _throwTrajectory[j].body.Position = _bodyToThrow.body.Position;
                        _throwTrajectory[j].body.Rotation = _bodyToThrow.body.Rotation;
                        _throwTrajectory[j].body.BodyType = BodyType.Dynamic;
                        _throwTrajectory[j].body.ApplyForce(lvl.throwForce);
                    }
                }
            }

            //стопорит объекты траектории на определенном расстоянии от бросаемого объекта
            int lengthCounter = 1;
            foreach (var e in _throwTrajectory)
            {
                if (_world.BodyList.Contains(e.Value.body))
                {
                    if (Math.Abs((e.Value.body.Position - lvl.bodyToThrow.Position).Length()) > (float)lengthCounter * 5f)/*!!!*/
                    {
                        e.Value.body.BodyType = BodyType.Static;
                    }
                }
                lengthCounter++;
            }
        }

        public void Draw(IsMouseLeftButtonPressed isMouseLeftButtonPressed)
        {
            if (isMouseLeftButtonPressed == IsMouseLeftButtonPressed.Yes)
            {
            }
        }

        public void SetBodyToThrow(ThrowableBody bodyToThrow)
        {
            _bodyToThrow = bodyToThrow;
            _throwTrajectory = bodyToThrow.throwableBodies;
        }
    }
}
