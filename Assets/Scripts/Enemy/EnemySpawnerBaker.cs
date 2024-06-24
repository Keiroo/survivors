using System;
using System.Collections.Generic;
using Unity.Entities;

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
                SpawnInterval = authoring.SpawnInterval,
                CameraSize = authoring.CameraSize,
                Margin = authoring.Margin,
                SpawnsPerCycle = authoring.SpawnsPerCycle
            });

            foreach (var enemy in authoring.EnemyScriptables)
            {
                Enum.TryParse<SurvivorsTag>(enemy.VisualsPrefab.tag, false, out var tag);
                enemyData.Add(new EnemyData
                {
                    Prefab = GetEntity(enemy.Prefab, TransformUsageFlags.None),
                    Tag = tag,
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
