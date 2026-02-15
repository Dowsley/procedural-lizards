using UnityEngine;

namespace Utils
{
    public static class Constraints
    {
        /// <summary>
        /// Satisfies constraint by projecting the point onto a sphere around the anchor.
        /// </summary>
        /// <remarks>
        /// Source: https://zalo.github.io/blog/constraints/#basic-distance-constraint
        /// </remarks>
        public static Vector3 BasicDistanceConstraint(Vector3 point, Vector3 anchor, float distance)
        {
            return (point - anchor).normalized * distance + anchor;
        }
    }
}
