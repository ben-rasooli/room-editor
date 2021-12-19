#region usings
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
#endregion

namespace Project
{
  public class GridNodeSelectorSystem : SystemBase
  {
    protected override void OnCreate()
    {
      _ECBSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
      _pointerPositionQuery = EntityManager.CreateEntityQuery(typeof(PointerPosition_event));
    }

    protected override void OnStartRunning()
    {
      Entities
        .WithStructuralChanges()
        .WithAll<GridNode_tag>()
        .ForEach((Entity entity, in Translation translation) =>
        {
          _gridNodes.Add(new GridNode { Entity = entity, Position = translation.Value });
        }).WithoutBurst().Run();
    }


    protected override void OnUpdate()
    {
      var ECB = _ECBSys.CreateCommandBuffer();

      // selecting node logic
      Entities
        .ForEach((in PointerDown_event pointerDown) =>
        {
          var pointerPosition = pointerDown.Value;
          var nearestNode = _gridNodes.OrderBy(e => math.distancesq(pointerPosition, e.Position)).First();
          ECB.AddSingleFrameComponent<SelectedGridNode_tag>(nearestNode.Entity);
        }).WithoutBurst().Run();

      // highlighting node logic
      var pointerPosition = _pointerPositionQuery.ToComponentDataArray<PointerPosition_event>(Allocator.Temp);
      if (pointerPosition.Length > 0)
      {
        var nearestNode = _gridNodes.OrderBy(e => math.distancesq(pointerPosition[0].Value, e.Position)).First();
        Entities
          .WithAll<GridNode_tag>()
          .ForEach((Entity entity) =>
          {
            if (entity.Equals(nearestNode.Entity))
            {
              if (!HasComponent<HighlightedGridNode_tag>(entity))
                ECB.AddComponent<HighlightedGridNode_tag>(entity);
            }
            else if (HasComponent<HighlightedGridNode_tag>(entity))
            {
              ECB.RemoveComponent<HighlightedGridNode_tag>(entity);
            }
          }).WithoutBurst().Run();

        _ECBSys.AddJobHandleForProducer(Dependency);
      }

      pointerPosition.Dispose();
    }
    EntityCommandBufferSystem _ECBSys;
    List<GridNode> _gridNodes = new List<GridNode>();
    EntityQuery _pointerPositionQuery;
  }

  struct GridNode
  {
    public Entity Entity;
    public float3 Position;
  }
}