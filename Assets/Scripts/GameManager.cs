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

    [Header("AI")]
    [SerializeField] float minimumThinkingTime;
    [SerializeField] float maximumThinkingTime;
    [SerializeField] float breakingAlgorithmTime;
    [SerializeField] int initialSearchingDepth;

    private UIManager UIManager;
    
    private enum GameMode { Player_Black, Player_White, AI_vs_AI }
    private GameMode gameMode;

    public enum AvailableAlgorithms { Adaptive_Minimax, Minimax, AB_Minimax}
    private AvailableAlgorithms selectedAlgorithm = AvailableAlgorithms.Adaptive_Minimax;

    private Board gameBoard;

    private string checkersTag;
    private string kingsTag;

    private Vector2Int selectedPiece;
    private List<Move> selectedPieceMoves;

    private List<Algorithm.AvailableMove> playerAvailableMoves;
    private float currentDifficultyRate;
    private List<float> difficultyRatesList;

    private float presetDifficultyRate = 50f;

    private int maxSearchingDepth;

    private bool playerCanJump;

    private bool gameOver;

    private bool moveAutomatically;

    private bool playerBlack;

    private int movesToDraw;

    //Movement
    private bool moving;
    private Transform movingPiece;
    private Transform movingTarget;

    #region MonoBehaviour
    void Start()
    {
        UIManager = GameObject.FindObjectOfType<UIManager>();

        selectedPiece = Vector2Int.zero;

        selectedPieceMoves = new List<Move>(0);

        playerAvailableMoves = new List<Algorithm.AvailableMove>(0);
        currentDifficultyRate = 50f;
        difficultyRatesList = new List<float>(0);

        playerCanJump = false;

        gameOver = true;

        maxSearchingDepth = initialSearchingDepth;

        moveAutomatically = false;

        movesToDraw = 20;

        moving = false;
    }

    void Update()
    {
        if (!gameOver)
        {
            if (moving) return;

            switch (gameMode)
            {
                case GameMode.Player_Black:

                    if (gameBoard.currentTurn == Board.Turn.Black)
                    {
                        PlayerTurn();
                    }
                    else
                    {
                        AdaptiveAITurn();
                    }

                    break;

                case GameMode.Player_White:

                    if (gameBoard.currentTurn == Board.Turn.White)
                    {
                        PlayerTurn();
                    }
                    else
                    {
                        AdaptiveAITurn();
                    }

                    break;

                case GameMode.AI_vs_AI:

                    if ((gameBoard.currentTurn == Board.Turn.Black && playerBlack) || (gameBoard.currentTurn == Board.Turn.White && !playerBlack))
                    {
                        PlayerAITurn();
                    }
                    else
                    {
                        AdaptiveAITurn();
                    }

                    break;
            }
        }
    }
    #endregion

    #region GameSettings
    public void SetGameMode(bool _player, bool _black)
    {
        if (_player)
        {
            if (_black)
            {
                gameMode = GameMode.Player_Black;
                checkersTag = "BlackChecker";
                kingsTag = "BlackKing";
            }
            else
            {
                gameMode = GameMode.Player_White;
                checkersTag = "WhiteChecker";
                kingsTag = "WhiteKing";
                
            }

            gameBoard = new Board(_black);
        }
        else
        {
            gameMode = GameMode.AI_vs_AI;
            gameBoard = new Board(_black);
        }

        playerBlack = _black;
    }

    public void SetAlgorithm(AvailableAlgorithms _algorithm)
    {
        selectedAlgorithm = _algorithm;
    }

    public void SetDifficultyRate(float _difficultyRate)
    {
        presetDifficultyRate = _difficultyRate;
    } 

    public void SetAutomaticMovement(bool _moveAutomatically)
    {
        moveAutomatically = _moveAutomatically;
    }

    public void StartGame()
    {
        gameOver = false;

        StartBoard(gameBoard);

        if (gameMode != GameMode.AI_vs_AI)
        {
            playerAvailableMoves = Algorithm.GetSortedMoves(gameBoard, gameBoard.currentTurn, 0, maxSearchingDepth, -Mathf.Infinity, Mathf.Infinity, Time.realtimeSinceStartup, breakingAlgorithmTime);
        }
    }
    #endregion 

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

                    if (playerCanJump)
                    {
                        selectedPieceMoves = gameBoard.GetPieceJumps(selectedPiece);
                    }
                    else
                    {
                        selectedPieceMoves = gameBoard.GetPieceMoves(selectedPiece);
                    }

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

                            if (difficultyRatesList.Count > 0)
                            {
                                UIManager.UpdateInfo(currentDifficultyRate, difficultyRatesList[difficultyRatesList.Count - 1]);
                            }

                            ChangeTurn();

                            selectedPiece = Vector2Int.zero;

                            selectedPieceMoves = new List<Move>(0);
                        }
                    }
                }
            }
        }
    }

    void AdaptiveAITurn()
    {
        if (Input.GetKeyDown(KeyCode.Space) || moveAutomatically)
        {
            float timeSinceAlgorithmCall = Time.realtimeSinceStartup;

            Algorithm.AvailableMove algorithmChosenMove;

            algorithmChosenMove = Algorithm.AdaptiveMinimax(gameBoard, gameBoard.currentTurn, 0, maxSearchingDepth, -Mathf.Infinity, Mathf.Infinity, timeSinceAlgorithmCall, breakingAlgorithmTime, currentDifficultyRate);

            if (algorithmChosenMove.move == null)
            {
                GameOver();
            }
            else
            {
                if (Time.realtimeSinceStartup - timeSinceAlgorithmCall < minimumThinkingTime && algorithmChosenMove.move.jumped.Count == 0)
                {
                    maxSearchingDepth++;
                }
                else if (maxSearchingDepth > 1 && Time.realtimeSinceStartup - timeSinceAlgorithmCall > maximumThinkingTime)
                {
                    maxSearchingDepth--;
                }

                gameBoard.MakeMove(algorithmChosenMove.move);
                UpdateBoard(algorithmChosenMove.move);
                ChangeTurn();

                if (gameMode != GameMode.AI_vs_AI)
                {
                    playerAvailableMoves = Algorithm.GetSortedMoves(gameBoard, gameBoard.currentTurn, 0, maxSearchingDepth, -Mathf.Infinity, Mathf.Infinity, Time.realtimeSinceStartup, breakingAlgorithmTime);
                }
            }
        }
    }

    void PlayerAITurn()
    {
        if (Input.GetKeyDown(KeyCode.Space) || moveAutomatically)
        {
            float timeSinceAlgorithmCall = Time.realtimeSinceStartup;

            Algorithm.AvailableMove algorithmChosenMove;

            switch (selectedAlgorithm)
            {
                case AvailableAlgorithms.Adaptive_Minimax:
                    algorithmChosenMove = Algorithm.AdaptiveMinimax(gameBoard, gameBoard.currentTurn, 0, maxSearchingDepth, -Mathf.Infinity, Mathf.Infinity, timeSinceAlgorithmCall, breakingAlgorithmTime, presetDifficultyRate, ref playerAvailableMoves);
                    break;
                case AvailableAlgorithms.Minimax:
                    algorithmChosenMove = Algorithm.RndMinimax(gameBoard, gameBoard.currentTurn, 0, maxSearchingDepth, timeSinceAlgorithmCall, breakingAlgorithmTime, ref playerAvailableMoves);
                    break;
                default:      
                    algorithmChosenMove = Algorithm.RndABMinimax(gameBoard, gameBoard.currentTurn, 0, maxSearchingDepth, -Mathf.Infinity, Mathf.Infinity, timeSinceAlgorithmCall, breakingAlgorithmTime, ref playerAvailableMoves);
                    break;
            }

            if (algorithmChosenMove.move == null)
            {
                GameOver();
            }
            else
            {
                if (Time.realtimeSinceStartup - timeSinceAlgorithmCall < minimumThinkingTime && algorithmChosenMove.move.jumped.Count == 0)
                {
                    maxSearchingDepth++;
                }
                else if (maxSearchingDepth > 1 && Time.realtimeSinceStartup - timeSinceAlgorithmCall > maximumThinkingTime)
                {
                    maxSearchingDepth--;
                }

                gameBoard.MakeMove(algorithmChosenMove.move);
                UpdateBoard(algorithmChosenMove.move);

                currentDifficultyRate = Algorithm.UpdateDifficultyRate(algorithmChosenMove.move, playerAvailableMoves, currentDifficultyRate, ref difficultyRatesList);

                if (difficultyRatesList.Count > 0)
                {
                    UIManager.UpdateInfo(currentDifficultyRate, difficultyRatesList[difficultyRatesList.Count - 1]);
                }

                ChangeTurn();
            }
        }
    }

    void ChangeTurn()
    {
        gameBoard.ChangeTurn();

        List<Move> moves = gameBoard.GetAllMoves();

        if (moves.Count == 0 || movesToDraw <= 0)
        {
            GameOver();
            return;
        }

        if (moves[0].jumped.Count > 0)
        {
            playerCanJump = true;
        }
        else
        {
            playerCanJump = false;
        }
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
            movesToDraw = 20;
            
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
            if (_piece.tag == "BlackChecker" || _piece.tag == "WhiteChecker")
            {
                movesToDraw = 20;
            }
            else
            {
                movesToDraw--;
            }
            
            while (elapsedTime < _time)
            {
                _piece.position = Vector3.Lerp(initPos, _target.position, (elapsedTime / _time));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        moving = false;

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

        UIManager.UpdateTurn(gameBoard.currentTurn == Board.Turn.Black);

        if (gameOver)
        {
            UIManager.GameOver(movesToDraw <= 0, gameBoard.currentTurn != Board.Turn.Black, currentDifficultyRate);
        }
    }
    #endregion

    #region Helpers
    void ShowHelpers()
    {
        RemoveHelpers();

        if (selectedPieceMoves.Count > 0)
        {
            VectorToTransform(selectedPiece).GetComponent<SpriteRenderer>().color = Color.green;
        }
        else
        {
            VectorToTransform(selectedPiece).GetComponent<SpriteRenderer>().color = Color.red;
        }

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