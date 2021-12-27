using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Project
{
  [Serializable]
  public struct AddDoorRequest_command : IComponentData
  {
    public float3 Position;
    public quaternion Rotation;
  }
}
