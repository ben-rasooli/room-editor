using Unity.Entities;
using Unity.Mathematics;

namespace Project
{
  public struct PointerPosition_event : IComponentData
  {
    public float3 Value;
    public quaternion PanelRotation;
  }
}