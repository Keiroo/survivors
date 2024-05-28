using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Survivors
{
    public class EnemySpawnerBaker : Baker<EnemySpawnerAuthoring>
    {
        public override void Bake(EnemySpawnerAuthoring authoring)
        {
            var enemySpawnerAuthoring = GetEntity(TransformUsageFlags.None);
            var enemyData = new List<EnemyData>();

            AddComponent(enemySpawnerAuthoring, new EnemySpawnerComponent
            {
                SpawnInterval = authoring.SpawnInterval
            });

            foreach (var enemy in authoring.EnemyScriptables)
            {
                enemyData.Add(new EnemyData
                {
                    Prefab = GetEntity(enemy.Prefab, TransformUsageFlags.None),
                    SpawnLevel = enemy.SpawnLevel,
                    Health = enemy.Health,
                    Damage = enemy.Damage,
                    MoveSpeed = enemy.MoveSpeed
                });
            }
            
            AddComponentObject(enemySpawnerAuthoring, new EnemyListComponent { Enemies = enemyData });
        }
    }
}
