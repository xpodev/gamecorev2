using UnityEngine;

namespace GameCore.WorldGeneration
{
    public class Plane
    {
        private Vector3[] m_vertices;

        public int Width { get; private set; }

        public int Height { get; private set; }

        public Plane(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public Vector3 this[int index]
        {
            get
            {
                return m_vertices[index];
            }
            set
            {
                m_vertices[index] = value;
            }
        }
        
        public Vector3 this[int x, int y, bool includeEnds=false]
        {
            get
            {
                return this[x + y * (includeEnds ? Width + 2 : Width) + (includeEnds ? 1 : 0)];
            }
        }
    }
}
