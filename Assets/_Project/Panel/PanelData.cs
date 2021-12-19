using System;
using Unity.Entities;

namespace Project
{
  [Serializable]
  public struct PanelData : IComponentData
  {
    public int Height;
    public int Thickness;
  }
}
