using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour 
{
    public BoardSO board;
    public Movement Move;
    public float offsetZ = 1f;
    /*public Tile[,] Tiles;
    public Piece[,] Pieces;
    public float[,,,] TilePos;
    public float[,,] TileCoord;*/

    public int view;
    Vector3 Origin;
    public int Wd;
    public int SubSize = 16;

    public float Xoffset = 0.5f; // 0.5 for 55 degree ISO
    public float offsetHeight = 0.35f; // 0.3
    public float height = 0.57f; // 0.43
    public float length = 0.795f;// 0.79

    public bool ZoomedInBoard;

    void Awake()
    {
        if (board != null) SetVariables(board);
    }

    public void SetVariables(BoardSO newboard)
    {
        board = newboard;
        Move = gameObject.GetComponentInChildren<Movement>();
        Move.ActiveBoard = this;
        /*Tiles = new Tile[newboard.BoardSize, newboard.BoardSize];
        Pieces = new Piece[newboard.BoardSize, newboard.BoardSize];
        TilePos = new float[newboard.BoardSize, newboard.BoardSize, 2, 2];
        TileCoord = new float[1, 1, 2];*/
        Wd = newboard.BoardSize;
        
    }

    public void BuildBoard(Board MyBoard) 
    {
        if (board == null) Debug.Log("THE BoardSO SCRIPT IS MISSING YOU RETARD");
        Build.Create(board.BoardSize, MyBoard);
        SpawnPieces();
        Build.build(board.BoardSize, board.currentView, MyBoard);
        view = board.currentView;
    }

    public void SpawnPieces()
    {
        Spawn.SpawnPiece(3, 0, 0, "Queen", this);
        Spawn.SpawnPiece(4, 7, 1, "Queen", this);

        Spawn.SpawnPiece(4, 0, 0, "King", this);
        Spawn.SpawnPiece(3, 7, 1, "King", this);

        Spawn.SpawnPiece(2, 0, 0, "Bishop", this);
        Spawn.SpawnPiece(5, 0, 0, "Bishop", this);
        Spawn.SpawnPiece(2, 7, 1, "Bishop", this);
        Spawn.SpawnPiece(5, 7, 1, "Bishop", this);

        Spawn.SpawnPiece(0, 0, 0, "Rook", this);
        Spawn.SpawnPiece(7, 0, 0, "Rook", this);
        Spawn.SpawnPiece(0, 7, 1, "Rook", this);
        Spawn.SpawnPiece(7, 7, 1, "Rook", this);

        Spawn.SpawnPiece(1, 0, 0, "Knight", this);
        Spawn.SpawnPiece(6, 0, 0, "Knight", this);
        Spawn.SpawnPiece(1, 7, 1, "Knight", this);
        Spawn.SpawnPiece(6, 7, 1, "Knight", this);

        Spawn.SpawnPiece(0, 1, 0, "Pawn", this);
        Spawn.SpawnPiece(1, 1, 0, "Pawn", this);
        Spawn.SpawnPiece(2, 1, 0, "Pawn", this);
        Spawn.SpawnPiece(3, 1, 0, "Pawn", this);
        Spawn.SpawnPiece(4, 1, 0, "Pawn", this);
        Spawn.SpawnPiece(5, 1, 0, "Pawn", this);
        Spawn.SpawnPiece(6, 1, 0, "Pawn", this);
        Spawn.SpawnPiece(7, 1, 0, "Pawn", this);
        Spawn.SpawnPiece(0, 6, 1, "Pawn", this);
        Spawn.SpawnPiece(1, 6, 1, "Pawn", this);
        Spawn.SpawnPiece(2, 6, 1, "Pawn", this);
        Spawn.SpawnPiece(3, 6, 1, "Pawn", this);
        Spawn.SpawnPiece(4, 6, 1, "Pawn", this);
        Spawn.SpawnPiece(5, 6, 1, "Pawn", this);
        Spawn.SpawnPiece(6, 6, 1, "Pawn", this);
        Spawn.SpawnPiece(7, 6, 1, "Pawn", this); 
    }

    
}

