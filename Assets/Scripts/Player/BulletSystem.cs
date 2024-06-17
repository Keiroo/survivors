using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using UnityEngine;
using Unity.Burst;
using Unity.Jobs;
using System;
using UnityEngine.SceneManagement;

namespace Survivors
{
    public enum CollisionLayer
    {
        Default = 1 << 0,
        Enemy = 1 << 6
    }
    
    [BurstCompile]
    public partial struct BulletSystem : ISystem
    {
        private EntityQuery bulletsQuery;
        private EntityQuery enemiesQuery;
        private Unity.Mathematics.Random random;

        public void OnCreate(ref SystemState state)
        {
            // random = new Unity.Mathematics.Random((uint)DateTimeOffset.Now.ToUnixTimeSeconds());
            // random = new Unity.Mathematics.Random((uint)DateTime.Now.Ticks);
            random = new Unity.Mathematics.Random((uint)GetHashCode());
            bulletsQuery = new EntityQueryBuilder(Allocator.Persistent)
                .WithAll<BulletComponent>()
                .WithAll<LocalTransform>()
                .Build(ref state);

            enemiesQuery = new EntityQueryBuilder(Allocator.Persistent)
                .WithAll<EnemyComponent>()
                .WithAll<LocalTransform>()
                .Build(ref state);
        }

        public void OnUpdate(ref SystemState state)
        {
            if (SceneManager.GetActiveScene().buildIndex != 1)
                return;
                
            // var playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

            var bulletArray = bulletsQuery.ToEntityArray(Allocator.TempJob);
            var bulletComponents = bulletsQuery.ToComponentDataArray<BulletComponent>(Allocator.TempJob);
            var bulletTransforms = bulletsQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
            
            var enemyArray = enemiesQuery.ToEntityArray(Allocator.TempJob);
            var enemyComponents = enemiesQuery.ToComponentDataArray<EnemyComponent>(Allocator.TempJob);
            var enemyTransforms = enemiesQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);

            var findTargetBuffers = new NativeArray<EntityCommandBuffer>(bulletArray.Length, Allocator.TempJob);            
            var moveBuffers = new NativeArray<EntityCommandBuffer>(bulletArray.Length, Allocator.TempJob);
            var findTargetHandles = new NativeList<JobHandle>(Allocator.TempJob);
            var moveHandles = new NativeList<JobHandle>(Allocator.TempJob);

            var destroyBuffer = new EntityCommandBuffer(Allocator.TempJob);

            for (int i = 0; i < bulletArray.Length; i++)
            {
                findTargetBuffers[i] = new EntityCommandBuffer(Allocator.TempJob);
                if (bulletComponents[i].Direction.Equals(float3.zero))
                {
                    var randomIndex = random.NextInt(0, enemyArray.Length);
                    var findTargetJob = new FindTargetJob
                    {
                        Buffer = findTargetBuffers[i],
                        BulletEntity = bulletArray[i],
                        BulletTransform = bulletTransforms[i],
                        BulletComponent = bulletComponents[i],
                        EnemyComponent = enemyComponents[randomIndex],
                        EnemyTransform = enemyTransforms[randomIndex]
                    };
                    // findTargetHandles.Add(findTargetJob.Schedule(findTargetHandles.Length > 0 ? findTargetHandles[i - 1] : default));
                    findTargetHandles.Add(findTargetJob.Schedule());
                }
            }
            var findTargetHandlesArray = findTargetHandles.ToArray(Allocator.TempJob);
            JobHandle.CompleteAll(findTargetHandlesArray);

