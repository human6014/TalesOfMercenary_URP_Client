using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankLoader : MonoBehaviour
{
    [SerializeField]
    private GameObject RankDataprefab;
    [SerializeField]
    private Transform RankDataparent;


    private List<RankScore> rankDataList;

    private void Awake()
    {
        rankDataList = new List<RankScore>();

        for(int i = 0; i < 10; i++)
        {
            GameObject clone = Instantiate(RankDataprefab, RankDataparent);
            rankDataList.Add(clone.GetComponent<RankScore>());
        }
        Getranklist();
    }
    private void Getranklist()
    {
        
        for(int i = 0; i < 10; i++)
        {
            SetRankData(rankDataList[i], i + 1, "-", 0);
        }

        //밑에 api에서 데이터 값을 리스트 형식으로 받아서 어차피 값 들어올때 api에서 랭킹순으로 들어올테니 리스트로 값 받아서 순서대로 rankDataList[i]로 값을 넘겨 주면 될듯
    }
    private void SetRankData(RankScore rankscore, int rank, string name, int score)
    {
        rankscore.Rank = rank;
        rankscore.Name = name;
        rankscore.Score = score;
    }
}
