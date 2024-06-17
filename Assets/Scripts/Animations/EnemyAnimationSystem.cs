using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Survivors
{
    public partial struct EnemyAnimationSystem : ISystem
    {
        private EntityManager entityManager;

        private void OnUpdate(ref SystemState state)
        {
            if (SceneManager.GetActiveScene().buildIndex != 1)
                return;

            if (!SystemAPI.ManagedAPI.TryGetSingleton(out AnimationPrefabsComponent animationPrefabs))
                return;
            if (!SystemAPI.TryGetSingletonEntity<PlayerComponent>(out var playerEntity))
                return;

            var buffer = new EntityCommandBuffer(Allocator.Temp);
            entityManager = state.EntityManager;
            var playerTransform = SystemAPI.GetComponentRO<LocalTransform>(playerEntity);

            foreach (var (transform, enemyComponent, entity) in SystemAPI.Query<LocalTransform, EnemyComponent>().WithEntityAccess())
            {
                if (!entityManager.HasComponent<VisualsReferenceComponent>(entity))
                {
                    var targetVisual = animationPrefabs.Enemies.FirstOrDefault(x => x.GetInstanceID() == enemyComponent.PrefabID);
                    var enemyVisuals = Object.Instantiate(targetVisual, Vector3.one * 1000, Quaternion.identity);
                    
                    buffer.AddComponent(entity, new VisualsReferenceComponent { GameObject = enemyVisuals });
                }
                else
                {
                    var enemyVisualsReference = entityManager.GetComponentData<VisualsReferenceComponent>(entity);
                    if (enemyVisualsReference.GameObject != null)
                    {
                        enemyVisualsReference.GameObject.transform.position = transform.Position;
                        if (enemyVisualsReference.GameObject.TryGetComponent<SpriteRenderer>(out var renderer))
                        {
                            renderer.flipX = playerTransform.ValueRO.Position.x - transform.Position.x < 0;
                        }

                    }
                }
            }

            buffer.Playback(entityManager);
            buffer.Dispose();
        }
    }
}
