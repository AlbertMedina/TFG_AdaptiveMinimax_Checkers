using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    
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

    public void SetGameMode(int _value)
    {
        if (_value == 0) player = true;
        else player = false;
    }

    public void SetColor(int _value)
    {
        if (_value == 0) black = true;
        else black = false;
    }

    public void SetAlgorithm(int _value)
    {
        switch (_value)
        {
            case 0:
                gameManager.SetAlgorithm(GameManager.AvailableAlgorithms.Adaptive_Minimax);
                break;
            case 1:
                gameManager.SetAlgorithm(GameManager.AvailableAlgorithms.Minimax);
                break;
            case 2:
                gameManager.SetAlgorithm(GameManager.AvailableAlgorithms.AB_Minimax);
                break;
        }
    }

    public void SetDifficultyRate(float _value)
    {
        gameManager.SetDifficultyRate(_value);
    }
}
