using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
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

    private Board gameBoard;

    private string checkersTag;
    private string kingsTag;

    private Vector2Int selectedPiece;
    private List<Move> selectedPieceMoves;

    private bool playerCanJump;

    private bool gameOver;

    void Start()
    {
        if(gameMode == GameMode.Player_White)
        {
            checkersTag = "WhiteChecker";
            kingsTag = "WhiteKing";
            gameBoard = new Board(false);
        }
        else
        {
            checkersTag = "BlackChecker";
            kingsTag = "BlackKing";
            gameBoard = new Board(true);
        }

        selectedPiece = Vector2Int.zero;

        selectedPieceMoves = new List<Move>(0);

        playerCanJump = false;

        gameOver = false;

        StartBoard(gameBoard);
    }

    void Update()
    {
        if (!gameOver)
        {
            switch (gameMode)
            {
                case GameMode.Player_Black:

                    if (gameBoard.currentTurn == Board.Turn.Black) PlayerTurn();
                    else AITurn();

                    break;

                case GameMode.Player_White:

                    if (gameBoard.currentTurn == Board.Turn.White) PlayerTurn();
                    else AITurn();

                    break;
                case GameMode.AI_vs_AI:

                    AITurn();

                    break;
            }
        }
    }  

    #region Turn
    void PlayerTurn()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.transform.childCount > 0 && (hit.transform.GetChild(0).tag == checkersTag || hit.transform.GetChild(0).tag == kingsTag))
                {
                    selectedPiece = TransformToVector(hit.transform);

                    if (playerCanJump) selectedPieceMoves = gameBoard.GetPieceJumps(selectedPiece);
                    else selectedPieceMoves = gameBoard.GetPieceMoves(selectedPiece);

                    ShowHelpers();
                }
                else
                {
                    foreach (Move m in selectedPieceMoves)
                    {
                        if (m.to == TransformToVector(hit.transform))
                        {
                            gameBoard.MakeMove(m);
                            UpdateBoard(m);
                            RemoveHelpers();

                            ChangeTurn();

                            selectedPiece = Vector2Int.zero;

                            selectedPieceMoves = new List<Move>(0);
                        }
                    }
                }
            }
        }
    }

    void AITurn()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Algorithm.AvailableMove chosenMove = Algorithm.RndMinimax(gameBoard, gameBoard.currentTurn, 0, 3);
            Algorithm.AvailableMove chosenMove = Algorithm.RndABMinimax(gameBoard, gameBoard.currentTurn, 0, 7, -Mathf.Infinity, Mathf.Infinity);

            if (chosenMove.move == null)
            {
                GameOver();
                return;
            }

            gameBoard.MakeMove(chosenMove.move);

            UpdateBoard(chosenMove.move);

            ChangeTurn();
        }
    }

    void ChangeTurn()
    {
        gameBoard.ChangeTurn();

        if ((gameMode == GameMode.Player_Black && gameBoard.currentTurn == Board.Turn.Black) || (gameMode == GameMode.Player_White && gameBoard.currentTurn == Board.Turn.White))
        {  
            playerCanJump = false;

            for (int i = 0; i < board.transform.childCount; i++)
            {
                for (int j = 0; j < board.transform.GetChild(i).childCount; j++)
                {
                    if (board.transform.GetChild(i).GetChild(j).childCount > 0 && (board.transform.GetChild(i).GetChild(j).GetChild(0).tag == checkersTag || board.transform.GetChild(i).GetChild(j).GetChild(0).tag == kingsTag))
                    {        
                        if (gameBoard.GetPieceJumps(new Vector2Int(j, i)).Count > 0)
                        {
                            playerCanJump = true;
                            return;
                        }
                    }
                }
            }

            for (int i = 0; i < board.transform.childCount; i++)
            {
                for (int j = 0; j < board.transform.GetChild(i).childCount; j++)
                {
                    if (board.transform.GetChild(i).GetChild(j).childCount > 0 && (board.transform.GetChild(i).GetChild(j).GetChild(0).tag == checkersTag || board.transform.GetChild(i).GetChild(j).GetChild(0).tag == kingsTag))
                    {
                        if (gameBoard.GetPieceMoves(new Vector2Int(j, i)).Count > 0) return;
                    }
                }
            }

            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log("GameOver " + gameBoard.currentTurn);
        gameOver = true;
    }
    #endregion

    #region Board
    void StartBoard(Board _board)
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

    void UpdateBoard(Move _move)
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
    #endregion

    #region Helpers
    void ShowHelpers()
    {
        RemoveHelpers();

        if (selectedPieceMoves.Count > 0) VectorToTransform(selectedPiece).GetComponent<SpriteRenderer>().color = Color.green;
        else VectorToTransform(selectedPiece).GetComponent<SpriteRenderer>().color = Color.red;

        VectorToTransform(selectedPiece).GetComponent<SpriteRenderer>().enabled = true;

        foreach (Move m in selectedPieceMoves)
        {
            VectorToTransform(m.to).GetComponent<SpriteRenderer>().color = Color.green;
            VectorToTransform(m.to).GetComponent<SpriteRenderer>().enabled = true;

            foreach (Vector2Int v in m.jumped)
            {
                VectorToTransform(v).GetComponent<SpriteRenderer>().color = Color.red;
                VectorToTransform(v).GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }

    void RemoveHelpers()
    {
        for (int i = 0; i < board.transform.childCount; i++)
        {
            for (int j = 0; j < board.transform.GetChild(i).childCount; j++)
            {
                if (board.transform.GetChild(i).GetChild(j).GetComponent<SpriteRenderer>() != null)
                {
                    board.transform.GetChild(i).GetChild(j).GetComponent<SpriteRenderer>().enabled = false;
                }
            }
        }
    }
    #endregion

    #region Math
    Vector2Int TransformToVector(Transform _t)
    {
        Vector2Int v = Vector2Int.zero;

        for (int i = 0; i < _t.parent.childCount; i++)
        {
            if (_t.parent.GetChild(i) == _t)
            {
                v.x = i;
                break;
            }
        }

        for (int i = 0; i < _t.parent.parent.childCount; i++)
        {
            if (_t.parent.parent.GetChild(i) == _t.parent)
            {
                v.y = i;
                break;
            }
        }

        return v;
    }

    Transform VectorToTransform(Vector2Int _v)
    {
        return board.transform.GetChild(_v.y).GetChild(_v.x);
    }
    #endregion
}