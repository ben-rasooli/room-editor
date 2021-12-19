using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Project
{
  [Serializable]
  public struct PanelAddDetails : IComponentData
  {
    public float3 Position;
    public float4 Rotation;
    public PanelDetails PanelDetails;
  }
}
