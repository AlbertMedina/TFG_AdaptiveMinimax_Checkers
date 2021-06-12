using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    [Header("MAIN MENU")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject algorithmSelector;
    [SerializeField] private GameObject difficultyRateSlider;
    [SerializeField] private Text difficultyRate;

    [Header("SETTINGS MENU")]
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private Text minimumThinkingTimeText;
    [SerializeField] private Text maximumThinkingTimeText;
    [SerializeField] private Text breakingAlgorithmTimeText;
    [SerializeField] private Text initialSearchingDepthText;

    [Header("IN-GAME UI")]
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject autoMoveText;
    [SerializeField] private Text currentTurn;
    [SerializeField] private Text meanPerformance;
    [SerializeField] private Text lastPerformance;

    [Header("GAME OVER MENU")]
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private Text winnerText;
    [SerializeField] private Text finalPerformanceText;

    private GameManager gameManager;

    private bool player = true;
    private bool black = true;

    private float minimumThinkingTime = 0.3f;
    private float maximumThinkingTime = 1f;
    private float breakingAlgorithmTime = 3f;
    private int initialSearchingDepth = 6;

    void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    #region MainMenu
    public void SetColor(int _value)
    {
        if (_value == 0)
        {
            black = true;
        }
        else
        {
            black = false;
        }
    }

    public void SetGameMode(int _value)
    {
        if (_value == 0)
        {
            player = true;
        }
        else
        {
            player = false;
        }

        algorithmSelector.SetActive(!player);
    }    

    public void SetAlgorithm(int _value)
    {
        switch (_value)
        {
            case 0:
                gameManager.SetAlgorithm(GameManager.AvailableAlgorithms.Adaptive_Minimax);
                difficultyRateSlider.SetActive(true);
                break;
            case 1:
                gameManager.SetAlgorithm(GameManager.AvailableAlgorithms.Minimax);
                difficultyRateSlider.SetActive(false);
                break;
            case 2:
                gameManager.SetAlgorithm(GameManager.AvailableAlgorithms.AB_Minimax);
                difficultyRateSlider.SetActive(false);
                break;
        }
    }

    public void SetDifficultyRate(float _value)
    {
        gameManager.SetDifficultyRate(_value);
        difficultyRate.text = Mathf.RoundToInt(_value).ToString();
    }

    public void StartGame()
    {
        gameManager.SetGameMode(player, black);
        gameManager.StartGame();
        mainMenu.SetActive(false);
        inGameUI.SetActive(true);
    }

    public void OpenSettingsMenu()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void Close()
    {
        Application.Quit();
    }
    #endregion

    #region Settings
    public void SetMinimumThinkingTime(float _value)
    {
        minimumThinkingTime = _value;
        minimumThinkingTimeText.text = _value.ToString("F1");
    }

    public void SetMaximumThinkingTime(float _value)
    {
        maximumThinkingTime = _value;
        maximumThinkingTimeText.text = _value.ToString("F1");
    }

    public void SetBreakingAlgorithmTime(float _value)
    {
        breakingAlgorithmTime = _value;
        breakingAlgorithmTimeText.text = _value.ToString("F1");
    }

    public void SetInitialSearchingDepth(float _value)
    {
        initialSearchingDepth = Mathf.RoundToInt(_value);
        initialSearchingDepthText.text = Mathf.RoundToInt(_value).ToString();
    }

    public void BackToMenu()
    {
        gameManager.UpdateSettings(minimumThinkingTime, maximumThinkingTime, breakingAlgorithmTime, initialSearchingDepth);
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
    #endregion

    #region InGame
    public void UpdateTurn(bool _black)
    {
        if (_black)
        {
            currentTurn.text = "<b>Current Turn:</b> Black";
        }
        else
        {
            currentTurn.text = "<b>Current Turn:</b> White";
        }
    }
    
    public void UpdateInfo(float _meanPerformance, float _lastPerformance)
    {
        meanPerformance.text = "<b>Mean Performance:</b> " + _meanPerformance.ToString("F0") + "%";
        lastPerformance.text = "<b>Last Move Performance:</b> " + _lastPerformance.ToString("F0") + "%";
    }

    public void AutomaticMovement(bool _moveAutomatically)
    {
        gameManager.SetAutomaticMovement(_moveAutomatically);
        autoMoveText.SetActive(!_moveAutomatically);
    }
    #endregion

    #region GameOver
    public void GameOver(bool _draw, bool _blackWinner, float _meanPerformance)
    {
        inGameUI.SetActive(false);
        gameOverMenu.SetActive(true);

        if (_draw)
        {
            winnerText.text = "DRAW!";
        }
        else if (_blackWinner)
        {
            winnerText.text = "BLACK WINS!";
        }
        else
        {
            winnerText.text = "WHITE WINS!";
        }

        finalPerformanceText.text = "MEAN PERFORMANCE: " + _meanPerformance.ToString("F0") + "%";
    }
    
    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    #endregion
}
