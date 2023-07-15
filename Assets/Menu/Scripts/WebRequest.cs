using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequest : MonoBehaviour
{
    private static readonly string m_URL = "localhost:8090";

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
       //StartCoroutine(RequestPostWin("test"));
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

    public static async Task<bool> RequestGetInfo(string playerID)
    {
        UnityWebRequest www = UnityWebRequest.Get(m_URL + m_GetInfo + "?playerId=" + playerID);

        www.SendWebRequest();

        while (!www.isDone) await Task.Yield();

        if (www.result == UnityWebRequest.Result.Success) return true;
        else return false;
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

    public static Task RequestPostLogin(string playerID, string password = "")
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

    public static Task RequestPostSignUp(string playerID, string name = "", string password = "")
    {
        if (name == "") name = playerID;

        var requestData = new
        {
            playerId = playerID,
            name = name,
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
