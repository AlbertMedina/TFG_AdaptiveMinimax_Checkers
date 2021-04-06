using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    struct AvailableMove
    {
        public Move move;
        public float score;
    }

    Board gameBoard;

    void Start()
    {
        gameBoard = new Board();

        AvailableMove m = Minimax(gameBoard, gameBoard.turn, 0, 2);

        m.move.DebugMove();

        Debug.Log(m.score);
    }

    void Update()
    {

    }

    AvailableMove Minimax(Board _board, Board.Turn _turn, int _currentDepth, int _maxDepth)
    {
        AvailableMove bestMove = new AvailableMove();

        bestMove.move = null;

        if (_currentDepth >= _maxDepth)
        {
            bestMove.score = _board.Evaluate();
            return bestMove;
        }

        List<Move> availableMoves = _board.GetAllMoves();

        if (availableMoves.Count == 0)
        {
            bestMove.score = _board.Evaluate();
            return bestMove;
        }

        if (_turn == _board.turn) bestMove.score = -Mathf.Infinity;
        else bestMove.score = Mathf.Infinity;

        AvailableMove currentMove = new AvailableMove();

        foreach (Move m in availableMoves)
        {
            Board newBoard;

            if(_board.turn == Board.Turn.Black) newBoard = new Board(_board.boardState, Board.Turn.White);
            else newBoard = new Board(_board.boardState, Board.Turn.Black);

            newBoard.MakeMove(m);
            
            currentMove = Minimax(newBoard, _turn, _currentDepth + 1, _maxDepth);
            
            if (_turn == _board.turn)
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
}