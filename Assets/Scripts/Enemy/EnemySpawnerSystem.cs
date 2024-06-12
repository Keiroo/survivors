using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Survivors
{
    [BurstCompile]
    public partial class EnemySpawnerSystem : SystemBase
    {
        private EnemySpawnerComponent enemySpawnerComponent;
        private EnemyListComponent enemyListComponent;
        private Entity enemySpawnerEntity;
        private float nextSpawnTime;
        private Random random;


        [BurstCompile]
        protected override void OnCreate()
        {
            random = Random.CreateFromIndex((uint)enemySpawnerComponent.GetHashCode());
        }

        [BurstCompile]
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
            EntityManager.AddComponentData(newEnemy, new EnemyComponent 
            {
                PrefabID = availableEnemies[index].PrefabID,
                CurrentHealth = availableEnemies[index].Health,
                MoveSpeed = availableEnemies[index].MoveSpeed,
                Damage = availableEnemies[index].Damage
            });

            nextSpawnTime = (float)SystemAPI.Time.ElapsedTime + enemySpawnerComponent.SpawnInterval;
        }

        private float3 GetRandomEnemyPosition()
        {
            var playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();
            var playerTransform = SystemAPI.GetComponentRO<LocalTransform>(playerEntity);
            var horizontal_spawn = random.NextBool();
            var margin = enemySpawnerComponent.Margin;
            var cameraSize = enemySpawnerComponent.CameraSize;
            var playerX = playerTransform.ValueRO.Position.x;
            var playerY = playerTransform.ValueRO.Position.y;


            if (horizontal_spawn)
            {
                var x1 = random.NextFloat(-cameraSize.x - margin, -cameraSize.x) + playerX;
                var x2 = random.NextFloat(cameraSize.x + margin, cameraSize.x) + playerX;
                var y = random.NextFloat(-cameraSize.y - margin, cameraSize.y + margin) + playerY;
                return new float3 (random.NextBool() ? x1 : x2, y, 0f);
            }
            else
            {
                var x = random.NextFloat(-cameraSize.x - margin, cameraSize.x + margin) + playerX;
                var y1 = random.NextFloat(-cameraSize.y - margin, -cameraSize.y) + playerY;
                var y2 = random.NextFloat(cameraSize.y + margin, cameraSize.y) + playerY;
                return new float3(x, random.NextBool() ? y1 : y2, 0f);
            }
        }
    }
}
