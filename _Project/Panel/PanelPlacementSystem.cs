#region usings
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
#endregion

namespace Project
{
  public class PanelPlacementSystem : ComponentSystem
  {
    protected override void OnCreate()
    {
      _ECBSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
      var ECB = _ECBSys.CreateCommandBuffer();

      Entities
        .ForEach((ref AddPanel_command command) =>
        {
          var newPanelEntity = ECB.Instantiate(command.PanelPrefab.Prefab);
          ECB.SetComponent(newPanelEntity, new Translation { Value = command.Position });
          ECB.SetComponent(newPanelEntity, new Rotation { Value = command.Rotation });
        });

      Entities
        .ForEach((ref RemovePanel_command command) =>
        {
          var position = command.Value;
          Entities
            .WithAll<Panel_tag>()
            .ForEach((Entity entity, ref Translation translation) =>
            {
              if (position.Equals(translation.Value))
                ECB.DestroyEntity(entity);
            });
        });
    }
    EntityCommandBufferSystem _ECBSys;
  }
}