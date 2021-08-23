using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoUtil.Extentsions
{
    public static class ColorExtentsions
    {
        public static Color GetRandom(this Color color, bool alpha = false)
        {
            return new Color(Random.value, Random.value, Random.value, alpha ? Random.value : 1);
        }
    }
}