using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Tiny;
using Unity.Tiny.Input;
using Unity.Transforms;

namespace Project
{
  public class CameraRotationSystem : PointerSystemBase
  {
    protected override void OnInputMove(int pointerId, float2 inputPos, float2 inputDelta)
    {
      float deltaTime = Time.DeltaTime;
      var sceneSettings = GetSingleton<SceneSettings>();

      Entities.WithAll<CameraController_tag>().ForEach((ref Translation translation, ref Rotation rotation) =>
      {
        rotateCamera(
          ref rotation,
          amount: inputDelta * sceneSettings.CameraRotationSpeed * deltaTime);
      }).WithoutBurst().Run();
    }

    protected override void OnSecondaryInputMove(int pointerId, float2 inputPos, float2 inputDelta)
    {
      float deltaTime = Time.DeltaTime;
      var sceneSettings = GetSingleton<SceneSettings>();

      Entities.WithAll<CameraController_tag>().ForEach((ref Translation translation, ref Rotation rotation) =>
      {
        moveCamera(
          ref translation,
          in rotation,
          amount: new float3(inputDelta.x, 0, inputDelta.y) * -1f * sceneSettings.CameraMoveSpeed * deltaTime,
          areaExtend: sceneSettings.AreaExtend);
      }).WithoutBurst().Run();
    }

    static void rotateCamera(ref Rotation rotation, float2 amount)
    {
      var rotationValueY = quaternion.AxisAngle(math.up(), amount.x);
      rotation.Value = math.normalize(math.mul(rotation.Value, rotationValueY));
    }

    static void moveCamera(ref Translation translation, in Rotation rotation, float3 amount, float3 areaExtend)
    {
      amount = math.mul(rotation.Value, amount);
      translation.Value += amount;
      translation.Value = math.clamp(translation.Value, -areaExtend, areaExtend);
    }
  }
}
