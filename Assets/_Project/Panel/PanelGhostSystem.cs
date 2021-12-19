#region usings
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Tiny.Rendering;
using Unity.Tiny;
using Unity.Mathematics;
#endregion

namespace Project
{
  public class PanelGhostSystem : SystemBase
  {
    protected override void OnCreate()
    {
      _placementNodeQuery = EntityManager.CreateEntityQuery(typeof(PlacementNode_tag), typeof(Translation));
    }

    protected override void OnStartRunning()
    {
      _ECBSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
      _sceneSettings = GetSingleton<SceneSettings>();
      _panelGhostEntity = GetSingletonEntity<PanelGhost_tag>();
      _eventsHolderEntity = GetSingletonEntity<EventsHolder_tag>();
    }

    protected override void OnUpdate()
    {
      var ECB = _ECBSys.CreateCommandBuffer();

      if (HasComponent<PointerPosition_event>(_eventsHolderEntity))
      {
        var panelRotation = GetSingleton<SceneSettings>().Rotation;
        ECB.SetComponent(_panelGhostEntity, panelRotation);

        float3 pointerPosition = GetComponent<PointerPosition_event>(_eventsHolderEntity).Value;
        float3 placementPosition = pointerPosition;

        //find the nearest grid node if present and update the pointerPosition
        var gridNodeTranslationArray = _placementNodeQuery.ToComponentDataArray<Translation>(Allocator.Temp);
        if (gridNodeTranslationArray.Length > 0)
        {
          var nearbyNodes = new NativeList<Translation>(Allocator.Temp);
          foreach (var item in gridNodeTranslationArray)
            if (math.distancesq(pointerPosition, item.Value) < _sceneSettings.GridNodeSelectionRadius)
              nearbyNodes.Add(item);

          float shortestDistance = 10f;
          foreach (var translation in nearbyNodes)
          {
            var currentDistance = math.distancesq(pointerPosition, translation.Value);
            if (currentDistance < shortestDistance)
            {
              shortestDistance = currentDistance;
              placementPosition = translation.Value;
            }
          }
          nearbyNodes.Dispose();
        }
        placementPosition.y += 0.001f;
        ECB.SetComponent(_panelGhostEntity, new Translation { Value = placementPosition });
        gridNodeTranslationArray.Dispose();
      }

      if (HasComponent<PanelSwitched_event>(_eventsHolderEntity))
      {
        Entity panelEntity = GetComponent<PanelSwitched_event>(_eventsHolderEntity).Value;
        var panelGhostEntity = GetSingletonEntity<PanelGhost_tag>();
        var currentRenderMesh = EntityManager.GetComponentData<MeshRenderer>(panelGhostEntity);
        currentRenderMesh.mesh = EntityManager.GetComponentData<MeshRenderer>(panelEntity).mesh;
        ECB.SetComponent(panelGhostEntity, currentRenderMesh);
      }
    }
    EntityCommandBufferSystem _ECBSys;
    SceneSettings _sceneSettings;
    Entity _panelGhostEntity;
    Entity _eventsHolderEntity;
    EntityQuery _placementNodeQuery;
  }
}