#region usings
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
#endregion

namespace Project
{
  public class RoomModificationModeSystem : ComponentSystem
  {
    protected override void OnCreate()
    {
      _ECBSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnStartRunning()
    {
      _sceneSettings = GetSingleton<SceneSettings>();

      Entities
        .WithAll<PanelGhost_tag>()
        .ForEach((Entity entity) =>
        {
          setPanelGhostColor(entity);
        });
    }

    protected override void OnUpdate()
    {
      var ECB = _ECBSys.CreateCommandBuffer();

      if (!TryGetSingletonEntity<ChangeModificationMode_command>(out Entity _))
        return;

      var modMode = GetSingleton<ModificationModeData>();
      modMode.Value = (modMode.Value == ModificationMode.Addition) ? ModificationMode.Subtraction : ModificationMode.Addition;
      SetSingleton<ModificationModeData>(modMode);

      Entities
        .WithAll<PanelGhost_tag>()
        .ForEach((Entity entity) =>
        {
          setPanelGhostColor(entity);
        });
    }
    EntityCommandBufferSystem _ECBSys;
    SceneSettings _sceneSettings;

    void setPanelGhostColor(Entity entity)
    {
      var modMode = GetSingleton<ModificationModeData>();
      float4 panelGhostColor = default;

      if (modMode.Value == ModificationMode.Addition)
        panelGhostColor = _sceneSettings.PanelGhostColor_additionMode;
      else
        panelGhostColor = _sceneSettings.PanelGhostColor_subtractionMode;

      EntityManager.SetComponentData(entity, new URPMaterialPropertyBaseColor { Value = panelGhostColor });
    }
  }
}