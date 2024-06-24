using Unity.Entities;
using UnityEngine;

namespace Survivors
{
    public class AnimationPrefabsComponent : IComponentData
    {
        public GameObject Player;
        public GameObject[] Enemies;
    }
}
