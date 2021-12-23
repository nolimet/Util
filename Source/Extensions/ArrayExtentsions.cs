using System;

namespace NoUtil.Extentsions
{
    public static class ArrayExtentsions
    {
        /// <summary>
        /// Resizes an array to new size.
        /// This function can be used to resize multidimensional arrays
        /// </summary>
        /// <param name="arr">The array to be resized</param>
        /// <param name="newSizes">the new size of the array</param>
        /// <returns>The resized array</returns>
        public static Array ResizeArray(this Array arr, int[] newSizes)
        {
            if (newSizes.Length != arr.Rank)
                throw new System.ArgumentException("arr must have the same number of dimensions " +
                                            "as there are elements in newSizes", "newSizes");

            var temp = Array.CreateInstance(arr.GetType().GetElementType(), newSizes);
            int length = arr.Length <= temp.Length ? arr.Length : temp.Length;
            Array.ConstrainedCopy(arr, 0, temp, 0, length);
            return temp;
        }
    }
}