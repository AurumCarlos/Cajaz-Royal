using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Random = System.Random;

public class WalkerGenerator : MonoBehaviourPunCallbacks
{
    public enum Grid
    {
        Floor,
        Floor2,
        Wall,
        Street,
        Empty
    }

    [SerializeField] private int seed;
    private Random _random;
    private Grid[,] _gridHandler;
    private List<WalkerObject> _walkers;
    [SerializeField] private Tilemap tileMap;
    [SerializeField] private Tilemap tileMapcoll;
    [SerializeField] private List<Tile> tiles;
    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;

    [SerializeField] private int _maximumWalkers;
    private int _tileCount;
    private int _roadCount;
    [SerializeField] private float fillPercentage;

    private Vector2Int p;

    public WalkerGenerator(List<WalkerObject> walkers)
    {
        _walkers = walkers;
    }

    void Start()
    {
        if (seed == 0) seed = (int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond%10000000);
        _random = new Random(seed);
        InitializeGrid(seed);
        
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            seed = (int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond%10000000);
            photonView.RPC("InitializeGrid", RpcTarget.AllBuffered, seed);
        }
        
    }
    [PunRPC]
    void InitializeGrid(int randomseed)
    {
        _random = new Random(randomseed);

        _tileCount = 0;
        _roadCount = 0;
        _gridHandler = new Grid[mapWidth, mapHeight];
        for (int x = 0; x < _gridHandler.GetLength(0); x++)
        {
            for (int y = 0; y < _gridHandler.GetLength(1); y++)
            {
                tileMap.SetTile(new Vector3Int(x, y, 0), null);
                tileMapcoll.SetTile(new Vector3Int(x, y, 0), null);
                _gridHandler[x, y] = Grid.Empty;
            }
        }

        _walkers = new List<WalkerObject>();

        Vector3Int tileCenter = new Vector3Int(_gridHandler.GetLength(0) / 2, _gridHandler.GetLength(1) / 2, 0);
        WalkerObject curWalker = new WalkerObject(new Vector2(tileCenter.x,tileCenter.y), GetDirection(), 0.4f,0);
        Debug.unityLogger.Log(tileCenter);
        _gridHandler[tileCenter.x, tileCenter.y] = Grid.Floor;
        tileMap.SetTile(tileCenter, tiles[0]);
        _walkers.Add(curWalker);

        _tileCount++;

        CreateFloors();
        Clean();
        _walkers = new List<WalkerObject>();
        _gridHandler[tileCenter.x, tileCenter.y] = Grid.Street;
        tileMap.SetTile(tileCenter, tiles[2]);
        curWalker.ChanceToChange = 0.03f;
        curWalker.Type = 2;
        curWalker.Size = new Vector2(_random.Next(10, 30), _random.Next(10, 30));
        _walkers.Add(curWalker);
        CreateRoads();
        CreateWalls();
    }

    Vector2 GetDirection()
    {
        int choice = _random.Next(4);
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
        while (_tileCount / (float)_gridHandler.Length < fillPercentage)
        {
            foreach (WalkerObject curWalker in _walkers)
            {
                Vector3Int curPos = new Vector3Int((int)curWalker.Position.x, (int)curWalker.Position.y, 0);

                if (_gridHandler[curPos.x, curPos.y] != Grid.Floor)
                {
                    if(_random.Next(3) == 0) tileMap.SetTile(curPos, tiles[0]);
                    else tileMap.SetTile(curPos, tiles[4]);
                    _tileCount++;
                    _gridHandler[curPos.x, curPos.y] = Grid.Floor;
                }
            }

            //Walker Methods
            ChanceToRemove();
            ChanceToRedirect();
            ChanceToCreate();
            UpdatePosition();
            
        }
    }

    void ChanceToRemove()
    {
        int updatedCount = _walkers.Count;
        for (int i = 0; i < updatedCount; i++)
        {
            if (_random.Next(0,100) < _walkers[i].ChanceToChange * 100 && _walkers.Count > 1)
            {
                if (_walkers[i].Type == 0) _walkers.RemoveAt(i);
                else if (_random.Next(5) == 0) _walkers.RemoveAt(i);
                break;
            }
        }
    }

    void ChanceToRedirect()
    {
        for (int i = 0; i < _walkers.Count; i++)
        {
            if (_random.Next(0,100) < _walkers[i].ChanceToChange * 100)
            {
                WalkerObject curWalker = _walkers[i];
                if (curWalker.Type == 0) curWalker.Direction = GetDirection();
                _walkers[i] = curWalker;
            }
        }
    }

    void ChanceToCreate()
    {
        int updatedCount = _walkers.Count;
        for (int i = 0; i < updatedCount; i++)
        {
            if (_random.Next(0,100) * _walkers.Count / (float)_maximumWalkers < _walkers[i].ChanceToChange * 100 && _walkers.Count < _maximumWalkers)
            {
                Vector2 newDirection = GetDirection();
                Vector2 newPosition = _walkers[i].Position;
                WalkerObject newWalker = new WalkerObject(newPosition, newDirection, 0.4f,0);
                if (_walkers[i].Type == 2)
                {
                    newWalker.Type = 2;
                    newWalker.ChanceToChange = 0.02f;
                } 
                _walkers.Add(newWalker);
            }
        }
    }

    void UpdatePosition()
    {          
        //Debug.Log("t");
        for (int i = 0; i < _walkers.Count; i++)
        {

            WalkerObject foundWalker = _walkers[i];
            foundWalker.Position += foundWalker.Direction;
            Vector2 newPosition = new Vector2(Mathf.Clamp(foundWalker.Position.x, 2, _gridHandler.GetLength(0) - 3), Mathf.Clamp(foundWalker.Position.y, 2, _gridHandler.GetLength(1) - 3));
            foundWalker.Position = newPosition;
            _walkers[i] = foundWalker;
        }
    }
    public void  Clean()
    {
        for (int x = 1; x < _gridHandler.GetLength(0) - 1; x++)
        {
            for (int y = 1; y < _gridHandler.GetLength(1) - 1; y++)
            {
                if (_gridHandler[x, y] == Grid.Floor)
                {
                    if (_gridHandler[x + 1, y] == Grid.Empty)
                    {
                        tileMap.SetTile(new Vector3Int(x + 1, y, 0), tiles[0]);
                        _gridHandler[x + 1, y] = Grid.Floor2;
                    }
                    if (_gridHandler[x - 1, y] == Grid.Empty)
                    {
                        tileMap.SetTile(new Vector3Int(x - 1, y, 0), tiles[0]);
                        _gridHandler[x - 1, y] = Grid.Floor2;
                    }
                    if (_gridHandler[x, y + 1] == Grid.Empty)
                    {
                        tileMap.SetTile(new Vector3Int(x, y + 1, 0), tiles[0]);
                        _gridHandler[x, y + 1] = Grid.Floor2;
                    }
                    if (_gridHandler[x, y - 1] == Grid.Empty)
                    {
                        tileMap.SetTile(new Vector3Int(x, y - 1, 0), tiles[0]);
                        _gridHandler[x, y - 1] = Grid.Floor2;
                    }
                    
                }
            }
        }
    }

    public void CreateRoads()
    {
        for (int x = 100; x < mapHeight; x+=100) placeroad(x);
    }

    public void  CreateWalls()
    {
        for (int x = 1; x < _gridHandler.GetLength(0) - 1; x++)
        {
            for (int y = 1; y < _gridHandler.GetLength(1) - 1; y++)
            {
                if (_gridHandler[x, y] == Grid.Floor || _gridHandler[x , y] == Grid.Street || _gridHandler[x , y] == Grid.Floor2)
                {
                    if (_gridHandler[x + 1, y] == Grid.Empty)
                    {
                        tileMapcoll.SetTile(new Vector3Int(x + 1, y, 0), tiles[1]);
                        _gridHandler[x + 1, y] = Grid.Wall;
                    }
                    if (_gridHandler[x - 1, y] == Grid.Empty)
                    {
                        tileMapcoll.SetTile(new Vector3Int(x - 1, y, 0), tiles[1]);
                        _gridHandler[x - 1, y] = Grid.Wall;
                    }
                    if (_gridHandler[x, y + 1] == Grid.Empty)
                    {
                        tileMapcoll.SetTile(new Vector3Int(x, y + 1, 0), tiles[1]);
                        _gridHandler[x, y + 1] = Grid.Wall;
                    }
                    if (_gridHandler[x, y - 1] == Grid.Empty)
                    {
                        tileMapcoll.SetTile(new Vector3Int(x, y - 1, 0), tiles[1]);
                        _gridHandler[x, y - 1] = Grid.Wall;
                    }
                    
                }
            }
        }
    }

    private void placeroad(int s)
    {
        p = new Vector2Int(0, s);
        int y = mapHeight / 20;
        for (int i = 0; i < y; i++)
        {
            Vector2Int temp = new Vector2Int(_random.Next(10,30), _random.Next(10,30));
            placesquare(temp,p);
            p.x += temp.x;
        }
        p = new Vector2Int(0,s);
        for (int i = 0; i < y; i++)
        {
            Vector2Int temp = new Vector2Int(_random.Next(10,30), _random.Next(10,30));
            placesquare(temp,new Vector2Int(p.x,p.y-temp.y));
            p.x += temp.x;
        }
    }
    private void placesquare(Vector2 Size, Vector2Int start)
    {
        Vector2Int temp = p;
        p = start;
        p = new Vector2Int(p.x + 1, p.y);
        for (int i = p.x; i < p.x + Size.x; i++)
        {
            for (int j = p.y; j < p.y + Size.y; j++)
            {
                if (i < 3 || j < 3 || j > mapWidth - 1  || i > mapHeight - 1 )
                {
                    p = temp;
                    return;
                }
                if (_gridHandler[i, j] == Grid.Empty)
                {
                    p = temp;
                    return;
                }
                
            }
        }
        
        for (int j = 0; j < Size.x; j++)
        {
            placeif(_gridHandler[p.x, p.y] == Grid.Floor || _gridHandler[p.x, p.y] == Grid.Floor2, p.x, p.y);
            p.x += 1;
        }

        for (int j = 0; j < Size.y; j++)
        {
            placeif(_gridHandler[p.x, p.y] == Grid.Floor|| _gridHandler[p.x, p.y] == Grid.Floor2, p.x, p.y);
            p.y += 1;
        }

        for (int j = 0; j < Size.x; j++)
        {
            placeif(_gridHandler[p.x, p.y] == Grid.Floor|| _gridHandler[p.x, p.y] == Grid.Floor2, p.x, p.y);
            p.x -= 1;
        }

        for (int j = 0; j < Size.y; j++)
        {
            placeif(_gridHandler[p.x, p.y] == Grid.Floor|| _gridHandler[p.x, p.y] == Grid.Floor2, p.x, p.y);
            p.y -= 1;
        }
        p = temp;
    }
    private void placeif(bool canPlace,int x, int y)
    {
        if (canPlace)
        {
            _roadCount++;
            tileMap.SetTile(new Vector3Int(x,y,0), tiles[2]);
            _gridHandler[x, y] = Grid.Street;
        }
    }



}