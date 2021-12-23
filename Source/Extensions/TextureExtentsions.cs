using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Util.Extensions
{
    public static class TextureExtentsions
    {
        public static Vector2 GetSize(this Texture2D obj)
        {
            return new Vector2(obj.width, obj.height);
        }
    }
}