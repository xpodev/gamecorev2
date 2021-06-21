using UnityEngine;

namespace GameCore.WorldGeneration
{
    /// <summary>
    /// This class describes a plane made of smaller planes.
    /// </summary>
    public class SuperPlane
    {
        /// <summary>
        /// Returns the center of the super-plane in world space.
        /// </summary>
        public Vector3 Center
        {
            get
            {
                return Root + (Directions.DirectionX * CountX * 0.5f) + (Directions.DirectionY * CountY * 0.5f);
            }
        }

        /// <summary>
        /// The number of planes in the direction DirectionX
        /// </summary>
        public int CountX { get; private set; }

        /// <summary>
        /// The number of planes in the direction DiectionY
        /// </summary>
        public int CountY { get; private set; }

        /// <summary>
        /// The directions of the super-plane
        /// </summary>
        public PlaneDirections Directions { get; private set; }

        /// <summary>
        /// The root of the super-plane.
        /// </summary>
        public Vector3 Root { get; private set; }

        public SuperPlane(Vector3 root, int numPlanesX, int numPlanesY, PlaneDirections directions)
        {
            Root = root;
            CountX = numPlanesX;
            CountY = numPlanesY;
            Directions = directions;
        }
    }
}
