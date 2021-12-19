using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Tiny.Input;
using Unity.Tiny.Rendering;
using Unity.Transforms;

namespace Project
{
    public class CameraZoomSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;
            var inputSystem = World.GetExistingSystem<InputSystem>();
            var sceneSettings = GetSingleton<SceneSettings>();

            Entities.WithoutBurst().WithAll<Camera>().ForEach((ref Translation translation, in Rotation rotation) =>
            {
                float zoomAmount;

                if (inputSystem.GetInputScrollDelta().y < 0)
                    zoomAmount = sceneSettings.CameraZoomSpeed * 2;
                else if (inputSystem.GetInputScrollDelta().y > 0)
                    zoomAmount = -sceneSettings.CameraZoomSpeed * 2;
                else
                    zoomAmount = 0;

                translation.Value += math.forward(rotation.Value) * zoomAmount * deltaTime;
            }).Run();
        }
    } 
}
