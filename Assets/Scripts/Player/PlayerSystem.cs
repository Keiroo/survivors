using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Survivors
{
    public partial struct PlayerSystem : ISystem
    {
        private EntityManager entityManager;
        private Entity playerEntity;
        private Entity inputEntity;
        private PlayerComponent playerComponent;
        private InputComponent inputComponent;

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
        }

        private void Move(ref SystemState state)
        {
            var playerTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);
            playerTransform.Position += new Unity.Mathematics.float3(inputComponent.Movement * playerComponent.MoveSpeed * SystemAPI.Time.DeltaTime, 0);
            entityManager.SetComponentData(playerEntity, playerTransform);
        }
    }
}
