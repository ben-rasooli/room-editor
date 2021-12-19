using Unity.Entities;
using Unity.Mathematics;

namespace Project
{
  public struct RemovePanel_command : IComponentData
  {
    public PanelDetails PanelDetails;
  }
}