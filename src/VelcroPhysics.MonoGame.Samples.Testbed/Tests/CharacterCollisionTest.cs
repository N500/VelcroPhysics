﻿/*
* Velcro Physics:
* Copyright (c) 2017 Ian Qvist
* 
* Original source Box2D:
* Copyright (c) 2006-2011 Erin Catto http://www.box2d.org 
* 
* This software is provided 'as-is', without any express or implied 
* warranty.  In no event will the authors be held liable for any damages 
* arising from the use of this software. 
* Permission is granted to anyone to use this software for any purpose, 
* including commercial applications, and to alter it and redistribute it 
* freely, subject to the following restrictions: 
* 1. The origin of this software must not be misrepresented; you must not 
* claim that you wrote the original software. If you use this software 
* in a product, an acknowledgment in the product documentation would be 
* appreciated but is not required. 
* 2. Altered source versions must be plainly marked as such, and must not be 
* misrepresented as being the original software. 
* 3. This notice may not be removed or altered from any source distribution. 
*/

using System;
using Genbox.VelcroPhysics.Collision.Shapes;
using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.MonoGame.Samples.Testbed.Framework;
using Genbox.VelcroPhysics.Shared;
using Genbox.VelcroPhysics.Templates;
using Genbox.VelcroPhysics.Utilities;
using Microsoft.Xna.Framework;

namespace Genbox.VelcroPhysics.MonoGame.Samples.Testbed.Tests
{
    internal class CharacterCollisionTest : Test
    {
        private readonly Body _character;

