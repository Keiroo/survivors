using Unity.Entities;
using UnityEngine;

namespace Survivors
{
    public class AnimationPrefabs : IComponentData
    {
        public GameObject Player;
        public GameObject[] Enemies;
    }
}
