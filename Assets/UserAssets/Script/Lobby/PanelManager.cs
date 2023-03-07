using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PanelManager : MonoBehaviour
{
    [SerializeField] private GameObject lobbyCanvas;
    [SerializeField] private GameObject banPickCanvas;
    [SerializeField] private GameObject shopCanvas;
    [SerializeField] private GameObject deckPanel;
    
    private GameObject currentCanvas;
    private BanPickManager banPickManager;
    private void Awake()
    {
        currentCanvas = lobbyCanvas;
        banPickManager = FindObjectOfType<BanPickManager>();
    }

    /// <summary>
    /// 로비 화면 가기
    /// </summary>
    public void SetLobbyCanvas()
    {
        currentCanvas.SetActive(false);
        lobbyCanvas.SetActive(true);
        currentCanvas = lobbyCanvas;
    }

    /// <summary>
    /// 매칭 잡힌 후 밴픽 화면 가기
    /// </summary>
    public void SetBanPickCanvas()
    {
        currentCanvas.SetActive(false);
        banPickCanvas.SetActive(true);
        currentCanvas = banPickCanvas;

        banPickManager.SetCardList();
    }

    /// <summary>
    /// 상점 화면 가기
    /// </summary>
    public void SetShopCanvas()
    {
        currentCanvas.SetActive(false);
        shopCanvas.SetActive(true);
        currentCanvas = shopCanvas;
    }
}
