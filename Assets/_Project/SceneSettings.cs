using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Project
{
  [Serializable]
  public struct SceneSettings : IComponentData
  {
    public float CameraZoomSpeed;
    public float CameraRotationSpeed;
    public float CameraMoveSpeed;
    public float GridNodeSelectionRadius;
    public float3 AreaExtend;
    public Rotation Rotation;
    public PanelDetails PanelDetails;
    public Entity DoorPrefab;
    public Entity GridNodeMarkerPrefab;
  }

  [Serializable]
  public struct PanelDetails : IBufferElementData
  {
    public PanelType PanelType;
    public Entity Prefab;
    public int Height;
    public int Thickness;
    public double Price;
  }
}
