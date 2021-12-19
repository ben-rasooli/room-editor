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
          if (modMode.Value == ModificationMode.Addition)
            _currentState = World.GetOrCreateSystem<PanelAdditionModeState>();
          else
            _currentState = World.GetOrCreateSystem<PanelSubtractionModeState>();
        });

      _currentState.Update();
    }
    ComponentSystemGroup _currentState;

    #region states
    [UpdateInGroup(typeof(InputPointerFSM))] public class PanelAdditionModeState : ComponentSystemGroup { }
    [UpdateInGroup(typeof(InputPointerFSM))] public class PanelSubtractionModeState : ComponentSystemGroup { }
    #endregion
  }
}
