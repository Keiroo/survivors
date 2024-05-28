using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Survivors
{
    [Serializable]
    public class RandTile
    {
        public Tile Tile;
        public float Weight;
    }

    [RequireComponent(typeof(Tilemap))]
    public class BackgroundGenerator : MonoBehaviour
    {
        public int MapMargin = 5;
        public RandTile[] Tiles;

        private Tilemap tilemap;
        private System.Random random;
        private float weightSum;

        private void Start()
        {
            random = new System.Random();
            tilemap = GetComponent<Tilemap>();
            weightSum = Tiles.Select(val => val.Weight).Sum();

            for (int x = -MapMargin; x < MapMargin; x++)
                for (int y = -MapMargin; y < MapMargin; y++)
                    if (TryGetRandomTile(out var tile))
                        tilemap.SetTile(new Vector3Int(x, y, 0), tile);
        }

        private void FixedUpdate()
        {
            var playerPos = GameManager.PlayerInstance.transform.position;
            var bounds_horizontal = new Vector2Int(Mathf.FloorToInt(playerPos.x) - MapMargin, Mathf.CeilToInt(playerPos.x) + MapMargin);
            var bounds_vertical = new Vector2Int(Mathf.FloorToInt(playerPos.y) - MapMargin, Mathf.CeilToInt(playerPos.y) + MapMargin);

            for (int x = bounds_horizontal.x; x < bounds_horizontal.y; x++)
                for (int y = bounds_vertical.x; y < bounds_vertical.y; y++)
                    if (tilemap.GetTile(new Vector3Int(x, y, 0)) == null)
                        if (TryGetRandomTile(out var tile))
                            tilemap.SetTile(new Vector3Int(x, y, 0), tile);
        }

        private Tile GetRandomTile()
        {
            if (Tiles.Length == 0)
                return ScriptableObject.CreateInstance<Tile>();

            var rand = random.NextDouble() * weightSum;
            return Tiles.First(val => (rand -= val.Weight) < 0).Tile;
        }

        private bool TryGetRandomTile(out Tile tile)
        {
            tile = null;
            if (Tiles.Length == 0)
                return false;

            var rand = random.NextDouble() * weightSum;
            tile = Tiles.First(val => (rand -= val.Weight) < 0).Tile;
            return true;
        }
    }
}
