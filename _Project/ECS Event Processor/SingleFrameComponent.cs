using Unity.Entities;

[GenerateAuthoringComponent]
public struct SingleFrameComponent : IBufferElementData
{
  public ComponentType TargetComponent;
}