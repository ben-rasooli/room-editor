using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Project
{
  [Serializable]
  public struct AddPanel_command : IComponentData
  {
    public float3 Position;
    public quaternion Rotation;
    public PanelDetails PanelDetails;
  }
}