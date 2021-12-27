#region usings
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny;
using Unity.Transforms;
#endregion

namespace Project
{
  [UpdateInGroup(typeof(InputPointerFSM.PanelAdditionModeState))]
  public class PanelAddPositionRefinerSystem : SystemBase
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

        var addPanelRequestCommand = new AddPanelRequest_command {
          Position = placementPosition,
          Rotation = panelRotation.Value.value,
          PanelDetails = panelDetails
        };
        ECB.AddSingleFrameComponent(_eventsHolderEntity, addPanelRequestCommand);
      }
    }
    EntityCommandBufferSystem _ECBSys;
    Entity _eventsHolderEntity;
    SceneSettings _sceneSettings;
    EntityQuery _gridNodeQuery;
  }

  //----------------------------------------------------------------

  [UpdateInGroup(typeof(InputPointerFSM.DoorAdditionModeState))]
  public class DoorAddPositionRefinerSystem : SystemBase
  {
    protected override void OnStartRunning()
    {
      _ECBSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
      _eventsHolderEntity = GetSingletonEntity<EventsHolder_tag>();
      _doorGhostEntity = GetSingletonEntity<PanelGhost_tag>();
    }

    protected override void OnUpdate()
    {
      var ECB = _ECBSys.CreateCommandBuffer();

      if (HasComponent<PointerDown_event>(_eventsHolderEntity))
      {
        float3 placementPosition = EntityManager.GetComponentData<Translation>(_doorGhostEntity).Value;
        quaternion placementRotation = EntityManager.GetComponentData<Rotation>(_doorGhostEntity).Value;

        var addDoorRequestCommand = new AddDoorRequest_command
        {
          Position = placementPosition,
          Rotation = placementRotation
        };
        ECB.AddSingleFrameComponent(_eventsHolderEntity, addDoorRequestCommand);
      }
    }
    EntityCommandBufferSystem _ECBSys;
    Entity _eventsHolderEntity;
    Entity _doorGhostEntity;
  }
}