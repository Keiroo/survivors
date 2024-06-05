using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Survivors
{
    public partial struct PlayerAnimationSystem : ISystem
    {
        private EntityManager entityManager;

        private void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.ManagedAPI.TryGetSingleton(out AnimationPrefabs animationPrefabs))
                return;

            var buffer = new EntityCommandBuffer(Allocator.Temp);
            entityManager = state.EntityManager;

            foreach (var (transform, playerComponent, entity) in SystemAPI.Query<LocalTransform, PlayerComponent>().WithEntityAccess())
            {
                if (!entityManager.HasComponent<VisualsReferenceComponent>(entity))
                {
                    var playerVisuals = Object.Instantiate(animationPrefabs.Player);
                    buffer.AddComponent(entity, new VisualsReferenceComponent { GameObject = playerVisuals });
                }
                else 
                {
                    var playerVisualsReference = entityManager.GetComponentData<VisualsReferenceComponent>(entity);
                    var inputComponent = SystemAPI.GetSingleton<InputComponent>();

                    playerVisualsReference.GameObject.transform.position = transform.Position;
                    if (playerVisualsReference.GameObject.TryGetComponent<Animator>(out var animator))
                    {
                        animator.SetFloat("HorizontalAxis", inputComponent.Movement.x);
                        animator.SetFloat("VerticalAxis", inputComponent.Movement.y);
                    }
                }
            }

            buffer.Playback(entityManager);
            buffer.Dispose();
        }        
    }
}
