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
using static UberBuilder.GameSystem.GameLevel1;

namespace UberBuilder.GameSystem.ControllableCharacters
{
    public class ThrowTrajectory
    {
        public World _world;
        public ScreenManager _screenManager;

        public Dictionary<int, Body> _throwTrajectory;
        public Sprite dots;

        public ThrowTrajectory(World world, ScreenManager screenManager)
        {
            _world = world;
            _screenManager = screenManager;

            dots = new Sprite(_screenManager.Assets.CircleTexture(1f, MaterialType.Blank, Color.Red, 1f, 24f));
            _throwTrajectory = new Dictionary<int, Body>();
        }

        public bool ThrowIsCalculated = false;
        public void Update(GameLevel1 lvl)
        {
            if (lvl.isMouseLeftButtonPressed == IsMouseLeftButtonPressed.Yes)
            {
                lvl.throwForce = (lvl._uberBuilder._originPosition - lvl.bodyToThrow.Position) * 500f;
                lvl.bodyToThrow.Rotation = 0f;

                //обновления объектов траектории один раз после остановки бросаемого объекта
                if (lvl._uberBuilder._body.LinearVelocity.Length() > new Vector2(0.5f, 0.5f).Length()) ThrowIsCalculated = false;
                if (lvl._uberBuilder._body.LinearVelocity.Length() <= new Vector2(0.5f, 0.5f).Length() && !ThrowIsCalculated)
                {
                    ThrowIsCalculated = true;
                    for (int j = 0; j < 5; j++)
                    {
                        if (_throwTrajectory.ContainsKey(j))
                        {
                            _world.Remove(_throwTrajectory[j]);
                            _throwTrajectory.Remove(j);
                        }

                        _throwTrajectory[j] = _world.CreateCircle(1f, 1f, lvl._uberBuilder._body.Position, BodyType.Dynamic);
                        Category c = (Category)((int)Math.Pow(2, (j + 2)));
                        _throwTrajectory[j].SetCollisionCategories(c);
                        _throwTrajectory[j].SetCollidesWith(c);
                        _throwTrajectory[j].ApplyForce(lvl.throwForce);
                    }
                }
            }

            //стопорит объекты траектории на определенном расстоянии от бросаемого объекта
            int i = 1;
            foreach (var e in _throwTrajectory)
            {
                if (Math.Abs((e.Value.Position - lvl._uberBuilder._body.Position).Length()) > (float)i * 5f)/*!!!*/
                {
                    e.Value.BodyType = BodyType.Static;
                }
                i++;
            }
        }

        public void Draw(IsMouseLeftButtonPressed isMouseLeftButtonPressed)
        {
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
        }
    }
}
