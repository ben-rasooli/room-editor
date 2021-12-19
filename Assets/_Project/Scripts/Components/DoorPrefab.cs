using Unity.Entities;

[GenerateAuthoringComponent]
public struct DoorPrefab : IComponentData
{
    public int thickness;
    public Entity Prefab;
}
