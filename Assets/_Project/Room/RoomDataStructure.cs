using System;
using System.Collections.Generic;
using Unity.Mathematics;

[Serializable]
public struct RoomDataStructure
{
  public List<panel> panels;
  public List<door> doors;
  public string name;

  [Serializable]
  public struct panel
  {
    public int height;
    public int thickness;
    public float3 position;
    public float4 rotation;
  }
  [Serializable]
  public struct door
  {
    public float3 position;
    public float4 rotation;
  }
}