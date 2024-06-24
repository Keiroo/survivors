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
                MoveSpeed = authoring.MoveSpeed,
                ShootCooldown = authoring.ShootCooldown,
                BulletPrefab = GetEntity(authoring.BulletPrefab, TransformUsageFlags.None),
            });
        }
    }
}
