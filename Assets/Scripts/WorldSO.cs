using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSO : ScriptableObject
{
    public int PlayerCount;
    public List<Player> Players { get; set; }
    public int WorldSize;
    public Piece[,] WorldPieces;
    public Tile[,] WorldTiles;
}
