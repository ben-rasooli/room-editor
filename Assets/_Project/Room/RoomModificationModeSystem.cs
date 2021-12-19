#region usings
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
#endregion

namespace Project
{
  public class RoomModificationModeSystem : SystemBase
  {
    protected override void OnCreate()
    {
      _ECBSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnStartRunning()
    {
      _modificationModeEntity = GetSingletonEntity<ModificationModeData>();
      var ECB = _ECBSys.CreateCommandBuffer();

      Entities
        .WithAll<PanelGhost_tag>()
        .ForEach((Entity entity) =>
        {
          enablePanelGhost(ref ECB, ref entity);
        }).WithoutBurst().Run();
    }

    protected override void OnUpdate()
    {
      var ECB = _ECBSys.CreateCommandBuffer();

      if (!TryGetSingletonEntity<ChangeModificationMode_command>(out Entity _))
        return;

      var modMode = GetSingleton<ModificationModeData>();
      modMode.Value = (modMode.Value == ModificationMode.Addition) ? ModificationMode.Subtraction : ModificationMode.Addition;
      ECB.SetComponent(_modificationModeEntity, modMode);

      Entities
        .WithEntityQueryOptions(EntityQueryOptions.IncludeDisabled)
        .WithAll<PanelGhost_tag>()
        .ForEach((Entity entity) =>
        {
          if (modMode.Value == ModificationMode.Subtraction)
            disablePanelGhost(ref ECB, ref entity);
          else
            enablePanelGhost(ref ECB, ref entity);
        }).WithoutBurst().Run();
    }
    EntityCommandBufferSystem _ECBSys;
    Entity _modificationModeEntity;

    void enablePanelGhost(ref EntityCommandBuffer ECB, ref Entity entity)
    {
      if (HasComponent<Disabled>(entity))
      {
        var child = GetBuffer<Child>(entity);
        ECB.RemoveComponent<Disabled>(child[0].Value);
        ECB.RemoveComponent<Disabled>(entity);
      }
    }

    void disablePanelGhost(ref EntityCommandBuffer ECB, ref Entity entity)
    {
      if (!HasComponent<Disabled>(entity))
      {
        var child = GetBuffer<Child>(entity);
        ECB.AddComponent<Disabled>(child[0].Value);
        ECB.AddComponent<Disabled>(entity);
      }
    }
  }
}