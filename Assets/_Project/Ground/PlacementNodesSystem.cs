using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Tiny;
using Unity.Transforms;

namespace Project
{
  public unsafe class PlacementNodesSystem : SystemBase
  {
    protected override void OnStartRunning()
    {
      _ECBSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
      _eventsHolderEntity = GetSingletonEntity<EventsHolder_tag>();
      _neighbouringNodesColliderEntity = GetSingletonEntity<NeighbouringNodes_tag>();
    }

    protected override void OnUpdate()
    {
      if (!HasComponent<AddPanel_command>(_eventsHolderEntity) &&
          !HasComponent<RemovePanel_command>(_eventsHolderEntity)) return;

      var buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
      var ECB = _ECBSys.CreateCommandBuffer();

      int panelsCount = RoomData.Value.panels.Count;
      var potentialPlacementNodes = new NativeList<UnsafeList<float3>>(panelsCount, Allocator.Temp);
      var overlappingPlacementNodes = new NativeList<float3>(Allocator.Temp);
      for (int i = 0; i < panelsCount; i++)
      {
        var panel = RoomData.Value.panels[i];
        // add placement nodes for this panel
        float3 position = panel.position;
        float widthExtend = 0.575f; // 1.15 * 0.5
        float thicknessExtend = panel.thickness * 0.005f; // 0.01 * 0.5

        // getting the local directions
        quaternion rot = panel.rotation;
        float3 leftDirection = math.mul(rot, math.left());
        float3 rightDirection = math.mul(rot, math.right());
        float3 forwardDirection = math.mul(rot, math.forward());
        float3 backDirection = math.mul(rot, math.back());

        // calculate sorounding placement points
        float3 leftPosition = position + leftDirection * widthExtend * 2f;
        float3 rightPosition = position + rightDirection * widthExtend * 2f;
        float3 front_leftPosition_1 = position + (forwardDirection * (widthExtend - thicknessExtend)) + (leftDirection * (widthExtend + thicknessExtend));
        float3 front_leftPosition_2 = position + (forwardDirection * (widthExtend + thicknessExtend)) + (leftDirection * (widthExtend - thicknessExtend));
        float3 front_rightPosition_1 = position + (forwardDirection * (widthExtend + thicknessExtend)) + (rightDirection * (widthExtend - thicknessExtend));
        float3 front_rightPosition_2 = position + (forwardDirection * (widthExtend - thicknessExtend)) + (rightDirection * (widthExtend + thicknessExtend));
        float3 back_leftPosition_1 = position + (backDirection * (widthExtend - thicknessExtend)) + (leftDirection * (widthExtend + thicknessExtend));
        float3 back_leftPosition_2 = position + (backDirection * (widthExtend + thicknessExtend)) + (leftDirection * (widthExtend - thicknessExtend));
        float3 back_rightPosition_1 = position + (backDirection * (widthExtend + thicknessExtend)) + (rightDirection * (widthExtend - thicknessExtend));
        float3 back_rightPosition_2 = position + (backDirection * (widthExtend - thicknessExtend)) + (rightDirection * (widthExtend + thicknessExtend));

        var nodePositions = new UnsafeList<float3>(10, Allocator.Persistent);
        nodePositions.Add(leftPosition);
        nodePositions.Add(rightPosition);
        nodePositions.Add(front_leftPosition_1);
        nodePositions.Add(front_leftPosition_2);
        nodePositions.Add(front_rightPosition_1);
        nodePositions.Add(front_rightPosition_2);
        nodePositions.Add(back_leftPosition_1);
        nodePositions.Add(back_leftPosition_2);
        nodePositions.Add(back_rightPosition_1);
        nodePositions.Add(back_rightPosition_2);
        potentialPlacementNodes.Add(nodePositions);
      }

      // remove placement nodes if overlapping with other panels
      for (int i = 0; i < panelsCount; i++)
      {
        var panel = RoomData.Value.panels[i];
        EntityManager.SetComponentData(_neighbouringNodesColliderEntity, new Translation { Value = panel.position });
        buildPhysicsWorld.Update();

        for (int ii = 0; ii < potentialPlacementNodes.Length; ii++)
        {
          if (ii == i) continue;

          for (int iii = potentialPlacementNodes[ii].Length - 1; iii >= 0; iii--)
          {
            float3 nodePosition = potentialPlacementNodes[ii][iii];

            var input = new PointDistanceInput
            {
              Position = nodePosition,
              MaxDistance = 0.0001f,
              Filter = new CollisionFilter()
              {
                BelongsTo = 1u << 3,
                CollidesWith = 1u << 2,
                GroupIndex = 0
              }
            };

            if (buildPhysicsWorld.PhysicsWorld.CalculateDistance(input, out DistanceHit hit))
              overlappingPlacementNodes.Add(nodePosition);
          }
        }
      }

      Entities.WithAll<PlacementNode_tag>().ForEach((Entity entity) =>
      { ECB.DestroyEntity(entity); }).Run();

      foreach (var nodeList in potentialPlacementNodes)
        foreach (var position in nodeList)
          if (!overlappingPlacementNodes.Contains(position))
          {
            Entity placementNode_entity = ECB.CreateEntity();
            ECB.AddComponent(placementNode_entity, new Translation { Value = position });
            ECB.AddComponent<PlacementNode_tag>(placementNode_entity);
          }

      _ECBSys.AddJobHandleForProducer(Dependency);
      foreach (var list in potentialPlacementNodes)
        list.Dispose();
      potentialPlacementNodes.Dispose();
      overlappingPlacementNodes.Dispose();
    }
    EntityCommandBufferSystem _ECBSys;
    Entity _eventsHolderEntity;
    Entity _neighbouringNodesColliderEntity;
  }
}
