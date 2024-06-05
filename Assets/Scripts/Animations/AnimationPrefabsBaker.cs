using Unity.Entities;

namespace Survivors
{
    public class AnimationPrefabsBaker : Baker<AnimationPrefabsAuthoring>
    {
        public override void Bake(AnimationPrefabsAuthoring authoring)
        {
            var playerEntity = GetEntity(TransformUsageFlags.None);

            AddComponentObject(playerEntity, new AnimationPrefabs
            {
                Player = authoring.Player,
                Enemies = authoring.Enemies
            });
        }
    }
}