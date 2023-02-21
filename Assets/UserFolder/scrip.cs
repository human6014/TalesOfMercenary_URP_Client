using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class scrip : MonoBehaviour
{
    private AuthenticationManager _authenticationManager;
    private ApiManager _apiManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _authenticationManager = FindObjectOfType<AuthenticationManager>();
        _apiManager = FindObjectOfType<ApiManager>();
    }

    public void OnClick()
    {
        Debug.Log("onLoginClicked ");
        string loginUrl = _authenticationManager.GetLoginUrl();
        Application.OpenURL(loginUrl);
        Debug.Log("onCallApiClick");
        _apiManager.CallTestApi();
    }
}
