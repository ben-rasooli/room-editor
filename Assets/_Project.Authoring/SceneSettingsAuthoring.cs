using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Project
{
  [DisallowMultipleComponent]
  public class SceneSettingsAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
  {
    [SerializeField] float _cameraZoomSpeed;
    [SerializeField] float _cameraRotationSpeed;
    [SerializeField] float _cameraMoveSpeed;
    [SerializeField] float _gridNodeSelectionRadius;
    [SerializeField] float3 _areaExtend;
    [Tooltip("the order should be the same order as PanelType enum")]
    [SerializeField] List<PanelDetailsAuthoring> _panelDetails;
    [SerializeField] Unity.Tiny.Color _panelGhostColor_additionMode;
    [SerializeField] Unity.Tiny.Color _panelGhostColor_subtractionMode;
    [SerializeField] GameObject _gridNodeMarkerPrefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
      Unity.Tiny.Debug.Log("-----start-----");
      var panelPrefabsBuffer = dstManager.AddBuffer<PanelDetails>(entity);
      foreach (PanelDetailsAuthoring detail in _panelDetails)
      {
        panelPrefabsBuffer.Add(new PanelDetails
        {
          PanelType = detail.PanelType,
          Prefab = conversionSystem.GetPrimaryEntity(detail.Prefab),
          Height = detail.Height,
          Thickness = detail.Thickness,
          Price = detail.Price
        });
      }

      var settings = new SceneSettings
      {
        CameraZoomSpeed = _cameraZoomSpeed,
        CameraRotationSpeed = _cameraRotationSpeed,
        CameraMoveSpeed = _cameraMoveSpeed,
        GridNodeSelectionRadius = _gridNodeSelectionRadius,
        AreaExtend = _areaExtend,
        Rotation = new Rotation { Value = new quaternion(0f, 0f, 0f, 1f) },
        PanelDetails = panelPrefabsBuffer[0],
        PanelGhostColor_additionMode = _panelGhostColor_additionMode.ToFloat4(),
        PanelGhostColor_subtractionMode = _panelGhostColor_subtractionMode.ToFloat4(),
        GridNodeMarkerPrefab = conversionSystem.GetPrimaryEntity(_gridNodeMarkerPrefab)
      };
      dstManager.AddComponentData(entity, settings);
      Unity.Tiny.Debug.Log("-----end-----");
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
      foreach (var detail in _panelDetails)
        referencedPrefabs.Add(detail.Prefab);
      referencedPrefabs.Add(_gridNodeMarkerPrefab);
    }
  }

  [Serializable]
  public struct PanelDetailsAuthoring
  {
    public PanelType PanelType;
    public GameObject Prefab;
    public int Height;
    public int Thickness;
    public double Price;
  }

}
