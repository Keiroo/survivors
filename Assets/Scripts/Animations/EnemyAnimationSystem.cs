using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Survivors
{
    public partial struct EnemyAnimationSystem : ISystem
    {
        private EntityManager entityManager;

        private void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.ManagedAPI.TryGetSingleton(out AnimationPrefabsComponent animationPrefabs))
                return;

            var buffer = new EntityCommandBuffer(Allocator.Temp);
            entityManager = state.EntityManager;

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
                    enemyVisualsReference.GameObject.transform.position = transform.Position;
                }
            }

            buffer.Playback(entityManager);
            buffer.Dispose();
        }
    }
}
