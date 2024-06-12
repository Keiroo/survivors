using Unity.Entities;

namespace Survivors
{
    public struct EnemyComponent : IComponentData
    {
        public int PrefabID;
        public float CurrentHealth;
        public float MoveSpeed;
        public float Damage;
    }
}
