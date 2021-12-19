using Unity.Collections;
using Unity.Entities;
using Unity.Tiny;

namespace Project
{
  public class RoomStatsSystem : SystemBase
  {
    protected override void OnCreate()
    {
      _ECBSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
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
      var roomStats = EntityManager.GetBuffer<RoomStat>(GetSingletonEntity<RoomStat>());

      Entities
        .WithNone<EventsHolder_tag>()
        .ForEach((in AddPanel_command command) =>
        {
          for (int i = 0; i < roomStats.Length; i++)
          {
            var roomStat = roomStats[i];
            if (roomStat.Type == command.PanelDetails.PanelType)
              roomStat.Count++;
            roomStats[i] = roomStat;
          }
        }).Run();

      Entities
        .WithNone<EventsHolder_tag>()
        .ForEach((in RemovePanel_command command) =>
        {
          for (int i = 0; i < roomStats.Length; i++)
          {
            var roomStat = roomStats[i];
            if (roomStat.Type == command.PanelDetails.PanelType && roomStat.Count > 0)
              roomStat.Count--;
            roomStats[i] = roomStat;
          }
        }).WithoutBurst().Run();
    }
    EntityCommandBufferSystem _ECBSys;
    Entity _eventsHolderEntity;
  }
}
