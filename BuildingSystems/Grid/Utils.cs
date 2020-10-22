using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameCore.BuildingSystem.Grid
{
    public static class Utils
    {
        public static float RoundToMultiple(float x, float multiple)
        {
            float m = multiple * (int)(x / multiple);
            float d = x - m;
            return multiple * Mathf.RoundToInt(d / multiple) + m;
        }

        public static float CeilToMultiple(float x, float multiple)
        {
            return multiple * (int)(x / multiple) + Mathf.Sign(x);
        }

        public static float FloorToMultiple(float x, float multiple)
        {
            return multiple * (int)(x / multiple);
        }

        public static void Translate(this Vector3 other, float x, float y, float z)
        {
            other.x += x;
            other.y += y;
            other.z += z;
        }

        public static void Translate(this Vector3 other, float offset)
        {
            other.x += offset;
            other.y += offset;
            other.z += offset;
        }

        public static Vector3 Translated(this Vector3 other, float x, float y, float z)
        {
            return new Vector3(other.x + x, other.y + y, other.z + z);
        }

        public static Vector3 Translated(this Vector3 other, float offset)
        {
            return new Vector3(other.x + offset, other.y + offset, other.z + offset);
        }

        public static void RoundToMultiple(this Vector3 other, float multiple)
        {
            other.x = RoundToMultiple(other.x, multiple);
            other.y = RoundToMultiple(other.y, multiple);
            other.z = RoundToMultiple(other.z, multiple);            
        }

        public static Vector3 RoundedToMultiple(this Vector3 v, float multiple)
        {
            return new Vector3(RoundToMultiple(v.x, multiple), RoundToMultiple(v.y, multiple), RoundToMultiple(v.z, multiple));
        }

        public static float GetDecimal(this float f)
        {
            return f - (f % 1);
        }

        public static Vector3 GetDecimal(this Vector3 other)
        {
            return new Vector3(GetDecimal(other.x), GetDecimal(other.y), GetDecimal(other.z));
        }
    }
}
