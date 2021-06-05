using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Algorithm
{
    static int discardedMovesPart = 3;

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

        List<Move> moves = _board.GetAllMoves();

        if (moves.Count == 0)
        {
            return new AvailableMove() { move = null, score = _board.Evaluate(_currentTurn) };
        }

        AvailableMove bestMove;

        if (_currentTurn == _board.currentTurn)
        {
            bestMove = new AvailableMove() { move = null, score = -Mathf.Infinity };
        }
        else
        {
            bestMove = new AvailableMove() { move = null, score = Mathf.Infinity };
        }

        AvailableMove currentMove = new AvailableMove();

        foreach (Move m in moves)
        {
            Board newBoard;

            if (_board.currentTurn == Board.Turn.Black)
            {
                newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.White, _board.playerBlack);
            }
            else
            {
                newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.Black, _board.playerBlack);
            }

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

        List<Move> moves = _board.GetAllMoves();

        if (moves.Count == 0)
        {
            return new AvailableMove() { move = null, score = _board.Evaluate(_currentTurn) };
        }

        List<AvailableMove> bestMoves = new List<AvailableMove>(0);

        if (_currentTurn == _board.currentTurn)
        {
            bestMoves.Add(new AvailableMove() { move = null, score = -Mathf.Infinity });
        }
        else
        {
            bestMoves.Add(new AvailableMove() { move = null, score = Mathf.Infinity });
        }

        AvailableMove currentMove = new AvailableMove();

        foreach (Move m in moves)
        {
            Board newBoard;

            if (_board.currentTurn == Board.Turn.Black)
            {
                newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.White, _board.playerBlack);
            }
            else
            {
                newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.Black, _board.playerBlack);
            }

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

    public static AvailableMove RndMinimax(Board _board, Board.Turn _currentTurn, int _currentDepth, int _maxDepth, float _startingTime, float _maxThinkingTime, ref List<AvailableMove> _availableMoves)
    {
        _availableMoves = new List<AvailableMove>(0);

        if (_currentDepth >= _maxDepth)
        {
            return new AvailableMove() { move = null, score = _board.Evaluate(_currentTurn) };
        }

        List<Move> moves = _board.GetAllMoves();

        if (moves.Count == 0)
        {
            return new AvailableMove() { move = null, score = _board.Evaluate(_currentTurn) };
        }

        List<AvailableMove> bestMoves = new List<AvailableMove>(0);

        if (_currentTurn == _board.currentTurn)
        {
            bestMoves.Add(new AvailableMove() { move = null, score = -Mathf.Infinity });
        }
        else
        {
            bestMoves.Add(new AvailableMove() { move = null, score = Mathf.Infinity });
        }

        AvailableMove currentMove = new AvailableMove();

        foreach (Move m in moves)
        {
            Board newBoard;

            if (_board.currentTurn == Board.Turn.Black)
            {
                newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.White, _board.playerBlack);
            }
            else
            {
                newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.Black, _board.playerBlack);
            }

            newBoard.MakeMove(m);

            currentMove = RndMinimax(newBoard, _currentTurn, _currentDepth + 1, _maxDepth, _startingTime, _maxThinkingTime);

            _availableMoves.Add(new AvailableMove() { move = m, score = currentMove.score });

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

        List<Move> moves = _board.GetAllMoves();

        if (moves.Count == 0)
        {
            return new AvailableMove() { move = null, score = _board.Evaluate(_currentTurn) };
        }

        AvailableMove bestMove;

        if (_currentTurn == _board.currentTurn)
        {
            bestMove = new AvailableMove() { move = null, score = -Mathf.Infinity };
        }
        else
        {
            bestMove = new AvailableMove() { move = null, score = Mathf.Infinity };
        }

        AvailableMove currentMove = new AvailableMove();

        foreach (Move m in moves)
        {
            Board newBoard;

            if (_board.currentTurn == Board.Turn.Black)
            {
                newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.White, _board.playerBlack);
            }
            else
            {
                newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.Black, _board.playerBlack);
            }

            newBoard.MakeMove(m);

            currentMove = ABMinimax(newBoard, _currentTurn, _currentDepth + 1, _maxDepth, _alpha, _beta);

            if (_currentTurn == _board.currentTurn)
            {
                if (currentMove.score > bestMove.score)
                {
                    bestMove.score = currentMove.score;
                    bestMove.move = m;

                    if (bestMove.score > _alpha)
                    {
                        _alpha = bestMove.score;
                    }

                    if (_beta <= _alpha)
                    {
                        break;
                    }
                }
            }
            else
            {
                if (currentMove.score < bestMove.score)
                {
                    bestMove.score = currentMove.score;
                    bestMove.move = m;

                    if (bestMove.score < _beta)
                    {
                        _beta = bestMove.score;
                    }

                    if (_beta <= _alpha)
                    {
                        break;
                    }
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

        List<Move> moves = _board.GetAllMoves();

        if (moves.Count == 0)
        {
            return new AvailableMove() { move = null, score = _board.Evaluate(_currentTurn) };
        }

        List<AvailableMove> bestMoves = new List<AvailableMove>(0);

        if (_currentTurn == _board.currentTurn)
        {
            bestMoves.Add(new AvailableMove() { move = null, score = -Mathf.Infinity });
        }
        else
        {
            bestMoves.Add(new AvailableMove() { move = null, score = Mathf.Infinity });
        }

        AvailableMove currentMove = new AvailableMove();

        foreach (Move m in moves)
        {
            Board newBoard;

            if (_board.currentTurn == Board.Turn.Black)
            {
                newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.White, _board.playerBlack);
            }
            else
            {
                newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.Black, _board.playerBlack);
            }

            newBoard.MakeMove(m);

            currentMove = RndABMinimax(newBoard, _currentTurn, _currentDepth + 1, _maxDepth, _alpha, _beta, _startingTime, _maxThinkingTime);

            if (_currentTurn == _board.currentTurn)
            {
                if (currentMove.score > bestMoves[0].score)
                {
                    bestMoves = new List<AvailableMove>(0);
                    bestMoves.Add(new AvailableMove() { move = m, score = currentMove.score });

                    if (bestMoves[0].score > _alpha)
                    {
                        _alpha = bestMoves[0].score;
                    }

                    if (_beta <= _alpha)
                    {
                        break;
                    }
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

                    if (bestMoves[0].score < _beta)
                    {
                        _beta = bestMoves[0].score;
                    }

                    if (_beta <= _alpha)
                    {
                        break;
                    }
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

    public static AvailableMove RndABMinimax(Board _board, Board.Turn _currentTurn, int _currentDepth, int _maxDepth, float _alpha, float _beta, float _startingTime, float _maxThinkingTime, ref List<AvailableMove> _availableMoves)
    {
        _availableMoves = new List<AvailableMove>(0);

        if (_currentDepth >= _maxDepth)
        {
            return new AvailableMove() { move = null, score = _board.Evaluate(_currentTurn) };
        }

        List<Move> moves = _board.GetAllMoves();

        if (moves.Count == 0)
        {
            return new AvailableMove() { move = null, score = _board.Evaluate(_currentTurn) };
        }

        List<AvailableMove> bestMoves = new List<AvailableMove>(0);

        if (_currentTurn == _board.currentTurn)
        {
            bestMoves.Add(new AvailableMove() { move = null, score = -Mathf.Infinity });
        }
        else
        {
            bestMoves.Add(new AvailableMove() { move = null, score = Mathf.Infinity });
        }

        AvailableMove currentMove = new AvailableMove();

        foreach (Move m in moves)
        {
            Board newBoard;

            if (_board.currentTurn == Board.Turn.Black)
            {
                newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.White, _board.playerBlack);
            }
            else
            {
                newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.Black, _board.playerBlack);
            }

            newBoard.MakeMove(m);

            currentMove = RndABMinimax(newBoard, _currentTurn, _currentDepth + 1, _maxDepth, _alpha, _beta, _startingTime, _maxThinkingTime);

            _availableMoves.Add(new AvailableMove() { move = m, score = currentMove.score });

            if (_currentTurn == _board.currentTurn)
            {
                if (currentMove.score > bestMoves[0].score)
                {
                    bestMoves = new List<AvailableMove>(0);
                    bestMoves.Add(new AvailableMove() { move = m, score = currentMove.score });

                    if (bestMoves[0].score > _alpha)
                    {
                        _alpha = bestMoves[0].score;
                    }

                    if (_beta <= _alpha)
                    {
                        break;
                    }
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

                    if (bestMoves[0].score < _beta)
                    {
                        _beta = bestMoves[0].score;
                    }

                    if (_beta <= _alpha)
                    {
                        break;
                    }
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

    public static AvailableMove AdaptiveABMinimax(Board _board, Board.Turn _currentTurn, int _currentDepth, int _maxDepth, float _alpha, float _beta, float _startingTime, float _maxThinkingTime, float _difficultyRate)
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
        }

        if (availableMoves.Count == 1) return availableMoves[0];

        List<float> scoresList = new List<float>(0);

        foreach (AvailableMove am in availableMoves)
        {
            scoresList.Add(am.score);
        }

        scoresList = scoresList.Distinct().ToList();

        scoresList.Sort();

        List<float> highestScoresList = new List<float>(0);

        for (int i = scoresList.Count / discardedMovesPart; i < scoresList.Count; i++)
        {
            highestScoresList.Add(scoresList[i]);
        }

        float score = highestScoresList[RoundFloatToInt((highestScoresList.Count - 1) * _difficultyRate / 100)];

        AvailableMove chosenMove = new AvailableMove();

        ShuffleList(ref availableMoves);

        foreach (AvailableMove am in availableMoves)
        {
            if (am.score == score)
            {
                chosenMove = am;
                break;
            }
        }

        return chosenMove;
    }

    public static AvailableMove AdaptiveABMinimax(Board _board, Board.Turn _currentTurn, int _currentDepth, int _maxDepth, float _alpha, float _beta, float _startingTime, float _maxThinkingTime, float _difficultyRate, ref List<AvailableMove> _availableMoves)
    {
        _availableMoves = new List<AvailableMove>(0);

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

            if (_board.currentTurn == Board.Turn.Black)
            {
                newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.White, _board.playerBlack);
            }
            else
            {
                newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.Black, _board.playerBlack);
            }

            newBoard.MakeMove(m);

            currentMove = RndABMinimax(newBoard, _currentTurn, _currentDepth + 1, _maxDepth, _alpha, _beta, _startingTime, _maxThinkingTime);

            _availableMoves.Add(new AvailableMove() { move = m, score = currentMove.score });
            availableMoves.Add(new AvailableMove() { move = m, score = currentMove.score });
        }

        if (availableMoves.Count == 1) return availableMoves[0];

        List<float> scoresList = new List<float>(0);

        foreach (AvailableMove am in availableMoves)
        {
            scoresList.Add(am.score);
        }

        scoresList = scoresList.Distinct().ToList();

        scoresList.Sort();

        List<float> highestScoresList = new List<float>(0);

        for (int i = scoresList.Count / discardedMovesPart; i < scoresList.Count; i++)
        {
            highestScoresList.Add(scoresList[i]);
        }

        float score = highestScoresList[RoundFloatToInt((highestScoresList.Count - 1) * _difficultyRate / 100)];

        AvailableMove chosenMove = new AvailableMove();

        ShuffleList(ref availableMoves);

        foreach (AvailableMove am in availableMoves)
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

        List<AvailableMove> availableMoves = new List<AvailableMove>(0);

        AvailableMove currentMove = new AvailableMove();

        foreach (Move m in moves)
        {
            Board newBoard;

            if (_board.currentTurn == Board.Turn.Black)
            {
                newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.White, _board.playerBlack);
            }
            else
            {
                newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.Black, _board.playerBlack);
            }

            newBoard.MakeMove(m);

            currentMove = RndABMinimax(newBoard, _currentTurn, _currentDepth + 1, _maxDepth, _alpha, _beta, _startingTime, _maxThinkingTime);

            availableMoves.Add(new AvailableMove() { move = m, score = currentMove.score });
        }

        return availableMoves;
    }

    public static float UpdateDifficultyRate(Move _chosenMove, List<AvailableMove> _movesList, float _lastDifficultyRate, ref List<float> _playerPerformancesList)
    {
        float score = 0;

        for (int i = 0; i < _movesList.Count; i++)
        {
            if (_movesList[i].move.from == _chosenMove.from && _movesList[i].move.to == _chosenMove.to)
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

        scoresList.Sort();

        if (scoresList.Count == 1)
        {
            return _lastDifficultyRate;
        }

        float currentPerformance = scoresList.IndexOf(score) * 100 / (scoresList.Count - 1);

        _playerPerformancesList.Add(currentPerformance);

        float sum = 0f;

        for (int i = 1; i <= _playerPerformancesList.Count; i++)
        {
            sum += _playerPerformancesList[i - 1] * i;
        }

        int n = _playerPerformancesList.Count * (_playerPerformancesList.Count + 1) / 2;

        return sum / n;
    }

    static void ShuffleList(ref List<AvailableMove> _list)
    {
        for (int i = 0; i < _list.Count; i++)
        {
            AvailableMove temp = _list[i];
            int idx = Random.Range(i, _list.Count);
            _list[i] = _list[idx];
            _list[idx] = temp;
        }
    }

    static int RoundFloatToInt(float _number)
    {
        int prev = (int)_number;

        if (_number - prev < prev + 1 - _number)
        {
            return prev;
        }
        else
        {
            return prev + 1;
        }
    }
}
