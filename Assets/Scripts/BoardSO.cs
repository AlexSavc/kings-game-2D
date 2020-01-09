using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BoardSO : ScriptableObject
{
    public int BoardSize;
    public int currentView = 1;
    [SerializeField]
    public static int boardSize;

    void OnEnable()
    {
        boardSize = BoardSize;
        Tiles = new Tile[boardSize, boardSize];
        Pieces = new Piece[boardSize, boardSize];
        TilePos = new float[boardSize, boardSize, 2, 2];
        TileCoord = new float[1, 1, 2];
    }

    public Tile[,] Tiles;
    public Piece[,] Pieces;
    public float[,,,] TilePos;
    public float[,,] TileCoord;

    public GameObject selectedObject;
    public Material[] PlayerColors = new Material[4];

    //terrain
    public GameObject[] DefaultGrassTiles = new GameObject[8];

    //selection
    public GameObject[] SelectionTracker = new GameObject[2];
    public GameObject[] MoveTracker = new GameObject[2];
    public GameObject[] AttackTracker = new GameObject[2];

    //pieces
    public GameObject[] King = new GameObject[8];
    public GameObject[] Queen = new GameObject[8];
    public GameObject[] Bishop = new GameObject[8];
    public GameObject[] Knight = new GameObject[8];
    public GameObject[] Rook = new GameObject[8];
    public GameObject[] Pawn = new GameObject[8];

    //structures
    public GameObject[] City = new GameObject[2];
    public GameObject[] Forest = new GameObject[2];
    public GameObject[] Tree = new GameObject[2];
    public GameObject[] Field = new GameObject[2];
    public GameObject[] Rocks = new GameObject[2];
    public GameObject[] Mountains = new GameObject[2];
}
