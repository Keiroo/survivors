using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Survivors
{
    [RequireComponent(typeof(Tilemap))]
    public class BackgroundGenerator : MonoBehaviour
    {
        public RandTile[] Tiles;

        private Tilemap tilemap;
        private System.Random random;
        private float weightSum;

        private void Start()
        {
            random = new System.Random();
            tilemap = GetComponent<Tilemap>();
            weightSum = Tiles.Select(val => val.Weight).Sum();

            for (int x = -5; x < 5; x++)
                for (int y = -5; y < 5; y++)
                    if (TryGetRandomTile(out var tile))
                        tilemap.SetTile(new Vector3Int(x, y, 0), tile);
        }

        private void FixedUpdate()
        {            

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

    [Serializable]
    public class RandTile
    {
        public Tile Tile;
        public float Weight;
    }
}
