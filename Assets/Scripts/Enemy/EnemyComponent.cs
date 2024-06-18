using Unity.Entities;

namespace Survivors
{
    public struct EnemyComponent : IComponentData
    {
        public SurvivorsTag Tag;
        public float CurrentHealth;
        public float MoveSpeed;
        public float Damage;
    }
}
