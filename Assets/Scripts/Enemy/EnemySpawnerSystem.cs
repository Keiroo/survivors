using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Survivors
{
    public partial class EnemySpawnerSystem : SystemBase
    {
        private EnemySpawnerComponent enemySpawnerComponent;
        private EnemyListComponent enemyListComponent;
        private Entity enemySpawnerEntity;
        private float nextSpawnTime;
        private Random random;


        protected override void OnCreate()
        {
            random = Random.CreateFromIndex((uint)enemySpawnerComponent.GetHashCode());
        }

        protected override void OnUpdate()
        {
            if (!SystemAPI.TryGetSingletonEntity<EnemySpawnerComponent>(out enemySpawnerEntity))
                return;

            enemySpawnerComponent = EntityManager.GetComponentData<EnemySpawnerComponent>(enemySpawnerEntity);
            enemyListComponent = EntityManager.GetComponentObject<EnemyListComponent>(enemySpawnerEntity);

            if (SystemAPI.Time.ElapsedTime > nextSpawnTime)
                SpawnEnemy();
        }

        private void SpawnEnemy()
        {
            var level = 0;
            var availableEnemies = new List<EnemyData>();

            foreach (var enemy in enemyListComponent.Enemies)
                if (enemy.SpawnLevel == level)
                    availableEnemies.Add(enemy);

            var index = 0;
            var newEnemy = EntityManager.Instantiate(availableEnemies[index].Prefab);
            EntityManager.SetComponentData(newEnemy, new LocalTransform
            {
                Position = GetRandomEnemyPosition(),
                Rotation = Quaternion.identity,
                Scale = 1
            });

            nextSpawnTime = (float)SystemAPI.Time.ElapsedTime + enemySpawnerComponent.SpawnInterval;
        }

        private float3 GetRandomEnemyPosition()
        {
            var margin = 2;
            var cameraSize = enemySpawnerComponent.CameraSize;

            var x1 = random.NextFloat(-cameraSize.x - margin, -cameraSize.x);
            var x2 = random.NextFloat(cameraSize.x + margin, cameraSize.x);
            var y1 = random.NextFloat(-cameraSize.y - margin, -cameraSize.y);
            var y2 = random.NextFloat(cameraSize.y + margin, cameraSize.y);

            return new float3(random.NextBool() ? x1 : x2, random.NextBool() ? y1 : y2, 0);
        }
    }
}
