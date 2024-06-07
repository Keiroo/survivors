using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Survivors
{
    public partial struct EnemyAISystem : ISystem
    {
        private EntityManager entityManager;
        private Entity playerEntity;

        private void OnUpdate(ref SystemState state)
        {
            entityManager = state.EntityManager;
            playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();

            foreach (var (enemyComponent, transformComponent) in SystemAPI.Query<EnemyComponent, RefRW<LocalTransform>>())
            {
                var direction = entityManager.GetComponentData<LocalTransform>(playerEntity).Position - transformComponent.ValueRO.Position;
                transformComponent.ValueRW.Position += enemyComponent.MoveSpeed * SystemAPI.Time.DeltaTime * math.normalize(direction);
            }
        }
    }
    
}