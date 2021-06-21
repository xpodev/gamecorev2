using UnityEngine;

namespace GameCore.WorldGeneration
{
    /// <summary>
    /// This struct holds 2 directions in world space of a plane
    /// </summary>
    public struct PlaneDirections
    {
        /// <summary>
        /// DirectionX is the direction at which the x coordinates of the plane increase.
        /// </summary>
        public Vector3 DirectionX { get; private set; }

        /// <summary>
        /// DirectionY is the direction at which the y coordinates of the plane increase.
        /// </summary>
        public Vector3 DirectionY { get; private set; }

        /// <summary>
        /// The normal of the plane normalized.
        /// </summary>
        public Vector3 Normal
        {
            get
            {
                return NormalRaw.normalized;
            }
        }

        /// <summary>
        /// The normal of the plane.
        /// </summary>
        public Vector3 NormalRaw
        {
            get
            {
                return Vector3.Cross(DirectionX, DirectionY);
            }
        }
    }
}
