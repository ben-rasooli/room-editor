#region usings
using Unity.Entities;
using Unity.Transforms;
#endregion

namespace Project
{
  public class PanelAddRemoveSystem : ComponentSystem
  {
    protected override void OnCreate()
    {
      _ECBSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
      var ECB = _ECBSys.CreateCommandBuffer();

      Entities
        .WithNone<EventsHolder_tag>()
        .ForEach((ref AddPanel_command command) =>
        {
          var panelDetails = command.PanelDetails;
          var newPanelEntity = ECB.Instantiate(panelDetails.Prefab);
          ECB.AddComponent<Panel_tag>(newPanelEntity);
          ECB.AddComponent(newPanelEntity, new PanelData { Height = panelDetails.Height, Thickness = panelDetails.Thickness});
          ECB.SetComponent(newPanelEntity, new Translation { Value = command.Position });
          ECB.SetComponent(newPanelEntity, new Rotation { Value = command.Rotation });
        });

      Entities
        .WithAll<Panel_tag, Remove_tag>()
        .ForEach((Entity entity) =>
        {
          ECB.DestroyEntity(entity);
        });
    }
    EntityCommandBufferSystem _ECBSys;
  }
}