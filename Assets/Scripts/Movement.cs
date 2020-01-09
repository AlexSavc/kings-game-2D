using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public BoardSO board;
    public Board ActiveBoard;

    public GameObject SelectedObject;
    public List<Tile> SelectedTiles =new List<Tile>();

    public GameObject ScreenMouseRay()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 5f;

        Vector2 v = Camera.main.ScreenToWorldPoint(mousePos);

        Collider2D collider = Physics2D.OverlapPoint(v);
        if (collider == null) return null;
        return collider.gameObject;
    }

    public void Select(GameObject obj)
    {
        if (obj == null) return;
        if(SelectedObject == obj.transform.root.gameObject)
        {
            ClearSelection();
            return;
        }
        if (SelectedObject != null)
        {
            int[] ObCoord = TileOps.GetCoordFromVector(obj.transform.root.gameObject.transform.position,ActiveBoard);
            Tile objTile = ActiveBoard.board.Tiles[ObCoord[0], ObCoord[1]];
            int[] selCoord = TileOps.GetCoordFromVector(SelectedObject.transform.position,ActiveBoard);
            Tile selectedTile = ActiveBoard.board.Tiles[selCoord[0], selCoord[1]];
            if (objTile.TrackerType == 2 || objTile.TrackerType == 3)
            {
                MovePiece(ObCoord, objTile, selectedTile);
                return;
            }
            else ClearSelection();
        }
        SelectedObject = obj.transform.root.gameObject;
        int[]Coord = TileOps.GetCoordFromVector(SelectedObject.transform.position,ActiveBoard);
        Vector3 pos = SelectedObject.transform.position;
        
        FindPossibleMoves(Coord[0], Coord[1]);
    }

    private void MovePiece(int[] VictimCoord, Tile Victim, Tile Attacker)
    {
        BoardSO myboard = ActiveBoard.board;
        Attacker.Occupied.PieceObj.transform.position = Victim.TileObj.transform.position;
        int[] AttackerCoord = TileOps.GetCoordFromVector(Attacker.TileObj.transform.position, ActiveBoard);
        if (Victim.Occupied != null) { Destroy(Victim.Occupied.PieceObj); }
        Victim.Occupied = Attacker.Occupied;
        myboard.Pieces[VictimCoord[0], VictimCoord[1]] = Attacker.Occupied;
        myboard.Pieces[AttackerCoord[0], AttackerCoord[1]] = null;
        Attacker.Occupied = null;

        int Facing = myboard.Pieces[VictimCoord[0], VictimCoord[1]].Facing;

        int X1 = AttackerCoord[0];
        int Y1 = AttackerCoord[1];
        int X2 = VictimCoord[0];
        int Y2 = VictimCoord[1];

        if (Y2 > Y1 && X2 == X1) Facing = 0; // Moved North
        if (Y2 < Y1 && X2 == X1) Facing = 4; // Moved South
        if (Y2 == Y1 && X2 > X1) Facing = 2; // Moved East
        if (Y2 == Y1 && X2 < X1) Facing = 6; // Moved West
        if (Y2 > Y1 && X2 > X1) Facing = 1; // Moved North East
        if (Y2 > Y1 && X2 < X1) Facing = 7; // Moved North West
        if (Y2 < Y1 && X2 > X1) Facing = 3; // Moved South East
        if (Y2 < Y1 && X2 < X1) Facing = 5; // Moved South West

        myboard.Pieces[VictimCoord[0], VictimCoord[1]].Facing = Facing;
        Destroy(Victim.Occupied.PieceObj);

        GameObject[] PieceToSpawn = new GameObject[8];
        if (Victim.Occupied.PieceType == "Pawn") { PieceToSpawn = board.Pawn; }
        if (Victim.Occupied.PieceType == "Queen") { PieceToSpawn = board.Queen; }
        if (Victim.Occupied.PieceType == "Bishop") { PieceToSpawn = board.Bishop; }
        if (Victim.Occupied.PieceType == "Rook") { PieceToSpawn = board.Rook; }
        if (Victim.Occupied.PieceType == "King") { PieceToSpawn = board.King; }
        if (Victim.Occupied.PieceType == "Knight") { PieceToSpawn = board.Knight; }

        GameObject tempPiece = Instantiate(PieceToSpawn[Build.SetDirection(board.currentView, Victim.Occupied.Facing)], Victim.TileObj.transform.position, Quaternion.identity);
        Victim.Occupied.PieceObj = tempPiece;
        Build.SetPieceColor(tempPiece, Victim.Occupied.PlayerColor, ActiveBoard);
        ClearSelection();
        return;
    }

    public GameObject SelectionTracker(int X, int Y, Vector3 pos, int TrackerType)
    {
        GameObject tracker = null;
        int Size = board.BoardSize;
        if (ActiveBoard.ZoomedInBoard == true) { Size = ActiveBoard.SubSize; }
        if (X < 0 || X > Size || Y < 0 || Y > Size) return tracker; // return null if coord is out of range

        //decide what selectionTracker to use
        if (TrackerType == 1) tracker = board.SelectionTracker[board.currentView % 2];
        if (TrackerType == 2) tracker = board.MoveTracker[board.currentView % 2];
        if (TrackerType == 3) tracker = board.AttackTracker[board.currentView % 2];

        // set all Tile parameters
        Tile tempTile = ActiveBoard.board.Tiles[X, Y];
        tempTile.TrackerType = TrackerType;
        tempTile.SelectionTracker = Instantiate(tracker, pos, Quaternion.identity);
        tempTile.SelectionTracker.name = "Tracker" + X + ", " + Y;
        SelectedTiles.Add(tempTile);

        return tracker;
    }

    public void ClearSelection()
    {
        foreach(Tile tile in SelectedTiles)
        {
            tile.TrackerType = 0;
            Destroy(tile.SelectionTracker);
        }
        SelectedTiles.Clear();
        SelectedObject = null;
    }

    public void FindPossibleMoves(int X, int Y)
    {
        BoardSO thisBoard = ActiveBoard.board;
        Tile Attacker = thisBoard.Tiles[X, Y];
        Tile Victim;
        SelectionTracker(X, Y, Attacker.TileObj.transform.position, 1);
        if (Attacker.Occupied != null)
        {
            string pieceType = Attacker.Occupied.PieceType;

            if (pieceType == "Pawn")
            {
                    // Moving

                    List<int[]> Moves = NorthTile(X, Y, 1);
                
                    foreach (int[] moves in Moves)
                    {
                        Victim = thisBoard.Tiles[X, Y + 1];
                        if (Victim.Occupied == null) // Move Only if empty
                            SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, 2);
                    }
                
                    Moves = EastTile(X, Y, 1);
                    foreach (int[] moves in Moves)
                    {
                        Victim = thisBoard.Tiles[X + 1, Y];
                        if (Victim.Occupied == null) // Move Only if empty
                            SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, 2);
                    }
                
                    Moves = WestTile(X, Y, 1);
                    foreach (int[] moves in Moves)
                    {
                        Victim = thisBoard.Tiles[X - 1, Y];
                        if (Victim.Occupied == null) // Move Only if empty
                            SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, 2);
                    }
                
                    Moves = SouthTile(X, Y, 1);
                    foreach (int[] moves in Moves)
                    {
                        Victim = thisBoard.Tiles[X, Y - 1];
                        if (Victim.Occupied == null) // Move Only if empty
                            SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, 2);
                    }
                

                    // Attacking
                
                
                    Moves = NorthEastTile(X, Y, 1);
                    foreach (int[] moves in Moves)
                    {
                        Victim = thisBoard.Tiles[X + 1, Y + 1];
                        if (Victim.Occupied != null && Victim.Occupied.PlayerColor != Attacker.Occupied.PlayerColor) // attack if enemy
                        {
                            SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]/* it should be "3" or attackTracker*/);
                        }
                    }
                
                    Moves = NorthWestTile(X, Y, 1);
                    foreach (int[] moves in Moves)
                    {
                        Victim = thisBoard.Tiles[X - 1, Y + 1];
                        if (Victim.Occupied != null && Victim.Occupied.PlayerColor != Attacker.Occupied.PlayerColor) // attack if enemy
                        {
                            SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]/* it should be "3" or attackTracker*/);
                        }
                    }
                
                    Moves = SouthEastTile(X, Y, 1);
                    foreach (int[] moves in Moves)
                    {
                        Victim = thisBoard.Tiles[X + 1, Y - 1];
                        if (Victim.Occupied != null && Victim.Occupied.PlayerColor != Attacker.Occupied.PlayerColor) // attack if enemy
                        {
                            SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]/* it should be "3" or attackTracker*/);
                        }
                    }

                    Moves = SouthWestTile(X, Y, 1);
                    foreach (int[] moves in Moves)
                    {
                        Victim = thisBoard.Tiles[X - 1, Y - 1];
                        if (Victim.Occupied != null && Victim.Occupied.PlayerColor != Attacker.Occupied.PlayerColor) // attack if enemy
                        {
                            SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]/* it should be "3" or attackTracker*/);
                        }
                    }
            }

            if (pieceType == "Queen")
            {
                int localRange = 7;
                List<int[]> Moves = NorthTile(X, Y, localRange);
                
                foreach (int[] moves in Moves)
                {
                        SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]);
                }

                Moves = EastTile(X, Y, localRange);
                foreach (int[] moves in Moves)
                {
                        SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]);
                }

                Moves = WestTile(X, Y, localRange);
                foreach (int[] moves in Moves)
                {
                        SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]);
                }

                Moves = SouthTile(X, Y, localRange);
                foreach (int[] moves in Moves)
                {
                        SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]);
                }

                Moves = NorthEastTile(X, Y, localRange);
                foreach (int[] moves in Moves)
                {
                        SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]/* it should be "3" or attackTracker*/);
                    
                }

                Moves = NorthWestTile(X, Y, localRange);
                foreach (int[] moves in Moves)
                {
                    {
                        SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]/* it should be "3" or attackTracker*/);
                    }
                }

                Moves = SouthEastTile(X, Y, localRange);
                foreach (int[] moves in Moves)
                {
                    {
                        SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]/* it should be "3" or attackTracker*/);
                    }
                }

                Moves = SouthWestTile(X, Y, localRange);
                foreach (int[] moves in Moves)
                {
                    {
                        SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]/* it should be "3" or attackTracker*/);
                    }
                }
            }

            if (pieceType == "Bishop")
            {
                int localRange = 7;
                List<int[]> Moves = NorthEastTile(X, Y, localRange);
                foreach (int[] moves in Moves)
                {
                    SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]/* it should be "3" or attackTracker*/);

                }

                Moves = NorthWestTile(X, Y, localRange);
                foreach (int[] moves in Moves)
                {
                    {
                        SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]/* it should be "3" or attackTracker*/);
                    }
                }

                Moves = SouthEastTile(X, Y, localRange);
                foreach (int[] moves in Moves)
                {
                    {
                        SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]/* it should be "3" or attackTracker*/);
                    }
                }

                Moves = SouthWestTile(X, Y, localRange);
                foreach (int[] moves in Moves)
                {
                    {
                        SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]/* it should be "3" or attackTracker*/);
                    }
                }
            }

            if (pieceType == "Rook")
            {
                int localRange = 7;
                List<int[]> Moves = NorthTile(X, Y, localRange);

                foreach (int[] moves in Moves)
                {
                    SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]);
                }

                Moves = EastTile(X, Y, localRange);
                foreach (int[] moves in Moves)
                {
                    SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]);
                }

                Moves = WestTile(X, Y, localRange);
                foreach (int[] moves in Moves)
                {
                    SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]);
                }

                Moves = SouthTile(X, Y, localRange);
                foreach (int[] moves in Moves)
                {
                    SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]);
                }
            }

            if (pieceType == "King")
            {
                int localRange = 1;
                List<int[]> Moves = NorthTile(X, Y, localRange);

                foreach (int[] moves in Moves)
                {
                    SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]);
                }

                Moves = EastTile(X, Y, localRange);
                foreach (int[] moves in Moves)
                {
                    SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]);
                }

                Moves = WestTile(X, Y, localRange);
                foreach (int[] moves in Moves)
                {
                    SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]);
                }

                Moves = SouthTile(X, Y, localRange);
                foreach (int[] moves in Moves)
                {
                    SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]);
                }

                Moves = NorthEastTile(X, Y, localRange);
                foreach (int[] moves in Moves)
                {
                    SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]);

                }

                Moves = NorthWestTile(X, Y, localRange);
                foreach (int[] moves in Moves)
                {
                    {
                        SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]);
                    }
                }

                Moves = SouthEastTile(X, Y, localRange);
                foreach (int[] moves in Moves)
                {
                    {
                        SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]);
                    }
                }

                Moves = SouthWestTile(X, Y, localRange);
                foreach (int[] moves in Moves)
                {
                    {
                        SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]);
                    }
                }
            }

            if (pieceType == "Knight")
            {
                List<int[]> Moves = KnightUpLeft(X, Y);
                foreach (int[] moves in Moves)
                {
                    SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]);
                }
                Moves = KnightUpRight(X, Y);
                foreach (int[] moves in Moves)
                {
                    SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]);
                }

                Moves = KnightDownRight(X, Y);
                foreach (int[] moves in Moves)
                {
                    SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]);
                }
                Moves = KnightDownLeft(X, Y);
                foreach (int[] moves in Moves)
                {
                    SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]);
                }

                Moves = KnightLeftUp(X, Y);
                foreach (int[] moves in Moves)
                {
                    SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]);
                }
                Moves = KnightLeftDown(X, Y);
                foreach (int[] moves in Moves)
                {
                    SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]);
                }

                Moves = KnightRightUp(X, Y);
                foreach (int[] moves in Moves)
                {
                    SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]);
                }
                Moves = KnightRightDown(X, Y);
                foreach (int[] moves in Moves)
                {
                    SelectionTracker(moves[0], moves[1], thisBoard.Tiles[moves[0], moves[1]].TileObj.transform.position, moves[2]);
                }
            }
        }
    }

    public List<int[]> NorthTile(int X, int Y, int range)
    {
        List<int[]> PossibleMoves = new List<int[]>();
        
        for (int i = 1; i <= range; i++)
        {
            if (Y + i == board.BoardSize) return PossibleMoves;
            Tile Attacker = ScriptableObject.CreateInstance<Tile>();
            Tile Victim = ScriptableObject.CreateInstance<Tile>();
            Victim = ActiveBoard.board.Tiles[X, Y + i];
            Attacker = ActiveBoard.board.Tiles[X, Y];

            int[] Coords = new int[3];

            if (Victim.Occupied != null && Victim.Occupied.PlayerColor != Attacker.Occupied.PlayerColor) // if Victim is enemy
            {
                Coords[0] = X;
                Coords[1] = Y + i;
                Coords[2] = 3; // Attack tracker 
                PossibleMoves.Add(Coords);
                return PossibleMoves;
            }
            // if Victim is friend, no tracker (illegal move)
            else if (Victim.Occupied != null && Victim.Occupied.PlayerColor == Attacker.Occupied.PlayerColor) return PossibleMoves;
            // if Victim is empty
            Coords[0] = X;
            Coords[1] = Y + i;
            Coords[2] = 2;  // Move tracker 
            PossibleMoves.Add(Coords);
        }
        return PossibleMoves;
    }

    public List<int[]> SouthTile(int X, int Y, int range)
    {
        List<int[]> PossibleMoves = new List<int[]>();

        for (int i = 1; i <= range; i++)
        {
            if (Y - i == -1) return PossibleMoves;
            Tile Attacker = ScriptableObject.CreateInstance<Tile>();
            Tile Victim = ScriptableObject.CreateInstance<Tile>();
            Victim = ActiveBoard.board.Tiles[X, Y - i];
            Attacker = ActiveBoard.board.Tiles[X, Y];
            int[] Coords = new int[3];
            if (Victim.Occupied != null && Victim.Occupied.PlayerColor != Attacker.Occupied.PlayerColor)
            {
                Coords[0] = X;
                Coords[1] = Y - i;
                Coords[2] = 3;
                PossibleMoves.Add(Coords);
                return PossibleMoves;
            }
            else if(Victim.Occupied != null && Victim.Occupied.PlayerColor == Attacker.Occupied.PlayerColor) return PossibleMoves;
            Coords[0] = X;
            Coords[1] = Y - i;
            Coords[2] = 2;
            PossibleMoves.Add(Coords);
        }
        return PossibleMoves;
    }

    public List<int[]> EastTile(int X, int Y, int range)
    {
        List<int[]> PossibleMoves = new List<int[]>();

        for (int i = 1; i <= range; i++)
        {
            if (X + i == board.BoardSize) return PossibleMoves;
            Tile Attacker = ScriptableObject.CreateInstance<Tile>();
            Tile Victim = ScriptableObject.CreateInstance<Tile>();
            Victim = ActiveBoard.board.Tiles[X+i, Y];
            Attacker = ActiveBoard.board.Tiles[X, Y];
            int[] Coords = new int[3];
            if (Victim.Occupied != null && Victim.Occupied.PlayerColor != Attacker.Occupied.PlayerColor)
            {
                Coords[0] = X + i;
                Coords[1] = Y;
                Coords[2] = 3;
                PossibleMoves.Add(Coords);
                return PossibleMoves;
            }
            else if (Victim.Occupied != null && Victim.Occupied.PlayerColor == Attacker.Occupied.PlayerColor) return PossibleMoves;
            Coords[0] = X + i;
            Coords[1] = Y;
            Coords[2] = 2;
            PossibleMoves.Add(Coords);
        }
        return PossibleMoves;
    }

    public List<int[]> WestTile(int X, int Y, int range)
    {
        List<int[]> PossibleMoves = new List<int[]>();

        for (int i = 1; i <= range; i++)
        {
            if (X - i == -1) return PossibleMoves;
            Tile Attacker = ScriptableObject.CreateInstance<Tile>();
            Tile Victim = ScriptableObject.CreateInstance<Tile>();
            Victim = ActiveBoard.board.Tiles[X - i, Y];
            Attacker = ActiveBoard.board.Tiles[X, Y];
            int[] Coords = new int[3];
            if (Victim.Occupied != null && Victim.Occupied.PlayerColor != Attacker.Occupied.PlayerColor)
            {
                Coords[0] = X - i;
                Coords[1] = Y;
                Coords[2] = 3;
                PossibleMoves.Add(Coords);
                return PossibleMoves;
            }
            else if (Victim.Occupied != null && Victim.Occupied.PlayerColor == Attacker.Occupied.PlayerColor) return PossibleMoves;
            Coords[0] = X - i;
            Coords[1] = Y;
            Coords[2] = 2;
            PossibleMoves.Add(Coords);
        }
        return PossibleMoves;
    }

    public List<int[]> NorthWestTile(int X, int Y, int range)
    {
        List<int[]> PossibleMoves = new List<int[]>();

        for (int i = 1; i <= range; i++)
        {
            if (X - i == -1 || Y + i == board.BoardSize) return PossibleMoves;
            Tile Attacker = ScriptableObject.CreateInstance<Tile>();
            Tile Victim = ScriptableObject.CreateInstance<Tile>();
            Victim = ActiveBoard.board.Tiles[X - i, Y + i];
            Attacker = ActiveBoard.board.Tiles[X, Y];
            int[] Coords = new int[3];
            if (Victim.Occupied != null && Victim.Occupied.PlayerColor != Attacker.Occupied.PlayerColor)
            {
                Coords[0] = X - i;
                Coords[1] = Y + i;
                Coords[2] = 3;
                PossibleMoves.Add(Coords);
                return PossibleMoves;
            }
            else if (Victim.Occupied != null && Victim.Occupied.PlayerColor == Attacker.Occupied.PlayerColor) return PossibleMoves;
            Coords[0] = X - i;
            Coords[1] = Y + i;
            Coords[2] = 2;
            PossibleMoves.Add(Coords);
        }
        return PossibleMoves;
    }

    public List<int[]> SouthWestTile(int X, int Y, int range)
    {
        List<int[]> PossibleMoves = new List<int[]>();

        for (int i = 1; i <= range; i++)
        {
            if (X - i == -1 || Y - i == -1) return PossibleMoves;
            Tile Attacker = ScriptableObject.CreateInstance<Tile>();
            Tile Victim = ScriptableObject.CreateInstance<Tile>();
            Victim = ActiveBoard.board.Tiles[X - i, Y - i];
            Attacker = ActiveBoard.board.Tiles[X, Y];
            int[] Coords = new int[3];
            if (Victim.Occupied != null && Victim.Occupied.PlayerColor != Attacker.Occupied.PlayerColor)
            {
                Coords[0] = X - i;
                Coords[1] = Y - i;
                Coords[2] = 3;
                PossibleMoves.Add(Coords);
                return PossibleMoves;
            }
            else if (Victim.Occupied != null && Victim.Occupied.PlayerColor == Attacker.Occupied.PlayerColor) return PossibleMoves;
            Coords[0] = X - i;
            Coords[1] = Y - i;
            Coords[2] = 2;
            PossibleMoves.Add(Coords);
        }
        return PossibleMoves;
    }

    public List<int[]> SouthEastTile(int X, int Y, int range)
    {
        List<int[]> PossibleMoves = new List<int[]>();

        for (int i = 1; i <= range; i++)
        {
            if (X + i == board.BoardSize || Y - i == -1) return PossibleMoves;
            Tile Attacker = ScriptableObject.CreateInstance<Tile>();
            Tile Victim = ScriptableObject.CreateInstance<Tile>();
            Victim = ActiveBoard.board.Tiles[X + i, Y - i];
            Attacker = ActiveBoard.board.Tiles[X, Y];
            int[] Coords = new int[3];
            if (Victim.Occupied != null && Victim.Occupied.PlayerColor != Attacker.Occupied.PlayerColor)
            {
                Coords[0] = X + i;
                Coords[1] = Y - i;
                Coords[2] = 3;
                PossibleMoves.Add(Coords);
                return PossibleMoves;
            }
            else if (Victim.Occupied != null && Victim.Occupied.PlayerColor == Attacker.Occupied.PlayerColor) return PossibleMoves;
            Coords[0] = X + i;
            Coords[1] = Y - i;
            Coords[2] = 2;
            PossibleMoves.Add(Coords);
        }
        return PossibleMoves;
    }

    public List<int[]> NorthEastTile(int X, int Y, int range)
    {
        List<int[]> PossibleMoves = new List<int[]>();

        for (int i = 1; i <= range; i++)
        {
            if (X + i == board.BoardSize || Y + i == board.BoardSize) return PossibleMoves;
            Tile Attacker = ScriptableObject.CreateInstance<Tile>();
            Tile Victim = ScriptableObject.CreateInstance<Tile>();
            Victim = ActiveBoard.board.Tiles[X + i, Y + i];
            Attacker = ActiveBoard.board.Tiles[X, Y];
            int[] Coords = new int[3];
            if (Victim.Occupied != null && Victim.Occupied.PlayerColor != Attacker.Occupied.PlayerColor)
            {
                Coords[0] = X + i;
                Coords[1] = Y + i;
                Coords[2] = 3;
                PossibleMoves.Add(Coords);
                return PossibleMoves;
            }
            else if (Victim.Occupied != null && Victim.Occupied.PlayerColor == Attacker.Occupied.PlayerColor) return PossibleMoves;
            Coords[0] = X + i;
            Coords[1] = Y + i;
            Coords[2] = 2;
            PossibleMoves.Add(Coords);
        }
        return PossibleMoves;
    }


    public List<int[]> KnightUpLeft(int X, int Y)
    {
        List<int[]> PossibleMoves = new List<int[]>();

        
        {
            if (X - 1 < 0 || Y + 2 >= board.BoardSize) return PossibleMoves;
            Tile Attacker = ScriptableObject.CreateInstance<Tile>();
            Tile Victim = ScriptableObject.CreateInstance<Tile>();
            Victim = ActiveBoard.board.Tiles[X - 1, Y + 2];
            Attacker = ActiveBoard.board.Tiles[X, Y];
            int[] Coords = new int[3];
            if (Victim.Occupied != null && Victim.Occupied.PlayerColor != Attacker.Occupied.PlayerColor)
            {
                Coords[0] = X - 1;
                Coords[1] = Y + 2;
                Coords[2] = 3;
                PossibleMoves.Add(Coords);
                return PossibleMoves;
            }
            else if (Victim.Occupied != null && Victim.Occupied.PlayerColor == Attacker.Occupied.PlayerColor) return PossibleMoves;
            Coords[0] = X - 1;
            Coords[1] = Y + 2;
            Coords[2] = 2;
            PossibleMoves.Add(Coords);
        }
        return PossibleMoves;
    }
    public List<int[]> KnightUpRight(int X, int Y)
    {
        List<int[]> PossibleMoves = new List<int[]>();


        {
            if (X + 1 >= board.BoardSize || Y + 2 >= board.BoardSize) return PossibleMoves;
            Tile Attacker = ScriptableObject.CreateInstance<Tile>();
            Tile Victim = ScriptableObject.CreateInstance<Tile>();
            Victim = ActiveBoard.board.Tiles[X + 1, Y + 2];
            Attacker = ActiveBoard.board.Tiles[X, Y];
            int[] Coords = new int[3];
            if (Victim.Occupied != null && Victim.Occupied.PlayerColor != Attacker.Occupied.PlayerColor)
            {
                Coords[0] = X + 1;
                Coords[1] = Y + 2;
                Coords[2] = 3;
                PossibleMoves.Add(Coords);
                return PossibleMoves;
            }
            else if (Victim.Occupied != null && Victim.Occupied.PlayerColor == Attacker.Occupied.PlayerColor) return PossibleMoves;
            Coords[0] = X + 1;
            Coords[1] = Y + 2;
            Coords[2] = 2;
            PossibleMoves.Add(Coords);
        }
        return PossibleMoves;
    }

    public List<int[]> KnightDownLeft(int X, int Y)
    {
        List<int[]> PossibleMoves = new List<int[]>();


        {
            if (X - 1 < 0 || Y - 2 < 0 ) return PossibleMoves;
            Tile Attacker = ScriptableObject.CreateInstance<Tile>();
            Tile Victim = ScriptableObject.CreateInstance<Tile>();
            Victim = ActiveBoard.board.Tiles[X - 1, Y - 2];
            Attacker = ActiveBoard.board.Tiles[X, Y];
            int[] Coords = new int[3];
            if (Victim.Occupied != null && Victim.Occupied.PlayerColor != Attacker.Occupied.PlayerColor)
            {
                Coords[0] = X - 1;
                Coords[1] = Y - 2;
                Coords[2] = 3;
                PossibleMoves.Add(Coords);
                return PossibleMoves;
            }
            else if (Victim.Occupied != null && Victim.Occupied.PlayerColor == Attacker.Occupied.PlayerColor) return PossibleMoves;
            Coords[0] = X - 1;
            Coords[1] = Y - 2;
            Coords[2] = 2;
            PossibleMoves.Add(Coords);
        }
        return PossibleMoves;
    }
    public List<int[]> KnightDownRight(int X, int Y)
    {
        List<int[]> PossibleMoves = new List<int[]>();


        {
            if (X + 1 >= board.BoardSize || Y - 2 < 0) return PossibleMoves;
            Tile Attacker = ScriptableObject.CreateInstance<Tile>();
            Tile Victim = ScriptableObject.CreateInstance<Tile>();
            Victim = ActiveBoard.board.Tiles[X + 1, Y - 2];
            Attacker = ActiveBoard.board.Tiles[X, Y];
            int[] Coords = new int[3];
            if (Victim.Occupied != null && Victim.Occupied.PlayerColor != Attacker.Occupied.PlayerColor)
            {
                Coords[0] = X + 1;
                Coords[1] = Y - 2;
                Coords[2] = 3;
                PossibleMoves.Add(Coords);
                return PossibleMoves;
            }
            else if (Victim.Occupied != null && Victim.Occupied.PlayerColor == Attacker.Occupied.PlayerColor) return PossibleMoves;
            Coords[0] = X + 1;
            Coords[1] = Y - 2;
            Coords[2] = 2;
            PossibleMoves.Add(Coords);
        }
        return PossibleMoves;
    }

    public List<int[]> KnightRightDown(int X, int Y)
    {
        List<int[]> PossibleMoves = new List<int[]>();


        {
            if (X + 2 >= board.BoardSize || Y - 1 < 0) return PossibleMoves;
            Tile Attacker = ScriptableObject.CreateInstance<Tile>();
            Tile Victim = ScriptableObject.CreateInstance<Tile>();
            Victim = ActiveBoard.board.Tiles[X +2 , Y - 1];
            Attacker = ActiveBoard.board.Tiles[X, Y];
            int[] Coords = new int[3];
            if (Victim.Occupied != null && Victim.Occupied.PlayerColor != Attacker.Occupied.PlayerColor)
            {
                Coords[0] = X + 2;
                Coords[1] = Y - 1;
                Coords[2] = 3;
                PossibleMoves.Add(Coords);
                return PossibleMoves;
            }
            else if (Victim.Occupied != null && Victim.Occupied.PlayerColor == Attacker.Occupied.PlayerColor) return PossibleMoves;
            Coords[0] = X + 2;
            Coords[1] = Y - 1;
            Coords[2] = 2;
            PossibleMoves.Add(Coords);
        }
        return PossibleMoves;
    }
    public List<int[]> KnightRightUp(int X, int Y)
    {
        List<int[]> PossibleMoves = new List<int[]>();


        {
            if (X + 2 >= board.BoardSize || Y + 1 >= board.BoardSize) return PossibleMoves;
            Tile Attacker = ScriptableObject.CreateInstance<Tile>();
            Tile Victim = ScriptableObject.CreateInstance<Tile>();
            Victim = ActiveBoard.board.Tiles[X + 2, Y + 1];
            Attacker = ActiveBoard.board.Tiles[X, Y];
            int[] Coords = new int[3];
            if (Victim.Occupied != null && Victim.Occupied.PlayerColor != Attacker.Occupied.PlayerColor)
            {
                Coords[0] = X + 2;
                Coords[1] = Y + 1;
                Coords[2] = 3;
                PossibleMoves.Add(Coords);
                return PossibleMoves;
            }
            else if (Victim.Occupied != null && Victim.Occupied.PlayerColor == Attacker.Occupied.PlayerColor) return PossibleMoves;
            Coords[0] = X + 2;
            Coords[1] = Y + 1;
            Coords[2] = 2;
            PossibleMoves.Add(Coords);
        }
        return PossibleMoves;
    }

    public List<int[]> KnightLeftDown(int X, int Y)
    {
        List<int[]> PossibleMoves = new List<int[]>();


        {
            if (X - 2 < 0 || Y - 1 < 0) return PossibleMoves;
            Tile Attacker = ScriptableObject.CreateInstance<Tile>();
            Tile Victim = ScriptableObject.CreateInstance<Tile>();
            Victim = ActiveBoard.board.Tiles[X - 2, Y - 1];
            Attacker = ActiveBoard.board.Tiles[X, Y];
            int[] Coords = new int[3];
            if (Victim.Occupied != null && Victim.Occupied.PlayerColor != Attacker.Occupied.PlayerColor)
            {
                Coords[0] = X - 2;
                Coords[1] = Y - 1;
                Coords[2] = 3;
                PossibleMoves.Add(Coords);
                return PossibleMoves;
            }
            else if (Victim.Occupied != null && Victim.Occupied.PlayerColor == Attacker.Occupied.PlayerColor) return PossibleMoves;
            Coords[0] = X - 2;
            Coords[1] = Y - 1;
            Coords[2] = 2;
            PossibleMoves.Add(Coords);
        }
        return PossibleMoves;
    }
    public List<int[]> KnightLeftUp(int X, int Y)
    {
        List<int[]> PossibleMoves = new List<int[]>();


        {
            if (X - 2 < 0 || Y + 1 >= board.BoardSize) return PossibleMoves;
            Tile Attacker = ScriptableObject.CreateInstance<Tile>();
            Tile Victim = ScriptableObject.CreateInstance<Tile>();
            Victim = ActiveBoard.board.Tiles[X - 2, Y + 1];
            Attacker = ActiveBoard.board.Tiles[X, Y];
            int[] Coords = new int[3];
            if (Victim.Occupied != null && Victim.Occupied.PlayerColor != Attacker.Occupied.PlayerColor)
            {
                Coords[0] = X - 2;
                Coords[1] = Y + 1;
                Coords[2] = 3;
                PossibleMoves.Add(Coords);
                return PossibleMoves;
            }
            else if (Victim.Occupied != null && Victim.Occupied.PlayerColor == Attacker.Occupied.PlayerColor) return PossibleMoves;
            Coords[0] = X - 2;
            Coords[1] = Y + 1;
            Coords[2] = 2;
            PossibleMoves.Add(Coords);
        }
        return PossibleMoves;
    }
}



