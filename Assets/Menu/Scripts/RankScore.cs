using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;

public class RankScore : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textrank;
    [SerializeField]
    private TextMeshProUGUI textname;
    [SerializeField]
    private TextMeshProUGUI textscore;

    private int rank;
    private string name;
    private int score=0;

    public GameObject RankingPage;
    public GameObject Canvas;
    public void Pagego()
    {
        RankingPage.SetActive(true);
    }
    public void Back()
    {
        SceneManager.LoadScene("1_Scenes/Stage1");

    }
    public int Rank
    {
        set{
            rank = value;
            textrank.text = rank.ToString();
        }
        get => rank;
    }
    public string Name
    {
        set
        {
            name = value;
            textname.text = name;
        }
        get => name;
    }
    public int Score
    {
        set
        {
            score = value;
            textscore.text = score.ToString();
        }
        get => score;
    }
}
