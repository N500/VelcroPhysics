using Microsoft.Xna.Framework;
using VelcroPhysics.Collision;
using VelcroPhysics.Common;

namespace VelcroPhysics.Dynamics.Contacts
{
    public static class WorldManifold
    {
        /// <summary>
        /// Evaluate the manifold with supplied transforms. This assumes
        /// modest motion from the original state. This does not change the
        /// point count, impulses, etc. The radii must come from the Shapes
        /// that generated the manifold.
        /// </summary>
        /// <param name="manifold">The manifold.</param>
        /// <param name="xfA">The transform for A.</param>
        /// <param name="radiusA">The radius for A.</param>
        /// <param name="xfB">The transform for B.</param>
        /// <param name="radiusB">The radius for B.</param>
        /// <param name="normal">World vector pointing from A to B</param>
        /// <param name="points">Torld contact point (point of intersection).</param>
        public static void Initialize(ref Manifold manifold, ref Transform xfA, float radiusA, ref Transform xfB, float radiusB, out Vector2 normal, out FixedArray2<Vector2> points)
        {
            normal = Vector2.Zero;
            points = new FixedArray2<Vector2>();

            if (manifold.PointCount == 0)
            {
                return;
            }

            switch (manifold.Type)
            {
                case ManifoldType.Circles:
                    {
                        normal = new Vector2(1.0f, 0.0f);
                        Vector2 pointA = MathUtils.Mul(ref xfA, manifold.LocalPoint);
                        Vector2 pointB = MathUtils.Mul(ref xfB, manifold.Points[0].LocalPoint);
                        if (Vector2.DistanceSquared(pointA, pointB) > Settings.Epsilon * Settings.Epsilon)
                        {
                            normal = pointB - pointA;
                            normal.Normalize();
                        }

                        Vector2 cA = pointA + radiusA * normal;
                        Vector2 cB = pointB - radiusB * normal;
                        points[0] = 0.5f * (cA + cB);
                    }
                    break;

                case ManifoldType.FaceA:
                    {
                        normal = MathUtils.Mul(xfA.q, manifold.LocalNormal);
                        Vector2 planePoint = MathUtils.Mul(ref xfA, manifold.LocalPoint);

                        for (int i = 0; i < manifold.PointCount; ++i)
                        {
                            Vector2 clipPoint = MathUtils.Mul(ref xfB, manifold.Points[i].LocalPoint);
                            Vector2 cA = clipPoint + (radiusA - Vector2.Dot(clipPoint - planePoint, normal)) * normal;
                            Vector2 cB = clipPoint - radiusB * normal;
                            points[i] = 0.5f * (cA + cB);
                        }
                    }
                    break;

                case ManifoldType.FaceB:
                    {
                        normal = MathUtils.Mul(xfB.q, manifold.LocalNormal);
                        Vector2 planePoint = MathUtils.Mul(ref xfB, manifold.LocalPoint);

                        for (int i = 0; i < manifold.PointCount; ++i)
                        {
                            Vector2 clipPoint = MathUtils.Mul(ref xfA, manifold.Points[i].LocalPoint);
                            Vector2 cB = clipPoint + (radiusB - Vector2.Dot(clipPoint - planePoint, normal)) * normal;
                            Vector2 cA = clipPoint - radiusA * normal;
                            points[i] = 0.5f * (cA + cB);
                        }

                        // Ensure normal points from A to B.
                        normal = -normal;
                    }
                    break;
            }
        }
    }
}