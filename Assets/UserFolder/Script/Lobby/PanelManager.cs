using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Panel끼리 이동만 담당함 (Scene 이동은 LobbyManager가 담당)
/// </summary>
public class PanelManager : MonoBehaviour
{
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject registerPanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject banPickPanel;
    [SerializeField] private GameObject deckPanel;

    [SerializeField] private Text loginIDText;
    [SerializeField] private Text loginPWText;

    [SerializeField] private Text registerIDText;
    [SerializeField] private Text registerPWText;

    private GameObject currentPanel;

    private void Awake()
    {
        currentPanel = loginPanel;
    }

    /// <summary>
    /// 로그인 화면으로 돌아가기
    /// </summary>
    public void SetLoginPanel(bool isRegister)
    {
        if (isRegister)
        {
            if (string.IsNullOrEmpty(registerIDText.text) || string.IsNullOrEmpty(registerPWText.text))
            {
                return;
            }
        }
        currentPanel.SetActive(false);
        loginPanel.SetActive(true);
        currentPanel = loginPanel;
    }

    /// <summary>
    /// 회원가입 화면으로 가기
    /// </summary>
    public void SetRegisterPanel()
    {
        currentPanel.SetActive(false);
        registerPanel.SetActive(true);
        currentPanel = registerPanel;
    }

    /// <summary>
    /// 로비 화면 가기
    /// </summary>
    public void SetLobbyPanel(bool isLogin)
    {
        if (isLogin)
        {
            if (string.IsNullOrEmpty(loginIDText.text) || string.IsNullOrEmpty(loginPWText.text))
            {
                return;
            }
        }
        currentPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        currentPanel = lobbyPanel;
    }

    /// <summary>
    /// 매칭 잡힌 후 밴픽 화면 가기
    /// </summary>
    public void SetBanPickPanel()
    {
        currentPanel.SetActive(false);
        banPickPanel.SetActive(true);
        currentPanel = banPickPanel;
    }


}
