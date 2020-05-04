/* Original source Farseer Physics Engine:
 * Copyright (c) 2014 Ian Qvist, http://farseerphysics.codeplex.com
 * Microsoft Permissive License (Ms-PL) v1.1
 */

using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Samples.DrawingSystem;
using tainicom.Aether.Physics2D.Samples.ScreenSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics.Joints;
using System;

namespace tainicom.Aether.Physics2D.Samples.Demos.Prefabs
{//сущность менеджера управляемых игровых объектов(1го в данном случае)
    public class AgentTEST
    {
        private Body _body;
        private Sprite _torso;
        private Body _head;
        private Sprite _face;
        private Body _leftJet;
        private Sprite _leftJetEngine;
        private Body _rightJet;
        private Sprite _rightJetEngine;

        private const float LimbAngularDamping = 7;
        //private float _offset;

        private Category _collidesWith;
        private Category _collisionCategories;

        private SpriteBatch _batch;

        public AgentTEST(World world, ScreenManager screenManager, Vector2 position)
        {
            _batch = screenManager.SpriteBatch;

            //_collidesWith = Category.All;
            //_collisionCategories = Category.All;

            CreateBody(world, position);
            CreateJoints(world);

            CreateGFX(screenManager.Assets);
        }

        //Torso
        private void CreateBody(World world, Vector2 position)
        {
            //Head
            _head = world.CreateCircle(1f, 10f);
            _head.BodyType = BodyType.Dynamic;
            _head.AngularDamping = LimbAngularDamping;
            _head.Mass = 2f;
            _head.Position = position;

            //_body
            _body = world.CreateRectangle(2f, 3f, 0.5f);
            _body.BodyType = BodyType.Dynamic;
            _body.Mass = 2f;
            _body.Position = position + new Vector2(0f, -2.5f);

            //JetEngines
            _leftJet = world.CreateCircle(0.5f, 10f);
            _leftJet.BodyType = BodyType.Dynamic;
            _leftJet.AngularDamping = LimbAngularDamping;
            _leftJet.Mass = 0.5f;
            _leftJet.Position = position + new Vector2(-1.5f, -1f);

            _rightJet = world.CreateCircle(0.5f, 10f);
            _rightJet.BodyType = BodyType.Dynamic;
            _rightJet.AngularDamping = LimbAngularDamping;
            _rightJet.Mass = 0.5f;
            _rightJet.Position = position + new Vector2(1.5f, -1f); ;
        }

        private void CreateGFX(AssetCreator assets)
        {
            _face = new Sprite(assets.CircleTexture(1f, MaterialType.Squares, Color.Gray, 1f, 24f));
            _torso = new Sprite(assets.TextureFromVertices(PolygonTools.CreateRectangle(2f, 4f), MaterialType.Squares, Color.LightSlateGray, 0.8f, 24f));
            _leftJetEngine = new Sprite(assets.CircleTexture(1f, MaterialType.Waves, Color.Gray, 1f, 24f));
            _rightJetEngine = new Sprite(assets.CircleTexture(1f, MaterialType.Waves, Color.Gray, 1f, 24f));
        }

        private void CreateJoints(World world)
        {
            const float dampingRatio = 1f;
            const float frequency = 25f;

            //head -> body
            DistanceJoint jHeadBody = new DistanceJoint(_head, _body, new Vector2(0f, -1f), new Vector2(0f, 1.5f));
            jHeadBody.CollideConnected = true;
            jHeadBody.DampingRatio = dampingRatio;
            jHeadBody.Frequency = frequency;
            jHeadBody.Length = 0.025f;
            world.Add(jHeadBody);

            //leftJet -> body
            DistanceJoint jLeftJetBody = new DistanceJoint(_leftJet, _body, new Vector2(0.5f, 0f), new Vector2(-1f, 1.5f));
            jLeftJetBody.CollideConnected = true;
            jLeftJetBody.DampingRatio = dampingRatio;
            jLeftJetBody.Frequency = frequency;
            jLeftJetBody.Length = 0.025f;
            world.Add(jLeftJetBody);

            //rightJet -> body
            DistanceJoint jRightJetBody = new DistanceJoint(_rightJet, _body, new Vector2(-0.5f, 0f), new Vector2(1f, 1.5f));
            jRightJetBody.CollideConnected = true;
            jRightJetBody.DampingRatio = dampingRatio;
            jRightJetBody.Frequency = frequency;
            jRightJetBody.Length = 0.025f;
            world.Add(jRightJetBody);
        }