public class Build : MonoBehaviour
{
    public static void Create(int worldSize, Board MyBoard)
    {
        BoardSO board = MyBoard.board;
        int subSize = MyBoard.SubSize;
        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                //Tile tile = TileDataAsset.CreateTileAsset();
                Tile tile = ScriptableObject.CreateInstance<Tile>();
                MyBoard.board.Tiles[x, y] = tile;
                tile.name = "Tile " + x + ", " + y;

                if (MyBoard.ZoomedInBoard == false)
                {
                    if (x == 3 && y == x) { Spawn.SpawnStructure(x, y, 0, "City", MyBoard); }
                }

                /*tile.SubTiles = new Tile[subSize, subSize];
                tile.subPos = new float[subSize, subSize, 2, 2];*/
                tile.Xcoord = x;
                tile.Ycoord = y;
                float alt = NoiseFunction(x, y, 0.9f);
                tile.Altitude = alt;
                //LayoutStructures(tile, MyBoard, false);
                
                /*for (int Sx = 0; Sx < subSize; Sx++)
                {
                    for (int Sy = 0; Sy < subSize; Sy++)
                    {
                        Tile tempsub = ScriptableObject.CreateInstance<Tile>();
                        tile.SubTiles[Sx, Sy] = tempsub;
                        tempsub.name = tile.name + ", square " + Sx + ", " + Sy;
                        tempsub.Parent = tile;
                        tempsub.SubX = Sx;
                        tempsub.Xcoord = x;
                        tempsub.SubY = Sy;
                        tempsub.Ycoord = y;
                        tempsub.SubAltitude = Mathf.PerlinNoise(Sx * tile.Xcoord * 0.9f, Sy * tile.Ycoord * 0.9f);
                        tile.SubAltitude = tempsub.SubAltitude;
                        LayoutStructures(tempsub, MyBoard, true);
                    }
                }*/
            }
        }
    }

    /*public static void buildSubTiles(int SQsize, int view, Board MyBoard, Tile tile)
    {
        BoardSO board = MyBoard.board;
        Movement Move = MyBoard.Move;
        {
            for (int x = 0; x < SQsize; x++)
            {
                for (int y = 0; y < SQsize; y++)
                {
                    Tile TempSquare = tile.SubTiles[x, y];
                    Vector3 pos = BuildSecond(x, y, board.BoardSize, view, MyBoard);
                    
                    pos.y += TempSquare.SubAltitude/3;
                    GameObject tileObj = Instantiate(board.DefaultGrassTiles[view], pos, Quaternion.identity); // Create new
                    tileObj.name = "tileObj " + x + ", " + y;

                    if (TempSquare.TileObj != null) Destroy(TempSquare.TileObj); // Destroy Previous
                    TempSquare.TileObj = tileObj;
                    //Debug.Log("TempTile " + TempTile.name + ", tileObj " + x + ", " + y);
                    tile.SubTiles[x, y] = TempSquare;
                    int View = board.currentView;

                    float[,,,] TilePos = tile.subPos;

                    TilePos[x, 0, 0, 0] = x;
                    TilePos[x, y, 0, 0] = y;
                    TilePos[x, y, 1, 0] = pos.x;
                    TilePos[x, y, 0, 1] = pos.y;
                    TilePos[x, y, 1, 1] = pos.z;
                    
                    if (TempSquare.Occupied != null)
                    {
                        Piece tempPiece = TempSquare.Occupied;
                        GameObject[] PieceToSpawn = new GameObject[8];

                        if (tempPiece.PieceType == "Pawn") { PieceToSpawn = board.Pawn; }
                        if (tempPiece.PieceType == "Queen") { PieceToSpawn = board.Queen; }
                        if (tempPiece.PieceType == "Bishop") { PieceToSpawn = board.Bishop; }
                        if (tempPiece.PieceType == "Rook") { PieceToSpawn = board.Rook; }
                        if (tempPiece.PieceType == "King") { PieceToSpawn = board.King; }
                        if (tempPiece.PieceType == "Knight") { PieceToSpawn = board.Knight; }

                        GameObject pieceToSpawn = PieceToSpawn[SetDirection(board.currentView, tempPiece.Facing)];
                        GameObject tempPieceObj = Instantiate(pieceToSpawn, pos, Quaternion.identity);

                        SetPieceColor(tempPieceObj, tempPiece.PlayerColor, MyBoard);

                        if (tempPiece != null) Destroy(tempPiece.PieceObj);
                        tempPiece.PieceObj = tempPieceObj;
                    }

                    if (TempSquare.TrackerType != 0)
                    {
                        int trackerType = TempSquare.TrackerType;
                        GameObject tracker = null;
                        
                        if (trackerType == 1) tracker = board.SelectionTracker[View % 2];
                        if (trackerType == 2) tracker = board.MoveTracker[View % 2];
                        if (trackerType == 3) tracker = board.AttackTracker[View % 2];

                        if (TempSquare.SelectionTracker != null) Destroy(TempSquare.SelectionTracker);
                        GameObject tempTracker = Instantiate(tracker, pos, Quaternion.identity);
                        TempSquare.SelectionTracker = tempTracker;
                        tempTracker.name = "SelectionTracker " + x + ", " + y;
                        Move.SelectedObject = TempSquare.TileObj;
                    }

                    if (TempSquare.Structure != null)
                    {
                        Structure Structure = TempSquare.Structure;
                        string StructureType = Structure.StructType;
                        GameObject structure = null;
                        if (StructureType == "City") structure = board.City[View % 2];
                        if (StructureType == "Forest") structure = board.Forest[View % 2];
                        if (StructureType == "Field") structure = board.Field[View % 2];
                        if (TempSquare.StructureObj != null) Destroy(TempSquare.StructureObj);
                        GameObject tempStructure = Instantiate(structure, pos, Quaternion.identity);
                        TempSquare.StructureObj = tempStructure;
                        tempStructure.name = StructureType + " " + x + ", " + y;
                    }
                }
            }
        }
    }*/

    public static void build(int worldsize, int view, Board MyBoard)
    {
        BoardSO board = MyBoard.board;
        Movement Move = MyBoard.Move;
        if (Move.SelectedObject != null) Move.ClearSelection();
        //if (MyBoard.battle == true) { buildSquares(worldsize, view, MyBoard); }
        for (int x = 0; x < worldsize; x++)
        {
            for (int y = 0; y < worldsize; y++)
            {
                Tile TempTile = board.Tiles[x, y];
                Vector3 pos = BuildSecond(x, y, board.BoardSize, view, MyBoard);
                //TempTile.Altitude = Mathf.PerlinNoise(x / 3.2f, y / 3.2f);
                

                pos.y += TempTile.Altitude;//TempTile.Altitude/2;
                
                GameObject tileObj = Instantiate(board.DefaultGrassTiles[view], pos, Quaternion.identity); // Create new
                tileObj.name = "tileObj " + x + ", " + y;

                if (TempTile.TileObj != null) Destroy(TempTile.TileObj); // Destroy Previous
                TempTile.TileObj = tileObj;
                //Debug.Log("TempTile " + TempTile.name + ", tileObj " + x + ", " + y);
                board.Tiles[x, y] = TempTile;
                int View = board.currentView;

                float[,,,] TilePos = MyBoard.board.TilePos;

                TilePos[x, 0, 0, 0] = x;
                TilePos[x, y, 0, 0] = y;
                TilePos[x, y, 1, 0] = pos.x;
                TilePos[x, y, 0, 1] = pos.y;
                TilePos[x, y, 1, 1] = pos.z;

                

                if (TempTile.Occupied != null)
                {
                    Piece tempPiece = TempTile.Occupied;
                    GameObject[] PieceToSpawn = new GameObject[8];

                    if (tempPiece.PieceType == "Pawn") { PieceToSpawn = board.Pawn; }
                    if (tempPiece.PieceType == "Queen") { PieceToSpawn = board.Queen; }
                    if (tempPiece.PieceType == "Bishop") { PieceToSpawn = board.Bishop; }
                    if (tempPiece.PieceType == "Rook") { PieceToSpawn = board.Rook; }
                    if (tempPiece.PieceType == "King") { PieceToSpawn = board.King; }
                    if (tempPiece.PieceType == "Knight") { PieceToSpawn = board.Knight; }

                    GameObject pieceToSpawn = PieceToSpawn[SetDirection(board.currentView, tempPiece.Facing)];
                    GameObject tempPieceObj = Instantiate(pieceToSpawn, pos, Quaternion.identity);

                    SetPieceColor(tempPieceObj, tempPiece.PlayerColor, MyBoard);

                    if (tempPiece != null) Destroy(tempPiece.PieceObj);
                    tempPiece.PieceObj = tempPieceObj;

                    //Debug.Log(TempTile.Occupied.PieceObj.transform.position);
                }

                if (TempTile.TrackerType != 0)
                {
                    int trackerType = TempTile.TrackerType;
                    GameObject tracker = null;
                    
                    
                    if (trackerType == 1) tracker = board.SelectionTracker[View % 2];
                    if (trackerType == 2) tracker = board.MoveTracker[View % 2];
                    if (trackerType == 3) tracker = board.AttackTracker[View % 2];

                    if (TempTile.SelectionTracker != null) Destroy(TempTile.SelectionTracker);
                    GameObject tempTracker = Instantiate(tracker, pos, Quaternion.identity);
                    TempTile.SelectionTracker = tempTracker;
                    tempTracker.name = "SelectionTracker " + x + ", " + y;
                    Move.SelectedObject = TempTile.TileObj;
                }

                if(TempTile.Structure != null)
                {
                    Structure Structure = TempTile.Structure;
                    string StructureType = Structure.StructType;
                    GameObject structure = null;
                    if (StructureType == "City") structure = board.City[View % 2];
                    if (StructureType == "Forest") structure = board.Forest[View % 2];
                    if (StructureType == "Field") structure = board.Field[View % 2];
                    if (StructureType == "Mountain") structure = board.Mountains[View % 2];
                    if (StructureType == "Rocks") structure = board.Rocks[View % 2];
                    if (TempTile.StructureObj != null) Destroy(TempTile.StructureObj);
                    GameObject tempStructure = Instantiate(structure, pos, Quaternion.identity);
                    TempTile.StructureObj = tempStructure;
                    tempStructure.name = StructureType + " " + x + ", " + y;
                }
            }
        }
    }

    public static void BuildBiome(int X, int Y)
    {
        float Altitude = Mathf.PerlinNoise(X/3.2f, Y/3.2f);
    }

    public static void LayoutStructures(Tile tile, Board MyBoard, bool ZoomedIn)
    {
        int X = tile.Xcoord;
        int Y = tile.Ycoord;
        int x = tile.SubX;
        int y = tile.SubY;


        if (ZoomedIn == false)
        {
            //Debug.Log(tile.Altitude);
            float alt = tile.Altitude;
            if (alt < 0.4f) // field
            {
                //nothing
            }
            else if (alt < 0.6f) // forest
            {
                Spawn.SpawnStructure(X, Y, 1, "Forest", MyBoard);
            }
            else //if (alt > 1.5f) // Mountain
            {
                Spawn.SpawnStructure(X, Y, 1, "Mountain", MyBoard);
            }
        }
        else //if (tile.Parent != null)
        {
            //Tile P = tile.Parent;
            //string BuildWhat = tile.Parent.Structure.StructType;
            string BuildWhat = "Mountain";
            if (BuildWhat == "Mountain")
            {
                float a = Random.Range(0, 1);
                if (a < 0.2f) Spawn.SpawnSubStructure(x, y, 1, "Field",tile, MyBoard);
            }

            if (BuildWhat == "Forest")
            {
                float a = Random.Range(0, 1);
                if (a < 0.75f) Spawn.SpawnSubStructure(x, y, 1, "Forest", tile, MyBoard);
            }

            if (BuildWhat == "Field")
            {
                float a = Random.Range(0, 1);
                if (a < 0.5f) Spawn.SpawnSubStructure(x, y, 1, "Field", tile, MyBoard);
            }
        }
        //for (int i = 0; i < 15; i++)
        {
            //int x = Random.Range(0, Size);
            //int y = Random.Range(0, Size);
            //if (x >= y) Spawn.SpawnStructure(x, y, 1, "Forest", MyBoard);
            //else Spawn.SpawnStructure(x, y, 1, "Field", MyBoard);
        }
    }

    public static Vector3 BuildSecond(int x, int y, int worldSize, int view, Board MyBoard)
    {
        float MiddlePoint = worldSize / 2;
        float Wd = worldSize;
        Vector3 pos;
        Vector3 Origin;
        Vector3 BoardPos = MyBoard.transform.position;
        float Xoffset = MyBoard.Xoffset;
        float offsetHeight = MyBoard.offsetHeight;
        float length = MyBoard.length;
        float height = MyBoard.height;
        float Xpos;
        float Ypos;
        float OffsetZ = MyBoard.offsetZ;

        if (view == 0 || view == 1)
        {
            if (view % 2 != 0) //1
            {
                if (Wd % 2 == 0) Origin = new Vector3(BoardPos.x - Wd * Xoffset + Xoffset, BoardPos.y, BoardPos.z);
                else Origin = new Vector3(BoardPos.x - Wd * Xoffset + Xoffset, BoardPos.y, BoardPos.z);
                Xpos = Origin.x + Xoffset * x + Xoffset * y;
                Ypos = Origin.y - offsetHeight * x + offsetHeight * y;
                pos = new Vector3(Xpos, Ypos, y * OffsetZ - x * OffsetZ);
            }

            else //0
            {
                if (Wd % 2 == 0) Origin = new Vector3(BoardPos.x - MiddlePoint * length + length / 2, BoardPos.y - MiddlePoint * height + height / 2, BoardPos.z);
                else Origin = new Vector3(BoardPos.x - MiddlePoint * length, BoardPos.y - MiddlePoint * height, BoardPos.z);
                Xpos = Origin.x + x * length;
                Ypos = Origin.y + y * height;
                pos = new Vector3(Xpos, Ypos, y * OffsetZ);
            }
        }
        else if (view == 2 || view == 3)
        {
            if (view % 2 != 0) //3
            {
                if (Wd % 2 == 0) Origin = new Vector3(BoardPos.x - Wd * offsetHeight + offsetHeight, BoardPos.y, BoardPos.z);
                else Origin = new Vector3(BoardPos.x - Wd * offsetHeight + offsetHeight, BoardPos.y, BoardPos.z);
                Xpos = Origin.x + offsetHeight * x + offsetHeight * y;
                Ypos = Origin.y + Xoffset * x - Xoffset * y;
                pos = new Vector3(-Ypos, -Xpos, x * -OffsetZ - y * OffsetZ);
            }
            //Ctrlz Marker
            else //2
            {
                if (Wd % 2 == 0) Origin = new Vector3(BoardPos.x + MiddlePoint * height - height / 2, BoardPos.y - MiddlePoint * length + length / 2, BoardPos.z);
                else Origin = new Vector3(BoardPos.x + MiddlePoint * height, BoardPos.y - MiddlePoint * length, BoardPos.z);
                Xpos = Origin.x - (x * height);
                Ypos = Origin.y + (y * length);

                pos = new Vector3(Ypos, Xpos, x * -OffsetZ);
            }
        }
        else if (view == 4 || view == 5)
        {
            if (view % 2 != 0) //5
            {
                if (Wd % 2 == 0) Origin = new Vector3(BoardPos.x - Wd * Xoffset + Xoffset, BoardPos.y, BoardPos.z);
                else Origin = new Vector3(BoardPos.x - Wd * Xoffset + Xoffset, BoardPos.y, BoardPos.z);
                Xpos = Origin.x + Xoffset * x + Xoffset * y;
                Ypos = Origin.y - offsetHeight * x + offsetHeight * y;
                pos = new Vector3(-Xpos, -Ypos, x * OffsetZ - y * OffsetZ);
            }

            else //4
            {
                if (Wd % 2 == 0) Origin = new Vector3(BoardPos.x + MiddlePoint * length - length / 2, BoardPos.y + MiddlePoint * height - height / 2, BoardPos.z);
                else Origin = new Vector3(BoardPos.x + MiddlePoint * length, BoardPos.y + MiddlePoint * height, BoardPos.z);
                Xpos = Origin.x - x * length;
                Ypos = Origin.y - y * height;
                pos = new Vector3(Xpos, Ypos, y * -OffsetZ);
            }
        }
        else //(view == 6 || view == 7)
        {
            if (view % 2 != 0) //7
            {
                if (Wd % 2 == 0) Origin = new Vector3(BoardPos.x - Wd * offsetHeight + offsetHeight / 2, BoardPos.y, BoardPos.z);
                else Origin = new Vector3(BoardPos.x - Wd * offsetHeight + offsetHeight, BoardPos.y, BoardPos.z);

                Xpos = Origin.x + offsetHeight * x + offsetHeight * y;
                Ypos = Origin.y + Xoffset * x - Xoffset * y;
                pos = new Vector3(Ypos, Xpos, x * OffsetZ + y * OffsetZ);
            }

            else //6
            {
                if (Wd % 2 == 0) Origin = new Vector3(BoardPos.x - MiddlePoint * height + height / 2, BoardPos.y + MiddlePoint * length - length / 2, BoardPos.z);
                else Origin = new Vector3(BoardPos.x - MiddlePoint * height, BoardPos.y + MiddlePoint * length, BoardPos.z);
                Xpos = Origin.x + x * height;
                Ypos = Origin.y - y * length;
                pos = new Vector3(Ypos, Xpos, x * OffsetZ);
            }
        } //(view == 6 || view == 7)

        return pos;
    }

    public static int SetDirection(int CurrentView, int Facing)
    {
        int i = (CurrentView + Facing) % 8;
        return i;
    }

    public static void SetPieceColor(GameObject Piece, int PlayerColor, Board MyBoard)
    {
        Renderer[] rends = Piece.GetComponentsInChildren<SpriteRenderer>();
        rends[0].material.color = MyBoard.board.PlayerColors[PlayerColor].color;
    }

    public static GameObject CenterTile()
    {
        Collider2D collider = Physics2D.OverlapPoint(Camera.main.transform.position);
        if (collider == null) return null;
        return collider.gameObject;
    }

    public static void Clear(Board MyBoard)
    {
        Movement Move = MyBoard.Move;
        if (Move.SelectedObject != null) Move.ClearSelection();
        int worldSize = MyBoard.Wd;

        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                Tile TempTile = MyBoard.board.Tiles[x, y];
                Piece tempPiece = null;
                if (MyBoard.ZoomedInBoard != true) { tempPiece = MyBoard.board.Pieces[x, y]; }
                Structure tempStruct = TempTile.Structure;
                if (TempTile.TileObj != null) Destroy(TempTile.TileObj); // Destroy Previous
                if(TempTile.Structure != null) Destroy(TempTile.StructureObj);
                if (tempPiece != null) Destroy(tempPiece.PieceObj);
            }
        }
    }

    public static float NoiseFunction(int X, int Y, float Amplitude)
    {
        float alt = 0;
        alt = Mathf.PerlinNoise(X * Amplitude, Y * Amplitude);
        alt = Mathf.PerlinNoise(alt * Amplitude/2, alt * Amplitude/2);
        return alt;
    }
}

