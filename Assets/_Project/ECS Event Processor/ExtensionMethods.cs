using Unity.Entities;

public static partial class ExtensionMethods
{
  /// <summary>
  /// The component will be added to a temporary entity.
  /// </summary>
  public static void CreateSingleFrameComponent<T>(this EntityCommandBuffer ecb, T component) where T : struct, IComponentData
  {
    var entity = ecb.CreateEntity();
    ecb.AddComponent(entity, component);
    ecb.AddComponent<SingleFrameEntity_tag>(entity);
  }

  /// <summary>
  /// The component will be added to a temporary entity.
  /// </summary>
  public static void CreateSingleFrameComponent<T>(this EntityCommandBuffer ecb) where T : struct, IComponentData
  {
    var entity = ecb.CreateEntity();
    ecb.AddComponent<T>(entity);
    ecb.AddComponent<SingleFrameEntity_tag>(entity);
  }

  /// <summary>
  /// The entity has to have a DynamicBuffer&lt; SingleFrameComponent > attached.
  /// </summary>
  public static void AddSingleFrameComponent<T>(this EntityCommandBuffer ecb, Entity entity, T component) where T : struct, IComponentData
  {
    ecb.AddComponent(entity, component);
    ecb.AppendToBuffer(entity, new SingleFrameComponent { TargetComponent = new ComponentType(typeof(T)) });
  }

  /// <summary>
  /// The entity has to have a DynamicBuffer&lt; SingleFrameComponent > attached.
  /// </summary>
  public static void AddSingleFrameComponent<T>(this EntityCommandBuffer ecb, Entity entity) where T : struct, IComponentData
  {
    ecb.AddComponent<T>(entity);
    ecb.AppendToBuffer(entity, new SingleFrameComponent { TargetComponent = new ComponentType(typeof(T)) });
  }
}
