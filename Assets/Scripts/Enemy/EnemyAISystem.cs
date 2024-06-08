using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

namespace Survivors
{
    [BurstCompile]
    public partial struct EnemyAISystem : ISystem
    {
        [BurstCompile]
        private void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            var playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();

            var entitiesQuery = new EntityQueryBuilder(Allocator.TempJob)
                .WithAll<EnemyComponent>()
                .WithAll<LocalTransform>()
                .Build(ref state);

            var entitiesArray = entitiesQuery.ToEntityArray(Allocator.TempJob);

            var buffers = new NativeArray<EntityCommandBuffer>(entitiesArray.Length, Allocator.TempJob);
            for (int i = 0; i < entitiesArray.Length; i++)
                buffers[i] = new EntityCommandBuffer(Allocator.TempJob);

            var handles = new NativeArray<JobHandle>(entitiesArray.Length, Allocator.TempJob);
            var enemyComponents = entitiesQuery.ToComponentDataArray<EnemyComponent>(Allocator.TempJob);
            var transforms = entitiesQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);

            var playerPosition = entityManager.GetComponentData<LocalTransform>(playerEntity).Position;
            var deltaTime = SystemAPI.Time.DeltaTime;

            for (int i = 0; i < entitiesArray.Length; i++)
            {
                var moveJob = new MoveJob
                {
                    Buffer = buffers[i],
                    EnemyEntity = entitiesArray[i],
                    PlayerPosition = playerPosition,
                    EnemyComponent = enemyComponents[i],
                    TransformComponent = transforms[i],
                    DeltaTime = deltaTime
                };
                handles[i] = moveJob.Schedule();
            }
            JobHandle.CompleteAll(handles);

            for (int i = 0; i < entitiesArray.Length; i++)
            {
                buffers[i].Playback(state.EntityManager);
                buffers[i].Dispose();
            }
            buffers.Dispose();
            entitiesArray.Dispose();
            handles.Dispose();
            enemyComponents.Dispose();
            transforms.Dispose();
        }

        [BurstCompile]
        private struct MoveJob : IJob
        {
            public EntityCommandBuffer Buffer;
            public Entity EnemyEntity;
            public float3 PlayerPosition;
            public EnemyComponent EnemyComponent;
            public LocalTransform TransformComponent;
            public float DeltaTime;

            public void Execute()
            {
                var direction = PlayerPosition - TransformComponent.Position;
                TransformComponent.Position += EnemyComponent.MoveSpeed * DeltaTime * math.normalizesafe(direction);
                Buffer.SetComponent(EnemyEntity, TransformComponent);
            }
        }
    }
    
}