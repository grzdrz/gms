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

namespace UberBuilder.GameSystem
{
    public class GameLevel1 : PhysicsGameScreen//, IDemoScreen
    {
        public Border _border;
        public BreakableObj1 _breakableObj;

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

            //DebugView.AppendFlags(DebugViewFlags.Shape);

            World.Gravity = Vector2.Zero;

            _border = new Border(World, ScreenManager, Camera);

            _breakableObj = new BreakableObj1(World, ScreenManager, new Vector2(0, 0), Camera);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            _breakableObj.Update(this._fixedMouseJoint);
        }

        //public override void HandleInput(InputHelper input, GameTime gameTime)
        //{
        //    if (input.IsNewMouseButtonPress(MouseButtons.RightButton) ||
        //        input.IsNewButtonPress(Buttons.B))
        //    {
        //        Vector2 cursorPos = Camera.ConvertScreenToWorld(input.Cursor);

        //        Vector2 min = cursorPos - new Vector2(10, 10);
        //        Vector2 max = cursorPos + new Vector2(10, 10);

        //        AABB aabb = new AABB(ref min, ref max);

        //        World.QueryAABB(fixture =>
        //        {
        //            Vector2 fv = fixture.Body.Position - cursorPos;
        //            fv.Normalize();
        //            fv *= 40;
        //            fixture.Body.ApplyLinearImpulse(ref fv);
        //            return true;
        //        }, ref aabb);
        //    }

        //    base.HandleInput(input, gameTime);
        //}

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.BatchEffect.View = Camera.View;
            ScreenManager.BatchEffect.Projection = Camera.Projection;
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, RasterizerState.CullNone, ScreenManager.BatchEffect);

            _breakableObj.Draw(ScreenManager.SpriteBatch);

            ScreenManager.SpriteBatch.End();

            _border.Draw();
            base.Draw(gameTime);
        }

        public override void UnloadContent()
        {
            //DebugView.RemoveFlags(DebugViewFlags.Shape);

            base.UnloadContent();
        }
    }
}

