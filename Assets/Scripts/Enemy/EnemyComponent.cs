using Unity.Entities;

namespace Survivors
{
    public class EnemyComponent : IComponentData
    {
        public int PrefabID;
        public float CurrentHealth;
        public float MoveSpeed;
        public float Damage;
    }
}
