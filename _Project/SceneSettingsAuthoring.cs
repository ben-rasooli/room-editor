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
    [Tooltip("the order should be the same order as PanelType enum")]
    [SerializeField] List<PanelDetailsAuthoring> _panelDetails;
    [SerializeField] GameObject _gridNodePrefab;
    [SerializeField] Color _panelGhostColor_additionMode;
    [SerializeField] Color _panelGhostColor_subtractionMode;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
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
        Rotation = new Rotation { Value = new quaternion(0f, 0f, 0f, 1f) },
        PanelDetails = panelPrefabsBuffer[0],
        GridNodePrefab = conversionSystem.GetPrimaryEntity(_gridNodePrefab),
        PanelGhostColor_additionMode = _panelGhostColor_additionMode.ToFloat4(),
        PanelGhostColor_subtractionMode = _panelGhostColor_subtractionMode.ToFloat4()
      };
      dstManager.AddComponentData(entity, settings);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
      foreach (var detail in _panelDetails)
        referencedPrefabs.Add(detail.Prefab);
      referencedPrefabs.Add(_gridNodePrefab);
    }
  }

  [Serializable]
  public struct SceneSettings : IComponentData
  {
    public Rotation Rotation;
    public PanelDetails PanelDetails;
    public Entity GridNodePrefab;
    public float4 PanelGhostColor_additionMode;
    public float4 PanelGhostColor_subtractionMode;
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
