using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NoUtil.Math
{
    public static partial class Math
    {
        /// <summary>
        /// Coverts a Angle to a directional Vector2
        /// </summary>
        /// <param name="angle">Angle in Degrees</param>
        /// <returns>Returns a vector to with values from -1 to 1 on each axis</returns>
        public static Vector2 AngleToVector(float angle)
        {
            Vector2 output;
            float radians = angle * Mathf.Deg2Rad;

            output = new Vector2((float)Mathf.Cos(radians), (float)Mathf.Sin(radians));
            return output;
        }

        /// <summary>
        /// Coverts a Vector2 into a Angle
        /// </summary>
        /// <param name="v2">A Vector to of which you want to know the direction in Degrees</param>
        /// <returns>Angle in degrees</returns>
        public static float VectorToAngle(Vector2 v2)
        {
            return Mathf.Atan2(v2.y, v2.x) * 180f / Mathf.PI;
        }

        /// <summary>
        /// Rotate a Vector2 to a angle
        /// </summary>
        /// <param name="v"></param>
        /// <param name="degrees">The angle to rotate it to</param>
        /// <returns>Rotated Vector2</returns>
        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            /*
            Actual Calculation

            float radians = degrees * Mathf.Deg2Rad;
             float sin = Mathf.Sin(radians);
             float cos = Mathf.Cos(radians);

             float tx = v.x;
             float ty = v.y;

             return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
            */
            return Quaternion.Euler(0, 0, degrees) * v;
        }

        /// <summary>
        /// Rotates a Vector2 to a Quaternion
        /// </summary>
        /// <param name="v"></param>
        /// <param name="rotation">The rotation used to to calculate the new vector</param>
        /// <returns>Rotated Vector2</returns>
        public static Vector2 Rotate(this Vector2 v, Quaternion rotation)
        {
            return new Quaternion(0, 0, rotation.z, rotation.w) * v;
        }

        //linePnt - point the line passes through
        //lineDir - unit vector in direction of line, either direction works
        //pnt - the point to find nearest on line for
        //Source https://forum.unity.com/threads/how-do-i-find-the-closest-point-on-a-line.340058/
        public static Vector3 NearestPointOnLine(Vector3 linePnt, Vector3 lineDir, Vector3 pnt)
        {
            lineDir.Normalize();//this needs to be a unit vector
            var v = pnt - linePnt;
            var d = Vector3.Dot(v, lineDir);
            return linePnt + lineDir * d;
        }

        /// <summary>
        /// Get the length of a vector
        /// </summary>
        /// <param name="obj">Vector of wich you want to know the length</param>
        /// <returns>Length of the vector</returns>
        public static float GetLength(this Vector2 obj)
        {
            return Mathf.Sqrt(Mathf.Pow(obj.x, 2) + Mathf.Pow(obj.y, 2));
        }
    }
}