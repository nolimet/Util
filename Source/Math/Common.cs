using UnityEngine;
using System.Collections;

namespace NoUtil.Math
{
    public static partial class Math
    {
        public static float CalculateJumpVerticalSpeed(float targetJumpHeight)
        {
            // From the jump height and gravity we deduce the upwards speed
            // for the character to reach at the apex.
            Debug.Log(2f * targetJumpHeight * Physics2D.gravity.y);
            return Mathf.Sqrt(2f * targetJumpHeight * Physics2D.gravity.y);
        }

        /// <summary>
        /// breaks up a number into pieces and return the value at the point of the power
        /// </summary>
        /// <param name="number">should not be seen. The number to exstract from</param>
        /// <param name="power">wat what point do i need to take the return number. In powers of 10( 3, 2 ,1 ,0 )</param>
        /// <returns>a number from 0-9</returns>
        public static int GetNumbAt(this int number, int power)
        {
            return number / (int)Mathf.Pow(10, power) % 10;
        }

        /// <summary>
        /// Coverts bool into a int
        /// </summary>
        /// <param name="b">Bool that will be converted</param>
        /// <returns>0 if false| 1 if true</returns>
        public static int ToInt(this bool b)
        {
            if (b)
                return 1;
            return 0;
        }
    }
}