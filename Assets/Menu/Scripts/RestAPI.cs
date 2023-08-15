using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using static RestAPI;
using System.Net;

public class RestAPI : MonoBehaviour
{
    
    [System.Serializable]
    public class PlayerData
    {
        public string playerId;
        public int ranking;
        public int score;
        public int win;
        public int lose;
        public string winningRate;
    }


   
    void Start()
    {

        //RequestPostSign(requestData);
        
        //StartCoroutine(SendDataToAPI());
        //StartCoroutine(UnityWebRequestGet());
    }
    // Get
    IEnumerator UnityWebRequestGet()
    {
        string playerID = "aaa";
        string baseUrl = "http://localhost:8080";
        string endpoint = "/api/player/info" + "/" + playerID;
        //string baseUrl = "http://localhost:8080";
        //string endpoint = "/api/player/info/aaa";
        string url = baseUrl + endpoint;
        
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        if (www.error == null)
        {
            string responseText = www.downloadHandler.text;
            Debug.Log("API response: " + responseText);

            PlayerData playerData = JsonUtility.FromJson<PlayerData>(responseText);
                                      
            Debug.Log("Player ID: " + playerData.playerId);
            Debug.Log("ranking: " + playerData.ranking);
            Debug.Log("Score: " + playerData.score);
            Debug.Log("Win: " + playerData.win);
            Debug.Log("Lose: " + playerData.lose);
            Debug.Log("winningRate: " + playerData.winningRate);            
        }
        else
        {
            Debug.Log("ERROR");
        }
    }
    [System.Serializable]
    public class RequestData
    {
        public string playerID;

    }
    //post
    IEnumerator SendDataToAPI()
    {
        string playerID = "aaa";
        //WWWForm form = new WWWForm();
        //form.AddField("playerId", playerID);

        string baseUrl = "http://localhost:8080";
        string endpoint = "/api/score/win";
        string url = baseUrl + endpoint;

        ////string jsonData = JsonUtility.ToJson(requestData);

        //using (UnityWebRequest webRequest = UnityWebRequest.Post(url, form))
        //{
        //    webRequest.SetRequestHeader("Content-Type", "application/json");

        //    yield return webRequest.SendWebRequest();

        //    if (webRequest.result != UnityWebRequest.Result.Success)
        //    {
        //        Debug.LogError("API request error: " + webRequest.error);
        //    }
        //    else
        //    {
        //        string responseText = webRequest.downloadHandler.text;
        //        Debug.Log("API response: " + responseText);

        //        // 응답 처리
        //    }
        //}
        WWWForm form = new WWWForm();
        form.AddField("playerId", playerID);

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success) Debug.Log(www.downloadHandler.text);
        else Debug.Log("Error: " + www.error);
    }
    IEnumerator RequestPostSign(Dictionary<string, string> data)
    {
        
        string baseUrl = "http://localhost:8080";
        string endpoint = "/api/player/signup";
        string url = baseUrl + endpoint;

        //WWWForm form = new WWWForm();
        //form.AddField("playerId", playerID);
        //form.AddField("password", password);
        
        string jsonData = JsonUtility.ToJson(data);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        Debug.Log("여기까지는 되네");
        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {webRequest.error}");
            }
            else
            {
                Debug.Log($"Response: {webRequest.downloadHandler.text}");
                // 여기서 서버 응답 처리를 할 수 있습니다.
            }
        }
    }
}
