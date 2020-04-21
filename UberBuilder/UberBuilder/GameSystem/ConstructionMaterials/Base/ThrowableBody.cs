using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;

namespace UberBuilder.GameSystem.ConstructionMaterials.Base
{
    public interface ThrowableBody
    {
        Body body { get; }
        Dictionary<int, ThrowableBody> throwableBodies { get; }

    }
}
