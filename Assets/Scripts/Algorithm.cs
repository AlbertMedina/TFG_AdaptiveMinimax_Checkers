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

    public static AvailableMove RndMinimax(Board _board, Board.Turn _currentTurn, int _currentDepth, int _maxDepth, float _startingTime, float _maxThinkingTime)
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

            currentMove = RndMinimax(newBoard, _currentTurn, _currentDepth + 1, _maxDepth, _startingTime, _maxThinkingTime);

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

            if (Time.realtimeSinceStartup - _startingTime > _maxThinkingTime) break;
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

    public static AvailableMove RndABMinimax(Board _board, Board.Turn _currentTurn, int _currentDepth, int _maxDepth, float _alpha, float _beta, float _startingTime, float _maxThinkingTime)
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

            currentMove = RndABMinimax(newBoard, _currentTurn, _currentDepth + 1, _maxDepth, _alpha, _beta, _startingTime, _maxThinkingTime);

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

            if (Time.realtimeSinceStartup - _startingTime > _maxThinkingTime) break;
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

    public static AvailableMove AdaptiveMinimax(Board _board, Board.Turn _currentTurn, int _currentDepth, int _maxDepth, float _alpha, float _beta, float _difficultyRate, float _startingTime, float _maxThinkingTime)
    {  
        if (_currentDepth >= _maxDepth)
        {
            return new AvailableMove() { move = null, score = _board.Evaluate(_currentTurn) };
        }

        List<Move> moves = _board.GetAllMoves();

        if (moves.Count == 0)
        {
            return new AvailableMove() { move = null, score = _board.Evaluate(_currentTurn) };
        }
        
        List<AvailableMove> availableMoves = new List<AvailableMove>(0);

        AvailableMove currentMove = new AvailableMove();

        foreach (Move m in moves)
        {
            Board newBoard;

            if (_board.currentTurn == Board.Turn.Black) newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.White, _board.playerBlack);
            else newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.Black, _board.playerBlack);

            newBoard.MakeMove(m);

            currentMove = RndABMinimax(newBoard, _currentTurn, _currentDepth + 1, _maxDepth, _alpha, _beta, _startingTime, _maxThinkingTime);

            availableMoves.Add(new AvailableMove() { move = m, score = currentMove.score });

            if (Time.realtimeSinceStartup - _startingTime > _maxThinkingTime) break;
        }

        availableMoves = ShuffleList(availableMoves);

        List<AvailableMove> sortedMoves;

        if (_currentTurn == _board.currentTurn)
        {
            sortedMoves = availableMoves.OrderBy(m => m.score).ToList();
        }
        else
        {
            //NECESSARY IF RECURSIVE
            sortedMoves = availableMoves.OrderByDescending(m => m.score).ToList();
        }

        if (sortedMoves.Count == 1) return sortedMoves[0];

        List<float> scoresList = new List<float>(0);

        foreach (AvailableMove am in sortedMoves)
        {
            scoresList.Add(am.score);
        }

        scoresList = scoresList.Distinct().ToList();

        float score = scoresList[Mathf.RoundToInt((scoresList.Count - 1) * _difficultyRate / 100)];

        AvailableMove chosenMove = new AvailableMove();

        foreach (AvailableMove am in sortedMoves)
        {
            if (am.score == score)
            {
                chosenMove = am;
                break;
            }
        }

        return chosenMove;
    }

    public static List<AvailableMove> GetSortedMoves(Board _board, Board.Turn _currentTurn, int _currentDepth, int _maxDepth, float _alpha, float _beta, float _startingTime, float _maxThinkingTime)
    {
        List<Move> moves = _board.GetAllMoves();

        foreach (Move am in moves)
        {
            Debug.Log("FROM " + am.from + " TO " + am.to);
        }

        List<AvailableMove> availableMoves = new List<AvailableMove>(0);

        AvailableMove currentMove = new AvailableMove();

        foreach (Move m in moves)
        {
            Board newBoard;

            if (_board.currentTurn == Board.Turn.Black) newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.White, _board.playerBlack);
            else newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.Black, _board.playerBlack);

            newBoard.MakeMove(m);

            currentMove = RndABMinimax(newBoard, _currentTurn, _currentDepth, _maxDepth, _alpha, _beta, _startingTime, _maxThinkingTime);

            availableMoves.Add(new AvailableMove() { move = m, score = currentMove.score });
        }

        List<AvailableMove> sortedMoves;

        sortedMoves = availableMoves.OrderBy(m => m.score).ToList();

        return sortedMoves;
    }

    public static float UpdateDifficultyRate(Move _move, List<AvailableMove> _movesList, float _lastDifficultyRate, ref List<float> _difficultyRatesList)
    {
        float score = 0f;

        for (int i = 0; i < _movesList.Count; i++)
        {
            if (_movesList[i].move.from == _move.from && _movesList[i].move.to == _move.to)
            {
                score = _movesList[i].score;
                break;
            }            
        }

        List<float> scoresList = new List<float>(0);

        foreach (AvailableMove am in _movesList)
        {
            scoresList.Add(am.score);
        }

        scoresList = scoresList.Distinct().ToList();

        float currentDifficultyRate;

        if (scoresList.Count > 1)
        {
            currentDifficultyRate = scoresList.IndexOf(score) * 100 / (scoresList.Count - 1);

            _difficultyRatesList.Add(currentDifficultyRate);

            float sum = 0f;

            for (int i = 1; i <= _difficultyRatesList.Count; i++)
            {
                sum += _difficultyRatesList[i - 1] * i;
            }

            int n = _difficultyRatesList.Count * (_difficultyRatesList.Count + 1) / 2;

            return sum / n;
        }
        else
        {
            return _lastDifficultyRate;
        }
    }

    static List<AvailableMove> ShuffleList(List<AvailableMove> _list)
    {
        for (int i = 0; i < _list.Count; i++)
        {
            AvailableMove temp = _list[i];
            int idx = Random.Range(i, _list.Count);
            _list[i] = _list[idx];
            _list[idx] = temp;
        }

        return _list;
    }
}
