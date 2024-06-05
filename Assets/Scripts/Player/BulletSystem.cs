using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;

namespace Survivors
{
    public enum CollisionLayer
    {
        Default = 1 << 0,
        Enemy = 1 << 6
    }

    partial struct BulletSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {        
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            NativeArray<Entity> allEntities = entityManager.GetAllEntities();
            var playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();
            var playerTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

            foreach (var bullet in allEntities)
            {
                if (entityManager.HasComponent<BulletComponent>(bullet))
                {
                    var bulletTransform = entityManager.GetComponentData<LocalTransform>(bullet);
                    var bulletComponent = entityManager.GetComponentData<BulletComponent>(bullet);

                    if (bulletComponent.Target == Entity.Null)
                    {
                        foreach (var enemy in allEntities)
                        {
                            if (entityManager.HasComponent<EnemyComponent>(enemy))
                            {
                                if (bulletComponent.Target == Entity.Null)
                                    bulletComponent.Target = enemy;
                                else
                                {
                                    var orgTargetTransform = entityManager.GetComponentData<LocalTransform>(bulletComponent.Target);
                                    var newTargetTransform = entityManager.GetComponentData<LocalTransform>(enemy);

                                    if (math.distance(playerTransform.Position, newTargetTransform.Position) < math.distance(playerTransform.Position, orgTargetTransform.Position))
                                        bulletComponent.Target = enemy;
                                }
                            }
                        }
                    }
                    var buffer = new EntityCommandBuffer(Allocator.Temp);
                    var targetTransform = entityManager.GetComponentData<LocalTransform>(bulletComponent.Target);
                    var targetVector = math.normalize(targetTransform.Position - bulletTransform.Position);
                    bulletTransform.Position += bulletComponent.Speed * SystemAPI.Time.DeltaTime * targetVector;

                    var hits = new NativeList<ColliderCastHit>(Allocator.Temp);
                    physicsWorld.SphereCastAll(bulletTransform.Position, 0.1f, float3.zero, 1f, ref hits, new CollisionFilter { BelongsTo = (uint)CollisionLayer.Default, CollidesWith = (uint)CollisionLayer.Enemy });

                    foreach (var hit in hits)
                        entityManager.SetEnabled(hit.Entity, false);
                    // {
                    //     entityManager.DestroyEntity(hit.Entity);
                    //     entityManager.DestroyEntity(bullet);
                    //     buffer.Dispose();
                    //     return;
                    // }

                    buffer.SetComponent(bullet, bulletComponent);
                    buffer.SetComponent(bullet, bulletTransform);
                    buffer.Playback(entityManager);
                    buffer.Dispose();
                }
            }
            
        }

        public void OnDestroy(ref SystemState state)
        {        
        }
    }
}
