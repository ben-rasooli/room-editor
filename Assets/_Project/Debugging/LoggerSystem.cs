#region usings
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Tiny;
using Unity.Tiny.Input;
using Unity.Transforms;
#endregion

namespace Project
{
  [AlwaysUpdateSystem]
  public class LoggerSystem : SystemBase
  {
    protected override void OnStartRunning()
    {
      _inputSystem = World.GetExistingSystem<InputSystem>();
      _ECBSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
      _sceneSettingsEntity = GetSingletonEntity<SceneSettings>();
      _eventsHolderEntity = GetSingletonEntity<EventsHolder_tag>();
      _gridNodeMarkerPrefab = GetSingleton<SceneSettings>().GridNodeMarkerPrefab;
    }

    protected override void OnUpdate()
    {
      var ECB = _ECBSys.CreateCommandBuffer();

      if (_inputSystem.GetKeyDown(KeyCode.Alpha1))
        ECB.AddSingleFrameComponent(_sceneSettingsEntity, new SwitchPanel_command { Value = PanelType.Thin_220 });
      else if (_inputSystem.GetKeyDown(KeyCode.Alpha2))
        ECB.AddSingleFrameComponent(_sceneSettingsEntity, new SwitchPanel_command { Value = PanelType.Thin_230 });
      else if (_inputSystem.GetKeyDown(KeyCode.Alpha3))
        ECB.AddSingleFrameComponent(_sceneSettingsEntity, new SwitchPanel_command { Value = PanelType.Thin_240 });
      else if (_inputSystem.GetKeyDown(KeyCode.Alpha4))
        ECB.AddSingleFrameComponent(_sceneSettingsEntity, new SwitchPanel_command { Value = PanelType.Thin_260 });
      else if (_inputSystem.GetKeyDown(KeyCode.Alpha5))
        ECB.AddSingleFrameComponent(_sceneSettingsEntity, new SwitchPanel_command { Value = PanelType.Thin_280 });
      else if (_inputSystem.GetKeyDown(KeyCode.Alpha6))
        ECB.AddSingleFrameComponent(_sceneSettingsEntity, new SwitchPanel_command { Value = PanelType.Thin_300 });
      else if (_inputSystem.GetKeyDown(KeyCode.Alpha7))
        ECB.AddSingleFrameComponent(_sceneSettingsEntity, new SwitchPanel_command { Value = PanelType.Thin_360 });
      else if (_inputSystem.GetKeyDown(KeyCode.Alpha8))
        ECB.AddSingleFrameComponent(_sceneSettingsEntity, new SwitchPanel_command { Value = PanelType.Thick_240 });
      else if (_inputSystem.GetKeyDown(KeyCode.Alpha9))
        ECB.AddSingleFrameComponent(_sceneSettingsEntity, new SwitchPanel_command { Value = PanelType.Thick_270 });

      if (
        _inputSystem.GetKeyDown(KeyCode.Alpha1) ||
        _inputSystem.GetKeyDown(KeyCode.Alpha2) ||
        _inputSystem.GetKeyDown(KeyCode.Alpha3) ||
        _inputSystem.GetKeyDown(KeyCode.Alpha4) ||
        _inputSystem.GetKeyDown(KeyCode.Alpha5) ||
        _inputSystem.GetKeyDown(KeyCode.Alpha6) ||
        _inputSystem.GetKeyDown(KeyCode.Alpha7) ||
        _inputSystem.GetKeyDown(KeyCode.Alpha8) ||
        _inputSystem.GetKeyDown(KeyCode.Alpha9))
      {
        ECB.CreateSingleFrameComponent(new ChangeModificationMode_command { Value = ModificationMode.PanelAddition });
      }
      
      if (_inputSystem.GetKeyDown(KeyCode.Alpha0))
      {
        ECB.AddSingleFrameComponent(_eventsHolderEntity, new PanelSwitched_event { Value = GetSingleton<SceneSettings>().DoorPrefab });
        ECB.CreateSingleFrameComponent(new ChangeModificationMode_command { Value = ModificationMode.DoorAddition });
      }

      // change modification mode
      if (_inputSystem.GetKeyDown(KeyCode.Space))
        ECB.CreateSingleFrameComponent( new ChangeModificationMode_command { Value = ModificationMode.Subtraction});

      // panel rotation logic
      if (_inputSystem.GetKeyDown(KeyCode.LeftArrow) || _inputSystem.GetKeyDown(KeyCode.RightArrow))
      {
        float angle = math.radians(90f);
        if (_inputSystem.GetKeyDown(KeyCode.LeftArrow)) angle = -angle;
        _sceneSettings = GetSingleton<SceneSettings>();
        _sceneSettings.Rotation.Value = math.mul(_sceneSettings.Rotation.Value, quaternion.RotateY(angle));
        SetSingleton(_sceneSettings);
      }


      if (HasComponent<AddPanel_command>(_eventsHolderEntity) ||
        HasComponent<RemovePanel_command>(_eventsHolderEntity))
      {
        ECB.AddSingleFrameComponent<Debug_event>(_eventsHolderEntity);
      }

      if (HasComponent<Debug_event>(_eventsHolderEntity))
      {
        Entities.WithAll<Debug_tag>().ForEach((Entity entity) =>
        { ECB.DestroyEntity(entity); }).Run();

        Entities
          .WithAll<PlacementNode_tag>()
          .ForEach((in Translation translation) =>
          {
            var cloneMarkerEntity = ECB.Instantiate(_gridNodeMarkerPrefab);
            ECB.SetComponent(cloneMarkerEntity, translation);
            ECB.AddComponent<Debug_tag>(cloneMarkerEntity);
          }).WithoutBurst().Run();
      }


      //Entities
      //  .WithChangeFilter<RoomStat>()
      //  .ForEach((in DynamicBuffer<RoomStat> roomStatsList) =>
      //  {
      //      double total = 0;
      //      foreach (var stat in roomStatsList)
      //      {
      //          Debug.Log($"{stat.Type}: {stat.Count} - ${stat.Count * stat.Price}");
      //          total += stat.Count * stat.Price;
      //      }
      //      Debug.Log($"Total: ${total}");
      //      Debug.Log("----------------");
      //  }).WithoutBurst().Run();

      //Entities.ForEach((Entity entity,in PointerPosition_listener pointerDown_Event) =>
      //{
      //  Debug.Log(entity.Index);
      //}).WithoutBurst().Run();

      _ECBSys.AddJobHandleForProducer(Dependency);
    }
    InputSystem _inputSystem;
    EntityCommandBufferSystem _ECBSys;
    Entity _sceneSettingsEntity;
    Entity _eventsHolderEntity;
    Entity _gridNodeMarkerPrefab;
    SceneSettings _sceneSettings;
  }

  public struct Debug_tag : IComponentData
  {

  }
}