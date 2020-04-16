using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Common.Decomposition;
using tainicom.Aether.Physics2D.Common.PhysicsLogic;
using tainicom.Aether.Physics2D.Common.PolygonManipulation;
using tainicom.Aether.Physics2D.Diagnostics;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Joints;
using tainicom.Aether.Physics2D.Samples.DrawingSystem;
using tainicom.Aether.Physics2D.Samples.ScreenSystem;

namespace UberBuilder.GameSystem.ConstructionMaterials
{
    public class BreakableObj
    {
        public World _world;
        public ScreenManager _screenManager;
        public Camera2D _camera;

        public BreakableBody _breakableBody;
        public SpriteBatch _batch;

        //public List<Sprite> _bodySprites;
        //public Sprite _mainBodySprite;

        //public List<Vector2> _bodiesPositions;
        //public List<Body> _particialBodies;

        public PrimitiveBatch batch;
        public List<Vertices> verticesList;
        public VertexPositionColor[] vertexPositionColors;
        public List<VertexPositionColor[]> TESTListOfVertices;

        IndexBuffer indexBuffer;
        ushort[] triangleCubeIndices3 =
{
                0,1,2
         };
        ushort[] triangleCubeIndices4 =
{
                0,1,2,
                2,3,0
         };
        ushort[] triangleCubeIndices5 =
{
                0,1,2,
                2,3,0,
                3,4,0
         };
        ushort[] triangleCubeIndices6 =
        {
                0,1,2,
                2,3,0,
                3,4,0,
                4,5,0
         };

        Vertices polygon;
        Vector2 centroid;
        public BreakableObj(World world, ScreenManager screenManager, Vector2 position, Camera2D camera)
        {
            _world = world;
            _screenManager = screenManager;
            _batch = _screenManager.SpriteBatch;
            _camera = camera;

            //Texture2D alphabet = ScreenManager.Content.Load<Texture2D>("Samples/alphabet");
            Texture2D alphabet = _screenManager.Content.Load<Texture2D>("Samples/car");

            uint[] data = new uint[alphabet.Width * alphabet.Height];
            alphabet.GetData(data);

            List<Vertices> list = PolygonTools.CreatePolygon(data, alphabet.Width, 3.5f, 20, true, true);

            for (int i = 0; i < list.Count; i++)
                list[i].Scale(new Vector2(1f, -1f)); // flip Vert

            List<Vertices> triangulated;
            for (int i = 0; i < list.Count; i++)
            {
                polygon = list[i];
                centroid = -polygon.GetCentroid();
                polygon.Translate(ref centroid);
                polygon = SimplifyTools.CollinearSimplify(polygon);
                polygon = SimplifyTools.ReduceByDistance(polygon, 4);
                triangulated = Triangulate.ConvexPartition(polygon, TriangulationAlgorithm.Bayazit);

                Vector2 vertScale = new Vector2(13.916667f, 23.25f) / new Vector2(alphabet.Width, alphabet.Height);
                foreach (Vertices vertices in triangulated)
                    vertices.Scale(ref vertScale);

                _breakableBody = new BreakableBody(_world, triangulated, 1);
                _breakableBody.MainBody.Position = new Vector2(0, 0);
                _breakableBody.Strength = 500;
            }

            //========================================================================================================

            #region "LLD"
            //var temp = _breakableBody.Parts
            //    .Select(a => ((PolygonShape)(a.Shape)).Vertices)
            //    .ToList();
            //IEnumerable<VertexPositionColor> temp2 = new VertexPositionColor[0];
            //TESTListOfVertices = new List<VertexPositionColor[]>();
            //for (int i = 0; i < temp.Count; i++)
            //{
            //    var tempUnsorted = temp[i]
            //        .Select(a => new VertexPositionColor(new Vector3(a.X, a.Y, 0f), Color.Crimson))
            //        .ToArray();
            //    Array.Sort(tempUnsorted, new ClockwiseVector2Comparer());
            //    TESTListOfVertices.Add(tempUnsorted);
            //    temp2 = temp2.Concat(tempUnsorted);
            //}
            //triangleVertices = temp2.ToArray();

            //int verticesCount = 0;
            //foreach (var e in TESTListOfVertices)
            //{
            //    verticesCount += e.Length;
            //}

            //// Создаем буфер вершин
            //vertexBuffer = new VertexBuffer(
            //    _screenManager.GraphicsDevice,
            //    typeof(VertexPositionColor),
            //    triangleVertices.Length,
            //    BufferUsage.None);
            //// установка буфера вершин
            ////vertexBuffer.SetData(triangleVertices);
            //// Создаем BasicEffect
            //effect = new BasicEffect(_screenManager.GraphicsDevice);
            //effect.VertexColorEnabled = true;
            #endregion

            vertexPositionColors = new VertexPositionColor[6];
            vertexPositionColors[0] = new VertexPositionColor(new Vector3(1.064862f, 11.57255f, 0f), Color.DarkCyan);
            vertexPositionColors[1] = new VertexPositionColor(new Vector3(4.312084f, 0.6741097f, 0f), Color.DarkCyan);
            vertexPositionColors[2] = new VertexPositionColor(new Vector3(3.384306f, -3.685265f, 0f), Color.DarkCyan);
            vertexPositionColors[3] = new VertexPositionColor(new Vector3(-3.226111f, -3.685265f, 0f), Color.DarkCyan);
            vertexPositionColors[4] = new VertexPositionColor(new Vector3(-4.037916f, 0.6741097f, 0f), Color.DarkCyan);
            vertexPositionColors[5] = new VertexPositionColor(new Vector3(-1.486527f, 11.57255f, 0f), Color.DarkCyan);

            // Создаем буфер вершин
            vertexBuffer = new VertexBuffer(
                _screenManager.GraphicsDevice,
                typeof(VertexPositionColor),
                vertexPositionColors.Length,
                BufferUsage.None);
            // установка буфера вершин
            vertexBuffer.SetData(vertexPositionColors);
            // Создаем BasicEffect
            effect = new BasicEffect(_screenManager.GraphicsDevice);
            effect.VertexColorEnabled = true;

            indexBuffer = new IndexBuffer(_screenManager.GraphicsDevice, typeof(ushort), 36, BufferUsage.WriteOnly);
            indexBuffer.SetData<ushort>(triangleCubeIndices6);
        }