public class Spawn
{

    public static void SpawnPiece(int Xcoord, int Ycoord, int PlayerColor, string PieceType, Board MyBoard)
    {
        Piece piece = ScriptableObject.CreateInstance<Piece>();
        MyBoard.board.Pieces[Xcoord, Ycoord] = piece;
        piece.PieceType = PieceType;
        piece.PlayerColor = PlayerColor;
        MyBoard.board.Tiles[Xcoord, Ycoord].Occupied = piece;
        //TEMPORARY 
        if (PlayerColor == 1 || PlayerColor == 3) { piece.Facing = 4; }
    }

    public static void SpawnStructure(int Xcoord, int Ycoord, int PlayerColor, string StructureType, Board MyBoard)
    {
        Structure structure = new Structure();
        MyBoard.board.Tiles[Xcoord, Ycoord].Structure = structure;
        structure.StructType = StructureType;
        structure.PlayerColor = PlayerColor;
    }

    public static void SpawnSubStructure(int Xcoord, int Ycoord, int PlayerColor, string StructureType,Tile tile, Board MyBoard)
    {
        Structure structure = new Structure();
        tile.Structure = structure;
        structure.StructType = StructureType;
        structure.PlayerColor = PlayerColor;
    }
}

public class Tile : ScriptableObject
{
    public int Xcoord { get; set; }
    public int Ycoord { get; set; }
    public float Altitude { get; set; }

