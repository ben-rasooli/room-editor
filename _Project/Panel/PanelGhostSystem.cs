#region usings
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
#endregion

namespace Project
{
  public class PanelGhostSystem : ComponentSystem
  {
    protected override void OnCreate()
    {
      _ECBSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    protected override void OnStartRunning()
    {
      _panelGhostEntity = GetSingletonEntity<PanelGhost_tag>();
      entities = EntityManager.GetAllEntities(Allocator.Persistent);
    }
    NativeArray<Entity> entities;

    protected override void OnDestroy()
    {
      entities.Dispose();
    }

    protected override void OnUpdate()
    {
      var ECB = _ECBSys.CreateCommandBuffer();
      var panelRotation = GetSingleton<SceneSettings>().Rotation;

      Entities
        .WithAll<HighlightedGridNode_tag>()
        .ForEach((ref Translation translation) =>
        {
          ECB.SetComponent(_panelGhostEntity, translation);
          ECB.SetComponent(_panelGhostEntity, panelRotation);
        });

      Entities
        .ForEach((ref PanelSwitched_event event_) =>
        {
          Entity panelEntity = event_.Value;
          var newMesh = EntityManager.GetSharedComponentData<RenderMesh>(panelEntity).mesh;
          var panelGhostEntity = GetSingletonEntity<PanelGhost_tag>();
          var currentRenderMesh = EntityManager.GetSharedComponentData<RenderMesh>(panelGhostEntity);
          currentRenderMesh.mesh = newMesh;
          ECB.SetSharedComponent(panelGhostEntity, currentRenderMesh);
        });
    }
    EntityCommandBufferSystem _ECBSys;
    Entity _panelGhostEntity;
  }
}