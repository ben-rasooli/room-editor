#region usings
using Unity.Entities;
using Unity.Transforms;
using System.Collections.Generic;
using Unity.Tiny;
#endregion

namespace Project
{
  [UpdateInGroup(typeof(InputPointerFSM.PanelAdditionModeState))]
  public class RoomModificationSystem_PanelAdditionModeState : ComponentSystem
  {
    protected override void OnStartRunning()
    {
      _ECBSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
      _eventsHolderEntity = GetSingletonEntity<EventsHolder_tag>();
    }

    protected override void OnUpdate()
    {
      var ECB = _ECBSys.CreateCommandBuffer();

      Entities
        .ForEach((ref PanelAddDetails panelAddDetails) =>
        {
          foreach (var panel in RoomData.Value.panels)
            if (panelAddDetails.Position.Equals(panel.position))
              return;

          var newPanel = new RoomDataStructure.panel
          {
            height = panelAddDetails.PanelDetails.Height,
            thickness = panelAddDetails.PanelDetails.Thickness,
            position = panelAddDetails.Position,
            rotation = panelAddDetails.Rotation
          };
          RoomData.Value.panels.Add(newPanel);

          ECB.CreateSingleFrameComponent(new AddPanel_command
          {
            Position = panelAddDetails.Position,
            Rotation = panelAddDetails.Rotation,
            PanelDetails = panelAddDetails.PanelDetails
          });
          ECB.AddSingleFrameComponent<AddPanel_command>(_eventsHolderEntity);
        });
    }
    EntityCommandBufferSystem _ECBSys;
    Entity _eventsHolderEntity;
  }

  //----------------------------------------------------------------

  [UpdateInGroup(typeof(InputPointerFSM.PanelSubtractionModeState))]
  public class RoomModificationSystem_PanelSubtractionModeState : ComponentSystem
  {
    protected override void OnStartRunning()
    {
      _ECBSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
      _panelDetailsEntity = GetSingletonEntity<SceneSettings>();
      _eventsHolderEntity = GetSingletonEntity<EventsHolder_tag>();
    }

    protected override void OnUpdate()
    {
      var ECB = _ECBSys.CreateCommandBuffer();

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
        });
    }
    EntityCommandBufferSystem _ECBSys;
    Entity _panelDetailsEntity;
    Entity _eventsHolderEntity;
  }
}