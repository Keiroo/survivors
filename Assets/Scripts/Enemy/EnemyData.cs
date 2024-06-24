using System;
using Unity.Entities;

namespace Survivors
{
    [Serializable]
    public struct EnemyData
    {
        public Entity Prefab;
        public SurvivorsTag Tag;
        public int SpawnLevel;
        public float Health;
        public float Damage;
        public float MoveSpeed;
    }
}
