using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Tiny;
using Unity.Tiny.JSON;
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

  [UpdateInGroup(typeof(InputPointerFSM.DoorAdditionModeState))]
  public class InputPointerSystem_DoorAdditionModeState : PointerSystemBase
  {
    protected override void OnStartRunning()
    {
      base.OnStartRunning();
      _ECBSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
      _eventsHolderEntity = GetSingletonEntity<EventsHolder_tag>();
    }

    protected override void OnUpdate()
    {
      // mouse position logic
      if (!m_InputSystem.GetMouseButton(0))
      {
        if (castRayAgainstPanels(m_InputSystem.GetInputPosition(), out Entity entity, out float3 hitPosition))
        {
          var panelRotation = GetComponent<Rotation>(entity);
          float3 panelPosition = GetComponent<Translation>(entity).Value;
          hitPosition.y = 0f;
          if (panelRotation.Value.value.RoughlyEquals(_0_degreeRotation) ||
              panelRotation.Value.value.RoughlyEquals(_0n_degreeRotation) || 
              panelRotation.Value.value.RoughlyEquals(_180_degreeRotation) ||
              panelRotation.Value.value.RoughlyEquals(_180n_degreeRotation))
            hitPosition.z = panelPosition.z;
          else
            hitPosition.x = panelPosition.x;
          EntityManager.AddComponentData(_eventsHolderEntity, new PointerPosition_event
          {
            Value = hitPosition,
            PanelRotation = panelRotation.Value
          });
        }
        else
          EntityManager.RemoveComponent<PointerPosition_event>(_eventsHolderEntity);
      }

      base.OnUpdate();
    }
    float4 _0_degreeRotation = new float4(0f, 0f, 0f, 1f);
    float4 _0n_degreeRotation = new float4(0f, 0f, 0f, -1f);
    float4 _180_degreeRotation = new float4(0f, 1f, 0f, 0f);
    float4 _180n_degreeRotation = new float4(0f, -1f, 0f, 0f);

    protected override void OnInputClick(int pointerId, float2 inputPos)
    {
      var ECB = _ECBSys.CreateCommandBuffer();
      Entities
        .WithAll<PointerDown_listener>()
        .ForEach((in PhysicsCollider physicsCollider) =>
        {
          if (castRayAgainstPanels(m_InputSystem.GetInputPosition(), out Entity entity, out float3 hitPosition))
          {
            float3 panelPosition = GetComponent<Translation>(entity).Value;
            hitPosition.y = 0f;
            if (math.abs(hitPosition.x) < math.abs(hitPosition.z))
              hitPosition.x = panelPosition.x;
            else
              hitPosition.z = panelPosition.z;
            ECB.AddSingleFrameComponent(_eventsHolderEntity, new PointerDown_event { Value = hitPosition });
          }
        }).WithoutBurst().Run();

      _ECBSys.AddJobHandleForProducer(Dependency);
    }
    EntityCommandBufferSystem _ECBSys;
    Entity _eventsHolderEntity;
  }

  //----------------------------------------------------------------

  [UpdateInGroup(typeof(InputPointerFSM.SubtractionModeState))]
  public class InputPointerSystem_SubtractionModeState : PointerSystemBase
  {
    protected override void OnStartRunning()
    {
      base.OnStartRunning();
      _ECBSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnInputClick(int pointerId, float2 inputPos)
    {
      var ECB = _ECBSys.CreateCommandBuffer();

      if (castRayAgainstPanelsAndDoors(inputPos, out Entity entity))
      {
        if (HasComponent<Panel_tag>(entity) || HasComponent<Door_tag>(entity))
          ECB.AddComponent<Remove_tag>(entity);
      }
    }
    EntityCommandBufferSystem _ECBSys;
  }
}
