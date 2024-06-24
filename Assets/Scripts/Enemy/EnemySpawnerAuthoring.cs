using System.Collections.Generic;
using UnityEngine;

namespace Survivors
{
    public class EnemySpawnerAuthoring : MonoBehaviour
    {
        public float SpawnInterval = 1f;
        public Vector2 CameraSize;
        public float Margin;
        public int SpawnsPerCycle = 1;
        public List<EnemyScriptable> EnemyScriptables = new();
    }
}
