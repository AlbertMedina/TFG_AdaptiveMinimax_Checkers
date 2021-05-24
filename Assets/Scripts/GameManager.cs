using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    [SerializeField] float breakingAlgorithmTime;
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
    private float currentDifficultyRate;
    private List<float> difficultyRatesList;

    private int maxSearchingDepth;

    private bool playerCanJump;

    private bool gameOver;

    //Movement
    private bool moving;
    private Transform movingPiece;
    private Transform movingTarget;

    void Start()
    {
        if (gameMode == GameMode.Player_White)
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
        currentDifficultyRate = 50f;
        difficultyRatesList = new List<float>(0);

        playerCanJump = false;

        gameOver = false;

        maxSearchingDepth = initialSearchingDepth;

        moving = false;

        StartBoard(gameBoard);

        playerAvailableMoves = Algorithm.GetSortedMoves(gameBoard, gameBoard.currentTurn, 0, maxSearchingDepth, -Mathf.Infinity, Mathf.Infinity, Time.realtimeSinceStartup, breakingAlgorithmTime);
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
                    else MainAITurn();

                    break;

                case GameMode.Player_White:

                    if (gameBoard.currentTurn == Board.Turn.White) PlayerTurn();
                    else MainAITurn();

                    break;
                case GameMode.AI_vs_AI:

                    if (gameBoard.currentTurn == Board.Turn.Black) MainAITurn();
                    else SecondaryAITurn();

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

                            currentDifficultyRate = Algorithm.UpdateDifficultyRate(m, playerAvailableMoves, currentDifficultyRate, ref difficultyRatesList);

                            ChangeTurn();

                            selectedPiece = Vector2Int.zero;

                            selectedPieceMoves = new List<Move>(0);
                        }
                    }
                }
            }
        }
    }

    void MainAITurn()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float timeSinceAlgorithmCall = Time.realtimeSinceStartup;

            Algorithm.AvailableMove algorithmChosenMove;

            algorithmChosenMove = Algorithm.MyMinimax(gameBoard, gameBoard.currentTurn, 0, maxSearchingDepth, -Mathf.Infinity, Mathf.Infinity, currentDifficultyRate, timeSinceAlgorithmCall, breakingAlgorithmTime);

            if (algorithmChosenMove.move == null)
            {
                GameOver();
            }
            else
            {
                if (Time.realtimeSinceStartup - timeSinceAlgorithmCall < minimumThinkingTime && algorithmChosenMove.move.jumped.Count == 0) maxSearchingDepth++;
                else if (maxSearchingDepth > 1 && Time.realtimeSinceStartup - timeSinceAlgorithmCall > maximumThinkingTime) maxSearchingDepth--;

                gameBoard.MakeMove(algorithmChosenMove.move);
                UpdateBoard(algorithmChosenMove.move);
                ChangeTurn();

                playerAvailableMoves = Algorithm.GetSortedMoves(gameBoard, gameBoard.currentTurn, 0, maxSearchingDepth, -Mathf.Infinity, Mathf.Infinity, Time.realtimeSinceStartup, breakingAlgorithmTime);
            }
        }
    }

    void SecondaryAITurn()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float timeSinceAlgorithmCall = Time.realtimeSinceStartup;

            Algorithm.AvailableMove algorithmChosenMove;

            algorithmChosenMove = Algorithm.MyMinimax(gameBoard, gameBoard.currentTurn, 0, maxSearchingDepth, -Mathf.Infinity, Mathf.Infinity, presetDifficultyRate, timeSinceAlgorithmCall, breakingAlgorithmTime);

            if (algorithmChosenMove.move == null)
            {
                GameOver();
            }
            else
            {
                if (Time.realtimeSinceStartup - timeSinceAlgorithmCall < minimumThinkingTime && algorithmChosenMove.move.jumped.Count == 0) maxSearchingDepth++;
                else if (maxSearchingDepth > 1 && Time.realtimeSinceStartup - timeSinceAlgorithmCall > maximumThinkingTime) maxSearchingDepth--;

                gameBoard.MakeMove(algorithmChosenMove.move);
                UpdateBoard(algorithmChosenMove.move);

                currentDifficultyRate = Algorithm.UpdateDifficultyRate(algorithmChosenMove.move, playerAvailableMoves, currentDifficultyRate, ref difficultyRatesList);
                Debug.Log(currentDifficultyRate);
                ChangeTurn();
            }
        }
    }

    void ChangeTurn()
    {
        gameBoard.ChangeTurn();

        List<Move> moves = gameBoard.GetAllMoves();

        if (moves.Count == 0)
        {
            GameOver();
            return;
        }

        if (moves[0].jumped.Count > 0) playerCanJump = true;
        else playerCanJump = false;
    }

    void GameOver()
    {
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
        movingPiece = VectorToTransform(_move.from).GetChild(0);
        movingTarget = VectorToTransform(_move.to);

        movingPiece.parent = movingTarget;

        moving = true;

        StartCoroutine(MovePiece(movingPiece, movingTarget, _move, 1f));
    }

    private IEnumerator MovePiece(Transform _piece, Transform _target, Move _move, float _time)
    {
        Vector3 initPos = _piece.position;
        float elapsedTime = 0;

        if (_move.jumped.Count > 0)
        {
            Vector2Int target = _move.from;

            for (int i = _move.jumped.Count - 1; i > 0; i--)
            {
                target += (_move.jumped[i] - target) * 2;

                while (elapsedTime < _time)
                {
                    _piece.position = Vector3.Lerp(initPos, VectorToTransform(target).position, (elapsedTime / _time));
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                Destroy(VectorToTransform(_move.jumped[i]).GetChild(0).gameObject);
                initPos = VectorToTransform(target).position;
                elapsedTime = 0;
            }

            while (elapsedTime < _time)
            {
                _piece.position = Vector3.Lerp(initPos, _target.position, (elapsedTime / _time));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            Destroy(VectorToTransform(_move.jumped[0]).GetChild(0).gameObject);
        }
        else
        {
            while (elapsedTime < _time)
            {
                _piece.position = Vector3.Lerp(initPos, _target.position, (elapsedTime / _time));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        moving = false;

        //KING
        if (_move.to.y <= 0 || _move.to.y >= gameBoard.boardState.GetLength(0) - 1)
        {
            if (movingPiece.tag == "BlackChecker")
            {
                Destroy(movingPiece.gameObject);
                movingPiece = Instantiate(blackKing, movingTarget.position, Quaternion.identity, movingTarget).transform;
            }
            else if (movingPiece.tag == "WhiteChecker")
            {
                Destroy(movingPiece.gameObject);
                movingPiece = Instantiate(whiteKing, movingTarget.position, Quaternion.identity, movingTarget).transform;
            }
        }

        if (gameOver)
        {
            Debug.Log("GameOver " + gameBoard.currentTurn);
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