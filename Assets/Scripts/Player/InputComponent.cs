using Unity.Entities;
using Unity.Mathematics;

namespace Survivors
{
    public struct InputComponent : IComponentData
    {
        public float2 Movement;
    }
}
