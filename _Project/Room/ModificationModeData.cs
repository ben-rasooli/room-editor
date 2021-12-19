using Unity.Entities;

namespace Project
{
  [GenerateAuthoringComponent]
  public struct ModificationModeData : IComponentData
  {
    public ModificationMode Value;
  }


  public enum ModificationMode
  {
    Addition,
    Subtraction
  }
}