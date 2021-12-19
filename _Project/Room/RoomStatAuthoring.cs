using Unity.Entities;
using UnityEngine;

namespace Project
{
  [DisallowMultipleComponent]
  public class RoomStatAuthoring : MonoBehaviour, IConvertGameObjectToEntity
  {
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
      dstManager.AddBuffer<RoomStat>(entity);
    }
  }

  public struct RoomStat : IBufferElementData
  {
    public PanelType Type;
    public int Count;
    public double Price;
  }
}
