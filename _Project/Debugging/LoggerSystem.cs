#region usings
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
#endregion

namespace Project
{
  public class ECBSystem : SystemBase
  {
    protected override void OnStartRunning()
    {
      var markerEntity = GetSingleton<SceneSettings>().GridNodePrefab;
      Entities
        .WithStructuralChanges()
        .WithAll<GridNode_tag>()
        .ForEach((Entity entity, in Translation translation) =>
        {
          // var cloneMarkerEntity = EntityManager.Instantiate(markerEntity);
          // EntityManager.SetComponentData(cloneMarkerEntity, translation);
        }).WithoutBurst().Run();
    }

    protected override void OnUpdate()
    {
      var ECB = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();
      var sceneSettings = GetSingletonEntity<SceneSettings>();

      if (Input.GetKeyDown(KeyCode.Alpha1))
        ECB.AddSingleFrameComponent(sceneSettings, new SwitchPanel_command { Value = PanelType._280 });
      else if (Input.GetKeyDown(KeyCode.Alpha2))
        ECB.AddSingleFrameComponent(sceneSettings, new SwitchPanel_command { Value = PanelType._300 });
      else if (Input.GetKeyDown(KeyCode.Alpha3))
        ECB.AddSingleFrameComponent(sceneSettings, new SwitchPanel_command { Value = PanelType._600 });
      Entities
        .WithChangeFilter<RoomStat>()
        .ForEach((in DynamicBuffer<RoomStat> roomStatsList) =>
        {
          double total = 0;
          foreach (var stat in roomStatsList)
          {
            Debug.Log($"{stat.Type}: {stat.Count} - ${stat.Count * stat.Price}");
            total += stat.Count * stat.Price;
          }
          Debug.Log($"Total: ${total}");
          Debug.Log("----------------");
        }).WithoutBurst().Run();
    }
  }
}