using System;
using Unity.Entities;

namespace Survivors
{
    [Serializable]
    public struct EnemyData
    {
        public Entity Prefab;
        public int PrefabID;
        public int SpawnLevel;
        public float Health;
        public float Damage;
        public float MoveSpeed;
    }
}