        public virtual void Update(FixedMouseJoint fixedMouseJoint)
        {
            if (_breakableBody.State == BreakableBody.BreakableBodyState.ShouldBreak)
            {
                // save MouseJoint position
                Vector2? worldAnchor = null;
                for (JointEdge je = _breakableBody.MainBody.JointList; je != null; je = je.Next)
                {
                    if (je.Joint == fixedMouseJoint)
                    {
                        worldAnchor = fixedMouseJoint.WorldAnchorA;
                        break;
                    }
                }

                // break body
                _breakableBody.Update();

                // restore MouseJoint
                if (worldAnchor != null && fixedMouseJoint == null)
                {
                    var ficture = _world.TestPoint(worldAnchor.Value);
                    if (ficture != null)
                    {
                        fixedMouseJoint = new FixedMouseJoint(ficture.Body, worldAnchor.Value);
                        fixedMouseJoint.MaxForce = 1000.0f * ficture.Body.Mass;
                        _world.Add(fixedMouseJoint);
                    }
                }
                //CreateGFX(_breakableBody);
            }
            else
            {
                _breakableBody.Update();
            }
        }

        public VertexPositionColor[] triangleVertices;
        public VertexBuffer vertexBuffer;
        public BasicEffect effect;
        public void Draw(SpriteBatch batch)
        {
            #region "LOW LEVEL DRAW"
            //for (int i = 0; i < TESTListOfVertices.Count; i++)
            //{
            //    vertexBuffer.SetData(TESTListOfVertices[i]);

            //    var p = _camera.Projection;
            //    var v = _camera.View;
            //    var w = Matrix.CreateRotationZ(_breakableBody.MainBody.Rotation);
            //    w *= Matrix.CreateWorld(
            //        new Vector3(_breakableBody.MainBody.Position.X, _breakableBody.MainBody.Position.Y, 0f),
            //        new Vector3(0, 0, -2),
            //        Vector3.Up);

            //    // установка буфера вершин
            //    _screenManager.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            //    //установка матриц эффекта
            //    effect.World = w;
            //    effect.View = v;
            //    effect.Projection = p;

            //    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            //    {
            //        pass.Apply();

            //        _screenManager.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
            //            PrimitiveType.TriangleList,
            //            TESTListOfVertices[i],
            //            0,
            //            (TESTListOfVertices[i].Length / 3));
            //    }
            //}
            #endregion

            var p = _camera.Projection;
            var v = _camera.View;
            var w = Matrix.CreateRotationZ(_breakableBody.MainBody.Rotation);
            w *= Matrix.CreateWorld(
                new Vector3(_breakableBody.MainBody.Position.X, _breakableBody.MainBody.Position.Y, 0f),
                new Vector3(0, 0, -2),
                Vector3.Up);

            // установка буфера вершин
            _screenManager.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            //установка матриц эффекта
            effect.World = w;
            effect.View = v;
            effect.Projection = p;

            //foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            //{
            //    pass.Apply();

            //    _screenManager.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
            //        PrimitiveType.TriangleStrip,
            //        vertexPositionColors,
            //        0,
            //        /*(vertexPositionColors.Length / 3)*/10);
            //}
            _screenManager.GraphicsDevice.Indices = indexBuffer;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _screenManager.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexPositionColors.Length, 0, vertexPositionColors.Length - 2);
            }
        }

        //public int PolygonsCount()
    }

    public class ClockwiseVector2Comparer : IComparer<VertexPositionColor>
    {
        public int Compare(VertexPositionColor v1, VertexPositionColor v2)
        {
            if (v1.Position.X >= 0)
            {
                if (v2.Position.X < 0)
                {
                    return -1;
                }
                return -Comparer<float>.Default.Compare(v1.Position.Y, v2.Position.Y);
            }
            else
            {
                if (v2.Position.X >= 0)
                {
                    return 1;
                }
                return Comparer<float>.Default.Compare(v1.Position.Y, v2.Position.Y);
            }
        }
    }
}
