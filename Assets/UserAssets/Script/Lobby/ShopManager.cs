using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShopManager : MonoBehaviour
{
    [SerializeField] private RectTransform randomPickCheckingPanel;
    private CardDataManager cardDataManager;

    private RectTransform currentPickedBuildingCard;
    private void Awake()
    {
        cardDataManager = GetComponent<CardDataManager>();
    }

    
    //public void StartCloudaddinventory()
    //{
    //    PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
    //    {
    //        FunctionName = "addinventory", // Arbitrary function name (must exist in your uploaded cloud.js file)
    //        FunctionParameter = new { text = "ItemsToUser" }, // The parameter provided to your function
    //        //FunctionName = "helloWorld", // Arbitrary function name (must exist in your uploaded cloud.js file)
    //        //FunctionParameter = new { text = "true" }, // The parameter provided to your function
    //        GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream            

    //    }, OnCloudUpdateStore, OnErrorShared);
    //}

    //private void OnCloudUpdateStore(ExecuteCloudScriptResult result)
    //{
    //    string itemId;
    //    int index;
        
    //    JsonObject jsonResult = (JsonObject)result.FunctionResult;
    //    jsonResult.TryGetValue("messageValue", out object messageValue);
    //    jsonResult.TryGetValue("id", out object id);
    //    jsonResult.TryGetValue("itemidx", out object itemidx);

    //    itemId = (string)id;
    //    index = int.Parse((string)itemidx);

    //    //Debug.Log(result.FunctionResult);
    //    //Debug.Log((string)messageValue);
    //    //Debug.Log(itemId);
    //    //Debug.Log(index + ", Type : " + index.GetType());

    //    DisplayPickedBuildingCard(index);
    //}

    //private static void OnErrorShared(PlayFabError error)
    //{
    //    Debug.Log(error.GenerateErrorReport());
    //}

    //public void DisplayPickedBuildingCard(int index)
    //{
    //    if (currentPickedBuildingCard != null) Destroy(currentPickedBuildingCard.gameObject); //»ø¿≤ æ»¡¡¿Ω -> πŸ≤‹≤®¿”
    //    currentPickedBuildingCard = Instantiate(cardDataManager.GetBuildingCardObj(index)).GetComponent<RectTransform>();

    //    currentPickedBuildingCard.SetParent(randomPickCheckingPanel);
    //    currentPickedBuildingCard.anchoredPosition = new Vector2(0, 60);
    //    currentPickedBuildingCard.SetAsLastSibling();
    //}
}
