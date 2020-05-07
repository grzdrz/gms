using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Common.Decomposition;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Samples.ScreenSystem;

namespace UberBuilder.GameSystem.ConstructionMaterials
{
    public class WoodBlock : BreakableObj1
    {
        public WoodBlock(
            World world, 
            ScreenManager screenManager,
            Vector2 position,
            Camera2D camera, 
            string texturePath,
            TriangulationAlgorithm triangulationAlgorithm, 
            Vector2 scale,
            float strength,
            float massKoef)
            : base(
                  world,
                  screenManager,
                  position,
                  camera,
                  texturePath,
                  triangulationAlgorithm, 
                  scale,
                  strength,
                  massKoef)
        {

        }
    }
}
