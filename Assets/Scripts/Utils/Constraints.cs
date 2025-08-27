using UnityEngine;

namespace Utils
{
    public static class Constraints
    {
        public static Vector2 ConstrainDistance(Vector2 point, Vector2 anchor, float distance)
        {
            return (point - anchor).normalized * distance + anchor;
        }
    }
}
