using UnityEngine;

namespace Survivors
{
    [CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/Enemy")]
    public class EnemyScriptable : ScriptableObject
    {
        public GameObject Prefab;
        public int SpawnLevel;
        public float Health;
        public float Damage;
        public float MoveSpeed;
    }
}
