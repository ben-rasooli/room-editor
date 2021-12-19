#region usings
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny;
using Unity.Transforms;
#endregion

namespace Project
{
  public class PlacementNodeSelectorSystem : SystemBase
  {
    protected override void OnCreate()
    {
      _gridNodeQuery = EntityManager.CreateEntityQuery(typeof(PlacementNode_tag), typeof(Translation));
    }

    protected override void OnStartRunning()
    {
      _ECBSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
      _eventsHolderEntity = GetSingletonEntity<EventsHolder_tag>();
      _sceneSettings = GetSingleton<SceneSettings>();
    }

    protected override void OnUpdate()
    {
      var ECB = _ECBSys.CreateCommandBuffer();

      if (HasComponent<PointerDown_event>(_eventsHolderEntity))
      {
        float3 pointerPosition = GetComponent<PointerDown_event>(_eventsHolderEntity).Value;
        var panelDetails = GetSingleton<SceneSettings>().PanelDetails;
        var panelRotation = GetSingleton<SceneSettings>().Rotation;
        float3 placementPosition = pointerPosition;

        // find the nearest grid node if present and update the pointerPosition
        var gridNodeTranslationArray = _gridNodeQuery.ToComponentDataArray<Translation>(Allocator.Temp);
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
        gridNodeTranslationArray.Dispose();

        var panelAddDetails = new PanelAddDetails {
          Position = placementPosition,
          Rotation = panelRotation.Value.value,
          PanelDetails = panelDetails
        };
        ECB.AddSingleFrameComponent(_eventsHolderEntity, panelAddDetails);
      }
    }
    EntityCommandBufferSystem _ECBSys;
    Entity _eventsHolderEntity;
    SceneSettings _sceneSettings;
    EntityQuery _gridNodeQuery;
  }
}