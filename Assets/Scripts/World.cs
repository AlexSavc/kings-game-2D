using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public Board myBoard;
    public BoardSO board;
    public Movement Move;

    public Board ActiveBoard;

    //public Board BattleBoard;
    //public BoardSO BattleBoardSO;

    public bool ZoomedIn = false;
    void Awake()
    {
        myBoard = gameObject.GetComponentInChildren<Board>();
        board = myBoard.board;
        Move = myBoard.Move;
        ActiveBoard = myBoard;
        myBoard.BuildBoard(myBoard);
        //BattleBoard = gameObject.AddComponent<Board>();
        Move.board = board;
        //BattleBoardSO.BoardSize = myBoard.SubSize;
        //BattleBoard.ZoomedInBoard = true;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) Move.Select(Move.ScreenMouseRay());

        if (Input.GetMouseButtonDown(1))
        {
            int view = ActiveBoard.board.currentView + 1;
            if (view == 8) { view = 0; }
            ActiveBoard.board.currentView = view;
            Build.build(ActiveBoard.board.BoardSize, view, ActiveBoard);
        }
        /*if (Camera.main.orthographicSize < 0.35f && ZoomedIn == false)
        {
            //myBoard.ZoomedInBoard = true;
            ZoomedIn = true;
            ZoomIn();
        }
        if (Camera.main.orthographicSize > 6f && ZoomedIn == true)
        {
            //myBoard.ZoomedInBoard = false;
            ZoomedIn = false;
            ZoomOut();
        }*/
    }

    /*void ZoomIn()
    {
        {
            Tile ZoomTile = null;
            if (Move.SelectedTiles.Count == 1) ZoomTile = Move.SelectedTiles[0];
            else if (Build.CenterTile() != null)
            {
                GameObject CenterTile = Build.CenterTile().transform.root.gameObject;
                //Debug.Log(CenterTile.gameObject.);
                int[] Coord = TileOps.GetCoordFromVector(CenterTile.transform.position, ActiveBoard);
                ZoomTile = board.Tiles[Coord[0], Coord[1]];
            }
            Build.Clear(myBoard);
            BattleBoard.SetVariables(BattleBoardSO);
            Build.buildSubTiles(myBoard.SubSize, board.currentView, BattleBoard, ZoomTile);
            Camera.main.orthographicSize = 5;
            BattleBoardSO.Tiles = ZoomTile.SubTiles;
            BattleBoardSO.TilePos = ZoomTile.subPos;
            ActiveBoard = BattleBoard;
            Move.ActiveBoard = BattleBoard;
        }
    }

    void ZoomOut()
    {
        {
            Build.Clear(BattleBoard);
            Build.build(myBoard.Wd, board.currentView, myBoard);
            Camera.main.orthographicSize = 0.4f;
            ActiveBoard = myBoard;
            Move.ActiveBoard = myBoard;
        }
    }*/
}

public class Player
{
    public int PlayerColor { get; set; }
    public List<Piece> Generals { get; set; }
    public Structure Capital { get; set; }
    public int Gold { get; set; }
}

/*public class General
{
    public int Xcoord { get; set; }
    public int Ycoord { get; set; }
    public string PieceType { get; set; }
    public GameObject PieceObj { get; set; }
    public int Facing { get; set; }
    public int PlayerColor { get; set; }
    public List<Piece> Retinue { get; set; }
    public int DailyRations { get; set; }
    public int TotalRations { get; set; }
}*/

/*public class WorldTile
{
    public Tile tile { get; set; }
    public int Xcoord { get; set; }
    public int Ycoord { get; set; }
    public int TileType { get; set; }
    public GameObject TileObj { get; set; }
    public General Occupied { get; set; }
    public int TrackerType { get; set; }
    public GameObject SelectionTracker { get; set; }
    public int PlayerColor { get; set; }
    public int Production { get; set; }
}*/

public class Structure
{
    public int Xcoord { get; set; }
    public int Ycoord { get; set; }
    public string StructType { get; set; }
    public GameObject StructObj { get; set; }
    public Tile[] Territories { get; set; }
    public int DailyTaxes { get; set; }
    public int PlayerColor { get; set; }
}
