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
        MeshGenerator meshGen = GetComponent<MeshGenerator>();
        meshGen.GenerateMesh(Map, 1);
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
    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for(int neighbourX = gridX -1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < Width && neighbourY >= 0 && neighbourY < Height)
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

}
