using Newtonsoft.Json;
using System.IO;
using UnityEngine;

[System.Serializable]
public class JsonClass
{
    public int id;
    public string playerName;
    public float floatData;

    public int[] ints;
    
}

public static class JsonFileManager
{

    private static string JSONFILE_FOLDER_PATH = Application.dataPath + "/JsonFiles";

    private static void checkOrCreateFolder(string folderPath)
    {
        if (Directory.Exists(folderPath) == true)
            return;

        Directory.CreateDirectory(folderPath);
    }

    /*
    // using Unity native code
    public static T loadJsonData<T>(string fileName)
    {
        string filePath = JSONFILE_FOLDER_PATH + "/" + fileName;

        if (File.Exists(filePath) == true)
        {
            string dataAsJson = File.ReadAllText(filePath);
            return JsonUtility.FromJson<T>(dataAsJson);
        }
        else
        {
            return default(T);
        }
    } 
    // use Unity native code
    public static void saveJsonData<T>(T dataClass, string fileName)
    {
        string dataAsJson = JsonUtility.ToJson(dataClass);

        string filePath = JSONFILE_FOLDER_PATH + "/" + fileName;
        File.WriteAllText(filePath, dataAsJson);

    }
    */
    
    // using JSON.NET(not support in iOS)
    public static T loadJsonData<T>(string fileName)
    {
        string filePath = JSONFILE_FOLDER_PATH + "/" + fileName;

        if (File.Exists(filePath) == true)
        {
            string dataAsJson = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(dataAsJson);

        }
        else
        {
            return default(T);
        }
    }
    // use JSON.NET(not support in iOS)
    public static void saveJsonData<T>(T dataClass, string fileName)
    {
        string dataAsJson = JsonConvert.SerializeObject(dataClass);

        string filePath = JSONFILE_FOLDER_PATH + "/" + fileName;
        File.WriteAllText(filePath, dataAsJson);

    }
    
}
