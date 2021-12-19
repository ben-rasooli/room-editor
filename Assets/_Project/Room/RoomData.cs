using System.Collections.Generic;

namespace Project
{
  public class RoomData
  {
    public static RoomDataStructure Value
    {
      get
      {
        if (_this == null)
        {
          _this = new RoomData();
          _this._value = new RoomDataStructure
          {
            panels = new List<RoomDataStructure.panel>(),
            doors = new List<RoomDataStructure.door>(),
            name = "sample name"
          };
        }

        return _this._value;
      }
    }
    static RoomData _this;
    RoomDataStructure _value;

  }
}
