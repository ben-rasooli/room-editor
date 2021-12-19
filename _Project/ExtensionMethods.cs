using Unity.Mathematics;
using UnityEngine;

public static partial class ExtensionMethods
{
  public static float4 ToFloat4(this Color c)
  {
    return new float4(c.r, c.g, c.b, c.a);
  }

  public static bool RoughlyEquals(this float3 lhs, float3 rhs)
  {
    return
      math.abs(lhs.x - rhs.x) < 0.0001f &&
      math.abs(lhs.y - rhs.y) < 0.0001f &&
      math.abs(lhs.z - rhs.z) < 0.0001f;
  }
}
