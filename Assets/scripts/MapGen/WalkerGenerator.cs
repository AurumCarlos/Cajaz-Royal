using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Unity.Mathematics;
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
        Bat,
        Object,
        Empty
    }

    private int seed = 0;
    private Random _random;
    private static Random _random1 = new Random(0);
    private static Grid[,] _gridHandler;
    private Grid[,] _gridHandlercoll;
    private bool[][] _forest;
    private bool[][] _fire;
    private bool[][] _city;
    private bool[][] Roads;

    private List<WalkerObject> _walkers;
    [SerializeField] private Tilemap tileMap;
    [SerializeField] private Tilemap tileMapBat;
    [SerializeField] private Tilemap tileMapcoll;
    [SerializeField] private List<Tile> tiles;
    [SerializeField] private List<GameObject> Bats;
    private int mapWidth = 300;
    private int mapHeight = 300;
    private int maximumWalkers = 20;
    private int _tileCount;
    [SerializeField] float fillPercentage;
    private Vector2Int p;

    [SerializeField] private int RoadDensity;

    public WalkerGenerator(List<WalkerObject> walkers)
    {
        _walkers = walkers;
    }

    void Start()
    {
        mapWidth = 300;
        mapHeight = 300;
        maximumWalkers = 20;
        //fillPercentage = 0.2f;
        seed = (int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond % 10000000);
        InitializeGrid(seed);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (PhotonNetwork.IsConnected)
                photonView.RPC("InitializeGrid", RpcTarget.AllBuffered,
                    (int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond % 10000000));
            else InitializeGrid((int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond % 10000000));
            //Start();
        }
    }

    [PunRPC]
    void InitializeGrid(int randomseed)
    {
        Roads = new bool[mapHeight / 3][];
        for (int i = 0; i < mapHeight / 3; i++)
        {
            Roads[i] = new bool[mapWidth / 3];
        }

        _random = new Random(randomseed);

        _tileCount = 0;
        _gridHandler = new Grid[mapWidth, mapHeight];
        _gridHandlercoll = new Grid[mapWidth, mapHeight];
        for (int x = 0; x < _gridHandler.GetLength(0); x++)
        {
            for (int y = 0; y < _gridHandler.GetLength(1); y++)
            {
                tileMap.SetTile(new Vector3Int(x, y, 0), tiles[1]);
                tileMapcoll.SetTile(new Vector3Int(x, y, 0), null);
                tileMapBat.SetTile(new Vector3Int(x, y, 0), null);
                _gridHandler[x, y] = Grid.Empty;
                _gridHandlercoll[x, y] = Grid.Empty;
            }
        }

        _walkers = new List<WalkerObject>();

        Vector3Int tileCenter = new Vector3Int(_gridHandler.GetLength(0) / 2, _gridHandler.GetLength(1) / 2, 0);
        WalkerObject curWalker = new WalkerObject(new Vector2(tileCenter.x, tileCenter.y), GetDirection(), 0.6f, 0);
        _gridHandler[tileCenter.x, tileCenter.y] = Grid.Floor;
        tileMap.SetTile(tileCenter, tiles[0]);
        _walkers.Add(curWalker);

        _tileCount++;
        displayperlin();
        CreateFloors();
        //Clean();

        _walkers = new List<WalkerObject>();
        curWalker.ChanceToChange = 0.03f;
        curWalker.Type = 2;
        curWalker.Size = new Vector2(_random.Next(10, 30), _random.Next(10, 30));
        _walkers.Add(curWalker);

        placetree();
        Tree t = new Tree(mapHeight / 3, mapWidth / 3, mapHeight / 3, mapWidth / 3);
        t.couper(true, RoadDensity, tileMap, _random, tiles[2], _gridHandler, Roads, mapHeight, mapWidth);
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                if (Roads[i][j] && _random.Next(6) != 0)
                {
                    tileMap.SetTile(new Vector3Int(mapHeight / 3 + j, 2 * mapWidth / 3 - i, 0), tiles[2]);
                    _gridHandler[mapHeight / 3 + i, mapWidth / 3 + j] = Grid.Street;
                }
            }
        }

        CityBuider.Initialize(Roads);
        List<((int, int), (int, int))> BatList = CityBuider.BindBatPlace();

        foreach (var bat in BatList)
        {
            (int, int) pos = bat.Item1;
            (int, int) size = bat.Item2;

            int n = 0;
            for (int i = pos.Item1; i < pos.Item1 + size.Item1; i++)
            {
                for (int j = pos.Item2; j < pos.Item2 + size.Item2; j++)
                {
                    if (_gridHandler[100 + j,200 - i + size.Item2 - 1] == Grid.Empty)
                    {
                        n += 1;
                    }
                }
            }
            if (n < size.Item1 * size.Item2 * 0.2)
            {
                for (int i = pos.Item1 ; i< pos.Item1 +  size.Item2; i++)
                {
                    for (int j = pos.Item2; j < pos.Item2 + size.Item1; j++)
                    {
                        _gridHandler[100 + j,200 - i + size.Item2 - 1] = Grid.Floor;
                        tileMapcoll.SetTile(new Vector3Int(100 + j,200 - i + size.Item2 - 1, 0), null);
                        tileMap.SetTile(new Vector3Int(100 + j,200 - i + size.Item2 - 1, 0), tiles[0]);
                        
                        //_gridHandler[i, j] = Grid.Empty;
                    }
                }

                if (true || size == (8, 9))
                {
                    GameObject temp = GameObject.Instantiate(Bats[0],tileMapBat.transform);
                    temp.transform.position = new Vector3Int(mapHeight / 3 + pos.Item2, 2 * mapWidth / 3 - pos.Item1, 0);
                    //tileMapBat.SetTile(new Vector3Int(mapHeight / 3 + pos.Item2, 2 * mapWidth / 3 - pos.Item1, 0), tiles[8]);
                }
                else
                {
                    Debug.Log((mapHeight / 3 + pos.Item2, 2 * mapWidth / 3 - pos.Item1));
                    //tileMapBat.SetTile(new Vector3Int(mapHeight / 3 + pos.Item2, 2 * mapWidth / 3 - pos.Item1, 0), tiles[9]);
                }
                _gridHandler[mapHeight / 3 + pos.Item2, mapWidth / 3 + pos.Item1] = Grid.Bat;
            }
        }

        if (!PhotonNetwork.IsConnected)
        {
            GameObject[] r =  GameObject.FindGameObjectsWithTag("Player");
            if (r.Length > 0)
            {
                (int, int) spawn = FindSpawn(); 
                r[0].transform.position = new Vector3(spawn.Item1, spawn.Item2, 0);
            }
        }
        
        Clean();
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
                    if (_fire[curPos.x][curPos.y]) tileMap.SetTile(curPos, tiles[7]);
                    else if(_random.Next(3) == 0) tileMap.SetTile(curPos, tiles[0]);
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
            if (_random.Next(0,100) * _walkers.Count / (float)maximumWalkers < _walkers[i].ChanceToChange * 100 && _walkers.Count < maximumWalkers)
            {
                Vector2 newDirection = GetDirection();
                Vector2 newPosition = _walkers[i].Position;
                WalkerObject newWalker = new WalkerObject(newPosition, newDirection, 0.6f,0);
                _walkers.Add(newWalker);
            }
        }
    }

    void UpdatePosition()
    {          
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


    public void  CreateWalls()
    {
        for (int x = 0; x < _gridHandler.GetLength(0); x++)
        {
            for (int y = 0; y < _gridHandler.GetLength(1); y++)
            {
                if (_gridHandler[x, y] == Grid.Empty)
                {
                    tileMapcoll.SetTile(new Vector3Int(x , y, 0), tiles[1]);
                    _gridHandlercoll[x, y] = Grid.Wall;
                }
            }
        }
        
        
        return;
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
    

    private void placetree()
    {
        for (int y = mapHeight; y > 0; y-=4)
        {
            for (int x = 0; x < mapWidth; x += _random.Next(3, 7))
            {
                if (!(y > mapHeight / 3 && y < 2 * mapHeight / 3 && x >  mapWidth / 3 && x < 2 * mapWidth / 3))
                {
                    int t = y + _random.Next(-1, 2);
                    if (t < 0 || t > mapHeight - 1) t = y;
                    else if (!_fire[x][t] && (_forest[x][t] || _random.Next(0, 10) == 0) &&
                             (_gridHandler[x, t] == Grid.Floor2 ||
                              _gridHandler[x, t] == Grid.Floor && _random.Next(0, 15) < 10))
                    {
                        tileMapcoll.SetTile(new Vector3Int(x, t + 1 * _random.Next(-1, 2), 0), tiles[5]);
                        _gridHandlercoll[x, t] = Grid.Object;
                    }

                    t = y + _random.Next(-1, 2);
                    if (t < 0 || t > mapHeight - 1) t = y;
                    else if (_fire[x][t] && (_gridHandler[x, t] == Grid.Floor2 || _gridHandler[x, t] == Grid.Floor) &&
                             _random.Next(0, 25) < 10)
                    {
                        if (_random.Next(6) == 0) tileMapcoll.SetTile(new Vector3Int(x, t +  _random.Next(-1, 2), 0), tiles[3]);
                        else  tileMapcoll.SetTile(new Vector3Int(x, t +  _random.Next(-1, 2), 0), tiles[6]);
                        _gridHandlercoll[x, t] = Grid.Object;
                    }
                }
            }
        }
    }

    private void displayperlin()
    {
        _forest = new bool[mapWidth][];
        for (int i = 0; i < _forest.Length; i++)
        {
            _forest[i] = new bool[mapHeight];
        } 
        _fire = new bool[mapWidth][];
        for (int i = 0; i < _fire.Length; i++)
        {
            _fire[i] = new bool[mapHeight];
        }
        float scale7 = (float)(mapWidth + mapHeight) / 10;
        int of = _random.Next(-1000, 1000);
        int of1 = _random.Next(-1000, 1000);
        int of2 = _random.Next(-1000, 1000);
        int of21 = _random.Next(-1000, 1000);
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float sampleX = x / scale7;
                float sampleY = y / scale7;
                
                _forest[x][y] = Mathf.PerlinNoise(sampleX + of, sampleY +  of1) < 0.45;
                
                
                //float temp = Mathf.PerlinNoise(sampleX + of, sampleY + of1);
                //temp *=  math.pow(math.clamp(100 / (math.sqrt(math.pow(x - 150, 2) + math.pow(y - 150, 2))),0,1),2);
                //tileMap.SetTile(new Vector3Int(x + mapWidth, y, 0), tiles[ temp > 1.5? 4 : temp>1? 2 : temp > 0.5 ? 0 : 1]);
                
                _fire[x][y] = Mathf.PerlinNoise(sampleX + of2, sampleY +  of21) < 0.3;
                //tileMap.SetTile(new Vector3Int(x + mapWidth, y, 0), tiles[_forest[x][y]? _fire[x][y]? 2 : 1 : 0]);
            }
        }
    }

    public static (int, int) FindSpawn()
    {
        _random1 = new Random((int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond % 10000000));
        int a;
        int b;
        for (int i = 0; i < 1000; i++)
        {
            a = _random1.Next(0,300);
            b = _random1.Next(0,300);

            if (_gridHandler[a, b] == Grid.Floor && (a < 100 || a > 200 || b < 100 || b > 200))
            {
                return (a, b);
            }
        }

        return (150, 150);
        //return (-1,-1);
    }
}
