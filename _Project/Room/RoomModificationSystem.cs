#region usings
using Unity.Entities;
using Unity.Transforms;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;
#endregion

namespace Project
{
  public class RoomModificationSystem : ComponentSystem
  {
    protected override void OnCreate()
    {
      base.OnCreate();
      _ECBSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
      _roomData = new RoomDataStructure
      {
        panels = new List<RoomDataStructure.panel>(),
        doors = new List<RoomDataStructure.door>(),
        name = "sample name"
      };
    }

    protected override void OnStartRunning()
    {
      _eventsHolderEntity = GetSingletonEntity<EventsHolder_tag>();
      _panelDetailsEntity = GetSingletonEntity<SceneSettings>();
    }

    protected override void OnUpdate()
    {
      var ECB = _ECBSys.CreateCommandBuffer();
      var modMode = GetSingleton<ModificationModeData>().Value;
      var panelDetails = GetSingleton<SceneSettings>().PanelDetails;
      var panelRotation = GetSingleton<SceneSettings>().Rotation;

      Entities
        .WithAll<SelectedGridNode_tag>()
        .ForEach((ref Translation translation) =>
        {
          var position = translation.Value;

          if (modMode == ModificationMode.Addition)
          {
            foreach (var panel in _roomData.panels)
              if (position.Equals(panel.position))
                return;

            var newPanel = new RoomDataStructure.panel
            {
              height = panelDetails.Height,
              thickness = panelDetails.Thickness,
              position = position,
              rotation = panelRotation.Value.value
            };
            _roomData.panels.Add(newPanel);

            ECB.AddSingleFrameComponent(_eventsHolderEntity,
              new AddPanel_command
              {
                Position = position,
                Rotation = panelRotation.Value,
                PanelPrefab = panelDetails
              });
          }
          else if (modMode == ModificationMode.Subtraction)
          {
            var panelIndex = _roomData.panels.FindIndex(p => position.Equals(p.position));
            if (panelIndex < 0) return;
            var panel = _roomData.panels[panelIndex];
            PanelDetails removedPanelDetails = default;
            var panelDetailsList = EntityManager.GetBuffer<PanelDetails>(_panelDetailsEntity);
            foreach (var item in panelDetailsList)
              if (item.Height == panel.height && item.Thickness == panel.thickness)
                removedPanelDetails = item;
            _roomData.panels.RemoveAt(panelIndex);
            ECB.CreateSingleFrameComponent(
              new RemovePanel_command
              {
                Value = position,
                PanelPrefab = removedPanelDetails
              }
            );
          }
        });
    }
    EntityCommandBufferSystem _ECBSys;
    RoomDataStructure _roomData;
    Entity _eventsHolderEntity;
    Entity _panelDetailsEntity;
  }
}