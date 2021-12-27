using Unity.Entities;
using Unity.Tiny.Text;
using Unity.Tiny.UI;

namespace Project
{
  public class UIManagerSystem : SystemBase
  {
    protected override void OnStartRunning()
    {
      _ECBSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
      _eventsHolderEntity = GetSingletonEntity<EventsHolder_tag>();
      ProcessUIEvents processUIEvents = World.GetExistingSystem<ProcessUIEvents>();
      _loadButton = processUIEvents.GetEntityByUIName("Load - Button");
      _saveButton = processUIEvents.GetEntityByUIName("Save - Button");
      _printButton = processUIEvents.GetEntityByUIName("Print - Button");
      _helpButton = processUIEvents.GetEntityByUIName("Help - Button");
      _messagePanel = processUIEvents.GetEntityByUIName("Message");
      _costText = processUIEvents.GetEntityByUIName("Cost - Text");

      setCostTextVisibility(false);
    }

    protected override void OnUpdate()
    {
      var ECB = _ECBSys.CreateCommandBuffer();
      var loadButtonState = GetComponent<UIState>(_loadButton);
      var saveButtonState = GetComponent<UIState>(_saveButton);
      var printButtonState = GetComponent<UIState>(_printButton);
      var helpButtonState = GetComponent<UIState>(_helpButton);
      var messageRectTransform = GetComponent<RectTransform>(_messagePanel);

      if (loadButtonState.IsClicked)
        ECB.AddSingleFrameComponent<LoadRoomData_command>(_eventsHolderEntity);

      if (saveButtonState.IsClicked)
        ECB.AddSingleFrameComponent<SaveRoomData_command>(_eventsHolderEntity);

      if (printButtonState.IsClicked)
        ECB.AddSingleFrameComponent<PrintRoomData_command>(_eventsHolderEntity);

      if (helpButtonState.IsPressed)
      {
        messageRectTransform.Hidden = false;
        SetComponent(_messagePanel, messageRectTransform);
      }
      else
      {
        messageRectTransform.Hidden = true;
        SetComponent(_messagePanel, messageRectTransform);
      }

      Entities
        .WithChangeFilter<RoomStat>()
        .ForEach((in DynamicBuffer<RoomStat> roomStatsList) =>
        {
          int cost = 0;
          foreach (var stat in roomStatsList)
            cost += (int)(stat.Count * stat.Price);
          if (cost > 0)
          {
            setCostTextVisibility(true);
            TextLayout.SetEntityTextRendererString(ECB, _costText,  costPrefix + cost.ToString());
          }
          else
          {
            setCostTextVisibility(false);
          }
        }).WithoutBurst().Run();
    }
    const string costPrefix = "Total: $";

    void setCostTextVisibility(bool visible)
    {
      if (_isCostTextVisible == visible) return;
      var component = EntityManager.GetComponentData<RectTransform>(_costText);
      component.Hidden = !visible;
      _isCostTextVisible = visible;
      EntityManager.SetComponentData(_costText, component);
    }

    EntityCommandBufferSystem _ECBSys;
    Entity _eventsHolderEntity;
    Entity _loadButton;
    Entity _saveButton;
    Entity _printButton;
    Entity _helpButton;
    Entity _messagePanel;
    Entity _costText;
    bool _isCostTextVisible = true;
  }
}
