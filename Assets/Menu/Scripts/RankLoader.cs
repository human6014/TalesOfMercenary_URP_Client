using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RankLoader : MonoBehaviour
{
    [SerializeField]
    private GameObject RankDataprefab;
    [SerializeField]
    private Transform RankDataparent;

    private static readonly string m_URL = "localhost:8080/api/score/rank";
    private int i=0;

    [System.Serializable]
    public class PlayerData
    {
        public string playerId;
        public int score;
        public int win;
        public int lose;
        public string winningRate;


    }

    private List<RankScore> rankDataList;

    private void Awake()
    {
        rankDataList = new List<RankScore>();

        for(int i = 0; i < 10; i++)
        {
            GameObject clone = Instantiate(RankDataprefab, RankDataparent);
            rankDataList.Add(clone.GetComponent<RankScore>());
        }
        for (int i = 0; i < 10; i++)
        {
            SetRankData(rankDataList[i], i + 1, "-", 0);
        }
        StartCoroutine(FetchRankingData());
    }

    private IEnumerator FetchRankingData()
    {

        UnityWebRequest www = UnityWebRequest.Get(m_URL);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error fetching data: " + www.error);
        }
        else
        {
            string jsonResponse = www.downloadHandler.text;
            Debug.Log("API response: " + jsonResponse);

            //foreach (Transform child in RankDataparent)
            //{
            //    Destroy(child.gameObject); // 기존 데이터 제거
            //}

            List<PlayerData> playerDataList = JsonConvert.DeserializeObject<List<PlayerData>>(jsonResponse);

            foreach (PlayerData playerData in playerDataList)
            {

                SetRankData(rankDataList[i], i + 1, playerData.playerId, playerData.score);
                ++i;
                Debug.Log("name: " + playerData.playerId);
                Debug.Log("Score: " + playerData.score);
                Debug.Log("Win: " + playerData.win);
                Debug.Log("Lose: " + playerData.lose);
                Debug.Log("winningRate: " + playerData.winningRate);
            }
            //foreach (PlayerData playerData in playerDataArray)
            //{
            //    //for (int i = 0; i < playerDataArray.Length; i++)
            //    //{
            //    //    SetRankData(rankDataList[i], i + 1, playerData.name, playerData.score);
            //    //}
            //    Debug.Log("name: " + playerData.name);
            //    Debug.Log("Score: " + playerData.score);
            //    Debug.Log("Win: " + playerData.win);
            //    Debug.Log("Lose: " + playerData.lose);
            //    Debug.Log("winningRate: " + playerData.winRate);
            //}
        }
    }
    //밑에 api에서 데이터 값을 리스트 형식으로 받아서 어차피 값 들어올때 api에서 랭킹순으로 들어올테니 리스트로 값 받아서 순서대로 rankDataList[i]로 값을 넘겨 주면 될듯

    private void SetRankData(RankScore rankscore, int rank, string playerid, int score)
    {
        rankscore.Rank = rank;
        rankscore.Name = playerid;
        rankscore.Score = score;
    }
}
