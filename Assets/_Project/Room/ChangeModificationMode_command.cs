using Unity.Entities;

namespace Project
{
  public struct ChangeModificationMode_command : IComponentData
  {
    public ModificationMode Value;
  }
}