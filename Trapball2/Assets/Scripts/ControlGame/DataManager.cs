using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public float musicVolume = 1f;
    public float fxVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SaveVolumeSettings(float musicVolume, float fxVolume)
    {
        this.musicVolume = musicVolume;
        this.fxVolume = fxVolume;
        PlayerPrefs.SetFloat("musicVolume", this.musicVolume);
        PlayerPrefs.SetFloat("fxVolume", this.fxVolume);
        PlayerPrefs.Save();
    }

    public (float musicVolume, float fxVolume) LoadVolumeSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat("musicVolume", 1f);
        float fxVolume = PlayerPrefs.GetFloat("fxVolume", 1f);
        return (musicVolume, fxVolume);
    }

}