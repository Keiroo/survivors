using Unity.Entities;
using Unity.Mathematics;

namespace Survivors
{
    public struct BulletComponent : IComponentData
    {
        public float Speed;
        public float3 Direction;
        public float LifeTime;
        public float MaxLifeTime;
    }
}
