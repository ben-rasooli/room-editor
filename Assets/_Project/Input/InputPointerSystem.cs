using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Tiny;
using Unity.Tiny.Rendering;
using Unity.Tiny.UI;
using Unity.Transforms;

namespace Project
{
  [UpdateInGroup(typeof(InputPointerFSM.PanelAdditionModeState))]
  public class InputPointerSystem_PanelAdditionModeState : PointerSystemBase
  {
    protected override void OnStartRunning()
    {
      base.OnStartRunning();
      _ECBSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
      _eventsHolderEntity = GetSingletonEntity<EventsHolder_tag>();
    }

    protected override void OnUpdate()
    {
      var ECB = _ECBSys.CreateCommandBuffer();

      // mouse position logic
      if (!m_InputSystem.GetMouseButton(0))
        Entities
            .WithAll<PointerPosition_listener>()
            .ForEach((Entity entity, in PhysicsCollider physicsCollider) =>
            {
              if (pointerHitGround(m_InputSystem.GetInputPosition(), ref physicsCollider.Value.Value, out float3 hitPosition))
                ECB.AddComponent(_eventsHolderEntity, new PointerPosition_event { Value = hitPosition });
              else
                ECB.RemoveComponent<PointerPosition_event>(_eventsHolderEntity);
            }).WithoutBurst().Run();

      _ECBSys.AddJobHandleForProducer(Dependency);

      base.OnUpdate();
    }

    protected override void OnInputClick(int pointerId, float2 inputPos)
    {
      var ECB = _ECBSys.CreateCommandBuffer();
      Entities
          .WithAll<PointerDown_listener>()
          .ForEach((in PhysicsCollider physicsCollider) =>
          {
            if (pointerHitGround(inputPos, ref physicsCollider.Value.Value, out float3 hitPosition))
              ECB.AddSingleFrameComponent(_eventsHolderEntity, new PointerDown_event { Value = hitPosition });
          }).WithoutBurst().Run();

      _ECBSys.AddJobHandleForProducer(Dependency);
    }
    EntityCommandBufferSystem _ECBSys;
    Entity _eventsHolderEntity;
  }

  //----------------------------------------------------------------

  [UpdateInGroup(typeof(InputPointerFSM.PanelSubtractionModeState))]
  public class InputPointerSystem_PanelSubtractionModeState : PointerSystemBase
  {
    protected override void OnStartRunning()
    {
      base.OnStartRunning();
      _ECBSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnInputClick(int pointerId, float2 inputPos)
    {
      var ECB = _ECBSys.CreateCommandBuffer();

      if (castRayAgainstPanels(inputPos, out Entity panelEntity))
        if (HasComponent<Panel_tag>(panelEntity))
          ECB.AddComponent<Remove_tag>(panelEntity);
    }
    EntityCommandBufferSystem _ECBSys;
  }
}
