using UnityEngine;

namespace GameCore.Unity.Extension
{
    /// <summary>
    /// contains extensions for <c>UnityEngine.Color</c>
    /// </summary>
    public static class ColorExtension
    {
        /// <summary>
        /// Returns a copy of the original color with the specified values.
        /// </summary>
        /// <param name="color">The color to change</param>
        /// <param name="r">The new red value or <c>null</c> to keep the original value</param>
        /// <param name="g">The new green value or <c>null</c> to keep the original value</param>
        /// <param name="b">The new blue value or <c>null</c> to keep the original value</param>
        /// <param name="a">The new alpha value or <c>null</c> to keep the original value</param>
        /// <returns></returns>
        public static Color With(this Color color, float? r = null, float? g = null, float? b = null, float? a = null)
        {
            color.r = r ?? color.r;
            color.g = g ?? color.g;
            color.b = b ?? color.b;
            color.a = a ?? color.a;
            return color;
        }
    }
}
