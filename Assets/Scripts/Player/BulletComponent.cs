using Unity.Entities;

namespace Survivors
{
    public struct BulletComponent : IComponentData
    {
        public float Speed;
        public Entity Target;
    }
}
