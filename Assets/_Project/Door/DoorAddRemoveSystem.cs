#region usings
using Unity.Entities;
using Unity.Transforms;
#endregion

namespace Project
{
  public class DoorAddRemoveSystem : ComponentSystem
  {
    protected override void OnCreate()
    {
      _ECBSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
      var ECB = _ECBSys.CreateCommandBuffer();
      var sceneSettings = GetSingleton<SceneSettings>();

      Entities
        .WithNone<EventsHolder_tag>()
        .ForEach((ref AddDoor_command command) =>
        {
          var newDoorEntity = ECB.Instantiate(sceneSettings.DoorPrefab);
          ECB.AddComponent<Door_tag>(newDoorEntity);
          ECB.SetComponent(newDoorEntity, new Translation { Value = command.Position });
          ECB.SetComponent(newDoorEntity, new Rotation { Value = command.Rotation });
        });

      Entities
        .WithAll<Door_tag, Remove_tag>()
        .ForEach((Entity entity) =>
        {
          ECB.DestroyEntity(entity);
        });
    }
    EntityCommandBufferSystem _ECBSys;
  }
}