using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;
using UnityEngine.Tilemaps;




public class Tree : MonoBehaviourPunCallbacks
{
    public (int,int) Pos{ get; set; }
    public (int,int) Size{ get; set; }
    [CanBeNull] public Tree Left{ get; set; }
    [CanBeNull] public Tree Right{ get; set; }
    
    public bool[][] Roadds { get; set; }

    public Tree(int x, int y, int sizex, int sizey)
    {
        Pos = (x,y);
        Size = (sizex,sizey);
        Left = null;
        Right = null;

    }

    public void couper(bool V, int loop, Tilemap tilemap, Random rand, Tile tile, WalkerGenerator.Grid[,] grid, bool[][] Roads, int mapheight, int mapwidth)
    {
        if (loop <= 0) return;

        if (V)
        {
            int t = (int)(Size.Item1 * 0.25);
            int i = rand.Next(t, Size.Item1 - t);
            Left = new Tree(Pos.Item1, Pos.Item2, i, Size.Item2);
            Right = new Tree(Pos.Item1 + i, Pos.Item2, Size.Item1 - i, Size.Item2);

            for (int j = 0; j < Size.Item2; j++)
            {
                if (grid[Pos.Item1 + i, Pos.Item2 + j] == WalkerGenerator.Grid.Floor ||
                    grid[Pos.Item1 + i, Pos.Item2 + j] == WalkerGenerator.Grid.Floor2)
                {
                    //tilemap.SetTile(new Vector3Int(i + Pos.Item1, Pos.Item2 + j, 0), tile);
                    Roads[i + Pos.Item1 - 100 + 1][j + Pos.Item2 - 100] = true;
                    Roads[i + Pos.Item1 - 100][j + Pos.Item2 - 100] = true;
                    Roads[i + Pos.Item1 - 100 - 1][j + Pos.Item2 - 100] = true;
                }
            }
            Left.couper(false, loop - 1, tilemap, rand, tile, grid, Roads, mapheight, mapwidth);
            Right.couper(false, loop - 1, tilemap, rand, tile, grid, Roads, mapheight, mapwidth);
        }
        else
        {
            int t = (int)(Size.Item2 * 0.25);
            int i = rand.Next(t, Size.Item2 - t);
            Left = new Tree(Pos.Item1, Pos.Item2, Size.Item1, i);
            Right = new Tree(Pos.Item1, Pos.Item2 + i, Size.Item1, Size.Item2 - i);

            for (int j = 0; j < Size.Item1; j++)
            {
                if (grid[Pos.Item1 + j, Pos.Item2 + i] == WalkerGenerator.Grid.Floor ||
                    grid[Pos.Item1 + j, Pos.Item2 + i] == WalkerGenerator.Grid.Floor2)
                {
                    //tilemap.SetTile(new Vector3Int(Pos.Item1 + j, Pos.Item2 + i, 0), tile);
                    Roads[Pos.Item1 + j-100][Pos.Item2 + i-100 + 1] = true;
                    Roads[Pos.Item1 + j-100][Pos.Item2 + i-100] = true;
                    Roads[Pos.Item1 + j-100][Pos.Item2 + i-100 - 1] = true;
                }
            }

            Left.couper(true, loop - 1, tilemap, rand, tile, grid, Roads, mapheight, mapwidth);
            Right.couper(true, loop - 1, tilemap, rand, tile, grid, Roads, mapheight, mapwidth);
        }


    }
    



    
    



}
