using Unity.Entities;

namespace Survivors
{
    public struct EnemySpawnerComponent : IComponentData
    {
        public float SpawnInterval;
    }
}
