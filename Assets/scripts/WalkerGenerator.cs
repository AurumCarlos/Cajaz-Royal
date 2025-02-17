using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

public class WalkerGenerator : MonoBehaviourPunCallbacks
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

    [SerializeField] private int Seed;
    private Random random;
    public Grid[,] gridHandler;
    public List<WalkerObject> Walkers;
    public Tilemap tileMap;
    public Tilemap tileMapcoll;
    [SerializeField] private List<Tile> Tiles;
    public int MapWidth = 30;
    public int MapHeight = 30;

    public int MaximumWalkers = 10;
    private int TileCount;
    private int RoadCount;
    public float FillPercentage = 0.4f;

    void Start()
    {
        if (Seed == 0) Seed = (int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond%10000000);
        random = new Random(Seed);
        InitializeGrid(Seed);
        
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Seed = (int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond%10000000);
            photonView.RPC("InitializeGrid", RpcTarget.AllBuffered, Seed);
        }
        
    }
    [PunRPC]
    void InitializeGrid(int seed)
    {
        random = new Random(seed);

        TileCount = 0;
        RoadCount = 0;
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

        WalkerObject curWalker = new WalkerObject(new Vector2(TileCenter.x,TileCenter.y), GetDirection(), 0.6f,0);
        gridHandler[TileCenter.x, TileCenter.y] = Grid.FLOOR;
        tileMap.SetTile(TileCenter, Tiles[0]);
        Walkers.Add(curWalker);

        TileCount++;

        StartCoroutine(CreateFloors());
        Clean();
        Walkers = new List<WalkerObject>();
        gridHandler[TileCenter.x, TileCenter.y] = Grid.STREET;
        tileMap.SetTile(TileCenter, Tiles[2]);
        curWalker.ChanceToChange = 0.03f;
        curWalker.Type = 2;
        Walkers.Add(curWalker);
        CreateRoads();
        CreateWalls();
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

    public IEnumerator CreateFloors()
    {
        while ((float)TileCount / (float)gridHandler.Length < FillPercentage)
        {
            foreach (WalkerObject curWalker in Walkers)
            {
                Vector3Int curPos = new Vector3Int((int)curWalker.Position.x, (int)curWalker.Position.y, 0);

                if (gridHandler[curPos.x, curPos.y] != Grid.FLOOR)
                {
                    if(random.Next(3) == 0) tileMap.SetTile(curPos, Tiles[0]);
                    else tileMap.SetTile(curPos, Tiles[4]);
                    TileCount++;
                    gridHandler[curPos.x, curPos.y] = Grid.FLOOR;
                }
            }

            //Walker Methods
            ChanceToRemove();
            ChanceToRedirect();
            ChanceToCreate();
            UpdatePosition();
            
        }

        if (false)
        {
            yield return new WaitForSeconds(0.5f);
        }
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
            if (random.Next(0,100) * Walkers.Count / MaximumWalkers < Walkers[i].ChanceToChange * 100 && Walkers.Count < MaximumWalkers)
            {
                Vector2 newDirection = GetDirection();
                Vector2 newPosition = Walkers[i].Position;
                WalkerObject newWalker = new WalkerObject(newPosition, newDirection, 0.6f,0);
                if (Walkers[i].Type == 2)
                {
                    newWalker.Type = 2;
                    newWalker.ChanceToChange = 0.02f;
                } 
                Walkers.Add(newWalker);
            }
        }
    }

    void UpdatePosition()
    {          
        //Debug.Log("t");
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
        for (int x = 1; x < gridHandler.GetLength(0) - 1; x++)
        {
            for (int y = 1; y < gridHandler.GetLength(1) - 1; y++)
            {
                if (gridHandler[x, y] == Grid.FLOOR)
                {
                    if (gridHandler[x + 1, y] == Grid.EMPTY)
                    {
                        tileMap.SetTile(new Vector3Int(x + 1, y, 0), Tiles[0]);
                        gridHandler[x + 1, y] = Grid.FLOOR2;
                    }
                    if (gridHandler[x - 1, y] == Grid.EMPTY)
                    {
                        tileMap.SetTile(new Vector3Int(x - 1, y, 0), Tiles[0]);
                        gridHandler[x - 1, y] = Grid.FLOOR2;
                    }
                    if (gridHandler[x, y + 1] == Grid.EMPTY)
                    {
                        tileMap.SetTile(new Vector3Int(x, y + 1, 0), Tiles[0]);
                        gridHandler[x, y + 1] = Grid.FLOOR2;
                    }
                    if (gridHandler[x, y - 1] == Grid.EMPTY)
                    {
                        tileMap.SetTile(new Vector3Int(x, y - 1, 0), Tiles[0]);
                        gridHandler[x, y - 1] = Grid.FLOOR2;
                    }
                    
                }
            }
        }
    }

    public void CreateRoads()
    {
        while ((float)10 * RoadCount / (float)gridHandler.Length < FillPercentage && Walkers.Count > 0)
        {
            for (int i = 0; i < Walkers.Count; i++)
            {
                WalkerObject curWalker = Walkers[i];
                Vector3Int curPos = new Vector3Int((int)curWalker.Position.x, (int)curWalker.Position.y, 0);
                if (gridHandler[curPos.x, curPos.y] == Grid.EMPTY) Walkers.RemoveAt(i);
                else
                {
                    if (gridHandler[curPos.x, curPos.y] != Grid.STREET)
                    {
                        RoadCount++;
                        tileMap.SetTile(curPos, Tiles[2]);
                        gridHandler[curPos.x, curPos.y] = Grid.STREET;
                    }
                    
                }
                
            }

            
            
            //Walker Methods
            ChanceToRemove();
            ChanceToRedirect();
            ChanceToCreate();
            UpdatePosition();
            
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
                        tileMapcoll.SetTile(new Vector3Int(x + 1, y, 0), Tiles[1]);
                        gridHandler[x + 1, y] = Grid.WALL;
                    }
                    if (gridHandler[x - 1, y] == Grid.EMPTY)
                    {
                        tileMapcoll.SetTile(new Vector3Int(x - 1, y, 0), Tiles[1]);
                        gridHandler[x - 1, y] = Grid.WALL;
                    }
                    if (gridHandler[x, y + 1] == Grid.EMPTY)
                    {
                        tileMapcoll.SetTile(new Vector3Int(x, y + 1, 0), Tiles[1]);
                        gridHandler[x, y + 1] = Grid.WALL;
                    }
                    if (gridHandler[x, y - 1] == Grid.EMPTY)
                    {
                        tileMapcoll.SetTile(new Vector3Int(x, y - 1, 0), Tiles[1]);
                        gridHandler[x, y - 1] = Grid.WALL;
                    }
                    
                }
            }
        }
    }



}