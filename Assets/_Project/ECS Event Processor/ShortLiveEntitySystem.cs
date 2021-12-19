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
        .ForEach((Entity entity, ref DynamicBuffer<SingleFrameComponent> components) =>
        {
          if (components.Length > 0)
            for (int i = components.Length - 1; i >= 0; i--)
            {
              ecb.RemoveComponent(entity, components[i].TargetComponent);
              components.RemoveAt(i);
            }
        }).WithoutBurst().Run();
    _ecbSystem.AddJobHandleForProducer(Dependency);
  }
  EndSimulationEntityCommandBufferSystem _ecbSystem;
}

public struct SingleFrameEntity_tag : IComponentData { }
public struct EventsHolder_tag : IComponentData { }
