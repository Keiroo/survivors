using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using UnityEngine;

namespace Survivors
{
    public enum CollisionLayer
    {
        Default = 1 << 0,
        Enemy = 1 << 6
    }

    partial struct BulletSystem : ISystem
    {

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            NativeArray<Entity> allEntities = entityManager.GetAllEntities();
            var playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

            foreach (var bullet in allEntities)
            {
                if (entityManager.HasComponent<BulletComponent>(bullet))
                {
                    var bulletTransform = entityManager.GetComponentData<LocalTransform>(bullet);
                    var bulletComponent = entityManager.GetComponentData<BulletComponent>(bullet);
                    var buffer = new EntityCommandBuffer(Allocator.Temp);

                    // Find target to shoot at
                    if (bulletComponent.Direction.Equals(float3.zero))
                    {
                        var direction = float3.zero;
                        Entity currentTarget = Entity.Null;
                        foreach (var enemy in allEntities)
                        {
                            if (entityManager.HasComponent<EnemyComponent>(enemy))
                            {
                                if (currentTarget == Entity.Null)
                                {
                                    currentTarget = enemy;
                                    var transform = entityManager.GetComponentData<LocalTransform>(enemy);
                                    direction = math.normalize(transform.Position - bulletTransform.Position);
                                }
                                else
                                {
                                    var orgTargetTransform = entityManager.GetComponentData<LocalTransform>(currentTarget);
                                    var newTargetTransform = entityManager.GetComponentData<LocalTransform>(enemy);

                                    if (math.distance(bulletTransform.Position, newTargetTransform.Position) < math.distance(bulletTransform.Position, orgTargetTransform.Position))
                                    {
                                        currentTarget = enemy;
                                        direction = math.normalize(newTargetTransform.Position - bulletTransform.Position);
                                    }
                                }
                            }
                        }
                        bulletComponent.Direction = direction;
                    }

                    bulletComponent.LifeTime += Time.unscaledDeltaTime;

                    // If target not found or timeout, destroy itself
                    if (bulletComponent.Direction.Equals(float3.zero) || bulletComponent.LifeTime > bulletComponent.MaxLifeTime)
                    {
                        entityManager.DestroyEntity(bullet);
                        buffer.PlaybackAndDispose(entityManager);
                        return;
                    }
                    
                    bulletTransform.Position += bulletComponent.Speed * SystemAPI.Time.DeltaTime * bulletComponent.Direction;

                    var hits = new NativeList<ColliderCastHit>(Allocator.Temp);
                    physicsWorld.SphereCastAll(bulletTransform.Position, 0.1f, float3.zero, 1f, ref hits, new CollisionFilter { BelongsTo = (uint)CollisionLayer.Default, CollidesWith = (uint)CollisionLayer.Enemy });
                    foreach (var hit in hits)
                    {
                        if (entityManager.HasComponent<VisualsReferenceComponent>(hit.Entity))
                        {
                            var enemyVisual = entityManager.GetComponentData<VisualsReferenceComponent>(hit.Entity);
                            GameObject.Destroy(enemyVisual.GameObject);
                        }                        
                        entityManager.DestroyEntity(hit.Entity);
                        entityManager.DestroyEntity(bullet);
                        buffer.PlaybackAndDispose(entityManager);
                        return;
                    }

                    buffer.SetComponent(bullet, bulletComponent);
                    buffer.SetComponent(bullet, bulletTransform);
                    buffer.PlaybackAndDispose(entityManager);
                }
            }            
        }    
    }
}
