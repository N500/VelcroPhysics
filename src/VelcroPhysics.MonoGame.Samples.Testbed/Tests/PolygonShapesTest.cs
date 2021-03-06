using Genbox.VelcroPhysics.Collision.Shapes;
using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.MonoGame.Samples.Testbed.Framework;
using Genbox.VelcroPhysics.MonoGame.Samples.Testbed.Framework.Input;
using Genbox.VelcroPhysics.Shared;
using Genbox.VelcroPhysics.Templates;
using Genbox.VelcroPhysics.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Genbox.VelcroPhysics.MonoGame.Samples.Testbed.Tests
{
    internal class PolygonShapesTest : Test
    {
        private const int _maxBodies = 256;

        private int _bodyIndex;
        private readonly Body[] _bodies = new Body[_maxBodies];
        private readonly PolygonShape[] _polygons = new PolygonShape[4];
        private readonly CircleShape _circle;

        /// <summary>
        /// This tests stacking. It also shows how to use b2World::Query
        /// and b2TestOverlap.
        /// This callback is called by b2World::QueryAABB. We find all the fixtures
        /// that overlap an AABB. Of those, we use b2TestOverlap to determine which fixtures
        /// overlap a circle. Up to 4 overlapped fixtures will be highlighted with a yellow border.
        /// </summary>
        private class PolygonShapesCallback
        {
            public const int MaxCount = 4;

            internal CircleShape _circle;
            internal Transform _transform;
            internal DebugView.DebugView _debugView;
            internal int _count;

            public PolygonShapesCallback()
            {
                _count = 0;
            }

            /// <summary>
            /// Called for each fixture found in the query AABB.
            /// @return false to terminate the query.
            /// </summary>
            public bool ReportFixture(Fixture fixture)
            {
                if (_count == MaxCount)
                    return false;

                Body body = fixture.Body;
                Shape shape = fixture.Shape;

                body.GetTransform(out Transform transform);
                bool overlap = Collision.Narrowphase.Collision.TestOverlap(shape, 0, _circle, 0, ref transform, ref _transform);

                if (overlap)
                {
                    Color color = new Color(0.95f, 0.95f, 0.6f);
                    Vector2 center = body.WorldCenter;
                    _debugView.DrawPoint(center, 5.0f, color);
                    ++_count;
                }

                return true;
            }
        }

        private PolygonShapesTest()
        {
            // Ground body
            {
                BodyDef bd = new BodyDef();
                Body ground = World.CreateBody(bd);

                EdgeShape shape = new EdgeShape();
                shape.SetTwoSided(new Vector2(-40.0f, 0.0f), new Vector2(40.0f, 0.0f));
                ground.CreateFixture(shape);
            }

            {
                Vertices vertices = new Vertices(3);
                vertices.Add(new Vector2(-0.5f, 0.0f));
                vertices.Add(new Vector2(0.5f, 0.0f));
                vertices.Add(new Vector2(0.0f, 1.5f));
                _polygons[0] = new PolygonShape(vertices, 1.0f);
            }

            {
                Vertices vertices = new Vertices(3);
                vertices.Add(new Vector2(-0.1f, 0.0f));
                vertices.Add(new Vector2(0.1f, 0.0f));
                vertices.Add(new Vector2(0.0f, 1.5f));
                _polygons[1] = new PolygonShape(vertices, 1.0f);
            }

            {
                float w = 1.0f;
                float b = w / (2.0f + MathUtils.Sqrt(2.0f));
                float s = MathUtils.Sqrt(2.0f) * b;

                Vertices vertices = new Vertices(8);
                vertices.Add(new Vector2(0.5f * s, 0.0f));
                vertices.Add(new Vector2(0.5f * w, b));
                vertices.Add(new Vector2(0.5f * w, b + s));
                vertices.Add(new Vector2(0.5f * s, w));
                vertices.Add(new Vector2(-0.5f * s, w));
                vertices.Add(new Vector2(-0.5f * w, b + s));
                vertices.Add(new Vector2(-0.5f * w, b));
                vertices.Add(new Vector2(-0.5f * s, 0.0f));

                _polygons[2] = new PolygonShape(vertices, 1.0f);
            }

            {
                _polygons[3] = new PolygonShape(1.0f);
                _polygons[3].SetAsBox(0.5f, 0.5f);
            }

            {
                _circle = new CircleShape(1.0f);
                _circle.Radius = 0.5f;
            }

            _bodyIndex = 0;
        }

        private void Create(int index)
        {
            if (_bodies[_bodyIndex] != null)
            {
                World.DestroyBody(_bodies[_bodyIndex]);
                _bodies[_bodyIndex] = null;
            }

            BodyDef bd = new BodyDef();
            bd.Type = BodyType.Dynamic;

            float x = Rand.RandomFloat(-2.0f, 2.0f);
            bd.Position = new Vector2(x, 10.0f);
            bd.Angle = Rand.RandomFloat(-MathConstants.Pi, MathConstants.Pi);

            if (index == 4)
                bd.AngularDamping = 0.02f;

            _bodies[_bodyIndex] = World.CreateBody(bd);

            if (index < 4)
            {
                FixtureDef fd = new FixtureDef();
                fd.Shape = _polygons[index];
                fd.Friction = 0.3f;
                _bodies[_bodyIndex].CreateFixture(fd);
            }
            else
            {
                FixtureDef fd = new FixtureDef();
                fd.Shape = _circle;
                fd.Friction = 0.3f;

                _bodies[_bodyIndex].CreateFixture(fd);
            }

            _bodyIndex = (_bodyIndex + 1) % _maxBodies;
        }

        private void DestroyBody()
        {
            for (int i = 0; i < _maxBodies; ++i)
                if (_bodies[i] != null)
                {
                    World.DestroyBody(_bodies[i]);
                    _bodies[i] = null;
                    return;
                }
        }

        public override void Keyboard(KeyboardManager keyboard)
        {
            if (keyboard.IsNewKeyPress(Keys.NumPad1))
            {
                Create(1);
            }
            else if (keyboard.IsNewKeyPress(Keys.NumPad2))
            {
                Create(2);
            }
            else if (keyboard.IsNewKeyPress(Keys.NumPad3))
            {
                Create(3);
            }
            else if (keyboard.IsNewKeyPress(Keys.NumPad4))
            {
                Create(4);
            }
            else if (keyboard.IsNewKeyPress(Keys.NumPad5))
            {
                Create(5);
            }
            else if (keyboard.IsNewKeyPress(Keys.A))
            {
                for (int i = 0; i < _maxBodies; i += 2)
                    if (_bodies[i] != null)
                    {
                        bool enabled = _bodies[i].Enabled;
                        _bodies[i].Enabled = !enabled;
                    }
            }
            else if (keyboard.IsNewKeyPress(Keys.D))
            {
                DestroyBody();
            }

            base.Keyboard(keyboard);
        }

        public override void Update(GameSettings settings, GameTime gameTime)
        {
            base.Update(settings, gameTime);

            PolygonShapesCallback callback = new PolygonShapesCallback();
            callback._circle.Radius = 2.0f;
            callback._circle.Position = new Vector2(0.0f, 1.1f);
            callback._transform.SetIdentity();
            callback._debugView = DebugView;

            AABB aabb;
            callback._circle.ComputeAABB(ref callback._transform, 0, out aabb);

            World.QueryAABB(callback.ReportFixture, ref aabb);

            Color color = new Color(0.4f, 0.7f, 0.8f);
            DebugView.DrawCircle(callback._circle.Position, callback._circle.Radius, color);

            DrawString($"Press 1-5 to drop stuff, maximum of {PolygonShapesCallback.MaxCount} overlaps detected");
            DrawString("Press 'a' to enable/disable some bodies");
            DrawString("Press 'd' to destroy a body");
        }

        internal static Test Create()
        {
            return new PolygonShapesTest();
        }
    }
}