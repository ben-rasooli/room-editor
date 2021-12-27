using Unity.Entities;
using Unity.Mathematics;

namespace Project
{
  public struct AddDoor_command : IComponentData
  {
    public float3 Position;
    public quaternion Rotation;
  }
}