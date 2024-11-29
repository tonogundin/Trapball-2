using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class LocationManager : MonoBehaviour
{
    public static LocationManager Instance;

    private Dictionary<string, string> localizedText;
    private string currentLanguage = "es"; // Idioma por defecto

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadLocalization(string languageCode)
    {
        currentLanguage = languageCode;
        string filePath = Application.dataPath + "/Location/location_" + languageCode + ".json";

        if (File.Exists(filePath))
        {
            try
            {
                // Leer el archivo JSON
                string dataAsJson = File.ReadAllText(filePath);

                // Comprobar si el contenido está envuelto en comillas (como una cadena)
                if (dataAsJson.StartsWith("\"") && dataAsJson.EndsWith("\""))
                {
                    // Deserializar como una cadena antes de continuar
                    dataAsJson = JsonConvert.DeserializeObject<string>(dataAsJson);
                }

                // Deserializar directamente a un Dictionary<string, string>
                localizedText = JsonConvert.DeserializeObject<Dictionary<string, string>>(dataAsJson);
                Debug.Log("Archivo de localización cargado correctamente.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error al cargar el archivo de localización: {ex.Message}");
            }
        }
        else
        {
            Debug.LogError($"El archivo de localización no existe: {filePath}");
        }
    }

    public string GetLocalizedValue(string key)
    {
        if (localizedText == null)
        {
            LoadLocalization(currentLanguage);
        }
        if (localizedText != null && localizedText.ContainsKey(key))
        {
            return localizedText[key];
        }
        return $"[{key}]"; // Retorna clave si no se encuentra la traducción
    }
}

[System.Serializable]
public class LocalizationData
{
    public List<LocalizationEntry> entries;

    public Dictionary<string, string> ToDictionary()
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        foreach (var entry in entries)
        {
            dictionary[entry.key] = entry.value;
        }
        return dictionary;
    }
}

[System.Serializable]
public class LocalizationEntry
{
    public string key;
    public string value;
}