﻿/*
* Box2D.XNA port of Box2D:
* Copyright (c) 2009 Brandon Furtwangler, Nathan Furtwangler
*
* Original source Box2D:
* Copyright (c) 2006-2009 Erin Catto http://www.gphysics.com 
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

using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using FarseerPhysics.TestBed.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FarseerPhysics.TestBed.Tests
{
    /// This tests bullet collision and provides an example of a gameplay scenario.
    public class PinballTest : Test
    {
        private RevoluteJoint _leftJoint;
        private RevoluteJoint _rightJoint;
        private Body _ball;

        private PinballTest()
        {
            Body ground;
            {
                ground = BodyFactory.CreateBody(World);
                Vertices vertices = new Vertices(5);
                vertices.Add(new Vector2(0.0f, -2.0f));
                vertices.Add(new Vector2(8.0f, 6.0f));
                vertices.Add(new Vector2(8.0f, 20.0f));
                vertices.Add(new Vector2(-8.0f, 20.0f));
                vertices.Add(new Vector2(-8.0f, 6.0f));

                PolygonShape shape = new PolygonShape();
                for (int i = 0; i < vertices.Count; i++)
                {
                    Vertices edge = PolygonTools.CreateEdge(vertices[i % 5], vertices[(i + 1) % 5]);
                    shape.Set(edge);
                    Fixture fixture = ground.CreateFixture(shape, 0);
                    fixture.Restitution = 0.4f;
                }
            }

            {
                var leftFlipper = BodyFactory.CreateBody(World, new Vector2(-2.0f, 0.8f));
                leftFlipper.BodyType = BodyType.Dynamic;

                var rightFlipper = BodyFactory.CreateBody(World, new Vector2(2.0f, 0.8f));
                rightFlipper.BodyType = BodyType.Dynamic;

                var box = new PolygonShape();
                box.SetAsBox(1.75f, 0.1f);

                leftFlipper.CreateFixture(box);
                rightFlipper.CreateFixture(box);

                _leftJoint = new RevoluteJoint(ground, leftFlipper, Vector2.Zero, Vector2.Zero);
                _leftJoint.LocalAnchorB = -Vector2.UnitX;
                _leftJoint.LocalAnchorA = ground.GetLocalPoint(leftFlipper.GetWorldPoint(_leftJoint.LocalAnchorB));
                _leftJoint.MaxMotorTorque = 1000.0f;
                _leftJoint.LimitEnabled = true;
                _leftJoint.MotorEnabled = true;
                _leftJoint.MotorSpeed = 0.0f;

                _leftJoint.LowerLimit = -30.0f * Settings.Pi / 180.0f;
                _leftJoint.UpperLimit = 5.0f * Settings.Pi / 180.0f;
                World.AddJoint(_leftJoint);

                _rightJoint = new RevoluteJoint(ground, rightFlipper, Vector2.Zero, Vector2.Zero);
                _rightJoint.LocalAnchorB = Vector2.UnitX;
                _rightJoint.LocalAnchorA = ground.GetLocalPoint(rightFlipper.GetWorldPoint(_rightJoint.LocalAnchorB));

                _rightJoint.MaxMotorTorque = 1000.0f;
                _rightJoint.LimitEnabled = true;
                _rightJoint.MotorEnabled = true;
                _rightJoint.MotorSpeed = 0.0f;

                _rightJoint.LowerLimit = -5.0f * Settings.Pi / 180.0f;
                _rightJoint.UpperLimit = 30.0f * Settings.Pi / 180.0f;
                World.AddJoint(_rightJoint);
            }

            {
                _ball = BodyFactory.CreateBody(World, new Vector2(1.0f, 15.0f));
                _ball.BodyType = BodyType.Dynamic;
                _ball.IsBullet = true;
                _ball.CreateFixture(new CircleShape(0.2f), 1.0f);
            }
        }

        public override void Keyboard(KeyboardState state, KeyboardState oldState)
        {
            if (state.IsKeyDown(Keys.A))
            {
                _leftJoint.MotorSpeed = 20.0f;
                _rightJoint.MotorSpeed = -20.0f;
            }
            if (state.IsKeyUp(Keys.A))
            {
                _leftJoint.MotorSpeed = -10.0f;
                _rightJoint.MotorSpeed = 10.0f;
            }
        }

        internal static Test Create()
        {
            return new PinballTest();
        }
    }
}