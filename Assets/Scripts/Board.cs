using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    public enum Square { Empty, Black_Checker, Black_King, White_Checker, White_King }

    public Square[,] boardState = new Square[8, 8];

    public enum Turn { Black, White }

    public Turn currentTurn;

    public bool playerBlack;

    private float checkersCountWeight = 1f;
    private float kingsCountWeight = 3f;
    private float threatenedCountWeight = 0.5f;
    private float movableCountWeight = 0.25f;
    private float almostKingWeight = 0.25f;

    public Board(bool _playerBlack)
    {
        if (_playerBlack)
        {
            boardState = new Square[,] { {Square.Empty,         Square.White_Checker,   Square.Empty,           Square.White_Checker,   Square.Empty,           Square.White_Checker,   Square.Empty,           Square.White_Checker},
                                         {Square.White_Checker, Square.Empty,           Square.White_Checker,   Square.Empty,           Square.White_Checker,   Square.Empty,           Square.White_Checker,   Square.Empty},
                                         {Square.Empty,         Square.White_Checker,   Square.Empty,           Square.White_Checker,   Square.Empty,           Square.White_Checker,   Square.Empty,           Square.White_Checker},
                                         {Square.Empty,         Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty},
                                         {Square.Empty,         Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty},
                                         {Square.Black_Checker, Square.Empty,           Square.Black_Checker,   Square.Empty,           Square.Black_Checker,   Square.Empty,           Square.Black_Checker,   Square.Empty},
                                         {Square.Empty,         Square.Black_Checker,   Square.Empty,           Square.Black_Checker,   Square.Empty,           Square.Black_Checker,   Square.Empty,           Square.Black_Checker},
                                         {Square.Black_Checker, Square.Empty,           Square.Black_Checker,   Square.Empty,           Square.Black_Checker,   Square.Empty,           Square.Black_Checker,   Square.Empty} };
        }
        else
        {
            boardState = new Square[,] { {Square.Empty,         Square.Black_Checker,   Square.Empty,           Square.Black_Checker,   Square.Empty,           Square.Black_Checker,   Square.Empty,           Square.Black_Checker},
                                         {Square.Black_Checker, Square.Empty,           Square.Black_Checker,   Square.Empty,           Square.Black_Checker,   Square.Empty,           Square.Black_Checker,   Square.Empty},
                                         {Square.Empty,         Square.Black_Checker,   Square.Empty,           Square.Black_Checker,   Square.Empty,           Square.Black_Checker,   Square.Empty,           Square.Black_Checker},
                                         {Square.Empty,         Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty},
                                         {Square.Empty,         Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty},
                                         {Square.White_Checker, Square.Empty,           Square.White_Checker,   Square.Empty,           Square.White_Checker,   Square.Empty,           Square.White_Checker,   Square.Empty},
                                         {Square.Empty,         Square.White_Checker,   Square.Empty,           Square.White_Checker,   Square.Empty,           Square.White_Checker,   Square.Empty,           Square.White_Checker},
                                         {Square.White_Checker, Square.Empty,           Square.White_Checker,   Square.Empty,           Square.White_Checker,   Square.Empty,           Square.White_Checker,   Square.Empty} };
        }

        currentTurn = Turn.Black;
        playerBlack = _playerBlack;

        /*boardState = new Square[,] { {Square.Empty,         Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty},
                                       {Square.Empty,         Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty},
                                       {Square.Empty,         Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty},
                                       {Square.Empty,         Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty},
                                       {Square.Empty,         Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty},
                                       {Square.Empty,         Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty},
                                       {Square.Empty,         Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty},
                                       {Square.Empty,         Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty,           Square.Empty} };*/

    }

    public Board(Square[,] _boardState, Turn _currentTurn, bool _playerBlack)
    {
        boardState = _boardState;
        currentTurn = _currentTurn;
        playerBlack = _playerBlack;
    }

    public void ChangeTurn()
    {
        if (currentTurn == Turn.Black)
        {
            currentTurn = Turn.White;
        }
        else
        {
            currentTurn = Turn.Black;
        }
    }

    public List<Move> GetAllMoves()
    {
        List<Move> availableMoves = new List<Move>(0);

        for (int i = 0; i < boardState.GetLength(0); i++)
        {
            for (int j = 0; j < boardState.GetLength(1); j++)
            {
                foreach (Move move in GetPieceJumps(new Vector2Int(j, i)))
                {
                    availableMoves.Add(move);
                }
            }
        }

        if (availableMoves.Count == 0)
        {
            for (int i = 0; i < boardState.GetLength(0); i++)
            {
                for (int j = 0; j < boardState.GetLength(1); j++)
                {
                    foreach (Move move in GetPieceMoves(new Vector2Int(j, i)))
                    {
                        availableMoves.Add(move);
                    }
                }
            }
        }

        return availableMoves;
    }

    public List<Move> GetPieceJumps(Vector2Int _piece)
    {
        List<Move> availableJumps = new List<Move>(0);

        Square currentChecker, currentKing;

        if (currentTurn == Turn.Black)
        {
            currentChecker = Square.Black_Checker;
            currentKing = Square.Black_King;
        }
        else
        {
            currentChecker = Square.White_Checker;
            currentKing = Square.White_King;
        }

        if (boardState[_piece.y, _piece.x] == currentKing)
        {
            foreach (Move move in GetJumps(_piece.y, _piece.x, new Vector2Int(_piece.x, _piece.y), new List<Vector2Int>(0), true))
            {
                availableJumps.Add(move);
            }
        }
        else if (boardState[_piece.y, _piece.x] == currentChecker)
        {
            foreach (Move move in GetJumps(_piece.y, _piece.x, new Vector2Int(_piece.x, _piece.y), new List<Vector2Int>(0), false))
            {
                availableJumps.Add(move);
            }
        }

        return availableJumps;
    }

    public List<Move> GetPieceMoves(Vector2Int _piece)
    {
        List<Move> availableMoves = new List<Move>(0);

        Square currentChecker, currentKing;

        if (currentTurn == Turn.Black)
        {
            currentChecker = Square.Black_Checker;
            currentKing = Square.Black_King;
        }
        else
        {
            currentChecker = Square.White_Checker;
            currentKing = Square.White_King;
        }

        if (boardState[_piece.y, _piece.x] == currentKing)
        {
            foreach (Move move in GetMoves(_piece.y, _piece.x, true))
            {
                availableMoves.Add(move);
            }
        }
        else if (boardState[_piece.y, _piece.x] == currentChecker)
        {
            foreach (Move move in GetMoves(_piece.y, _piece.x, false))
            {
                availableMoves.Add(move);
            }
        }

        return availableMoves;
    }

    List<Move> GetJumps(int _row, int _col, Vector2Int _initPos, List<Vector2Int> _alreadyJumped, bool _king)
    {
        List<Move> availableJumps = new List<Move>(0);

        int offset;

        if ((currentTurn == Turn.Black && playerBlack) || (currentTurn == Turn.White && !playerBlack))
        {
            offset = 1;
        }
        else
        {
            offset = -1;
        }

        Square adversaryChecker, adversaryKing;

        if (currentTurn == Turn.Black)
        {
            adversaryChecker = Square.White_Checker;
            adversaryKing = Square.White_King;
        }
        else
        {
            adversaryChecker = Square.Black_Checker;
            adversaryKing = Square.Black_King;
        }

        bool forward = _row - 2 * offset >= 0 && _row - 2 * offset < boardState.GetLength(0);
        bool backward = _row + 2 * offset >= 0 && _row + 2 * offset < boardState.GetLength(0);
        bool right = _col + 2 * offset >= 0 && _col + 2 * offset < boardState.GetLength(1);
        bool left = _col - 2 * offset >= 0 && _col - 2 * offset < boardState.GetLength(1);

        int jumpsCounter;

        if (forward && right)
        {
            if ((boardState[_row - 2 * offset, _col + 2 * offset] == Square.Empty || new Vector2(_col + 2 * offset, _row - 2 * offset) == _initPos) && (boardState[_row - offset, _col + offset] == adversaryChecker || boardState[_row - offset, _col + offset] == adversaryKing) && !_alreadyJumped.Contains(new Vector2Int(_col + offset, _row - offset)))
            {
                jumpsCounter = availableJumps.Count;

                List<Vector2Int> alreadyJumped = _alreadyJumped;

                alreadyJumped.Add(new Vector2Int(_col + offset, _row - offset));

                foreach (Move m in GetJumps(_row - 2 * offset, _col + 2 * offset, _initPos, alreadyJumped, _king))
                {
                    m.jumped.Add(new Vector2Int(_col + offset, _row - offset));
                    availableJumps.Add(m);
                }

                alreadyJumped.RemoveAt(alreadyJumped.Count - 1);

                if (availableJumps.Count == jumpsCounter)
                {
                    List<Vector2Int> jumped = new List<Vector2Int>(0);
                    jumped.Add(new Vector2Int(_col + offset, _row - offset));

                    availableJumps.Add(new Move(_initPos, new Vector2Int(_col + 2 * offset, _row - 2 * offset), jumped));
                }
            }
        }

        if (forward && left)
        {
            if ((boardState[_row - 2 * offset, _col - 2 * offset] == Square.Empty || new Vector2(_col - 2 * offset, _row - 2 * offset) == _initPos) && (boardState[_row - offset, _col - offset] == adversaryChecker || boardState[_row - offset, _col - offset] == adversaryKing) && !_alreadyJumped.Contains(new Vector2Int(_col - offset, _row - offset)))
            {
                jumpsCounter = availableJumps.Count;

                List<Vector2Int> alreadyJumped = _alreadyJumped;

                alreadyJumped.Add(new Vector2Int(_col - offset, _row - offset));

                foreach (Move m in GetJumps(_row - 2 * offset, _col - 2 * offset, _initPos, alreadyJumped, _king))
                {
                    m.jumped.Add(new Vector2Int(_col - offset, _row - offset));
                    availableJumps.Add(m);
                }

                alreadyJumped.RemoveAt(alreadyJumped.Count - 1);

                if (availableJumps.Count == jumpsCounter)
                {
                    List<Vector2Int> jumped = new List<Vector2Int>(0);
                    jumped.Add(new Vector2Int(_col - offset, _row - offset));

                    availableJumps.Add(new Move(_initPos, new Vector2Int(_col - 2 * offset, _row - 2 * offset), jumped));
                }
            }
        }

        if (_king)
        {
            if (backward && right)
            {
                if ((boardState[_row + 2 * offset, _col + 2 * offset] == Square.Empty || new Vector2(_col + 2 * offset, _row + 2 * offset) == _initPos) && (boardState[_row + offset, _col + offset] == adversaryChecker || boardState[_row + offset, _col + offset] == adversaryKing) && !_alreadyJumped.Contains(new Vector2Int(_col + offset, _row + offset)))
                {
                    jumpsCounter = availableJumps.Count;

                    List<Vector2Int> alreadyJumped = _alreadyJumped;

                    alreadyJumped.Add(new Vector2Int(_col + offset, _row + offset));

                    foreach (Move m in GetJumps(_row + 2 * offset, _col + 2 * offset, _initPos, alreadyJumped, _king))
                    {
                        m.jumped.Add(new Vector2Int(_col + offset, _row + offset));
                        availableJumps.Add(m);
                    }

                    alreadyJumped.RemoveAt(alreadyJumped.Count - 1);

                    if (availableJumps.Count == jumpsCounter)
                    {
                        List<Vector2Int> jumped = new List<Vector2Int>(0);
                        jumped.Add(new Vector2Int(_col + offset, _row + offset));

                        availableJumps.Add(new Move(_initPos, new Vector2Int(_col + 2 * offset, _row + 2 * offset), jumped));
                    }
                }
            }

            if (backward && left)
            {
                if ((boardState[_row + 2 * offset, _col - 2 * offset] == Square.Empty || new Vector2(_col - 2 * offset, _row + 2 * offset) == _initPos) && (boardState[_row + offset, _col - offset] == adversaryChecker || boardState[_row + offset, _col - offset] == adversaryKing) && !_alreadyJumped.Contains(new Vector2Int(_col - offset, _row + offset)))
                {
                    jumpsCounter = availableJumps.Count;

                    List<Vector2Int> alreadyJumped = _alreadyJumped;

                    alreadyJumped.Add(new Vector2Int(_col - offset, _row + offset));

                    foreach (Move m in GetJumps(_row + 2 * offset, _col - 2 * offset, _initPos, alreadyJumped, _king))
                    {
                        m.jumped.Add(new Vector2Int(_col - offset, _row + offset));
                        availableJumps.Add(m);
                    }

                    alreadyJumped.RemoveAt(alreadyJumped.Count - 1);

                    if (availableJumps.Count == jumpsCounter)
                    {
                        List<Vector2Int> jumped = new List<Vector2Int>(0);
                        jumped.Add(new Vector2Int(_col - offset, _row + offset));

                        availableJumps.Add(new Move(_initPos, new Vector2Int(_col - 2 * offset, _row + 2 * offset), jumped));
                    }
                }
            }
        }

        return availableJumps;
    }

    List<Move> GetMoves(int _row, int _col, bool _king)
    {
        List<Move> availableMoves = new List<Move>(0);

        int offset;

        if ((currentTurn == Turn.Black && playerBlack) || (currentTurn == Turn.White && !playerBlack))
        {
            offset = 1;
        }
        else
        {
            offset = -1;
        }

        bool forward = _row - offset >= 0 && _row - offset < boardState.GetLength(0);
        bool backward = _row + offset >= 0 && _row + offset < boardState.GetLength(0);
        bool right = _col + offset >= 0 && _col + offset < boardState.GetLength(1);
        bool left = _col - offset >= 0 && _col - offset < boardState.GetLength(1);

        if (forward && right)
        {
            if (boardState[_row - offset, _col + offset] == Square.Empty)
            {
                availableMoves.Add(new Move(new Vector2Int(_col, _row), new Vector2Int(_col + offset, _row - offset)));
            }
        }

        if (forward && left)
        {
            if (boardState[_row - offset, _col - offset] == Square.Empty)
            {
                availableMoves.Add(new Move(new Vector2Int(_col, _row), new Vector2Int(_col - offset, _row - offset)));
            }
        }

        if (_king)
        {
            if (backward && right)
            {
                if (boardState[_row + offset, _col + offset] == Square.Empty)
                {
                    availableMoves.Add(new Move(new Vector2Int(_col, _row), new Vector2Int(_col + offset, _row + offset)));
                }
            }

            if (backward && left)
            {
                if (boardState[_row + offset, _col - offset] == Square.Empty)
                {
                    availableMoves.Add(new Move(new Vector2Int(_col, _row), new Vector2Int(_col - offset, _row + offset)));
                }
            }
        }

        return availableMoves;
    }

    public void MakeMove(Move _move)
    {
        boardState[_move.to.y, _move.to.x] = boardState[_move.from.y, _move.from.x];
        boardState[_move.from.y, _move.from.x] = Square.Empty;

        foreach (Vector2Int p in _move.jumped)
        {
            boardState[p.y, p.x] = Square.Empty;
        }

        if (_move.to.y <= 0 || _move.to.y >= boardState.GetLength(0) - 1)
        {
            if (boardState[_move.to.y, _move.to.x] == Square.Black_Checker)
            {
                boardState[_move.to.y, _move.to.x] = Square.Black_King;
            }
            else if (boardState[_move.to.y, _move.to.x] == Square.White_Checker)
            {
                boardState[_move.to.y, _move.to.x] = Square.White_King;
            }
        }
    }

    public float Evaluate(Turn _currentTurn)
    {
        if(GetAllMoves().Count == 0)
        {
            return GameOver(_currentTurn);
        }

        float score = 0f;

        for (int i = 0; i < boardState.GetLength(0); i++)
        {
            for (int j = 0; j < boardState.GetLength(1); j++)
            {
                score += CheckersCount(boardState[i, j]);

                score += KingsCount(boardState[i, j]);

                score += ThreatenedCount(boardState[i, j], new Vector2Int(j, i));

                score += MovableCount(boardState[i, j], new Vector2Int(j, i));

                score += AlmostKingCount(boardState[i, j], new Vector2Int(j, i));
            }
        }

        if (_currentTurn == Turn.Black)
        {
            return score;
        }
        else
        {
            return -score;
        }
    }

    private float GameOver(Turn _currentTurn)
    {
        if (_currentTurn == currentTurn)
        {
            return -1000f;
        }
        else
        {
            return 1000f;
        }
    }

    private float CheckersCount(Square _square)
    {
        if (_square == Square.Black_Checker)
        {
            return checkersCountWeight;
        }
        else if (_square == Square.White_Checker)
        {
            return -checkersCountWeight;
        }

        return 0f;
    }

    private float KingsCount(Square _square)
    {
        if (_square == Square.Black_King)
        {
            return kingsCountWeight;
        }
        else if (_square == Square.White_King)
        {
            return -kingsCountWeight;
        }

        return 0f;
    }

    private float ThreatenedCount(Square _square, Vector2Int _pos)
    {
        float threatenedScore = 0f;

        if (currentTurn == Turn.Black && (_square == Square.Black_Checker || _square == Square.Black_King))
        {
            foreach (Move m in GetPieceJumps(_pos))
            {
                foreach (Vector2 v in m.jumped)
                {
                    threatenedScore += threatenedCountWeight;
                }
            }
        }
        else if (currentTurn == Turn.White && (_square == Square.White_Checker || _square == Square.White_King))
        {
            foreach (Move m in GetPieceJumps(_pos))
            {
                foreach (Vector2 v in m.jumped)
                {
                    threatenedScore -= threatenedCountWeight;
                }
            }
        }

        return threatenedScore;
    }

    private float MovableCount(Square _square, Vector2Int _pos)
    {
        if (_square == Square.Black_Checker || _square == Square.Black_King)
        {
            if (GetPieceJumps(_pos).Count > 0 || GetPieceMoves(_pos).Count > 0)
            {
                return movableCountWeight;
            }
        }
        else if (_square == Square.White_Checker || _square == Square.White_King)
        {
            if (GetPieceJumps(_pos).Count > 0 || GetPieceMoves(_pos).Count > 0)
            {
                return -movableCountWeight;
            }
        }

        return 0f;
    }

    private float AlmostKingCount(Square _square, Vector2Int _pos)
    {
        if(_square == Square.Black_Checker)
        {
            if ((playerBlack && _pos.y < boardState.GetLength(0) / 2) || (!playerBlack && _pos.y > boardState.GetLength(0) / 2))
            {
                return almostKingWeight;
            }
        }
        else if (_square == Square.White_Checker)
        {
            if ((playerBlack && _pos.y > boardState.GetLength(0) / 2) || (!playerBlack && _pos.y < boardState.GetLength(0) / 2))
            {
                return -almostKingWeight;
            }
        }

        return 0f;
    }
}