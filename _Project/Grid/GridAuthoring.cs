using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Project
{
  [DisallowMultipleComponent]
  public class GridAuthoring : MonoBehaviour, IConvertGameObjectToEntity
  {
    [Header("(choose an odd number)")] [SerializeField] int _gridSize;
    [SerializeField] float _panelSize;
    [SerializeField] GameObject _gridNodePrefab;
    public void Convert(Entity e, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
      int extend = (int)((_gridSize - 1) / 2);
      for (int y = -extend; y <= extend; y++)
        for (int x = -extend; x <= extend; x++)
        {
          var position = new float3(x * (_panelSize * 0.5f), 0, y * (_panelSize * 0.5f));

          Entity gridNode_entity = conversionSystem.CreateAdditionalEntity(gameObject);
          dstManager.SetName(gridNode_entity, $"Grid Node [{x}, {y}]");
          dstManager.AddComponentData(gridNode_entity, new Translation { Value = position });
          dstManager.AddComponent<GridNode_tag>(gridNode_entity);
          dstManager.AddBuffer<SingleFrameComponent>(gridNode_entity);
        }
    }
  }
}
