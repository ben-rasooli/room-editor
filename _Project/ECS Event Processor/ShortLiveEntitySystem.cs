using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public class ShortLiveEntitySystem : SystemBase
{
  protected override void OnCreate()
  {
    _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    if (!HasSingleton<EventsHolder_tag>())
    {
      var entity = EntityManager.CreateEntity(typeof(EventsHolder_tag));
      EntityManager.AddBuffer<SingleFrameComponent>(entity);
    }
  }

  protected override void OnUpdate()
  {
    EntityCommandBuffer ecb = _ecbSystem.CreateCommandBuffer();

    Entities.WithAll<SingleFrameEntity_tag>().ForEach((Entity entity) =>
    {
      ecb.DestroyEntity(entity);
    }).Schedule();

    Entities
        .WithAll<SingleFrameComponents_tag>()
        .ForEach((Entity entity, in DynamicBuffer<SingleFrameComponent> components) =>
        {
          foreach (var component in components)
            ecb.RemoveComponent(entity, component.TargetComponent);
          ecb.SetBuffer<SingleFrameComponent>(entity);
          ecb.RemoveComponent<SingleFrameComponents_tag>(entity);
        }).Schedule();
    _ecbSystem.AddJobHandleForProducer(Dependency);
  }
  EndSimulationEntityCommandBufferSystem _ecbSystem;
}

public struct SingleFrameEntity_tag : IComponentData { }
public struct SingleFrameComponents_tag : IComponentData { }
public struct EventsHolder_tag : IComponentData { }
