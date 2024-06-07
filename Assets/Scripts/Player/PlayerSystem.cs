using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Survivors
{
    public partial struct PlayerSystem : ISystem
    {
        private EntityManager entityManager;
        private Entity playerEntity;
        private Entity inputEntity;
        private PlayerComponent playerComponent;
        private InputComponent inputComponent;
        private float NextShootTime;

        public void OnCreate(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            entityManager = state.EntityManager;
            playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();
            inputEntity = SystemAPI.GetSingletonEntity<InputComponent>();

            playerComponent = entityManager.GetComponentData<PlayerComponent>(playerEntity);
            inputComponent = entityManager.GetComponentData<InputComponent>(inputEntity);
            Move(ref state);
            Shoot(ref state);
        }

        private void Move(ref SystemState state)
        {
            var playerTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);
            playerTransform.Position += new float3(inputComponent.Movement * playerComponent.MoveSpeed * SystemAPI.Time.DeltaTime, 0);
            entityManager.SetComponentData(playerEntity, playerTransform);
        }

        private void Shoot(ref SystemState state)
        {
            if (NextShootTime < SystemAPI.Time.ElapsedTime)
            {
                var buffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
                var bulletEntity = entityManager.Instantiate(playerComponent.BulletPrefab);
                var bulletTransform = entityManager.GetComponentData<LocalTransform>(bulletEntity);
                var playerTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);
                
                bulletTransform.Position = playerTransform.Position;
                buffer.AddComponent(bulletEntity, new BulletComponent { Speed = 2f, Direction = float3.zero, LifeTime = 0f, MaxLifeTime = 5f });
                buffer.SetComponent(bulletEntity, bulletTransform);
                buffer.Playback(entityManager);

                NextShootTime = (float)SystemAPI.Time.ElapsedTime + playerComponent.ShootCooldown;
            }
        }
    }
}
