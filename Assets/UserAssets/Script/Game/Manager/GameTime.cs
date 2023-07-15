using Photon.Pun;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameTime : MonoBehaviour
{
    public TextMeshProUGUI TextTMP;
    public float starTime;
    public PhotonView PV;
    private void Awake()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            starTime = Time.realtimeSinceStartup;
            PV.RPC("Sync", RpcTarget.Others ,starTime);
        }
    }

    private void Update()
    {
        TextTMP.text = string.Format("time = {0}", Time.realtimeSinceStartup - starTime);
    }

    [PunRPC]
    void Sync(float time)
    {
        starTime = time;
    }

}
