﻿using Microsoft.Xna.Framework;
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
    public class BreakableObj1
    {
        public World _world;
        public ScreenManager _screenManager;
        public Camera2D _camera;

        public SegmentableBody _breakableBody { get; set; }
        public List<VertexPositionTexture[]> TESTListOfVertices { get; set; }
        public List<VertexBuffer> vertexBuffers { get; set; }
        public VertexPositionTexture[] triangleVertices { get; set; }
        public BasicEffect effect { get; set; }
        public Texture2D texture { get; set; }

        public List<Vector2> centroids { get; set; }
        public Sprite TESTCentroid { get; set; }

        IndexBuffer indexBuffer3;
        IndexBuffer indexBuffer4;
        IndexBuffer indexBuffer5;
        IndexBuffer indexBuffer6;
        IndexBuffer indexBuffer7;
        IndexBuffer indexBuffer8;

        short[] triangleCubeIndices3_1 =
{
                0,1,2
         };
        short[] triangleCubeIndices4_1 =
{
                0,1,2,
                2,3,0
         };
        short[] triangleCubeIndices5_1 =
{
                0,1,2,
                2,3,0,
                3,4,0
         };
        short[] triangleCubeIndices6_1 =
        {
                0,1,2,
                2,3,0,
                3,4,0,
                4,5,0
         };
        short[] triangleCubeIndices7_1 =
{
                0,1,2,
                2,3,0,
                3,4,0,
                4,5,0,
                5,6,0
         };
        short[] triangleCubeIndices8_1 =
{
                0,1,2,
                2,3,0,
                3,4,0,
                4,5,0,
                5,6,0,
                6,7,0
         };

        Vertices polygon { get; set; }
        Vector2 centroid;

        string _texturePath;
        public BreakableObj1(World world, ScreenManager screenManager, Vector2 position, Camera2D camera, string texturePath, TriangulationAlgorithm triangulationAlgorithm)
        {
            _world = world;
            _screenManager = screenManager;
            //_batch = _screenManager.SpriteBatch;
            _camera = camera;
            _texturePath = texturePath;

            var vp = screenManager.GraphicsDevice.Viewport;
            float height = 30f; // 30 meters height
            float width = height * vp.AspectRatio;
            width -= 1.5f; // 1.5 meters border
            height -= 1.5f;
            float halfWidth = width / 2f;
            float halfHeight = height / 2f;

            //1)Триангуляция текстуры в полигоны
            //Texture2D alphabet = ScreenManager.Content.Load<Texture2D>("Samples/alphabet");
            Texture2D alphabet = _screenManager.Content.Load<Texture2D>(_texturePath);

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
                triangulated = Triangulate.ConvexPartition(polygon, triangulationAlgorithm);

                Vector2 vertScale = new Vector2(13.916667f, 23.25f) / new Vector2(alphabet.Width, alphabet.Height);
                foreach (Vertices vertices in triangulated)
                    vertices.Scale(ref vertScale);

                _breakableBody = new SegmentableBody(_world, triangulated, 1);
                _breakableBody.MainBody.Position = /*new Vector2(0, 0)*/position;
                _breakableBody.Strength = 1200;
            }

            //========================================================================================================
            Random random = new Random();
            //2)Массивы для индексного рассчета вершин под графен
            //2.1)лист вершин для каждого полигона
            List<Vertices> temp = _breakableBody.Parts
                .Select(a => ((PolygonShape)(a.Shape)).Vertices)
                .ToList();
            //2.2)коллекция для подсчета суммарного количества вершин
            IEnumerable<VertexPositionTexture> temp2 = new VertexPositionTexture[0];
            //2.3)буфер вершин под каждый полигон
            vertexBuffers = new List<VertexBuffer>();
            //2.4)координаты центров полигонов для сортировки вершин этих полигонов относительно их центров
            centroids = new List<Vector2>();
            //2.5)сортированый массив вершин и их цветов полигонов для отрисовки
            TESTListOfVertices = new List<VertexPositionTexture[]>();

            //Поиск смещения объекта в отрицательную часть координатной плоскости
            float leftOffsetFromZero = temp.First().First().X;//для всего объекта, а не отдельных полигонов
            float downOffsetFromZero = temp.First().First().Y;
            for (int i = 0; i < temp.Count; i++)
            {
                for (int j = 0; j < temp[i].Count; j++)
                {
                    if (temp[i][j].X <= leftOffsetFromZero) leftOffsetFromZero = temp[i][j].X;
                    if (temp[i][j].Y <= downOffsetFromZero) downOffsetFromZero = temp[i][j].Y;
                }
            }
            leftOffsetFromZero = Math.Abs(leftOffsetFromZero);
            downOffsetFromZero = Math.Abs(downOffsetFromZero);

            for (int i = 0; i < temp.Count; i++)
            {
                var tempUnsorted = temp[i]
                    .Select(a => new VertexPositionTexture(
                        new Vector3(a.X, a.Y, 0f),
                        /*Color.Crimson*/new Vector2(a.X, a.Y)))
                    .ToArray();

                temp2 = temp2.Concat(tempUnsorted);

                var centr = temp[i].GetCentroid();
                centroids.Add(centr);

                var temp3 = VertexClockwiseSort(tempUnsorted, centr);

                for (int j = 0; j < temp3.Length; j++)
                {
                    temp3[j].TextureCoordinate.X = (temp3[j].TextureCoordinate.X + leftOffsetFromZero) / ((width + leftOffsetFromZero));
                    temp3[j].TextureCoordinate.Y = (temp3[j].TextureCoordinate.Y + downOffsetFromZero) / ((height + downOffsetFromZero));
                }

                TESTListOfVertices.Add(temp3);
            }
            triangleVertices = temp2.ToArray();

            //(2.2)
            int verticesCount = 0;
            foreach (var e in TESTListOfVertices)
            {
                verticesCount += e.Length;
            }
            //(2.3)
            foreach (var e in TESTListOfVertices)
            {
                var vb = new VertexBuffer(
                        _screenManager.GraphicsDevice,
                        typeof(VertexPositionTexture),
                        e.Length,
                        BufferUsage.None);
                vb.SetData(e);
                vertexBuffers.Add(vb);
            }

            effect = new BasicEffect(_screenManager.GraphicsDevice);
            texture = _screenManager.Content.Load<Texture2D>("wood2");
            effect.TextureEnabled = true;
            effect.Texture = texture;

            //буферы индексов для полигонов с разным числом вершин
            indexBuffer3 = new IndexBuffer(_screenManager.GraphicsDevice, typeof(short), 36, BufferUsage.WriteOnly);
            indexBuffer3.SetData<short>(triangleCubeIndices3_1);
            indexBuffer4 = new IndexBuffer(_screenManager.GraphicsDevice, typeof(short), 36, BufferUsage.WriteOnly);
            indexBuffer4.SetData<short>(triangleCubeIndices4_1);
            indexBuffer5 = new IndexBuffer(_screenManager.GraphicsDevice, typeof(short), 36, BufferUsage.WriteOnly);
            indexBuffer5.SetData<short>(triangleCubeIndices5_1);
            indexBuffer6 = new IndexBuffer(_screenManager.GraphicsDevice, typeof(short), 36, BufferUsage.WriteOnly);
            indexBuffer6.SetData<short>(triangleCubeIndices6_1);
            indexBuffer7 = new IndexBuffer(_screenManager.GraphicsDevice, typeof(short), 36, BufferUsage.WriteOnly);
            indexBuffer7.SetData<short>(triangleCubeIndices7_1);
            indexBuffer8 = new IndexBuffer(_screenManager.GraphicsDevice, typeof(short), 36, BufferUsage.WriteOnly);
            indexBuffer8.SetData<short>(triangleCubeIndices8_1);

            //точка для отрисовки центрода(ТЕСТ)
            TESTCentroid = new Sprite(_screenManager.Assets.CircleTexture(0.1f, MaterialType.Squares, Color.Black, 1f, 24f));
        }

        public virtual void Update(FixedMouseJoint fixedMouseJoint)
        {
            if (_breakableBody.State == SegmentableBody.BreakableBodyState.ShouldBreak)
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

        public void Draw(SpriteBatch batch)
        {
            for (int i = 0; i < TESTListOfVertices.Count; i++)
            {
                var p = _camera.Projection;
                var v = _camera.View;
                //поворот и координаты(именно в таком порядке) полигона
                Matrix w;
                if (_breakableBody.State == SegmentableBody.BreakableBodyState.Unbroken)
                {
                    w = Matrix.CreateRotationZ(_breakableBody.MainBody.Rotation);
                    w *= Matrix.CreateWorld(
                        new Vector3(_breakableBody.MainBody.Position.X, _breakableBody.MainBody.Position.Y, 0f),
                        new Vector3(0, 0, -2),
                        Vector3.Up);
                }
                else
                {
                    w = Matrix.CreateRotationZ(_breakableBody._bodiesAfterSegmented[i].Rotation);
                    w *= Matrix.CreateWorld(
                        new Vector3(
                            _breakableBody._bodiesAfterSegmented[i].Position.X,
                            _breakableBody._bodiesAfterSegmented[i].Position.Y,
                            0f),
                        new Vector3(0, 0, -2),
                        Vector3.Up);
                }

                // установка буфера вершин
                _screenManager.GraphicsDevice.SetVertexBuffer(vertexBuffers[i]);
                //установка матриц эффекта
                effect.World = w;
                effect.View = v;
                effect.Projection = p;

                int _debugger_;
                short[] indexes = triangleCubeIndices3_1;
                if (TESTListOfVertices[i].Length == 3)
                {
                    _screenManager.GraphicsDevice.Indices = indexBuffer3;
                    indexes = triangleCubeIndices3_1;
                }
                else if (TESTListOfVertices[i].Length == 4)
                {
                    _screenManager.GraphicsDevice.Indices = indexBuffer4;
                    indexes = triangleCubeIndices4_1;
                }
                else if (TESTListOfVertices[i].Length == 5)
                {
                    _screenManager.GraphicsDevice.Indices = indexBuffer5;
                    indexes = triangleCubeIndices5_1;
                }
                else if (TESTListOfVertices[i].Length == 6)
                {
                    _screenManager.GraphicsDevice.Indices = indexBuffer6;
                    indexes = triangleCubeIndices6_1;
                }
                else if (TESTListOfVertices[i].Length == 7)
                {
                    _screenManager.GraphicsDevice.Indices = indexBuffer7;
                    indexes = triangleCubeIndices7_1;
                }
                else if (TESTListOfVertices[i].Length == 8)
                {
                    _screenManager.GraphicsDevice.Indices = indexBuffer8;
                    indexes = triangleCubeIndices8_1;
                }
                else
                    _debugger_ = 0;

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    _screenManager.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>(
                        PrimitiveType.TriangleList,
                        TESTListOfVertices[i],
                        0,
                        TESTListOfVertices[i].Length,
                        indexes,
                        0,
                        TESTListOfVertices[i].Length - 2);
                }


                //_batch.Draw(//тестовая точка
                //    TESTCentroid.Texture,
                //    centroids[i],
                //    null,
                //    Color.White,
                //    0f,
                //    TESTCentroid.Origin,
                //    TESTCentroid.Size * TESTCentroid.TexelSize * (1f / 24f),
                //    SpriteEffects.FlipVertically,
                //    0f);
            }
        }

        public VertexPositionTexture[] VertexClockwiseSort(VertexPositionTexture[] vpc, Vector2 centroid)
        {
            var test = vpc.Select(a => new TwoVectorsContainer1(a, a.Position - new Vector3(centroid.X, centroid.Y, 0f))).ToArray();
            Array.Sort(test, new ClockwiseVector2Comparer1());
            return test.Select(b => b.origin).ToArray();
        }

    }

    public class TwoVectorsContainer1//контейнер для сортировки вершин относительно центройда их полигона
    {
        public VertexPositionTexture origin;
        public Vector3 offseted;

        public TwoVectorsContainer1(VertexPositionTexture v1, Vector3 v2)
        {
            origin = v1;
            offseted = v2;
        }
    }

    //сортировка относительно центра полигона, а не центра поля!!!!
    public class ClockwiseVector2Comparer1 : IComparer<TwoVectorsContainer1>
    {
        public int Compare(TwoVectorsContainer1 v1, TwoVectorsContainer1 v2)
        {
            //координаты радиус векторов до точек равны координатам точек
            var point1 = (TwoVectorsContainer1)v1;
            var point2 = (TwoVectorsContainer1)v2;

            double angle1 = Math.Atan2(point1.offseted.Y, point1.offseted.X);
            double angle2 = Math.Atan2(point2.offseted.Y, point2.offseted.X);

            //на выходе угол получается в диапазоне от -pi до pi,
            //а нужно в диапазоне от 0 до 2pi 
            if (angle1 < 0) angle1 += 2 * Math.PI;
            if (angle2 < 0) angle2 += 2 * Math.PI;

            return angle2.CompareTo(angle1);
        }
    }
}