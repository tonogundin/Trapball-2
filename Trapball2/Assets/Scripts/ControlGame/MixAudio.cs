using UnityEngine;
using TMPro;

public class MixAudio : MonoBehaviour
{
    public GameObject Music;
    public GameObject FX;

    private TextMeshProUGUI textMusic;
    private TextMeshProUGUI textFX;

    private float volumenBankMaster = 1;
    private float volumenBankMusic = 1;
    private FMOD.Studio.Bus musicBusMaster;
    private FMOD.Studio.Bus musicBusMusic;
    private float scaleIncrement= 0.05f;
    private const string nameGameObjectUnit = "Unit";

    // Start is called before the first frame update
    void Start()
    {
        musicBusMaster = FMODUnity.RuntimeManager.GetBus("bus:/MASTER");
        musicBusMusic = FMODUnity.RuntimeManager.GetBus("bus:/MUSIC");
        textMusic = Music.transform.Find(nameGameObjectUnit).GetComponentInChildren<TextMeshProUGUI>();
        textFX = FX.transform.Find(nameGameObjectUnit).GetComponentInChildren<TextMeshProUGUI>();
    }

    public void upVolumeMusic()
    {
        SetMusicVolume(true);
    }
    public void downVolumeMusic()
    {
        SetMusicVolume(false);
    }
    public void upVolumeFX()
    {
        SetFXVolume(true);
    }
    public void downVolumeFX()
    {
        SetFXVolume(false);
    }

    public void buttonExit()
    {
        gameObject.SetActive(false);
        GameEvents.instance.pauseScene.Invoke();
    }

    private string getVolumeMusicPercent()
    {
        return (volumenBankMusic * 100).ToString("0");
    }
    private string getVolumeFXPercent()
    {
        return (volumenBankMaster * 100).ToString("0");
    }

    private void SetMusicVolume(bool increment)
    {
        volumenBankMusic = setLimitsAudio(increment ? volumenBankMusic + scaleIncrement : volumenBankMusic - scaleIncrement);
        musicBusMusic.setVolume(volumenBankMusic);
        textMusic.text = getVolumeMusicPercent();
    }
    private void SetFXVolume(bool increment)
    {
        volumenBankMaster = setLimitsAudio(increment ? volumenBankMaster + scaleIncrement : volumenBankMaster - scaleIncrement);
        musicBusMaster.setVolume(volumenBankMaster);
        textFX.text = getVolumeFXPercent();
    }

    private float setLimitsAudio(float volume)
    {
        return volume > 1 ? 1 : volume < 0 ? 0 : volume;
    }
}
