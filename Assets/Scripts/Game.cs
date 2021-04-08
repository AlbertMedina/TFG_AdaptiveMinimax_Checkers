using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject board;
    [SerializeField] GameObject blackChecker;
    [SerializeField] GameObject blackKing;
    [SerializeField] GameObject whiteChecker;
    [SerializeField] GameObject whiteKing;

    [Header("Settings")]
    [SerializeField] GameMode gameMode;


    private enum GameMode { Player_Black, Player_White, AI_vs_AI }

    private struct AvailableMove
    {
        public Move move;
        public float score;
    }

    private Board gameBoard;

    private Vector2Int selectedPiece;
    private List<Move> selectedPieceMoves;

    void Start()
    {
        gameBoard = new Board();

        selectedPiece = Vector2Int.zero;

        selectedPieceMoves = new List<Move>(0);

        DrawBoard(gameBoard);
    }

    void Update()
    {
        switch (gameMode)
        {
            case GameMode.Player_Black:

                if(gameBoard.turn == Board.Turn.Black)
                {
                    PlayerTurn();
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        AvailableMove chosenMove = Minimax(gameBoard, gameBoard.turn, 0, 3);

                        gameBoard.MakeMove(chosenMove.move);

                        DrawMove(chosenMove.move);

                        gameBoard.ChangeTurn();
                    }
                }

                break;

            case GameMode.Player_White:
                break;
            case GameMode.AI_vs_AI:

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    AvailableMove chosenMove = Minimax(gameBoard, gameBoard.turn, 0, 3);

                    gameBoard.MakeMove(chosenMove.move);

                    DrawMove(chosenMove.move);

                    gameBoard.ChangeTurn();
                }

                break;
        }
    }

    void PlayerTurn()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if(hit.transform.childCount > 0 && hit.transform.GetChild(0).tag == "BlackChecker")
                {
                    selectedPiece = GetPositionVector(hit.transform);

                    selectedPieceMoves = gameBoard.GetPieceMoves(selectedPiece);

                    Debug.Log(selectedPiece);

                    foreach (Move m in selectedPieceMoves)
                    {
                        Debug.Log(m.to);
                    }
                    //Debug.Log(hit.transform.name + ", " + hit.transform.parent.name);
                }
                else
                {
                    foreach (Move m in selectedPieceMoves)
                    {
                        if (m.to == GetPositionVector(hit.transform))
                        {
                            gameBoard.MakeMove(m);
                            DrawMove(m);

                            gameBoard.ChangeTurn();

                            selectedPiece = Vector2Int.zero;

                            selectedPieceMoves = new List<Move>(0);
                        }
                    }
                }
                
                
            }
        }
    }

    Vector2Int GetPositionVector(Transform t)
    {
        Vector2Int v = Vector2Int.zero;

        for (int i = 0; i < t.parent.childCount; i++)
        {
            if (t.parent.GetChild(i) == t)
            {
                v.x = i;
                break;
            }
        }       

        for (int i = 0; i < t.parent.parent.childCount; i++)
        {
            if (t.parent.parent.GetChild(i) == t.parent)
            {
                v.y = i;
                break;
            }
        }
        
        return v;
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