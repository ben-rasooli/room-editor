using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Tiny;
using Unity.Tiny.Input;
using Unity.Tiny.Rendering;
using Unity.Tiny.UI;

namespace Project
{
  /// <summary>
  ///     Translate mouse and touch inputs into overrideable functions
  ///     Create a convenience function to detect object under pointer
  /// </summary>
  public abstract class PointerSystemBase : SystemBase
  {
    protected InputSystem m_InputSystem;
    protected ScreenToWorld m_ScreenToWorld;

    protected CollisionFilter m_CollisionFilter;
    protected float m_RayDistance = 200f;

    protected override void OnCreate()
    {
      m_InputSystem = World.GetExistingSystem<InputSystem>();
      m_ScreenToWorld = World.GetExistingSystem<ScreenToWorld>();

      // Category names are defined in Assets/PhysicsCategoryNames and assgined to PhysicsShape components in Editor
      m_CollisionFilter = new CollisionFilter
      {
        BelongsTo = 1u << 30,           // Set raycast to Input category
        CollidesWith = 0xffffffff,      // Target everything
        GroupIndex = 0,
      };
    }

    protected override void OnUpdate()
    {
      // Touch
      if (m_InputSystem.IsTouchSupported() && m_InputSystem.TouchCount() > 0)
      {
        for (var i = 0; i < m_InputSystem.TouchCount(); i++)
        {
          var touch = m_InputSystem.GetTouch(i);
          var inputPos = new float2(touch.x, touch.y);

          switch (touch.phase)
          {
            case TouchState.Began:
              _pointerDownPosition = inputPos;
              OnInputDown(i, inputPos);
              break;

            case TouchState.Moved:
              var inputDelta = new float2(touch.deltaX, touch.deltaY);
              OnInputMove(i, inputPos, inputDelta);
              break;

            case TouchState.Stationary:
              break;

            case TouchState.Ended:
              if (math.distancesq(inputPos, _pointerDownPosition) < _pointerClickThreshold)
                OnInputClick(-1, inputPos);
              OnInputUp(i, inputPos);
              break;

            case TouchState.Canceled:
              OnInputCanceled(i);
              break;
          }
        }
      }

      // Mouse
      else if (m_InputSystem.IsMousePresent())
      {
        var inputPos = m_InputSystem.GetInputPosition();

        if (m_InputSystem.GetMouseButtonDown(0))
        {
          _pointerDownPosition = inputPos;
          OnInputDown(-1, inputPos);
        }
        else if (m_InputSystem.GetMouseButton(0))
        {
          OnInputMove(-1, inputPos, m_InputSystem.GetInputDelta());
          _pointerStartedOnUI = false;
          Entities.ForEach((in UIState state) =>
          {
            if (state.IsPressed)
              _pointerStartedOnUI = true;
          }).WithoutBurst().Run();
        }
        else if (m_InputSystem.GetMouseButtonUp(0))
        {
          OnInputUp(-1, inputPos);
          if (!_pointerStartedOnUI)
            if (math.distancesq(inputPos, _pointerDownPosition) < _pointerClickThreshold)
              OnInputClick(-1, inputPos);
        }

        if (m_InputSystem.GetMouseButton(1))
          OnSecondaryInputMove(-1, inputPos, m_InputSystem.GetInputDelta());
      }
    }
    float2 _pointerDownPosition;
    float _pointerClickThreshold = 25f;
    bool _pointerStartedOnUI;

    protected virtual void OnInputDown(int pointerId, float2 inputPos) { }

    protected virtual void OnInputMove(int pointerId, float2 inputPos, float2 inputDelta) { }

    protected virtual void OnSecondaryInputMove(int pointerId, float2 inputPos, float2 inputDelta) { }

    protected virtual void OnInputUp(int pointerId, float2 inputPos) { }

    protected virtual void OnInputClick(int pointerId, float2 inputPos) { }

    protected virtual void OnInputCanceled(int pointerId) { }

    protected bool castRayAgainstPanels(float2 inputPos, out Entity panelEntity)
    {
      var result = castRay(
        inputPos, new CollisionFilter()
        {
          BelongsTo = 1u << 0,
          CollidesWith = 1u << 0,
          GroupIndex = 0
        },
        out Entity entity);

      panelEntity = entity;
      return result;
    }

    protected bool castRay(float2 inputPos, CollisionFilter filter, out Entity entity)
    {
      ref PhysicsWorld physicsWorld = ref World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld;

      // Convert input position to ray going from screen to world
      float3 rayOrigin, rayDirection;
      m_ScreenToWorld.InputPosToWorldSpaceRay(inputPos, out rayOrigin, out rayDirection);

      var RaycastInput = new RaycastInput
      {
        Start = rayOrigin,
        End = rayOrigin + rayDirection * m_RayDistance,
        Filter = filter
      };

      if (physicsWorld.CastRay(RaycastInput, out RaycastHit hit))
      {
        entity = physicsWorld.Bodies[hit.RigidBodyIndex].Entity; ;
        return true;
      }
      else
      {
        entity = Entity.Null;
        return false;
      }
    }

    protected bool pointerHitGround(float2 inputPos, ref Collider physicsCollider, out float3 hitPosition)
    {
      ref PhysicsWorld physicsWorld = ref World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld;

      // Convert input position to ray going from screen to world
      float3 rayOrigin, rayDirection;
      m_ScreenToWorld.InputPosToWorldSpaceRay(inputPos, out rayOrigin, out rayDirection);

      var raycastInput = new RaycastInput
      {
        Start = rayOrigin,
        End = rayOrigin + rayDirection * m_RayDistance,
        Filter = new CollisionFilter()
        {
          BelongsTo = 1u << 1,
          CollidesWith = 1u << 1,
          GroupIndex = 0
        }
      };

      // Return top-most entity that was hit by ray
      bool haveHit = physicsCollider.CastRay(raycastInput, out RaycastHit hit);
      if (haveHit)
      {
        hitPosition = hit.Position;
        return true;
      }

      hitPosition = float3.zero;
      return false;
    }
  }
}