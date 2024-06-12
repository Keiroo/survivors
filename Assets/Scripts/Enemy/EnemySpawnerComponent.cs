using Unity.Entities;
using Unity.Mathematics;

namespace Survivors
{
    public struct EnemySpawnerComponent : IComponentData
    {
        public float SpawnInterval;
        public float2 CameraSize;
        public float Margin;
    }
}
