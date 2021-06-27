using UnityEngine;

/// <summary>
/// contains extensions for built-in Unity classes to make life easier
/// </summary>
namespace GameCore.Unity.Extension
{
    /// <summary>
    /// contains extensions for <c>UnityEngine.Vector3</c>
    /// </summary>
    public static class VectorExtension
    {
        /// <summary>
        /// Returns a copy of the original vector with the specified values
        /// </summary>
        /// <param name="vector">The vector to change</param>
        /// <param name="x">The new x value or <c>null</c> to keep the original value</param>
        /// <param name="y">The new y value or <c>null</c> to keep the original value</param>
        /// <param name="z">The new z value or <c>null</c> to keep the original value</param>
        /// <returns>A copy of the original vector with the specified values</returns>
        public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)
        {
            vector.x = x ?? vector.x;
            vector.y = y ?? vector.y;
            vector.z = z ?? vector.z;
            return vector;
        }

        /// <summary>
        /// Returns a copy of the original vector with the specified values
        /// </summary>
        /// <param name="vector">The vector to change</param>
        /// <param name="x">The new x value or <c>null</c> to keep the original value</param>
        /// <param name="y">The new y value or <c>null</c> to keep the original value</param>
        /// <returns>A copy of the original vector with the specified values</returns>
        public static Vector2 With(this Vector2 vector, float? x = null, float? y = null)
        {
            vector.x = x ?? vector.x;
            vector.y = y ?? vector.y;
            return vector;
        }

        /// <summary>
        /// Returns a copy of the original vector with the specified values
        /// </summary>
        /// <param name="vector">The vector to change</param>
        /// <param name="x">The new x value or <c>null</c> to keep the original value</param>
        /// <param name="y">The new y value or <c>null</c> to keep the original value</param>
        /// <param name="z">The new z value or <c>null</c> to keep the original value</param>
        /// <param name="w">The new w value or <c>null</c> to keep the original value</param>
        /// <returns>A copy of the original vector with the specified values</returns>
        public static Vector4 With(this Vector4 vector, float? x = null, float? y = null, float? z = null, float? w = null)
        {
            vector.x = x ?? vector.x;
            vector.y = y ?? vector.y;
            vector.z = z ?? vector.z;
            vector.w = w ?? vector.w;
            return vector;
        }

        /// <summary>
        /// Returns a copy of the original vector with the specified values
        /// </summary>
        /// <param name="vector">The vector to change</param>
        /// <param name="x">The new x value or <c>null</c> to keep the original value</param>
        /// <param name="y">The new y value or <c>null</c> to keep the original value</param>
        /// <param name="z">The new z value or <c>null</c> to keep the original value</param>
        /// <returns>A copy of the original vector with the specified values</returns>
        public static Vector3Int With(this Vector3Int vector, int? x = null, int? y = null, int? z = null)
        {
            vector.x = x ?? vector.x;
            vector.y = y ?? vector.y;
            vector.z = z ?? vector.z;
            return vector;
        }

        /// <summary>
        /// Returns a copy of the original vector with the specified values
        /// </summary>
        /// <param name="vector">The vector to change</param>
        /// <param name="x">The new x value or <c>null</c> to keep the original value</param>
        /// <param name="y">The new y value or <c>null</c> to keep the original value</param>
        /// <returns>A copy of the original vector with the specified values</returns>
        public static Vector2Int With(this Vector2Int vector, int? x = null, int? y = null)
        {
            vector.x = x ?? vector.x;
            vector.y = y ?? vector.y;
            return vector;
        }
    }
}
