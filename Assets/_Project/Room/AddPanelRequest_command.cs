using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Project
{
  [Serializable]
  public struct AddPanelRequest_command : IComponentData
  {
    public float3 Position;
    public float4 Rotation;
    public PanelDetails PanelDetails;
  }
}
