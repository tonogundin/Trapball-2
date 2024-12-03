using System.IO;
using UnityEngine;

public static class StorageUnit
{
    private static string storagePath = Application.persistentDataPath + "/data.pkg"; // Nombre ambiguo

    public static void SaveData(object data)
    {
        string json = JsonUtility.ToJson(data);
        string transformedData = Transformer.Encode(json); // Transformación (encriptar)
        File.WriteAllText(storagePath, transformedData);
        Debug.Log("Data saved.");
    }

    public static T LoadData<T>()
    {
        if (File.Exists(storagePath))
        {
            string transformedData = File.ReadAllText(storagePath);
            string json = Transformer.Decode(transformedData); // Transformación inversa (desencriptar)
            return JsonUtility.FromJson<T>(json);
        }
        else
        {
            Debug.LogWarning("No data found.");
            return default;
        }
    }

    public static void ClearData()
    {
        if (File.Exists(storagePath))
        {
            File.Delete(storagePath);
            Debug.Log("Data cleared.");
        }
    }
}
