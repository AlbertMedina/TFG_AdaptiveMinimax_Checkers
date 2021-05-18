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

    [Header("AI")]
    [SerializeField] float minimumThinkingTime;
    [SerializeField] float maximumThinkingTime;
    [SerializeField] int initialSearchingDepth;

    [Header("Secondary AI")]
    [SerializeField] [Range(0.0f, 100.0f)] float presetDifficultyRate;

    private enum GameMode { Player_Black, Player_White, AI_vs_AI }

    private Board gameBoard;

    private string checkersTag;
    private string kingsTag;

    private Vector2Int selectedPiece;
    private List<Move> selectedPieceMoves;

    private List<Algorithm.AvailableMove> playerAvailableMoves;
    private float difficultyRate;

    private bool playerCanJump;

    private bool gameOver;

    private int maxSearchingDepth;

    //Movement
    private bool moving;
    private Transform movingPiece;
    private Transform movingTarget;

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

        playerAvailableMoves = new List<Algorithm.AvailableMove>(0);

        difficultyRate = 50f;

        playerCanJump = false;

        gameOver = false;

        maxSearchingDepth = initialSearchingDepth;

        moving = false;

        StartBoard(gameBoard);

        playerAvailableMoves = Algorithm.GetSortedMoves(gameBoard, gameBoard.currentTurn, 0, maxSearchingDepth, -Mathf.Infinity, Mathf.Infinity, Time.realtimeSinceStartup, 5f);
    }

    void Update()
    {
        if (!gameOver)
        {
            if (moving)
            {
                return;
            }

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

                            difficultyRate = Algorithm.UpdateDifficultyRate(difficultyRate, m, playerAvailableMoves);

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
            float timeSinceAlgorithmCall = Time.realtimeSinceStartup;

            Algorithm.AvailableMove chosenMove;

            if (gameMode == GameMode.AI_vs_AI && gameBoard.currentTurn == Board.Turn.White)
            {
                chosenMove = Algorithm.MyMinimax(gameBoard, gameBoard.currentTurn, 0, maxSearchingDepth, -Mathf.Infinity, Mathf.Infinity, presetDifficultyRate, timeSinceAlgorithmCall, 5f);
            }
            else
            {
                chosenMove = Algorithm.MyMinimax(gameBoard, gameBoard.currentTurn, 0, maxSearchingDepth, -Mathf.Infinity, Mathf.Infinity, difficultyRate, timeSinceAlgorithmCall, 5f);
            }

            if (chosenMove.move == null)
            {
                GameOver();
            }
            else
            {
                if (Time.realtimeSinceStartup - timeSinceAlgorithmCall < minimumThinkingTime && chosenMove.move.jumped.Count == 0) maxSearchingDepth++;
                else if (maxSearchingDepth > 1 && Time.realtimeSinceStartup - timeSinceAlgorithmCall > maximumThinkingTime) maxSearchingDepth--;

                gameBoard.MakeMove(chosenMove.move);
                UpdateBoard(chosenMove.move);
                ChangeTurn();
            }
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
                            playerAvailableMoves = Algorithm.GetSortedMoves(gameBoard, gameBoard.currentTurn, 0, maxSearchingDepth, -Mathf.Infinity, Mathf.Infinity, Time.realtimeSinceStartup, 5f);
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
                        if (gameBoard.GetPieceMoves(new Vector2Int(j, i)).Count > 0)
                        {
                            playerAvailableMoves = Algorithm.GetSortedMoves(gameBoard, gameBoard.currentTurn, 0, maxSearchingDepth, -Mathf.Infinity, Mathf.Infinity, Time.realtimeSinceStartup, 5f);
                            return;
                        }
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
        movingPiece = board.transform.GetChild(_move.from.y).GetChild(_move.from.x).GetChild(0).transform;
        movingTarget = board.transform.GetChild(_move.to.y).GetChild(_move.to.x).transform;

        movingPiece.parent = movingTarget;

        moving = true;

        //KING
        if (_move.to.y <= 0 || _move.to.y >= gameBoard.boardState.GetLength(0) - 1)
        {
            if (movingPiece.tag == "BlackChecker")
            {
                Destroy(movingPiece.gameObject);
                movingPiece = Instantiate(blackKing, board.transform.GetChild(_move.to.y).GetChild(_move.to.x).transform.position, Quaternion.identity, board.transform.GetChild(_move.to.y).GetChild(_move.to.x).transform).transform;
            }
            else if (movingPiece.tag == "WhiteChecker")
            {
                Destroy(movingPiece.gameObject);
                movingPiece = Instantiate(whiteKing, board.transform.GetChild(_move.from.y).GetChild(_move.from.x).transform.position, Quaternion.identity, board.transform.GetChild(_move.to.y).GetChild(_move.to.x).transform).transform;
            }
        }

        //DESTROY JUMPED
        foreach (Vector2Int jumped in _move.jumped)
        {
            Destroy(board.transform.GetChild(jumped.y).GetChild(jumped.x).GetChild(0).gameObject);
        }

        StartCoroutine(SmoothLerp(movingPiece, movingTarget, 1f));

        //board.transform.GetChild(_move.from.y).GetChild(_move.from.x).GetChild(0).transform.position = board.transform.GetChild(_move.to.y).GetChild(_move.to.x).transform.position;
        //board.transform.GetChild(_move.from.y).GetChild(_move.from.x).GetChild(0).transform.parent = board.transform.GetChild(_move.to.y).GetChild(_move.to.x).transform;  
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

    private IEnumerator SmoothLerp(Transform _piece, Transform _target, float _time)
    {
        Vector3 initPos = _piece.position;
        float elapsedTime = 0;

        while (elapsedTime < _time)
        {
            _piece.position = Vector3.Lerp(initPos, _target.position, (elapsedTime / _time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        moving = false;
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