    public int TileType { get; set; }
    public GameObject TileObj { get; set; }

    public Piece Occupied { get; set; }
    public Tile Landlord { get; set; }

    public int TrackerType { get; set; }
    public GameObject SelectionTracker { get; set;}

    public Structure Structure { get; set; }
    public GameObject StructureObj { get; set; }

    public int PlayerColor { get; set; }
    public int Production { get; set; }

    public Tile Parent;
    public string SubType;
    public Tile[,] SubTiles;
    public float[,,,] subPos;
    public int SubX { get; set; }
    public int SubY { get; set; }
    public float SubAltitude { get; set; }
}

public class Piece : ScriptableObject
{
    public int Xcoord { get; set; }
    public int Ycoord { get; set; }
    public string PieceType { get; set; }
    public GameObject PieceObj { get; set; }
    public int Facing { get; set; }
    public int PlayerColor { get; set; }
    public Piece General;
}

//public class General : Piece
//{
   // public int Range { get; set; }
    //public List<Piece> Retinue { get; set; }
    //public int DailyRations { get; set; }
   // public int TotalRations { get; set; }
//}

public class TileOps
{

    public static int[] GetCoordFromVector(Vector3 pos, Board MyBoard)
    {
        int Size = MyBoard.Wd;
        if (MyBoard.ZoomedInBoard == true) Size = MyBoard.SubSize;
        int [] Coord = new int[2];
        for (int x = 0; x <= Size - 1; x++)
        {
            for (int y = 0; y <= Size - 1; y++)
            {
                float j = pos.x;
                float b = MyBoard.board.TilePos[x, y, 1, 0];

                float i = pos.y;
                float v = MyBoard.board.TilePos[x, y, 0, 1];

                if (j == b && i == v)
                {
                    Coord[0] = x;
                    Coord[1] = y;
                }
            }
        }
        return Coord;
    }

}

