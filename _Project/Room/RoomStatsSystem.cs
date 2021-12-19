using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Project
{
  public class RoomStatsSystem : SystemBase
  {
    protected override void OnCreate()
    {
      _ECBSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
      _removePanelEvent_query = EntityManager.CreateEntityQuery(typeof(RemovePanel_command));
    }

    protected override void OnStartRunning()
    {
      _eventsHolderEntity = GetSingletonEntity<EventsHolder_tag>();
      var panelDetailsList = EntityManager.GetBuffer<PanelDetails>(GetSingletonEntity<SceneSettings>());
      var roomStatsList = EntityManager.GetBuffer<RoomStat>(GetSingletonEntity<RoomStat>());
      foreach (var panel in panelDetailsList)
        roomStatsList.Add(new RoomStat
        {
          Type = panel.PanelType,
          Count = 0,
          Price = panel.Price
        });
    }

    protected override void OnUpdate()
    {
      if (HasComponent<AddPanel_command>(_eventsHolderEntity))
      {
        var panelType = GetComponent<AddPanel_command>(_eventsHolderEntity).PanelPrefab.PanelType;
        Entities
          .ForEach((ref DynamicBuffer<RoomStat> roomStatsList) =>
          {
            for (int i = 0; i < roomStatsList.Length; i++)
            {
              var roomStats = roomStatsList[i];
              if (roomStats.Type == panelType)
                roomStats.Count++;
              roomStatsList[i] = roomStats;
            }
          }).Run();
      }

      var removePanelEvent = _removePanelEvent_query.ToComponentDataArray<RemovePanel_command>(Allocator.Temp);
      if (removePanelEvent.Length > 0)
        Entities
          .ForEach((ref DynamicBuffer<RoomStat> roomStatsList) =>
          {
            for (int i = 0; i < roomStatsList.Length; i++)
            {
              var roomStats = roomStatsList[i];
              if (roomStats.Type == removePanelEvent[0].PanelPrefab.PanelType && roomStats.Count > 0)
                roomStats.Count--;
              roomStatsList[i] = roomStats;
            }
          }).WithDisposeOnCompletion(removePanelEvent).WithoutBurst().Run();
    }
    EntityCommandBufferSystem _ECBSys;
    EntityQuery _addPanelEvent_query;
    EntityQuery _removePanelEvent_query;
    Entity _eventsHolderEntity;
  }
}
