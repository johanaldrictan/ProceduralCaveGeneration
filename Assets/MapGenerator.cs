using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapGenerator : MonoBehaviour {

    public int Width;
    public int Height;

    public string Seed;
    public bool UseRandomSeed;
    public int SmoothFactor = 5;

    public int wallThresholdSize = 50;
    public int roomThresholdSize = 50;

    [Range(0,100)]
    public int RandomFillPercent;

    int[,] Map;
    //coordinate with 0 is empty coordinate with 1 has a wall

    void Start()
    {
        GenerateMap();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GenerateMap();
        }
    }
    void GenerateMap()
    {
        Map = new int[Width, Height];
        RandomFillMap();
        for(int i = 0; i< SmoothFactor; i++)
        {
            SmoothMap();
        }
        ProcessMap();

        MeshGenerator meshGen = GetComponent<MeshGenerator>();
        meshGen.GenerateMesh(Map, 1);

    }

    void ProcessMap()
    {
        List<List<Coord>> wallRegions = GetRegions(1);
        foreach(List<Coord> wallRegion in wallRegions)
        {
            if(wallRegion.Count < wallThresholdSize)
            {
                foreach(Coord tile in wallRegion)
                {
                    Map[tile.TileX, tile.TileY] = 0;
                }
            }
        }

        List<List<Coord>> roomRegions = GetRegions (0);
        
        foreach (List<Coord> roomRegion in roomRegions) {
            if (roomRegion.Count < roomThresholdSize) {
                foreach (Coord tile in roomRegion) {
                    Map[tile.TileX,tile.TileY] = 1;
                }
            }
        }

    }

    void RandomFillMap()
    {
        if (UseRandomSeed)
        {
            Seed = DateTime.Now.ToString();
        }
        System.Random pseudoRandom = new System.Random(Seed.GetHashCode());
        for(int x = 0; x < Width; x++)
        {
            for(int y = 0; y < Height; y++)
            {
                if (x == 0 || x == Width - 1 || y == 0 || y == Height - 1)
                {
                    Map[x, y] = 1;
                }
                else
                {
                    Map[x, y] = pseudoRandom.Next(0, 100) < RandomFillPercent ? 1 : 0;
                }
            }
        }
    
        
    }
    void SmoothMap()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);
                if (neighbourWallTiles > 4)
                    Map[x, y] = 1;
                else if (neighbourWallTiles < 4)
                    Map[x, y] = 0;
            }
        }
    }
    void PerformGameOfLife()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                int neighbourCount = GetSurroundingWallCount(x, y);
                if(Map[x,y] == 1)
                {
                    if (neighbourCount < 2)
                        Map[x, y] = 0;
                    else if (neighbourCount > 3)
                        Map[x, y] = 0;
                    else
                        continue;
                }
                else
                {
                    if (neighbourCount == 3)
                        Map[x, y] = 1;
                }
            }
        }
    }
    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for(int neighbourX = gridX -1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (IsInMapRange(neighbourX, neighbourY))
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += Map[neighbourX, neighbourY];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }
        return wallCount;
    }

    List<List<Coord>> GetRegions(int tileType)
    {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapFlags = new int[Width, Height];
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if(mapFlags[x,y] == 0 && Map[x,y] == tileType)
                {
                    List<Coord> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);

                    foreach(Coord tile in newRegion)
                    {
                        mapFlags[tile.TileX, tile.TileY] = 1;
                    }
                }
            }
        }
        return regions;
    }

    //Flood-Fill Algorithm
    List<Coord> GetRegionTiles(int startX, int startY)
    {
        List<Coord> tiles = new List<Coord>();
        int[,] mapFlags = new int[Width, Height];
        int tileType = Map[startX, startY];

        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY));
        mapFlags[startX, startY] = 1;

        while(queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);
            for(int x = tile.TileX - 1; x <= tile.TileX + 1; x++)
            {
                for (int y = tile.TileY - 1; y <= tile.TileY + 1; y++)
                {
                    if (IsInMapRange(x, y) && (y == tile.TileY || x == tile.TileX))
                    {
                        if(mapFlags[x,y] == 0 && Map[x,y] == tileType)
                        {
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }
        }
        return tiles;
    }

    bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    struct Coord
    {
        public int TileX;
        public int TileY;

        public Coord(int x, int y)
        {
            TileX = x;
            TileY = y;
        }
    }
}
