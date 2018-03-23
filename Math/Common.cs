using UnityEngine;
using System.Collections

namespace Utils.Math{
  public partial static class Math{
  
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
  }
}
