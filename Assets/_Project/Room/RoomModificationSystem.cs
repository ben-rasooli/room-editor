#region usings
using Unity.Entities;
using Unity.Transforms;
using System.Collections.Generic;
using Unity.Tiny;
#endregion

namespace Project
{
  //[AlwaysUpdateSystem]
  //[UpdateInGroup(typeof(InputPointerFSM.PanelAdditionModeState))]
  public class RoomModificationSystem_PanelAdditionModeState : SystemBase
  {
    protected override void OnStartRunning()
    {
      Debug.Log("RoomModificationSystem_PanelAdditionModeState");
      _ECBSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
      _eventsHolderEntity = GetSingletonEntity<EventsHolder_tag>();
      _panelDetailsEntity = GetSingletonEntity<SceneSettings>();
    }

    protected override void OnUpdate()
    {
      var ECB = _ECBSys.CreateCommandBuffer();

      Entities
        .ForEach((in AddPanelRequest_command command) =>
        {
          foreach (var panel in RoomData.Value.panels)
            if (command.Position.Equals(panel.position))
              return;

          var newPanel = new RoomDataStructure.panel
          {
            height = command.PanelDetails.Height,
            thickness = command.PanelDetails.Thickness,
            position = command.Position,
            rotation = command.Rotation
          };
          RoomData.Value.panels.Add(newPanel);

          ECB.CreateSingleFrameComponent(new AddPanel_command
          {
            Position = command.Position,
            Rotation = command.Rotation,
            PanelDetails = command.PanelDetails
          });
          ECB.AddSingleFrameComponent<AddPanel_command>(_eventsHolderEntity);
        }).WithoutBurst().Run();

      Entities
        .ForEach((in AddDoorRequest_command command) =>
        {
          foreach (var door in RoomData.Value.doors)
            if (command.Position.Equals(door.position))
              return;

          var newDoor = new RoomDataStructure.door
          {
            position = command.Position,
            rotation = command.Rotation.value
          };
          RoomData.Value.doors.Add(newDoor);

          ECB.CreateSingleFrameComponent(new AddDoor_command
          {
            Position = command.Position,
            Rotation = command.Rotation
          });
          ECB.AddSingleFrameComponent<AddDoor_command>(_eventsHolderEntity);
        }).WithoutBurst().Run();

      Entities
        .WithAll<Panel_tag, Remove_tag>()
        .ForEach((ref Translation translation) =>
        {
          var position = translation.Value;
          var panelIndex = RoomData.Value.panels.FindIndex(p => position.Equals(p.position));
          if (panelIndex < 0) return;
          var panel = RoomData.Value.panels[panelIndex];
          RoomData.Value.panels.RemoveAt(panelIndex);
          PanelDetails removedPanelDetails = default;
          var panelDetailsList = EntityManager.GetBuffer<PanelDetails>(_panelDetailsEntity);
          foreach (var item in panelDetailsList)
            if (item.Height == panel.height && item.Thickness == panel.thickness)
              removedPanelDetails = item;
          ECB.CreateSingleFrameComponent(new RemovePanel_command { PanelDetails = removedPanelDetails });
          ECB.AddSingleFrameComponent<RemovePanel_command>(_eventsHolderEntity);
        }).WithoutBurst().Run();

      Entities
        .WithAll<Door_tag, Remove_tag>()
        .ForEach((ref Translation translation) =>
        {
          var position = translation.Value;
          var doorIndex = RoomData.Value.doors.FindIndex(d => position.Equals(d.position));
          if (doorIndex < 0) return;
          RoomData.Value.doors.RemoveAt(doorIndex);
          ECB.CreateSingleFrameComponent<RemoveDoor_command>();
          ECB.AddSingleFrameComponent<RemoveDoor_command>(_eventsHolderEntity);
        }).WithoutBurst().Run();

      _ECBSys.AddJobHandleForProducer(Dependency);
    }
    EntityCommandBufferSystem _ECBSys;
    Entity _eventsHolderEntity;
    Entity _panelDetailsEntity;
  }

  //----------------------------------------------------------------

  //[UpdateInGroup(typeof(InputPointerFSM.DoorAdditionModeState))]
  //public class RoomModificationSystem_DoorAdditionModeState : ComponentSystem
  //{
  //  protected override void OnStartRunning()
  //  {
  //    Debug.Log("RoomModificationSystem_PanelAdditionModeState");
  //    _ECBSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
  //    _eventsHolderEntity = GetSingletonEntity<EventsHolder_tag>();
  //  }

  //  protected override void OnUpdate()
  //  {
  //    var ECB = _ECBSys.CreateCommandBuffer();

  //    Entities
  //      .ForEach((ref DoorAddDetails doorAddDetails) =>
  //      {
  //        foreach (var panel in RoomData.Value.panels)
  //          if (doorAddDetails.Position.Equals(panel.position))
  //            return;

  //        var newDoor = new RoomDataStructure.door
  //        {
  //          position = doorAddDetails.Position,
  //          rotation = doorAddDetails.Rotation
  //        };
  //        RoomData.Value.doors.Add(newDoor);

  //        ECB.CreateSingleFrameComponent(new AddDoor_command
  //        {
  //          Position = doorAddDetails.Position,
  //          Rotation = doorAddDetails.Rotation
  //        });
  //        ECB.AddSingleFrameComponent<AddDoor_command>(_eventsHolderEntity);
  //      });
  //  }
  //  EntityCommandBufferSystem _ECBSys;
  //  Entity _eventsHolderEntity;
  //}

  //----------------------------------------------------------------

  //[UpdateInGroup(typeof(InputPointerFSM.SubtractionModeState))]
  //public class RoomModificationSystem_SubtractionModeState : ComponentSystem
  //{
  //  protected override void OnStartRunning()
  //  {
  //    _ECBSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
  //    _panelDetailsEntity = GetSingletonEntity<SceneSettings>();
  //    _eventsHolderEntity = GetSingletonEntity<EventsHolder_tag>();
  //  }

  //  protected override void OnUpdate()
  //  {
  //    var ECB = _ECBSys.CreateCommandBuffer();


  //  }
  //  EntityCommandBufferSystem _ECBSys;
  //  Entity _panelDetailsEntity;
  //  Entity _eventsHolderEntity;
  //}
}