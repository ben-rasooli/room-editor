/*using Unity.Entities;
using System.Runtime.InteropServices;
using System;
using Unity.Tiny;
using Unity.Tiny.JSON;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;

public class EntityGenerator : SystemBase
{
    #region JavaScript communication
    internal class MonoPInvokeCallbackAttribute : Attribute
    {
        public MonoPInvokeCallbackAttribute() { }
        public MonoPInvokeCallbackAttribute(Type t) { }
    }

    delegate void RoomDataCallbackDelegate(string data);

    [DllImport("__Internal")]
    static extern string GetRoomData(RoomDataCallbackDelegate callback);

    [MonoPInvokeCallback(typeof(RoomDataCallbackDelegate))]
    static void onDataReceived(string data)
    {
        TinyJsonInterface jsonData = new TinyJsonInterface(
            data,
            Allocator.Persistent);

        _panelsData = jsonData.Object["panels"].AsArray();
        //_doorsData = jsonData.Object["doors"].AsArray();
        _isRoomDataAvailable = true;
        //jsonData.Dispose();
    }
    #endregion
    protected override void OnStartRunning()
    {

       // onDataReceived(sampleData);

        //GetRoomData(onDataReceived);

    }

    protected override void OnUpdate()
    {
        var ecb = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();

        if (_isRoomDataAvailable)
        {
            _isRoomDataAvailable = false;

            //Entities.WithoutBurst().ForEach((Entity entity, in Panel_tag panel_Tag) => {
            //    ecb.DestroyEntity(entity);
            //}).Run();

            //Entities.WithoutBurst().ForEach((in PanelPrefab panelPrefab) =>
            //{
            //    foreach (var panel in _panelsData)
            //    {
            //        if (panelPrefab.Size.z == panel["thickness"].AsInt() && panelPrefab.Size.y == panel["height"].AsInt())
            //        {
            //            var panelEntity = ecb.Instantiate(panelPrefab.Prefab);
            //            var translation = new Translation();
            //            translation.Value.x = panel["position"]["x"].AsFloat();
            //            translation.Value.y = panel["position"]["y"].AsFloat();
            //            translation.Value.z = panel["position"]["z"].AsFloat();
            //            ecb.SetComponent(panelEntity, translation);
            //            var rotation = new Rotation();
            //            rotation.Value = quaternion.EulerXYZ(
            //                math.radians(
            //                new float3(panel["rotation"]["x"].AsFloat(), panel["rotation"]["y"].AsFloat(), panel["rotation"]["z"].AsFloat())));
            //            ecb.SetComponent(panelEntity, rotation);
            //        }
            //    }
            //}).Run();

            /*Entities.WithoutBurst().ForEach((in DoorPrefab doorPrefab) =>
            {
                foreach (var door in _doorsData)
                {
                    if (doorPrefab.thickness == door["thickness"].AsInt())
                    {
                        var doorEntity = ecb.Instantiate(doorPrefab.Prefab);
                        var translation = new Translation();
                        translation.Value.x = door["position"]["x"].AsFloat();
                        translation.Value.y = door["position"]["y"].AsFloat();
                        translation.Value.z = door["position"]["z"].AsFloat();
                        ecb.SetComponent(doorEntity, translation);
                        var rotation = new Rotation();
                        rotation.Value = quaternion.EulerXYZ(
                            math.radians(
                            new float3(door["rotation"]["x"].AsFloat(), door["rotation"]["y"].AsFloat(), door["rotation"]["z"].AsFloat())));
                        ecb.SetComponent(doorEntity, rotation);
                    }
                }
            }).Run();
        }
    }
    static bool _isRoomDataAvailable = false;
    static TinyJsonArray _panelsData;
    static TinyJsonArray _doorsData;

    string sampleData = "{\"name\":\"sample name\",\"doors\":[{\"position\":{\"x\":0,\"y\":0,\"z\":0},\"rotation\":{\"x\":0,\"y\":90,\"z\":0},\"thickness\":5}],\"panels\":[{\"height\":260,\"position\":{\"x\":0,\"y\":0,\"z\":0},\"rotation\":{\"x\":0,\"y\":90,\"z\":0},\"thickness\":5},{\"height\":260,\"position\":{\"x\":0,\"y\":0,\"z\":-1.15},\"rotation\":{\"x\":0,\"y\":90,\"z\":0},\"thickness\":5},{\"height\":260,\"position\":{\"x\":0,\"y\":0,\"z\":-2.3},\"rotation\":{\"x\":0,\"y\":90,\"z\":0},\"thickness\":5},{\"height\":260,\"position\":{\"x\":3.5,\"y\":0,\"z\":0},\"rotation\":{\"x\":0,\"y\":90,\"z\":0},\"thickness\":5},{\"height\":260,\"position\":{\"x\":3.5,\"y\":0,\"z\":-1.15},\"rotation\":{\"x\":0,\"y\":90,\"z\":0},\"thickness\":5},{\"height\":260,\"position\":{\"x\":2.35,\"y\":0,\"z\":-2.3},\"rotation\":{\"x\":0,\"y\":90,\"z\":0},\"thickness\":5},{\"height\":260,\"position\":{\"x\":0,\"y\":0,\"z\":0},\"rotation\":{\"x\":0,\"y\":0,\"z\":0},\"thickness\":5},{\"height\":260,\"position\":{\"x\":1.15,\"y\":0,\"z\":0},\"rotation\":{\"x\":0,\"y\":0,\"z\":0},\"thickness\":5},{\"height\":260,\"position\":{\"x\":2.3,\"y\":0,\"z\":0},\"rotation\":{\"x\":0,\"y\":0,\"z\":0},\"thickness\":5},{\"height\":260,\"position\":{\"x\":0,\"y\":0,\"z\":-3.4},\"rotation\":{\"x\":0,\"y\":0,\"z\":0},\"thickness\":5},{\"height\":260,\"position\":{\"x\":1.15,\"y\":0,\"z\":-3.4},\"rotation\":{\"x\":0,\"y\":0,\"z\":0},\"thickness\":5},{\"height\":260,\"position\":{\"x\":2.3,\"y\":0,\"z\":-2.25},\"rotation\":{\"x\":0,\"y\":0,\"z\":0},\"thickness\":5}]}";
}
*/