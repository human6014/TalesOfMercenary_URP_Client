using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class ReceiveEvent : MonoBehaviour, IOnEventCallback
{
    public enum EEventCode : byte
    {
        #region Host
        HplayerDrawedCard = 0, 
        HplayerBuildingUpgraded = 1, 
        HplayerSpawnedUnit = 2,
        HplayerNexusUpgraded = 3, 
        HunitAttack = 4, 
        HunitMovement_vector = 5,
        HunitMovement_target = 6,
        HunitDied = 7,
        HplayerUseMagicCard = 8,
        #endregion

        #region Client
        CplayerDrawedCard = 9, 
        CnexusUpgraded = 10,
        CunitDestinationInput = 11, 
        CuseMagicCard = 12, 
        CbuildingUpgrad = 13,
        CspawnUnit = 14,
        #endregion
    }
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        Debug.Log("메시지 받음 ( " + "이벤트 코드" + eventCode + ") ");
        object[] data = (object[])photonEvent.CustomData;

        switch(eventCode)
        {
            case (byte)EEventCode.HplayerDrawedCard:
                HplayerDrawedCard(data);
                break;
            case (byte)EEventCode.HplayerBuildingUpgraded:
                HplayerBuildingUpgraded(data);
                break;
            case (byte)EEventCode.HplayerSpawnedUnit:
                HplayerSpawnedUnit(data);
                break;

            case (byte)EEventCode.HplayerNexusUpgraded:
                HplayerNexusUpgraded(data);
                break;
            case (byte)EEventCode.HunitAttack:
                HunitAttack(data);
                break;

            case (byte)EEventCode.HunitMovement_vector:
                HunitMovement_vector(data);
                break;
            case (byte)EEventCode.HunitMovement_target:
                HunitMovement_target(data);
                break;

            case (byte)EEventCode.HunitDied:
                HunitDied(data);
                break;
            case (byte)EEventCode.HplayerUseMagicCard:
                HplayerUseMagicCard(data);
                break;
            case (byte)EEventCode.CplayerDrawedCard:
                CplayerDrawedCard(data);
                break;
            case (byte)EEventCode.CnexusUpgraded:
                CnexusUpgraded(data);
                break;

            case (byte)EEventCode.CunitDestinationInput:
                CunitDestinationInput(data);
                break;
            case (byte)EEventCode.CuseMagicCard:
                CuseMagicCard(data);
                break;

            case (byte)EEventCode.CbuildingUpgrad:
                CbuildingUpgrad(data);
                break;
            case (byte)EEventCode.CspawnUnit:
                CspawnUnit(data);
                break;
            default:
                Debug.Assert(false);
                break;
        }
    }

    #region Host receive

    public void HplayerDrawedCard(object[] data)
    {

    }

    public void HplayerBuildingUpgraded(object[] data)
    {

    }

    public void HplayerSpawnedUnit(object[] data)
    {

    }

    public void HplayerNexusUpgraded(object[] data)
    {

    }

    public void HunitAttack(object[] data)
    {

    }

    public void HunitMovement_vector(object[] data)
    {

    }

    public void HunitMovement_target(object[] data)
    {

    }

    public void HunitDied(object[] data)
    {

    }

    public void HplayerUseMagicCard(object[] data)
    {

    }
    #endregion

    #region Clint recei

    public void CplayerDrawedCard(object[] data)
    {

    }

    public void CnexusUpgraded(object[] data)
    {

    }

    public void CunitDestinationInput(object[] data)
    {

    }

    public void CuseMagicCard(object[] data)
    {

    }

    public void CbuildingUpgrad(object[] data)
    {

    }

    public void CspawnUnit(object[] data)
    {

    }
    #endregion
}