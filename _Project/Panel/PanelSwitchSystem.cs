#region usings
using System.Linq;
using Unity.Entities;
using Unity.Jobs;
#endregion

namespace Project
{
  public class PanelSwitchSystem : SystemBase
  {
    protected override void OnCreate()
    {
      _ECBSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
      var ECB = _ECBSys.CreateCommandBuffer();
      Entities
        .ForEach((ref SceneSettings settings, in DynamicBuffer<PanelDetails> prefabs, in SwitchPanel_command command) =>
        {
          foreach (var item in prefabs)
          {
            if (item.PanelType == command.Value)
            {
              settings.PanelDetails = item;
              ECB.CreateSingleFrameComponent(new PanelSwitched_event { Value = item.Prefab });
            }
          }
        }).Run();
    }
    EntityCommandBufferSystem _ECBSys;
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