using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class Algorithm
{
    public struct AvailableMove
    {
        public Move move;
        public float score;
    }

    public static AvailableMove Minimax(Board _board, Board.Turn _currentTurn, int _currentDepth, int _maxDepth)
    {
        if (_currentDepth >= _maxDepth)
        {
            return new AvailableMove() { move = null, score = _board.Evaluate(_currentTurn) };
        }

        List<Move> availableMoves = _board.GetAllMoves();

        if (availableMoves.Count == 0)
        {
            return new AvailableMove() { move = null, score = _board.Evaluate(_currentTurn) };
        }

        AvailableMove bestMove;

        if (_currentTurn == _board.currentTurn) bestMove = new AvailableMove() { move = null, score = -Mathf.Infinity };
        else bestMove = new AvailableMove() { move = null, score = Mathf.Infinity };

        AvailableMove currentMove = new AvailableMove();

        foreach (Move m in availableMoves)
        {
            Board newBoard;

            if (_board.currentTurn == Board.Turn.Black) newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.White, _board.playerBlack);
            else newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.Black, _board.playerBlack);

            newBoard.MakeMove(m);

            currentMove = Minimax(newBoard, _currentTurn, _currentDepth + 1, _maxDepth);

            if (_currentTurn == _board.currentTurn)
            {
                if (currentMove.score > bestMove.score)
                {
                    bestMove.score = currentMove.score;
                    bestMove.move = m;
                }
            }
            else
            {
                if (currentMove.score < bestMove.score)
                {
                    bestMove.score = currentMove.score;
                    bestMove.move = m;
                }
            }
        }

        return bestMove;
    }

    public static AvailableMove RndMinimax(Board _board, Board.Turn _currentTurn, int _currentDepth, int _maxDepth)
    {
        if (_currentDepth >= _maxDepth)
        {
            return new AvailableMove() { move = null, score = _board.Evaluate(_currentTurn) };
        }

        List<Move> availableMoves = _board.GetAllMoves();

        if (availableMoves.Count == 0)
        {
            return new AvailableMove() { move = null, score = _board.Evaluate(_currentTurn) };
        }

        List<AvailableMove> bestMoves = new List<AvailableMove>(0);

        if (_currentTurn == _board.currentTurn) bestMoves.Add(new AvailableMove() { move = null, score = -Mathf.Infinity });
        else bestMoves.Add(new AvailableMove() { move = null, score = Mathf.Infinity });

        AvailableMove currentMove = new AvailableMove();

        foreach (Move m in availableMoves)
        {
            Board newBoard;

            if (_board.currentTurn == Board.Turn.Black) newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.White, _board.playerBlack);
            else newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.Black, _board.playerBlack);

            newBoard.MakeMove(m);

            currentMove = RndMinimax(newBoard, _currentTurn, _currentDepth + 1, _maxDepth);

            if (_currentTurn == _board.currentTurn)
            {
                if (currentMove.score > bestMoves[0].score)
                {
                    bestMoves = new List<AvailableMove>(0);
                    bestMoves.Add(new AvailableMove() { move = m, score = currentMove.score });
                }
                else if (currentMove.score == bestMoves[0].score)
                {
                    bestMoves.Add(new AvailableMove() { move = m, score = currentMove.score });
                }
            }
            else
            {
                if (currentMove.score < bestMoves[0].score)
                {
                    bestMoves = new List<AvailableMove>(0);
                    bestMoves.Add(new AvailableMove() { move = m, score = currentMove.score });
                }
                else if (currentMove.score == bestMoves[0].score)
                {
                    bestMoves.Add(new AvailableMove() { move = m, score = currentMove.score });
                }
            }
        }
        
        if (bestMoves.Count > 1)
        {
            return bestMoves[Random.Range(0, bestMoves.Count)];
        }
        else
        {
            return bestMoves[0];
        }    
    }

    public static AvailableMove ABMinimax(Board _board, Board.Turn _currentTurn, int _currentDepth, int _maxDepth, float _alpha, float _beta)
    {
        if (_currentDepth >= _maxDepth)
        {
            return new AvailableMove() { move = null, score = _board.Evaluate(_currentTurn) };
        }

        List<Move> availableMoves = _board.GetAllMoves();

        if (availableMoves.Count == 0)
        {
            return new AvailableMove() { move = null, score = _board.Evaluate(_currentTurn) };
        }

        AvailableMove bestMove;

        if (_currentTurn == _board.currentTurn) bestMove = new AvailableMove() { move = null, score = -Mathf.Infinity };
        else bestMove = new AvailableMove() { move = null, score = Mathf.Infinity };

        AvailableMove currentMove = new AvailableMove();

        foreach (Move m in availableMoves)
        {
            Board newBoard;

            if (_board.currentTurn == Board.Turn.Black) newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.White, _board.playerBlack);
            else newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.Black, _board.playerBlack);

            newBoard.MakeMove(m);

            currentMove = ABMinimax(newBoard, _currentTurn, _currentDepth + 1, _maxDepth, _alpha, _beta);

            if (_currentTurn == _board.currentTurn)
            {
                if (currentMove.score > bestMove.score)
                {
                    bestMove.score = currentMove.score;
                    bestMove.move = m;

                    if (bestMove.score > _alpha) _alpha = bestMove.score;

                    if (_beta <= _alpha) break;
                }
            }
            else
            {
                if (currentMove.score < bestMove.score)
                {
                    bestMove.score = currentMove.score;
                    bestMove.move = m;

                    if (bestMove.score < _beta) _beta = bestMove.score;

                    if (_beta <= _alpha) break;
                }
            }
        }

        return bestMove;
    }

    public static AvailableMove RndABMinimax(Board _board, Board.Turn _currentTurn, int _currentDepth, int _maxDepth, float _alpha, float _beta)
    {
        if (_currentDepth >= _maxDepth)
        {
            return new AvailableMove() { move = null, score = _board.Evaluate(_currentTurn) };
        }

        List<Move> availableMoves = _board.GetAllMoves();

        if (availableMoves.Count == 0)
        {
            return new AvailableMove() { move = null, score = _board.Evaluate(_currentTurn) };
        }

        List<AvailableMove> bestMoves = new List<AvailableMove>(0);

        if (_currentTurn == _board.currentTurn) bestMoves.Add(new AvailableMove() { move = null, score = -Mathf.Infinity });
        else bestMoves.Add(new AvailableMove() { move = null, score = Mathf.Infinity });

        AvailableMove currentMove = new AvailableMove();

        foreach (Move m in availableMoves)
        {
            Board newBoard;

            if (_board.currentTurn == Board.Turn.Black) newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.White, _board.playerBlack);
            else newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.Black, _board.playerBlack);

            newBoard.MakeMove(m);

            currentMove = RndABMinimax(newBoard, _currentTurn, _currentDepth + 1, _maxDepth, _alpha, _beta);

            if (_currentTurn == _board.currentTurn)
            {
                if (currentMove.score > bestMoves[0].score)
                {
                    bestMoves = new List<AvailableMove>(0);
                    bestMoves.Add(new AvailableMove() { move = m, score = currentMove.score });

                    if (bestMoves[0].score > _alpha) _alpha = bestMoves[0].score;

                    if (_beta <= _alpha) break;
                }
                else if (currentMove.score == bestMoves[0].score)
                {
                    bestMoves.Add(new AvailableMove() { move = m, score = currentMove.score });
                }
            }
            else
            {
                if (currentMove.score < bestMoves[0].score)
                {
                    bestMoves = new List<AvailableMove>(0);
                    bestMoves.Add(new AvailableMove() { move = m, score = currentMove.score });

                    if (bestMoves[0].score < _beta) _beta = bestMoves[0].score;

                    if (_beta <= _alpha) break;
                }
                else if (currentMove.score == bestMoves[0].score)
                {
                    bestMoves.Add(new AvailableMove() { move = m, score = currentMove.score });
                }
            }
        }

        if (bestMoves.Count > 1)
        {
            return bestMoves[Random.Range(0, bestMoves.Count)];
        }
        else
        {
            return bestMoves[0];
        }
    }

    public static AvailableMove MyMinimax(Board _board, Board.Turn _currentTurn, int _currentDepth, int _maxDepth, float _alpha, float _beta, float _difficultyRate)
    {
        if (_currentDepth >= _maxDepth)
        {
            return new AvailableMove() { move = null, score = _board.Evaluate(_currentTurn) };
        }

        List<Move> moves = _board.GetAllMoves();

        //Debug.Log(availableMoves.Count);

        if (moves.Count == 0)
        {
            return new AvailableMove() { move = null, score = _board.Evaluate(_currentTurn) };
        }

        List<AvailableMove> availableMoves = new List<AvailableMove>(0);

        //if (_currentTurn == _board.currentTurn) bestMoves.Add(new AvailableMove() { move = null, score = -Mathf.Infinity });
        //else bestMoves.Add(new AvailableMove() { move = null, score = Mathf.Infinity });

        AvailableMove currentMove = new AvailableMove();

        foreach (Move m in moves)
        {
            Board newBoard;

            if (_board.currentTurn == Board.Turn.Black) newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.White, _board.playerBlack);
            else newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.Black, _board.playerBlack);

            newBoard.MakeMove(m);

            currentMove = RndABMinimax(newBoard, _currentTurn, _currentDepth + 1, _maxDepth, _alpha, _beta);

            availableMoves.Add(new AvailableMove() { move = m, score = currentMove.score });         
        }

        List<AvailableMove> sortedMoves;

        if (_currentTurn == _board.currentTurn)
        {
            sortedMoves = availableMoves.OrderBy(m => m.score).ToList();
        }
        else
        {
            sortedMoves = availableMoves.OrderByDescending(m => m.score).ToList();
        }

        sortedMoves = availableMoves.OrderByDescending(m => m.score).ToList();

        foreach (AvailableMove am in sortedMoves) Debug.Log(am.score);

        if (availableMoves.Count == 1)
        {
            return availableMoves[0];
        }
        else
        {
            Debug.Log("i = " + (availableMoves.Count - 1) * (int)_difficultyRate / 100);
            return availableMoves[(availableMoves.Count - 1) * (int)_difficultyRate / 100];
            
        }
    }
}
