using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RankScore : MonoBehaviour
{
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
}
