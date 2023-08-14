 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HttpClient : MonoBehaviour
{
    enum Method
    {
        POST,
        GET,
        UPDATE,
        DELETE
    }

    private string headerUsername = "username";
    private string headerPassword = "password";

    string _baseUrl = "https://localhost:44351/api";

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
