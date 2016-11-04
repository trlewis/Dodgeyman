namespace Dodgeyman.Code.Extensions
{
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
    }
}
