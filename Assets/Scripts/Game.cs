using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] GameObject board;
    [SerializeField] GameObject blackChecker;
    [SerializeField] GameObject blackKing;
    [SerializeField] GameObject whiteChecker;
    [SerializeField] GameObject whiteKing;

    struct AvailableMove
    {
        public Move move;
        public float score;
    }

    Board gameBoard;

    void Start()
    {
        gameBoard = new Board();

        DrawBoard(gameBoard);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            AvailableMove chosenMove = Minimax(gameBoard, gameBoard.turn, 0, 5);

            gameBoard.MakeMove(chosenMove.move);

            DrawMove(chosenMove.move);

            gameBoard.ChangeTurn();
        }
    }

    void DrawBoard(Board _board)
    {
        for (int i = 0; i < _board.boardState.GetLength(0); i++)
        {
            for (int j = 0; j < _board.boardState.GetLength(1); j++)
            {
                switch (_board.boardState[i, j])
                {
                    case Board.Square.Black_Checker:
                        Instantiate(blackChecker, board.transform.GetChild(i).GetChild(j).transform.position, Quaternion.identity, board.transform.GetChild(i).GetChild(j).transform);
                        break;
                    case Board.Square.White_Checker:
                        Instantiate(whiteChecker, board.transform.GetChild(i).GetChild(j).transform.position, Quaternion.identity, board.transform.GetChild(i).GetChild(j).transform);
                        break;
                }
            }
        }
    }

    void DrawMove(Move _move)
    {
        board.transform.GetChild(_move.from.y).GetChild(_move.from.x).GetChild(0).transform.position = board.transform.GetChild(_move.to.y).GetChild(_move.to.x).transform.position;
        board.transform.GetChild(_move.from.y).GetChild(_move.from.x).GetChild(0).transform.parent = board.transform.GetChild(_move.to.y).GetChild(_move.to.x).transform;

        if (_move.to.y <= 0 || _move.to.y >= gameBoard.boardState.GetLength(0) - 1)
        {
            if (board.transform.GetChild(_move.to.y).GetChild(_move.to.x).GetChild(0).tag == "BlackChecker")
            {
                Destroy(board.transform.GetChild(_move.to.y).GetChild(_move.to.x).GetChild(0).gameObject);
                Instantiate(blackKing, board.transform.GetChild(_move.to.y).GetChild(_move.to.x).transform.position, Quaternion.identity, board.transform.GetChild(_move.to.y).GetChild(_move.to.x).transform);
            }
            else if (board.transform.GetChild(_move.to.y).GetChild(_move.to.x).GetChild(0).tag == "WhiteChecker")
            {
                Destroy(board.transform.GetChild(_move.to.y).GetChild(_move.to.x).GetChild(0).gameObject);
                Instantiate(whiteKing, board.transform.GetChild(_move.to.y).GetChild(_move.to.x).transform.position, Quaternion.identity, board.transform.GetChild(_move.to.y).GetChild(_move.to.x).transform);
            }
        }


        foreach (Vector2Int jumped in _move.jumped)
        {
            Destroy(board.transform.GetChild(jumped.y).GetChild(jumped.x).GetChild(0).gameObject);
        }
    }

    AvailableMove Minimax(Board _board, Board.Turn _turn, int _currentDepth, int _maxDepth)
    {
        AvailableMove bestMove = new AvailableMove();

        bestMove.move = null;

        if (_currentDepth >= _maxDepth)
        {
            bestMove.score = _board.Evaluate(_turn);
            return bestMove;
        }

        List<Move> availableMoves = _board.GetAllMoves();

        if (availableMoves.Count == 0)
        {
            bestMove.score = _board.Evaluate(_turn);
            return bestMove;
        }

        if (_turn == _board.turn) bestMove.score = -Mathf.Infinity;
        else bestMove.score = Mathf.Infinity;

        AvailableMove currentMove = new AvailableMove();

        foreach (Move m in availableMoves)
        {
            Board newBoard;

            if (_board.turn == Board.Turn.Black) newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.White);
            else newBoard = new Board((Board.Square[,])_board.boardState.Clone(), Board.Turn.Black);

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

            //if(_currentDepth == 0) Debug.Log(currentMove.score);
        }

        return bestMove;
    }
}