using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Survivors
{
    public class FollowEntity : MonoBehaviour
    {
        public Entity Target;
        public float3 Offset = float3.zero;

        private Entity playerEntity;
        private EntityManager entityManager;

        private void Awake()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        private void LateUpdate()
        {
            // if (Target == null) return;
            var targetTransform = entityManager.GetComponentData<LocalTransform>(Target);
            transform.position = targetTransform.Position + Offset;
        }
    }
}
