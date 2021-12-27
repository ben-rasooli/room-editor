using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Project
{
  [Serializable]

  public struct DoorAddDetails : IComponentData
  {
    public float3 Position;
    public float4 Rotation;
  }
}
