using Unity.Entities;


namespace Project
{
  [AlwaysUpdateSystem]
  public class InputPointerFSM : ComponentSystemGroup
  {
    protected override void OnStartRunning()
    {
      _currentState = World.GetOrCreateSystem<PanelAdditionModeState>();
    }
    protected override void OnUpdate()
    {
      Entities
        .ForEach((ref ModificationModeData modMode) =>
        {
          if (modMode.Value == ModificationMode.PanelAddition)
            _currentState = World.GetOrCreateSystem<PanelAdditionModeState>();
          else if (modMode.Value == ModificationMode.DoorAddition)
            _currentState = World.GetOrCreateSystem<DoorAdditionModeState>();
          else
            _currentState = World.GetOrCreateSystem<SubtractionModeState>();
        });

      _currentState.Update();
    }
    ComponentSystemGroup _currentState;

    #region states
    [UpdateInGroup(typeof(InputPointerFSM))] public class PanelAdditionModeState : ComponentSystemGroup { }
    [UpdateInGroup(typeof(InputPointerFSM))] public class DoorAdditionModeState : ComponentSystemGroup { }
    [UpdateInGroup(typeof(InputPointerFSM))] public class SubtractionModeState : ComponentSystemGroup { }
    #endregion
  }
}
