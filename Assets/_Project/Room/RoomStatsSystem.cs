using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Tiny.JSON;
#if UNITY_DOTSRUNTIME
using Unity.Tiny.GameSave;
#endif

namespace Project
{
  public class RoomStatsSystem : SystemBase
  {
    #region JavaScript communication
    internal class MonoPInvokeCallbackAttribute : Attribute
    {
      public MonoPInvokeCallbackAttribute(Type t) { }
    }

    delegate void PriceListCallbackDelegate(string data);

    [DllImport("__Internal")]
    static extern string GetPriceList(PriceListCallbackDelegate callback);

    [MonoPInvokeCallback(typeof(PriceListCallbackDelegate))]
    static void onDataReceived(string data)
    {
      _priceListJsonData = new TinyJsonInterface(data, Allocator.Persistent);
      _isPriceListAvailable = true;
    }
    #endregion
    protected override void OnStartRunning()
    {
      _eventsHolderEntity = GetSingletonEntity<EventsHolder_tag>();
      var panelDetailsList = EntityManager.GetBuffer<PanelDetails>(GetSingletonEntity<SceneSettings>());
      var roomStats = EntityManager.GetBuffer<RoomStat>(GetSingletonEntity<RoomStat>());
      foreach (var panel in panelDetailsList)
        roomStats.Add(new RoomStat { Type = panel.PanelType, Count = 0, Price = 0 });
      roomStats.Add(new RoomStat { Type = PanelType.Door, Count = 0, Price = 0 });
      GetPriceList(onDataReceived);
    }

    protected override void OnUpdate()
    {
      var roomStats = EntityManager.GetBuffer<RoomStat>(GetSingletonEntity<RoomStat>());
      
      if (_isPriceListAvailable)
      {
        _isPriceListAvailable = false;

        var priceList = _priceListJsonData.Object["price_list"].AsArray();
        foreach (var item in priceList)
          for (int i = 0; i < roomStats.Length; i++)
          {
            var roomStat = roomStats[i];
            if (item["name"].AsString() == roomStat.Type.ToString(true))
            {
              roomStat.Price = (double)item["price"].AsFloat();
              roomStats[i] = roomStat;
            }
          }
      }

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
        .ForEach((in AddDoor_command command) =>
        {
          for (int i = 0; i < roomStats.Length; i++)
          {
            var roomStat = roomStats[i];
            if (roomStat.Type == PanelType.Door)
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

      Entities
        .WithNone<EventsHolder_tag>()
        .ForEach((in RemoveDoor_command command) =>
        {
          for (int i = 0; i < roomStats.Length; i++)
          {
            var roomStat = roomStats[i];
            if (roomStat.Type == PanelType.Door && roomStat.Count > 0)
              roomStat.Count--;
            roomStats[i] = roomStat;
          }
        }).WithoutBurst().Run();
    }
    Entity _eventsHolderEntity;
    static bool _isPriceListAvailable = false;
    static TinyJsonInterface _priceListJsonData;
  }
}