        private CharacterCollisionTest()
        {
            // Ground body
            {
                BodyDef bd = new BodyDef();
                Body ground = World.CreateBody(bd);

                EdgeShape shape = new EdgeShape(new Vector2(-20.0f, 0.0f), new Vector2(20.0f, 0.0f));
                ground.CreateFixture(shape);
            }

            // Collinear edges with no adjacency information.
            // This shows the problematic case where a box shape can hit
            // an internal vertex.
            {
                BodyDef bd = new BodyDef();
                Body ground = World.CreateBody(bd);

                EdgeShape shape = new EdgeShape(new Vector2(-8.0f, 1.0f), new Vector2(-6.0f, 1.0f));
                ground.CreateFixture(shape);
                shape.SetTwoSided(new Vector2(-6.0f, 1.0f), new Vector2(-4.0f, 1.0f));
                ground.CreateFixture(shape);
                shape.SetTwoSided(new Vector2(-4.0f, 1.0f), new Vector2(-2.0f, 1.0f));
                ground.CreateFixture(shape);
            }

            // Chain shape
            {
                BodyDef bd = new BodyDef();
                bd.Angle = 0.25f * MathConstants.Pi;
                Body ground = World.CreateBody(bd);

                Vertices vs = new Vertices(4);
                vs.Add(new Vector2(5.0f, 7.0f));
                vs.Add(new Vector2(6.0f, 8.0f));
                vs.Add(new Vector2(7.0f, 8.0f));
                vs.Add(new Vector2(8.0f, 7.0f));
                ChainShape shape = new ChainShape(vs, true);
                ground.CreateFixture(shape);
            }

            // Square tiles. This shows that adjacency shapes may
            // have non-smooth collision. There is no solution
            // to this problem.
            {
                BodyDef bd = new BodyDef();
                Body ground = World.CreateBody(bd);

                PolygonShape shape = new PolygonShape(0.0f);
                shape.SetAsBox(1.0f, 1.0f, new Vector2(4.0f, 3.0f), 0.0f);
                ground.CreateFixture(shape);
                shape.SetAsBox(1.0f, 1.0f, new Vector2(6.0f, 3.0f), 0.0f);
                ground.CreateFixture(shape);
                shape.SetAsBox(1.0f, 1.0f, new Vector2(8.0f, 3.0f), 0.0f);
                ground.CreateFixture(shape);
            }

            // Square made from an edge loop. Collision should be smooth.
            {
                BodyDef bd = new BodyDef();
                Body ground = World.CreateBody(bd);

                Vertices vs = new Vertices(4);
                vs.Add(new Vector2(-1.0f, 3.0f));
                vs.Add(new Vector2(1.0f, 3.0f));
                vs.Add(new Vector2(1.0f, 5.0f));
                vs.Add(new Vector2(-1.0f, 5.0f));
                ChainShape shape = new ChainShape(vs, true);
                ground.CreateFixture(shape);
            }

            // Edge loop. Collision should be smooth.
            {
                BodyDef bd = new BodyDef();
                bd.Position = new Vector2(-10.0f, 4.0f);
                Body ground = World.CreateBody(bd);

                Vertices vs = new Vertices(10);
                vs.Add(new Vector2(0.0f, 0.0f));
                vs.Add(new Vector2(6.0f, 0.0f));
                vs.Add(new Vector2(6.0f, 2.0f));
                vs.Add(new Vector2(4.0f, 1.0f));
                vs.Add(new Vector2(2.0f, 2.0f));
                vs.Add(new Vector2(0.0f, 2.0f));
                vs.Add(new Vector2(-2.0f, 2.0f));
                vs.Add(new Vector2(-4.0f, 3.0f));
                vs.Add(new Vector2(-6.0f, 2.0f));
                vs.Add(new Vector2(-6.0f, 0.0f));
                ChainShape shape = new ChainShape(vs, true);
                ground.CreateFixture(shape);
            }

            // Square character 1
            {
                BodyDef bd = new BodyDef();
                bd.Position = new Vector2(-3.0f, 8.0f);
                bd.Type = BodyType.Dynamic;
                bd.FixedRotation = true;
                bd.AllowSleep = false;

                Body body = World.CreateBody(bd);

                PolygonShape shape = new PolygonShape(20.0f);
                shape.SetAsBox(0.5f, 0.5f);

                FixtureDef fd = new FixtureDef();
                fd.Shape = shape;
                body.CreateFixture(fd);
            }

            // Square character 2
            {
                BodyDef bd = new BodyDef();
                bd.Position = new Vector2(-5.0f, 5.0f);
                bd.Type = BodyType.Dynamic;
                bd.FixedRotation = true;
                bd.AllowSleep = false;

                Body body = World.CreateBody(bd);

                PolygonShape shape = new PolygonShape(20.0f);
                shape.SetAsBox(0.25f, 0.25f);

                FixtureDef fd = new FixtureDef();
                fd.Shape = shape;
                body.CreateFixture(fd);
            }

            // Hexagon character
            {
                BodyDef bd = new BodyDef();
                bd.Position = new Vector2(-5.0f, 8.0f);
                bd.Type = BodyType.Dynamic;
                bd.FixedRotation = true;
                bd.AllowSleep = false;

                Body body = World.CreateBody(bd);

                float angle = 0.0f;
                float delta = MathConstants.Pi / 3.0f;
                Vertices vertices = new Vertices(6);
                for (int i = 0; i < 6; ++i)
                {
                    vertices.Add(new Vector2(0.5f * (float)Math.Cos(angle), 0.5f * (float)Math.Sin(angle)));
                    angle += delta;
                }

                PolygonShape shape = new PolygonShape(vertices, 20.0f);

                FixtureDef fd = new FixtureDef();
                fd.Shape = shape;
                body.CreateFixture(fd);
            }

            // Circle character
            {
                BodyDef bd = new BodyDef();
                bd.Position = new Vector2(3.0f, 5.0f);
                bd.Type = BodyType.Dynamic;
                bd.FixedRotation = true;
                bd.AllowSleep = false;

                Body body = World.CreateBody(bd);

                CircleShape shape = new CircleShape(0.5f, 20.0f);

                FixtureDef fd = new FixtureDef();
                fd.Shape = shape;
                body.CreateFixture(fd);
            }

            // Circle character
            {
                BodyDef bd = new BodyDef();
                bd.Position = new Vector2(-7.0f, 6.0f);
                bd.Type = BodyType.Dynamic;
                bd.AllowSleep = false;

                _character = World.CreateBody(bd);

                CircleShape shape = new CircleShape(0.25f, 20.0f);

                FixtureDef fd = new FixtureDef();
                fd.Shape = shape;
                fd.Friction = 1.0f;
                _character.CreateFixture(fd);
            }
        }

        public override void Update(GameSettings settings, GameTime gameTime)
        {
            Vector2 v = _character.LinearVelocity;
            v.X = -5.0f;
            _character.LinearVelocity = v;

            DrawString("This tests various character collision shapes.");
            DrawString("Limitation: square and hexagon can snag on aligned boxes.");
            DrawString("Feature: edge chains have smooth collision inside and out.");

            base.Update(settings, gameTime);
        }

        internal static Test Create()
        {
            return new CharacterCollisionTest();
        }
    }
}