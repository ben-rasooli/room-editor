using Unity.Entities;
using System.Runtime.InteropServices;
using System;
using Unity.Tiny.JSON;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

namespace Project
{
  public class RoomSaveLoad : SystemBase
  {
    #region JavaScript communication
    internal class MonoPInvokeCallbackAttribute : Attribute
    {
      public MonoPInvokeCallbackAttribute() { }
      public MonoPInvokeCallbackAttribute(Type t) { }
    }

    delegate void RoomDataCallbackDelegate(string data);

    [DllImport("__Internal")]
    static extern string LoadRoomData(RoomDataCallbackDelegate callback);

    [MonoPInvokeCallback(typeof(RoomDataCallbackDelegate))]
    static void onDataReceived(string data)
    {
      _roomJsonData = new TinyJsonInterface(data, Allocator.Persistent);
      _isRoomDataAvailable = true;
    }

    [DllImport("__Internal")]
    static extern void SaveRoomData(string roomData);
    #endregion

    protected override void OnCreate()
    {
      _ECBSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
      _eventsHolderEntity = GetSingletonEntity<EventsHolder_tag>();
    }

    protected override void OnUpdate()
    {
      var ECB = _ECBSys.CreateCommandBuffer();

      if (HasComponent<LoadRoomData_command>(_eventsHolderEntity))
        LoadRoomData(onDataReceived);

      if (HasComponent<SaveRoomData_command>(_eventsHolderEntity))
      {
        var roomJsonData = new TinyJsonStreamingWriter(Allocator.Temp);
        roomJsonData.PushArrayField("panels");
        Entities.
          WithAll<Panel_tag>()
          .ForEach((in PanelData panelData, in Translation translation, in Rotation rotation) =>
          {
            roomJsonData.PushObjectToArray();
            roomJsonData.PushValueField("height", panelData.Height);
            roomJsonData.PushValueField("thickness", panelData.Thickness);
            roomJsonData.PushObjectField("position");
            roomJsonData.PushValueField("x", translation.Value.x);
            roomJsonData.PushValueField("y", translation.Value.y);
            roomJsonData.PushValueField("z", translation.Value.z);
            roomJsonData.PopObject();
            roomJsonData.PushObjectField("rotation");
            roomJsonData.PushValueField("x", rotation.Value.value.x);
            roomJsonData.PushValueField("y", rotation.Value.value.y);
            roomJsonData.PushValueField("z", rotation.Value.value.z);
            roomJsonData.PushValueField("w", rotation.Value.value.w);
            roomJsonData.PopObject();
            roomJsonData.PopObject();
          }).WithoutBurst().Run();
        roomJsonData.PopArray();
        SaveRoomData(roomJsonData.WriteToString().ToString());
        roomJsonData.Dispose();
      }

      if (_isRoomDataAvailable)
      {
        _isRoomDataAvailable = false;

        DynamicBuffer<PanelDetails> panelDetailsBuffer =
          GetBuffer<PanelDetails>(
            GetSingletonEntity<SceneSettings>()
          );

        //var doorsData = _roomJsonData.Object["doors"].AsArray();
        var panelsData = _roomJsonData.Object["panels"].AsArray();
        foreach (var panel in panelsData)
        {
          var position = new float3(panel["position"]["x"].AsFloat(), panel["position"]["y"].AsFloat(), panel["position"]["z"].AsFloat());
          var rotation = new float4(panel["rotation"]["x"].AsFloat(), panel["rotation"]["y"].AsFloat(), panel["rotation"]["z"].AsFloat(), panel["rotation"]["w"].AsFloat());
          PanelDetails panelDetails = default;
          foreach (var details in panelDetailsBuffer)
            if (details.Thickness == panel["thickness"].AsInt() && details.Height == panel["height"].AsInt())
              panelDetails = details;

          var panelAddDetails = new PanelAddDetails
          {
            Position = position,
            Rotation = rotation,
            PanelDetails = panelDetails
          };
          ECB.CreateSingleFrameComponent(panelAddDetails);
        }
        _roomJsonData.Dispose();
      }
    }
    EntityCommandBufferSystem _ECBSys;
    Entity _eventsHolderEntity;
    static bool _isRoomDataAvailable = false;
    static TinyJsonInterface _roomJsonData;
  }

  public struct LoadRoomData_command : IComponentData { }
  public struct SaveRoomData_command : IComponentData { }
}