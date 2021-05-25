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

    [Header("IN-GAME UI")]
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private Text currentRate;
    [SerializeField] private Text lastPerformance;

    [Header("GAME OVER MENU")]
    [SerializeField] private GameObject gameOverMenu;

    private GameManager gameManager;

    private bool player = true;
    private bool black = true;

    void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    #region MainMenu
    public void SetColor(int _value)
    {
        if (_value == 0) black = true;
        else black = false;
    }

    public void SetGameMode(int _value)
    {
        if (_value == 0) player = true;
        else player = false;

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
    }

    public void StartGame()
    {
        gameManager.SetGameMode(player, black);
        gameManager.StartGame();
        mainMenu.SetActive(false);
        inGameUI.SetActive(true);
    }

    public void Close()
    {
        Application.Quit();
    }
    #endregion

    #region InGame
    public void UpdateInfo(float _currentRate, float _lastPerformance = 50f)
    {
        currentRate.text = "Current Rate: " + _currentRate + " %";
        lastPerformance.text = "Last Move Performance: " + _lastPerformance + " %";
    }

    public void AutomaticMovement(bool _moveAutomatically)
    {
        gameManager.SetAutomaticMovement(_moveAutomatically);
    }
    #endregion

    #region GameOver
    public void GameOver(float _currentRate = 0f /* ... */ )
    {
        inGameUI.SetActive(false);
        gameOverMenu.SetActive(true);
    }
    
    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    #endregion
}
