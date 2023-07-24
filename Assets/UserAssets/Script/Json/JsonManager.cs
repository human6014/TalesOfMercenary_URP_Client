using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Text;

public class JsonManager : MonoBehaviour
{
    private string JsonDataPath;

    private void Awake() => JsonDataPath = Application.dataPath + "/UserAssets/UnitData";
    
    private void Start()
    {
        /*
        UnitJsonData jtc = new UnitJsonData(true);
        string jsonData = ObjectToJson(jtc);
        Debug.Log(jsonData);
        */

        /*
        var jtc2 = LoadJsonFile<UnitJsonData>("0_Warrior_Data");
        jtc2.Print();
        */

        /*
        UnitJsonData jtc = new(true);
        string jsonData = ObjectToJson(jtc);
        string fileName = jtc.unitID + "_" + jtc.unitName + "_" + "Data";
        CreateJsonFile(fileName, jsonData);
        */

    }

#if UNITY_EDITOR
    private void CreateJsonFile(string fileName, string jsonData)
    {
        FileStream fileStream = new(string.Format("{0}/{1}.json", JsonDataPath, fileName), FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }

    private string ObjectToJson(object obj) => JsonUtility.ToJson(obj);

#endif
    /*
    public T LoadJsonFile<T>(string fileName)
    {
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", JsonDataPath, fileName), FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();
        string jsonData = Encoding.UTF8.GetString(data);
        return JsonConvert.DeserializeObject<T>(jsonData);
    }
    */

    public static T LoadJsonFile<T>(string fileName)
    {
        FileStream fileStream = new(string.Format("{0}/{1}.json", Application.dataPath + "/UserAssets/UnitData", fileName), FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();
        string jsonData = Encoding.UTF8.GetString(data);
        return JsonConvert.DeserializeObject<T>(jsonData);
    }

    private T JsonToOject<T>(string jsonData) => JsonUtility.FromJson<T>(jsonData);
}
