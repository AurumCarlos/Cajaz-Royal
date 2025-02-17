using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

public class WalkerGenerator : MonoBehaviour
{
    public enum Grid
    {
        FLOOR,
        FLOOR2,
        WALL,
        STREET,
        STREET2,
        EMPTY
    }

    [SerializeField] private int seed;
    private Random random;
    public Grid[,] gridHandler;
    public List<WalkerObject> Walkers;
    public Tilemap tileMap;
    public Tilemap tileMapcoll;
    public Tile Floor;
    public Tile Floor2;
    public Tile Wall;
    public Tile Street;
    public int MapWidth = 30;
    public int MapHeight = 30;

    public int MaximumWalkers = 10;
    public int TileCount = default;
    public float FillPercentage = 0.4f;

    void Start()
    {
        if (seed == 0) seed = (int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond%10000000);
        random = new Random(seed);
        InitializeGrid();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TileCount = 0;
            seed = (int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond%100000);
            random = new Random(seed);
            InitializeGrid();
        }
        
    }

    void InitializeGrid()
    {
        Debug.Log("reset");

        gridHandler = new Grid[MapWidth, MapHeight];

        for (int x = 0; x < gridHandler.GetLength(0); x++)
        {
            for (int y = 0; y < gridHandler.GetLength(1); y++)
            {
                tileMap.SetTile(new Vector3Int(x, y, 0), null);
                tileMapcoll.SetTile(new Vector3Int(x, y, 0), null);
                gridHandler[x, y] = Grid.EMPTY;
            }
        }

        Walkers = new List<WalkerObject>();

        Vector3Int TileCenter = new Vector3Int(gridHandler.GetLength(0) / 2, gridHandler.GetLength(1) / 2, 0);

        WalkerObject curWalker = new WalkerObject(new Vector2(TileCenter.x, TileCenter.y), GetDirection(), 0.5f,0);
        gridHandler[TileCenter.x, TileCenter.y] = Grid.FLOOR;
        tileMap.SetTile(TileCenter, Floor);
        Walkers.Add(curWalker);

        TileCount++;

        //StartCoroutine(CreateFloors());
        CreateFloors();
    }

    Vector2 GetDirection()
    {
        int choice = random.Next(4);
        switch (choice)
        {
            case 0:
                return Vector2.down;
            case 1:
                return Vector2.left;
            case 2:
                return Vector2.up;
            case 3:
                return Vector2.right;
            default:
                return Vector2.zero;
        }
    }

    public void CreateFloors()
    {
        while ((float)TileCount / (float)gridHandler.Length < FillPercentage)
        {
            foreach (WalkerObject curWalker in Walkers)
            {
                Vector3Int curPos = new Vector3Int((int)curWalker.Position.x, (int)curWalker.Position.y, 0);

                if (gridHandler[curPos.x, curPos.y] != Grid.FLOOR && gridHandler[curPos.x, curPos.y] != Grid.STREET)
                {
                    TileCount++;
                    if (curWalker.Type == 0)
                    {
                        tileMap.SetTile(curPos, Floor);
                        gridHandler[curPos.x, curPos.y] = Grid.FLOOR;
                    }
                    else
                    {
                        tileMap.SetTile(curPos, Street);
                        gridHandler[curPos.x, curPos.y] = Grid.STREET;
                    }
                }
            }

            //Walker Methods
            ChanceToRemove();
            ChanceToRedirect();
            ChanceToCreate();
            UpdatePosition();

            //if (hasCreatedFloor)
            //{
            //    yield return new WaitForSeconds(WaitTime);
            //}
        }
        Debug.Log("reset");

        //StartCoroutine(CreateWalls());
        Clean();
        //Road();
        CreateWalls();
    }

    void ChanceToRemove()
    {
        int updatedCount = Walkers.Count;
        for (int i = 0; i < updatedCount; i++)
        {
            if (random.Next(0,100) < Walkers[i].ChanceToChange * 100 && Walkers.Count > 1)
            {
                if (Walkers[i].Type == 0) Walkers.RemoveAt(i);
                else if (random.Next(5) == 0) Walkers.RemoveAt(i);
                break;
            }
        }
    }

    void ChanceToRedirect()
    {
        for (int i = 0; i < Walkers.Count; i++)
        {
            if (random.Next(0,100) < Walkers[i].ChanceToChange * 100)
            {
                WalkerObject curWalker = Walkers[i];
                if (curWalker.Type == 0) curWalker.Direction = GetDirection();
                Walkers[i] = curWalker;
            }
        }
    }

    void ChanceToCreate()
    {
        int updatedCount = Walkers.Count;
        for (int i = 0; i < updatedCount; i++)
        {
            if (random.Next(0,100) < Walkers[i].ChanceToChange * 100 && Walkers.Count < MaximumWalkers)
            {
                Vector2 newDirection = GetDirection();
                Vector2 newPosition = Walkers[i].Position;
                WalkerObject newWalker = new WalkerObject(newPosition, newDirection, 0.5f,0);
                if (random.Next(0, 100) == 0)
                {
                    newWalker.Type = 1;
                    newWalker.ChanceToChange = 0.5f;
                } 
                Walkers.Add(newWalker);
            }
        }
    }

    void UpdatePosition()
    {
        for (int i = 0; i < Walkers.Count; i++)
        {
            WalkerObject FoundWalker = Walkers[i];
            FoundWalker.Position += FoundWalker.Direction;
            FoundWalker.Position.x = Mathf.Clamp(FoundWalker.Position.x, 2, gridHandler.GetLength(0) - 3);
            FoundWalker.Position.y = Mathf.Clamp(FoundWalker.Position.y, 2, gridHandler.GetLength(1) - 3);
            Walkers[i] = FoundWalker;
        }
    }
    public void  Clean()
    {
        for (int x = 0; x < gridHandler.GetLength(0) - 1; x++)
        {
            for (int y = 0; y < gridHandler.GetLength(1) - 1; y++)
            {
                if (gridHandler[x, y] == Grid.FLOOR)
                {
                    if (gridHandler[x + 1, y] == Grid.EMPTY)
                    {
                        tileMap.SetTile(new Vector3Int(x + 1, y, 0), Floor2);
                        gridHandler[x + 1, y] = Grid.FLOOR2;
                    }
                    if (gridHandler[x - 1, y] == Grid.EMPTY)
                    {
                        tileMap.SetTile(new Vector3Int(x - 1, y, 0), Floor2);
                        gridHandler[x - 1, y] = Grid.FLOOR2;
                    }
                    if (gridHandler[x, y + 1] == Grid.EMPTY)
                    {
                        tileMap.SetTile(new Vector3Int(x, y + 1, 0), Floor2);
                        gridHandler[x, y + 1] = Grid.FLOOR2;
                    }
                    if (gridHandler[x, y - 1] == Grid.EMPTY)
                    {
                        tileMap.SetTile(new Vector3Int(x, y - 1, 0), Floor2);
                        gridHandler[x, y - 1] = Grid.FLOOR2;
                    }
                    
                }
            }
        }
    }

    public void Road()
    {
        for (int x = 0; x < gridHandler.GetLength(0) - 1; x++)
        {
            for (int y = 0; y < gridHandler.GetLength(1) - 1; y++)
            {
                if (gridHandler[x, y] == Grid.STREET)
                {
                    tileMap.SetTile(new Vector3Int(x + 1, y, 0), Street);
                    gridHandler[x + 1, y] = Grid.STREET2;
                    tileMap.SetTile(new Vector3Int(x - 1, y, 0), Street);
                    gridHandler[x - 1, y] = Grid.STREET2;
                    tileMap.SetTile(new Vector3Int(x, y + 1, 0), Street);
                    gridHandler[x, y + 1] = Grid.STREET2;
                    tileMap.SetTile(new Vector3Int(x, y - 1, 0), Street);
                    gridHandler[x, y - 1] = Grid.STREET2;
                    
                    tileMap.SetTile(new Vector3Int(x + 1, y + 1, 0), Street);
                    gridHandler[x + 1, y + 1] = Grid.STREET2;
                    tileMap.SetTile(new Vector3Int(x - 1, y + 1, 0), Street);
                    gridHandler[x - 1, y + 1] = Grid.STREET2;
                    tileMap.SetTile(new Vector3Int(x + 1, y - 1, 0), Street);
                    gridHandler[x + 1, y - 1] = Grid.STREET2;
                    tileMap.SetTile(new Vector3Int(x - 1, y - 1, 0), Street);
                    gridHandler[x - 1, y - 1] = Grid.STREET2;
                }
            }
        }
    }

    public void  CreateWalls()
    {
        for (int x = 1; x < gridHandler.GetLength(0) - 1; x++)
        {
            for (int y = 1; y < gridHandler.GetLength(1) - 1; y++)
            {
                if (gridHandler[x, y] == Grid.FLOOR || gridHandler[x , y] == Grid.STREET || gridHandler[x , y] == Grid.FLOOR2)
                {
                    if (gridHandler[x + 1, y] == Grid.EMPTY)
                    {
                        tileMapcoll.SetTile(new Vector3Int(x + 1, y, 0), Wall);
                        gridHandler[x + 1, y] = Grid.WALL;
                    }
                    if (gridHandler[x - 1, y] == Grid.EMPTY)
                    {
                        tileMapcoll.SetTile(new Vector3Int(x - 1, y, 0), Wall);
                        gridHandler[x - 1, y] = Grid.WALL;
                    }
                    if (gridHandler[x, y + 1] == Grid.EMPTY)
                    {
                        tileMapcoll.SetTile(new Vector3Int(x, y + 1, 0), Wall);
                        gridHandler[x, y + 1] = Grid.WALL;
                    }
                    if (gridHandler[x, y - 1] == Grid.EMPTY)
                    {
                        tileMapcoll.SetTile(new Vector3Int(x, y - 1, 0), Wall);
                        gridHandler[x, y - 1] = Grid.WALL;
                    }
                    
                }
            }
        }
    }

}