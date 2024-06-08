using Unity.Entities;

namespace Survivors
{
    public struct PlayerComponent : IComponentData
    {
        public float MoveSpeed;
        public float ShootCooldown;
        public Entity BulletPrefab;
    }
}
