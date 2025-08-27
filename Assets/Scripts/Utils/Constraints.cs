using UnityEngine;

namespace Utils
{
    public static class Constraints
    {
        /// <summary>
        /// Satisfies constraint by projecting the point onto a circle around the anchor.
        /// </summary>
        /// <remarks>
        /// Source: https://zalo.github.io/blog/constraints/#basic-distance-constraint
        /// </remarks>
        /// <param name="point"></param>
        /// <param name="anchor"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static Vector2 BasicDistanceConstraint(Vector2 point, Vector2 anchor, float distance)
        {
            return (point - anchor).normalized * distance + anchor;
        }
    }
}