        public Category CollisionCategories
        {
            get { return _collisionCategories; }
            set
            {
                _collisionCategories = value;
                Body.SetCollisionCategories(value);
            }
        }

        public Category CollidesWith
        {
            get { return _collidesWith; }
            set
            {
                _collidesWith = value;
                Body.SetCollidesWith(value);
            }
        }

        public Body Body
        {
            get { return _body; }
        }

        public Body LeftJetBody
        {
            get { return _leftJet; }
        }

        public Body RightJetBody
        {
            get { return _rightJet; }
        }

        public void Draw()
        {
            _batch.Draw(_torso.Texture, _body.Position, null, Color.White, _body.Rotation, _torso.Origin, new Vector2(2f, 3f) * _torso.TexelSize, SpriteEffects.FlipVertically, 0f);
            _batch.Draw(_face.Texture, _head.Position, null, Color.White, _head.Rotation, _face.Origin, new Vector2(2f * 1f) * _face.TexelSize, SpriteEffects.FlipVertically, 0f);
            _batch.Draw(_leftJetEngine.Texture, _leftJet.Position, null, Color.White, _leftJet.Rotation, _leftJetEngine.Origin, new Vector2(1f, 1f) * _leftJetEngine.TexelSize, SpriteEffects.FlipVertically, 0f);
            _batch.Draw(_rightJetEngine.Texture, _rightJet.Position, null, Color.White, _rightJet.Rotation, _rightJetEngine.Origin, new Vector2(1f, 1f) * _rightJetEngine.TexelSize, SpriteEffects.FlipVertically, 0f);
        }

        public void DrawCentralJetFlame()
        {
            float HalfPI = ((float)Math.PI / 2f);
            float angle = _body.Rotation + HalfPI;
            float directingCosx = (float)Math.Cos(angle);
            float directingCosy = (float)Math.Cos(HalfPI - angle);
            Vector2 unitVector = new Vector2(directingCosx, directingCosy);
            Vector2 test = unitVector * 1.5f;
            Vector2 test2 = _body.Position - test;

            _batch.Draw(_torso.Texture, test2, null, Color.White, _body.Rotation, _torso.Origin, new Vector2(2f, 3f) * _torso.TexelSize, SpriteEffects.FlipVertically, 0f); 
        }
    }
}

//1)При отрисовке используется данные текстуры спрайта. 

//2)Чтобы размер отрисовки объекта совпадал с размером текстуры нужно размер текстуры скалярно умножить на TexelSize текстуры в параметре Scale метода Draw.
//Размер текстуры - это прямоугольник описывающий форму(форма вписанна в этот прямоугольник), размер составлен из указанных величин * 2(указанные величины это чтото типо радиуса прямоугольника)
//НО в объекте спрайта размер пересчитан и отличается от указанных размеров * 2,
//а именно - указанный размер умножается на указанный pixelPerMeter + 1, т.е. получается перевод безразмерной длины в пиксели
//24 пикселя на единицу длины в нашем случае
//TexelSize - это размер текстуры((указанные размеры * 24 + 1) * 2), у которой каждая стороная возведена в -1 степень.

//3)В параметре origin у шара формула : _knob.Origin - new Vector2(0f, _offset) * _knob.Size / new Vector2(2f * 0.5f) == 
// == начальная координата объекта -+ смещение, где смещение = коэффициенту маштабирования смещения по определенной оси(в виде вектора) *
// * на размер самого шара(деление на ед.вектор координат размера дает сам вектор размера, т.е. ниче не меняет),
//т.е. в нашем случае это значит сместить шар вверх на величину его диаметра * коэффициент маштабирования смещения по оси Y = 1(Vector(0,1))