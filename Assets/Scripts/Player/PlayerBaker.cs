using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Survivors
{
    public class PlayerBaker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            var playerEntity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(playerEntity, new PlayerComponent
            {
                MoveSpeed = authoring.MoveSpeed
            });
            


            // AddComponent(playerEntity, new LocalTransform
            // {
            //     Position = float3.zero,
            //     Rotation = Quaternion.identity,
            //     Scale = 1
            // });
        }
    }
}
