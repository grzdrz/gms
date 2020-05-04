﻿/* Original source Farseer Physics Engine:
 * Copyright (c) 2014 Ian Qvist, http://farseerphysics.codeplex.com
 * Microsoft Permissive License (Ms-PL) v1.1
 */

using System.Text;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Samples.DrawingSystem;
using tainicom.Aether.Physics2D.Samples.ScreenSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Samples.DemosTEST.Prefabs;

namespace tainicom.Aether.Physics2D.Samples.Demos
{
    internal class GomuGomyNoTEST : PhysicsGameScreen, IDemoScreen
    {
        private Border _border;
        //private _body[] _ball = new _body[6];
        private Body _circle;
        private Sprite _circleSprite;

        #region IDemoScreen Members

        public string GetTitle()
        {
            return "Restitution";
        }

        public string GetDetails()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("This demo shows several bodys with varying restitution.");
            sb.AppendLine(string.Empty);
            sb.AppendLine("GamePad:");
            sb.AppendLine("  - Move cursor: left thumbstick");
            sb.AppendLine("  - Grab object (beneath cursor): A button");
            sb.AppendLine("  - Drag grabbed object: left thumbstick");
            sb.AppendLine("  - Exit to menu: Back button");
            sb.AppendLine(string.Empty);
            sb.AppendLine("Keyboard:");
            sb.AppendLine("  - Exit to menu: Escape");
            sb.AppendLine(string.Empty);
            sb.AppendLine("Mouse / Touchscreen");
            sb.AppendLine("  - Grab object (beneath cursor): Left click");
            sb.AppendLine("  - Drag grabbed object: move mouse / finger");
            return sb.ToString();
        }

        #endregion

        public override void LoadContent()
        {
            base.LoadContent();

            World.Gravity = new Vector2(0f, -20f);

            _border = new Border(World, ScreenManager, Camera);

            Vector2 position = new Vector2(-15f, 8f);
            float restitution = 0.7f;//1f - возврат на исходную позицию

            _circle = World.CreateCircle(1.5f, 1f, position);
            _circle.BodyType = BodyType.Dynamic;
            _circle.SetRestitution(restitution);
            //for (int i = 0; i < 6; ++i)
            //{
            //    _ball[i] = World.CreateCircle(1.5f, 1f, position);
            //    _ball[i].BodyType = BodyType.Dynamic;
            //    _ball[i].SetRestitution(restitution);
            //    position.X += 6f;
            //    restitution += 0.2f;
            //}

            // create sprite based on body
            _circleSprite = new Sprite(ScreenManager.Assets.TextureFromShape(_circle.FixtureList[0].Shape, MaterialType.Waves, Color.Brown, 1f));
            //_circleSprite = new Sprite(ScreenManager.Assets.TextureFromShape(_ball[0].FixtureList[0].Shape, MaterialType.Waves, Color.Brown, 1f));
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.BatchEffect.View = Camera.View;
            ScreenManager.BatchEffect.Projection = Camera.Projection;
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, RasterizerState.CullNone, ScreenManager.BatchEffect);
            ScreenManager.SpriteBatch.Draw(_circleSprite.Texture, _circle.Position, null, Color.White, _circle.Rotation, _circleSprite.Origin, new Vector2(2f * 1.5f) * _circleSprite.TexelSize, SpriteEffects.FlipVertically, 0f);
            //for (int i = 0; i < 6; ++i)
            //{
            //    ScreenManager.SpriteBatch.Draw(_circleSprite.Texture, _ball[i].Position, null, Color.White, _ball[i].Rotation, _circleSprite.Origin, new Vector2(2f * 1.5f) * _circleSprite.TexelSize, SpriteEffects.FlipVertically, 0f);
            //}
            ScreenManager.SpriteBatch.End();
            _border.Draw();
            base.Draw(gameTime);
        }
    }
}