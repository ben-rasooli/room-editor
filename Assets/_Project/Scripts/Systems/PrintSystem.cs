using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Project
{
  public class PrintSystem : SystemBase
  {
    [DllImport("__Internal")]
    static extern void PrintRoomData(string roomData);

    protected override void OnCreate()
    {
      _eventsHolderEntity = GetSingletonEntity<EventsHolder_tag>();
    }

    protected override void OnUpdate()
    {
      if (!HasComponent<PrintRoomData_command>(_eventsHolderEntity))
        return;

      string roomData = default;
      roomData += "Panel Type\tCount";
      roomData += "\n-------------------------";
      var roomStats = EntityManager.GetBuffer<RoomStat>(GetSingletonEntity<RoomStat>());
      foreach (var stat in roomStats)
        if (stat.Count > 0)
          roomData += newLine + stat.Type.ToString(0) + "\t" + stat.Count + newLine;
      PrintRoomData(roomData);
    }
    Entity _eventsHolderEntity;
    string newLine = "\n";
  }

  public struct PrintRoomData_command : IComponentData { }
}
