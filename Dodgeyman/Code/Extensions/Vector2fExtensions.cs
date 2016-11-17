namespace Dodgeyman.Code.Extensions
{
    using System;
    using SFML.System;

    public static class Vector2FExtensions
    {
        /// <summary>
        /// Returns the cross product of <paramref name="vec"/> cross <paramref name="other"/>
        /// </summary>
        /// <param name="vec">The left vector of the cross product</param>
        /// <param name="other">The right vector of the corss product</param>
        /// <returns></returns>
        public static float Cross(this Vector2f vec, Vector2f other)
        {
            return vec.X*other.Y - vec.Y * other.X;
        }

        public static float Magnitude(this Vector2f vec)
        {
            //may need a MagnitudeSquared function at some point, could make some calculations quicker
            return (float) Math.Sqrt(vec.X*vec.X + vec.Y*vec.Y);
        }

        public static Vector2f Normalize(this Vector2f vec)
        {
            var mag = vec.Magnitude();
            return new Vector2f(vec.X/mag, vec.Y/mag);
        }
    }
}
