using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("MAIN MENU")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject algorithmSelector;
    [SerializeField] private GameObject difficultyRateSlider;
    
    private GameManager gameManager;

    private bool player = true;
    private bool black = true;

    void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    void Update()
    {
        
    }

    public void StartGame()
    {
        gameManager.SetGameMode(player, black);
        gameManager.StartGame();
        mainMenu.SetActive(false);
    }

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
}
