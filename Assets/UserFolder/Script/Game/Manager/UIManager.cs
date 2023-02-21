using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField] private Text timeText;
    [SerializeField] private Text moneyText;


    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }

    private void FixedUpdate()
    {
        timeText.text = gameManager.CurrentTime.ToString() + "\n";
        timeText.text += gameManager.CurrentTimeSecond.ToString();

        moneyText.text = gameManager.CurrentGold.ToString() + " / " + gameManager.MaxGold.ToString()
            + " / " + (Mathf.Round(gameManager.IncreseGoldTime * 100) * 0.01f).ToString();
    }

    public void ExitGame()
    {
        SceneManager.LoadScene(0);
    }
}