using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    public enum Square { Empty, Black_Checker, Black_Queen, White_Checker, White_Queen }

    public Square[,] boardState = new Square[8, 8];

    public enum Turn { Black, White }

    public Turn turn;

    public Board()
    {
        boardState = new Square[,] { {Square.Empty, Square.White_Checker, Square.Empty, Square.White_Checker, Square.Empty, Square.White_Checker, Square.Empty, Square.White_Checker},
                               {Square.White_Checker, Square.Empty, Square.White_Checker, Square.Empty, Square.White_Checker, Square.Empty, Square.White_Checker, Square.Empty},
                               {Square.Empty, Square.White_Checker, Square.Empty, Square.White_Checker, Square.Empty, Square.White_Checker, Square.Empty, Square.White_Checker},
                               {Square.Empty, Square.Empty, Square.Empty, Square.Empty, Square.Empty, Square.Empty, Square.Empty, Square.Empty},
                               {Square.Empty, Square.Empty, Square.Empty, Square.Empty, Square.Empty, Square.Empty, Square.Empty, Square.Empty},
                               {Square.Black_Checker, Square.Empty, Square.Black_Checker, Square.Empty, Square.Black_Checker, Square.Empty, Square.Black_Checker, Square.Empty},
                               {Square.Empty, Square.Black_Checker, Square.Empty, Square.Black_Checker, Square.Empty, Square.Black_Checker, Square.Empty, Square.Black_Checker},
                               {Square.Black_Checker, Square.Empty, Square.Black_Checker, Square.Empty, Square.Black_Checker, Square.Empty, Square.Black_Checker, Square.Empty} };

        turn = Turn.White;
    }

    public Board(Square[,] _board, Turn _turn)
    {
        boardState = _board;
        turn = _turn;
    }

    public List<Move> GetAllMoves()
    {
        List<Move> availableMoves = new List<Move>(0);

        Square currentChecker, currentQueen;

        if (turn == Turn.Black)
        {
            currentChecker = Square.Black_Checker;
            currentQueen = Square.Black_Queen;
        }
        else
        {
            currentChecker = Square.White_Checker;
            currentQueen = Square.White_Queen;
        }

        for (int i = 0; i < boardState.GetLength(0); i++)
        {
            for (int j = 0; j < boardState.GetLength(1); j++)
            {
                if (boardState[i, j] == currentQueen)
                {
                    foreach (Move move in GetJumps(i, j, new Vector2Int(i, j), new List<Vector2Int>(0), true))
                    {
                        availableMoves.Add(move);
                    }
                }
                else if (boardState[i, j] == currentChecker)
                {
                    foreach (Move move in GetJumps(i, j, new Vector2Int(i, j), new List<Vector2Int>(0), false))
                    {
                        availableMoves.Add(move);
                    }
                }
            }
        }

        if (availableMoves.Count == 0)
        {
            for (int i = 0; i < boardState.GetLength(0); i++)
            {
                for (int j = 0; j < boardState.GetLength(1); j++)
                {
                    if (boardState[i, j] == currentQueen)
                    {
                        foreach (Move move in GetMoves(i, j, true))
                        {
                            availableMoves.Add(move);
                        }
                    }
                    else if (boardState[i, j] == currentChecker)
                    {
                        foreach (Move move in GetMoves(i, j, false))
                        {
                            availableMoves.Add(move);
                        }
                    }
                }
            }
        }

        return availableMoves;
    }


    List<Move> GetJumps(int _row, int _col, Vector2Int _initPos, List<Vector2Int> _alreadyJumped, bool _queen)
    {
        List<Move> availableJumps = new List<Move>(0);

        int offset;

        if (turn == Turn.Black) offset = 1;
        else offset = -1;

        Square adversaryChecker, adversaryQueen;

        if (turn == Turn.Black)
        {
            adversaryChecker = Square.White_Checker;
            adversaryQueen = Square.White_Queen;
        }
        else
        {
            adversaryChecker = Square.Black_Checker;
            adversaryQueen = Square.Black_Queen;
        }

        bool forward = _row - 2 * offset >= 0 && _row - 2 * offset < boardState.GetLength(0);
        bool backward = _row + 2 * offset >= 0 && _row + 2 * offset < boardState.GetLength(0);
        bool right = _col + 2 * offset >= 0 && _col + 2 * offset < boardState.GetLength(1);
        bool left = _col - 2 * offset >= 0 && _col - 2 * offset < boardState.GetLength(1);

        int jumpsCounter;

        if (forward && right)
        {
            if ((boardState[_row - 2 * offset, _col + 2 * offset] == Square.Empty || new Vector2(_row - 2 * offset, _col + 2 * offset) == _initPos) && (boardState[_row - offset, _col + offset] == adversaryChecker || boardState[_row - offset, _col + offset] == adversaryQueen) && !_alreadyJumped.Contains(new Vector2Int(_row - offset, _col + offset)))
            {
                jumpsCounter = availableJumps.Count;

                List<Vector2Int> alreadyJumped = _alreadyJumped;

                alreadyJumped.Add(new Vector2Int(_row - offset, _col + offset));

                foreach (Move m in GetJumps(_row - 2 * offset, _col + 2 * offset, _initPos, alreadyJumped, _queen))
                {
                    m.jumped.Add(new Vector2Int(_row - offset, _col + offset));
                    availableJumps.Add(m);
                }

                alreadyJumped.RemoveAt(alreadyJumped.Count - 1);

                if (availableJumps.Count == jumpsCounter)
                {
                    List<Vector2Int> jumped = new List<Vector2Int>(0);
                    jumped.Add(new Vector2Int(_row - offset, _col + offset));

                    availableJumps.Add(new Move(_initPos, new Vector2Int(_row - 2 * offset, _col + 2 * offset), jumped));
                }
            }
        }

        if (forward && left)
        {
            if ((boardState[_row - 2 * offset, _col - 2 * offset] == Square.Empty || new Vector2(_row - 2 * offset, _col - 2 * offset) == _initPos) && (boardState[_row - offset, _col - offset] == adversaryChecker || boardState[_row - offset, _col - offset] == adversaryQueen) && !_alreadyJumped.Contains(new Vector2Int(_row - offset, _col - offset)))
            {
                jumpsCounter = availableJumps.Count;

                List<Vector2Int> alreadyJumped = _alreadyJumped;

                alreadyJumped.Add(new Vector2Int(_row - offset, _col - offset));

                foreach (Move m in GetJumps(_row - 2 * offset, _col - 2 * offset, _initPos, alreadyJumped, _queen))
                {
                    m.jumped.Add(new Vector2Int(_row - offset, _col - offset));
                    availableJumps.Add(m);
                }

                alreadyJumped.RemoveAt(alreadyJumped.Count - 1);

                if (availableJumps.Count == jumpsCounter)
                {
                    List<Vector2Int> jumped = new List<Vector2Int>(0);
                    jumped.Add(new Vector2Int(_row - offset, _col - offset));

                    availableJumps.Add(new Move(_initPos, new Vector2Int(_row - 2 * offset, _col - 2 * offset), jumped));
                }
            }
        }

        if (_queen)
        {
            if (backward && right)
            {
                if ((boardState[_row + 2 * offset, _col + 2 * offset] == Square.Empty || new Vector2(_row + 2 * offset, _col + 2 * offset) == _initPos) && (boardState[_row + offset, _col + offset] == adversaryChecker || boardState[_row + offset, _col + offset] == adversaryQueen) && !_alreadyJumped.Contains(new Vector2Int(_row + offset, _col + offset)))
                {
                    jumpsCounter = availableJumps.Count;

                    List<Vector2Int> alreadyJumped = _alreadyJumped;

                    alreadyJumped.Add(new Vector2Int(_row + offset, _col + offset));

                    foreach (Move m in GetJumps(_row + 2 * offset, _col + 2 * offset, _initPos, alreadyJumped, _queen))
                    {
                        m.jumped.Add(new Vector2Int(_row + offset, _col + offset));
                        availableJumps.Add(m);
                    }

                    alreadyJumped.RemoveAt(alreadyJumped.Count - 1);

                    if (availableJumps.Count == jumpsCounter)
                    {
                        List<Vector2Int> jumped = new List<Vector2Int>(0);
                        jumped.Add(new Vector2Int(_row + offset, _col + offset));

                        availableJumps.Add(new Move(_initPos, new Vector2Int(_row + 2 * offset, _col + 2 * offset), jumped));
                    }
                }
            }

            if (backward && left)
            {
                if ((boardState[_row + 2 * offset, _col - 2 * offset] == Square.Empty || new Vector2(_row + 2 * offset, _col - 2 * offset) == _initPos) && (boardState[_row + offset, _col - offset] == adversaryChecker || boardState[_row + offset, _col - offset] == adversaryQueen) && !_alreadyJumped.Contains(new Vector2Int(_row + offset, _col - offset)))
                {
                    jumpsCounter = availableJumps.Count;

                    List<Vector2Int> alreadyJumped = _alreadyJumped;

                    alreadyJumped.Add(new Vector2Int(_row + offset, _col - offset));

                    foreach (Move m in GetJumps(_row + 2 * offset, _col - 2 * offset, _initPos, alreadyJumped, _queen))
                    {
                        m.jumped.Add(new Vector2Int(_row + offset, _col - offset));
                        availableJumps.Add(m);
                    }

                    alreadyJumped.RemoveAt(alreadyJumped.Count - 1);

                    if (availableJumps.Count == jumpsCounter)
                    {
                        List<Vector2Int> jumped = new List<Vector2Int>(0);
                        jumped.Add(new Vector2Int(_row + offset, _col - offset));

                        availableJumps.Add(new Move(_initPos, new Vector2Int(_row + 2 * offset, _col - 2 * offset), jumped));
                    }
                }
            }
        }

        return availableJumps;
    }

    List<Move> GetMoves(int _row, int _col, bool _queen)
    {
        List<Move> availableMoves = new List<Move>(0);

        int offset;

        if (turn == Turn.Black) offset = 1;
        else offset = -1;

        bool forward = _row - offset >= 0 && _row - offset < boardState.GetLength(0);
        bool backward = _row + offset >= 0 && _row + offset < boardState.GetLength(0);
        bool right = _col + offset >= 0 && _col + offset < boardState.GetLength(1);
        bool left = _col - offset >= 0 && _col - offset < boardState.GetLength(1);

        if (forward && right)
        {
            if (boardState[_row - offset, _col + offset] == Square.Empty)
            {
                availableMoves.Add(new Move(new Vector2Int(_row, _col), new Vector2Int(_row - offset, _col + offset)));
            }
        }

        if (forward && left)
        {
            if (boardState[_row - offset, _col - offset] == Square.Empty)
            {
                availableMoves.Add(new Move(new Vector2Int(_row, _col), new Vector2Int(_row - offset, _col - offset)));
            }
        }

        if (_queen)
        {
            if (backward && right)
            {
                if (boardState[_row + offset, _col + offset] == Square.Empty)
                {
                    availableMoves.Add(new Move(new Vector2Int(_row, _col), new Vector2Int(_row + offset, _col + offset)));
                }
            }

            if (backward && left)
            {
                if (boardState[_row + offset, _col - offset] == Square.Empty)
                {
                    availableMoves.Add(new Move(new Vector2Int(_row, _col), new Vector2Int(_row + offset, _col - offset)));
                }
            }
        }

        return availableMoves;
    }

    public void MakeMove(Move _move)
    {
        boardState[_move.to.x, _move.to.y] = boardState[_move.from.x, _move.from.y];
        boardState[_move.from.x, _move.from.y] = Square.Empty;

        foreach (Vector2Int p in _move.jumped)
        {
            boardState[p.x, p.y] = Square.Empty;
        }
    }

    public float Evaluate()
    {
        return CheckersCount() * 1f + KingsCount() * 3f;
    }

    private float CheckersCount()
    {
        float score = 0;

        foreach (Square square in boardState)
        {
            if (square == Square.Black_Checker)
            {
                score++;
            }
            else if (square == Square.White_Checker)
            {
                score--;
            }
        }

        return score;
    }

    private float KingsCount()
    {
        float score = 0;

        foreach (Square square in boardState)
        {
            if (square == Square.Black_Queen)
            {
                score++;
            }
            else if (square == Square.White_Queen)
            {
                score--;
            }
        }

        return score;
    }
}

