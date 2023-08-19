using Newtonsoft.Json;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using static RankLoader;
using static RestAPI;

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


public class WebRequest : MonoBehaviour
{

    private static readonly string m_URL = "localhost:8080";

    #region Post
    private static readonly string m_PostLose = "/api/score/lose";
    private static readonly string m_PostWin = "/api/score/win";
    private static readonly string m_PostLogin = "/api/player/login";
    private static readonly string m_PostSingUp = "/api/player/signup";
    #endregion
    #region Get
    private static readonly string m_GetRank = "/api/score/rank";
    private static readonly string m_GetAll = "/api/player/all";
    private static readonly string m_GetInfo = "/api/player/info";
#endregion

    private void Start()
    {

        //StartCoroutine(RequestGetInfo("aaa"));
        //CallAPIAsync();
    }

    #region Get
    public static IEnumerator RequestGetRank()
    {
        UnityWebRequest www = UnityWebRequest.Get(m_URL + m_GetRank);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success) Debug.Log(www.downloadHandler.text);
        else Debug.Log("Error: " + www.error);
    }

    public static IEnumerator RequestGetAll()
    {
        UnityWebRequest www = UnityWebRequest.Get(m_URL + m_GetAll);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success) Debug.Log(www.downloadHandler.text);
        else Debug.Log("Error: " + www.error);
    }

    public static async Task CallAPIAsync(string playerid)
    {
        PlayerData responseData = await GetAPIAsync(playerid);


        Launcher.ReceivePlayerData(responseData);

        return;
        //Debug.Log("Player ID: " + responseData.playerId);
        //Debug.Log("ranking: " + responseData.ranking);
        //Debug.Log("Score: " + responseData.score);
        //Debug.Log("Win: " + responseData.win);
        //Debug.Log("Lose: " + responseData.lose);
        //Debug.Log("winningRate: " + responseData.winningRate);
        // responseData를 사용하여 UI 업데이트 또는 처리
    }
    //로비부분에서 welcome! ooo 텍스트 부분에서 -> 이름,ranking,win,lose
    //데이터값을 api에서 받아오기 위한 함수 requestgetinfo() 수정
    public static async Task<PlayerData> GetAPIAsync(string playerid)
    {
        string e_url = "/api/player/info/" + playerid;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(m_URL + e_url))
        {
            var asyncOperation = webRequest.SendWebRequest();

            while (!asyncOperation.isDone)
            {
                await Task.Yield(); // Unity 메인 스레드를 블로킹하지 않음
            }

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("API request error: " + webRequest.error);
                return null;
            }
            else
            {
                string responseText = webRequest.downloadHandler.text;
                Debug.Log("API response: " + responseText);

                // JSON 데이터 파싱 및 역직렬화
                PlayerData responseData = JsonUtility.FromJson<PlayerData>(responseText);
                return responseData;
            }
        }
    }
    #endregion
    #region Post
    public static IEnumerator RequestPostLose(string playerID)
    {
        WWWForm form = new WWWForm();
        form.AddField("playerId", playerID);

        UnityWebRequest www = UnityWebRequest.Post(m_URL + m_PostLose, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success) Debug.Log(www.downloadHandler.text);
        else Debug.Log("Error: " + www.error);
    }

    public static IEnumerator RequestPostWin(string playerID)
    {
        WWWForm form = new WWWForm();
        form.AddField("playerId", playerID);

        UnityWebRequest www = UnityWebRequest.Post(m_URL + m_PostWin, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success) Debug.Log(www.downloadHandler.text);
        else Debug.Log("Error: " + www.error);
    }

    private static Task WaitSend(UnityWebRequest www)
    {
        www.SetRequestHeader("Content-Type", "application/json");

        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        UnityWebRequestAsyncOperation asyncOp = www.SendWebRequest();
        asyncOp.completed += (operation) =>
        {
            if (www.result == UnityWebRequest.Result.Success) tcs.SetResult(true);
            else tcs.SetResult(false);
        };

        return tcs.Task;
    }

    public static Task RequestPostLogin(string playerID, string password)
    {
        var requestData = new
        {
            playerId = playerID,
            password = password
        };

        string jsonData = JsonConvert.SerializeObject(requestData);
        byte[] postData = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest www = UnityWebRequest.Post(m_URL + m_PostLogin, jsonData);

        www.uploadHandler = new UploadHandlerRaw(postData);

        return WaitSend(www);
    }

    public static Task RequestPostSignUp(string playerID, string password)
    {
        var requestData = new
        {
            playerId = playerID,
            password = password
        };

        string jsonData = JsonConvert.SerializeObject(requestData);
        byte[] postData = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest www = UnityWebRequest.Post(m_URL + m_PostSingUp, jsonData);

        www.uploadHandler = new UploadHandlerRaw(postData);

        return WaitSend(www);
    }
    #endregion
}