            for (int i = 0; i < bulletArray.Length; i++)
            {
                moveBuffers[i] = new EntityCommandBuffer(Allocator.TempJob);
                // If target not found or timeout, destroy itself
                if (bulletComponents[i].LifeTime > bulletComponents[i].MaxLifeTime)
                {               
                    destroyBuffer.DestroyEntity(bulletArray[i]);
                }
                else
                {
                    if (!bulletComponents[i].Direction.Equals(float3.zero))
                    {
                        var hitConfirm = false;
                        var hits = new NativeList<ColliderCastHit>(Allocator.Temp);
                        physicsWorld.SphereCastAll(bulletTransforms[i].Position, 0.1f, float3.zero, 1f, ref hits, new CollisionFilter { BelongsTo = (uint)CollisionLayer.Default, CollidesWith = (uint)CollisionLayer.Enemy });
                        foreach (var hit in hits)
                        {
                            hitConfirm = true;
                            if (state.EntityManager.HasComponent<VisualsReferenceComponent>(hit.Entity))
                            {
                                var enemyVisual = state.EntityManager.GetComponentData<VisualsReferenceComponent>(hit.Entity);
                                GameObject.Destroy(enemyVisual.GameObject);
                            }
                            destroyBuffer.DestroyEntity(hit.Entity);
                            destroyBuffer.DestroyEntity(bulletArray[i]);
                            break;
                        }

                        if (!hitConfirm)
                        {
                            var moveJob = new MoveJob
                            {
                                Buffer = moveBuffers[i],
                                BulletEntity = bulletArray[i],
                                BulletComponent = bulletComponents[i],
                                BulletTransform = bulletTransforms[i],
                                DeltaTime = SystemAPI.Time.DeltaTime,
                            };
                            moveHandles.Add(moveJob.Schedule());
                        }
                    }
                }
            }
            var moveHandlesArray = moveHandles.ToArray(Allocator.TempJob);
            JobHandle.CompleteAll(moveHandlesArray);

            for (int i = 0; i < findTargetBuffers.Length; i++)
            {
                findTargetBuffers[i].Playback(state.EntityManager);
                findTargetBuffers[i].Dispose();
            }
            for (int i = 0; i < moveBuffers.Length; i++)
            {
                moveBuffers[i].Playback(state.EntityManager);
                moveBuffers[i].Dispose();
            }
            destroyBuffer.Playback(state.EntityManager);
            destroyBuffer.Dispose();
            findTargetBuffers.Dispose();
            moveBuffers.Dispose();
            bulletArray.Dispose();
            findTargetHandles.Dispose();
            bulletComponents.Dispose();
            bulletTransforms.Dispose();
            enemyArray.Dispose();
            moveHandles.Dispose();
            enemyComponents.Dispose();
            enemyTransforms.Dispose();
            findTargetHandlesArray.Dispose();
            moveHandlesArray.Dispose();
        }

        [BurstCompile]
        private struct FindTargetJob : IJob
        {
            public EntityCommandBuffer Buffer;
            public Entity BulletEntity;
            public LocalTransform BulletTransform;
            public BulletComponent BulletComponent;
            public EnemyComponent EnemyComponent;
            public LocalTransform EnemyTransform;

            public void Execute()
            {
                BulletComponent.Direction = math.normalize(EnemyTransform.Position - BulletTransform.Position);
                Buffer.SetComponent(BulletEntity, BulletComponent);
            }
        }

        [BurstCompile]
        private struct FindClosestTargetJob : IJob
        {
            public EntityCommandBuffer Buffer;
            public Entity BulletEntity;
            public LocalTransform BulletTransform;
            public BulletComponent BulletComponent;
            public NativeArray<Entity> EnemyArray;
            public NativeArray<EnemyComponent> EnemyComponents;
            public NativeArray<LocalTransform> EnemyTransforms;

            public void Execute()
            {
                var direction = float3.zero;
                Entity currentTarget = Entity.Null;
                var lastEnemyIndex = 0;

                for (int i = 0; i < EnemyComponents.Length; i++)
                {
                    if (currentTarget == Entity.Null)
                    {
                        currentTarget = EnemyArray[i];
                        var transform = EnemyTransforms[i];
                        lastEnemyIndex = i;
                        direction = math.normalize(transform.Position - BulletTransform.Position);
                    }
                    else
                    {
                        var orgTargetTransform = EnemyTransforms[lastEnemyIndex];
                        var newTargetTransform = EnemyTransforms[i];

                        if (math.distance(BulletTransform.Position, newTargetTransform.Position) < math.distance(BulletTransform.Position, orgTargetTransform.Position))
                        {
                            currentTarget = EnemyArray[i];
                            direction = math.normalize(newTargetTransform.Position - BulletTransform.Position);
                        }
                    }
                }
                BulletComponent.Direction = direction;
                Buffer.SetComponent(BulletEntity, BulletComponent);
            }
        }

        [BurstCompile]
        private struct MoveJob : IJob
        {
            public EntityCommandBuffer Buffer;
            public Entity BulletEntity;
            public BulletComponent BulletComponent;
            public LocalTransform BulletTransform;
            public float DeltaTime;

            public void Execute()
            {
                BulletComponent.LifeTime += DeltaTime;
                BulletTransform.Position += BulletComponent.Speed * DeltaTime * BulletComponent.Direction;
                Buffer.SetComponent(BulletEntity, BulletComponent);
                Buffer.SetComponent(BulletEntity, BulletTransform);
            }
        }
    }
}
