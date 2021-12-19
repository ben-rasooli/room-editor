#region usings
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
#endregion

namespace Project
{
  public class ProcessInputSystem : SystemBase
  {
    protected override void OnCreate()
    {
      base.OnCreate();
      _ECBSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    protected override void OnStartRunning()
    {
      base.OnStartRunning();
      _mainCamera = Camera.main;
    }


    protected override void OnUpdate()
    {
      var ECB = _ECBSys.CreateCommandBuffer();
      var screenRay = _mainCamera.ScreenPointToRay(Input.mousePosition);

      // panel selection logic
      if (Input.GetMouseButtonDown(0))
      {
        Entities
          .WithAll<PointerDown_listener>()
          .ForEach((Entity entity, in PhysicsCollider physicsCollider) =>
          {
            if (pointerHitGround(ref screenRay, ref physicsCollider.Value.Value, out float3 hitPosition))
            {
              ECB.AddSingleFrameComponent(entity, new PointerDown_event { Value = hitPosition });
            }
          }).WithoutBurst().Run();
      }

      // mouse position logic
      Entities
        .WithAll<PointerPosition_listener>()
        .ForEach((Entity entity, in PhysicsCollider physicsCollider) =>
        {
          if (pointerHitGround(ref screenRay, ref physicsCollider.Value.Value, out float3 hitPosition))
            ECB.AddComponent(entity, new PointerPosition_event { Value = hitPosition });
          else
            ECB.RemoveComponent<PointerPosition_event>(entity);
        }).WithoutBurst().Run();

      // panel rotation logic
      if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
      {
        var sceneSettings = GetSingleton<SceneSettings>();
        float angle = math.radians(90f);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) angle = -angle;
        sceneSettings.Rotation.Value = math.mul(sceneSettings.Rotation.Value, quaternion.RotateY(angle));
        SetSingleton<SceneSettings>(sceneSettings);
      }

      if (Input.GetKeyDown(KeyCode.Space))
        ECB.CreateSingleFrameComponent<ChangeModificationMode_command>();

      _ECBSys.AddJobHandleForProducer(Dependency);
    }
    EntityCommandBufferSystem _ECBSys;

    public static bool pointerHitGround(ref UnityEngine.Ray screenRay, ref Unity.Physics.Collider physicsCollider, out float3 hitPosition)
    {
      RaycastInput input = new RaycastInput()
      {
        Start = screenRay.origin,
        End = screenRay.GetPoint(200),
        Filter = new CollisionFilter()
        {
          BelongsTo = ~0u,
          CollidesWith = ~0u,
          GroupIndex = 0
        }
      };

      bool haveHit = physicsCollider.CastRay(input, out Unity.Physics.RaycastHit hit);
      if (haveHit)
      {
        hitPosition = hit.Position;
        return true;
      }

      hitPosition = float3.zero;
      return false;
    }
    static Camera _mainCamera;
  }
}