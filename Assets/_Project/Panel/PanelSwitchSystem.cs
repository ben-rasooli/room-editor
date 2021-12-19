#region usings
using Unity.Entities;
using Unity.Jobs;
using Unity.Tiny;
#endregion

namespace Project
{
  public class PanelSwitchSystem : SystemBase
  {
    protected override void OnStartRunning()
    {
      _ECBSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
      _eventsHolderEntity = GetSingletonEntity<EventsHolder_tag>();
    }

    protected override void OnUpdate()
    {
      var ECB = _ECBSys.CreateCommandBuffer();
      Entities
        .ForEach((ref SceneSettings settings, in DynamicBuffer<PanelDetails> panelDetailsBuffer, in SwitchPanel_command command) =>
        {
          foreach (var item in panelDetailsBuffer)
          {
            if (item.PanelType == command.Value)
            {
              settings.PanelDetails = item;
              ECB.AddSingleFrameComponent(_eventsHolderEntity, new PanelSwitched_event { Value = item.Prefab });
            }
          }
        }).WithoutBurst().Run();
    }
    EntityCommandBufferSystem _ECBSys;
    Entity _eventsHolderEntity;
  }

  public struct SwitchPanel_command : IComponentData
  {
    public PanelType Value;
  }

  public struct PanelSwitched_event : IComponentData
  {
    public Entity Value;
  }
}