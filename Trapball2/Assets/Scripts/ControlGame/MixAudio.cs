using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MixAudio : MonoBehaviour
{
    public static MixAudio Instance;

    public GameObject Music;
    public GameObject FX;

    private TextMeshProUGUI textMusic;
    private TextMeshProUGUI textFX;

    private float volumenBankMaster = 1;
    private float volumenBankMusic = 1;
    private float scaleIncrement= 0.05f;
    private const string nameGameObjectUnit = "Unit";

    private Button menuSettingsVolumeMusicUp;
    private Button menuSettingsVolumeMusicDown;
    private Button menuSettingsVolumeFXUp;
    private Button menuSettingsVolumeFXDown;
    private Button menuPauseExitButton;


    public void Start()
    {

        textMusic = Music.transform.Find(nameGameObjectUnit).GetComponentInChildren<TextMeshProUGUI>();
        textFX = FX.transform.Find(nameGameObjectUnit).GetComponentInChildren<TextMeshProUGUI>();

        var (musicVolume, fxVolume) = FMODUtils.getVolumeSettings();
        volumenBankMaster = fxVolume;
        volumenBankMusic = musicVolume;
        setMusicVolume(volumenBankMusic);
        setFXVolume(volumenBankMaster);
        menuSettingsVolumeMusicUp = transform.Find("Music").transform.Find("ButtonUp").GetComponent<Button>();
        menuSettingsVolumeMusicDown = transform.Find("Music").transform.Find("ButtonDown").GetComponent<Button>();
        menuSettingsVolumeFXUp = transform.Find("FX").transform.Find("ButtonUp").GetComponent<Button>();
        menuSettingsVolumeFXDown = transform.Find("FX").transform.Find("ButtonDown").GetComponent<Button>();
        menuPauseExitButton = transform.Find("ExitButton").GetComponent<Button>();
    }

    public void upVolumeMusic()
    {
        SetSelectSound(menuSettingsVolumeMusicUp, false);
        setMusicVolumeByStep(true);
        menuSettingsVolumeMusicUp.interactable = false;
        menuSettingsVolumeMusicUp.interactable = true;
        menuSettingsVolumeMusicUp.Select();
        SetSelectSound(menuSettingsVolumeMusicUp, true);
    }
    public void downVolumeMusic()
    {
        SetSelectSound(menuSettingsVolumeMusicDown, false);
        setMusicVolumeByStep(false);
        menuSettingsVolumeMusicDown.interactable = false;
        menuSettingsVolumeMusicDown.interactable = true;
        menuSettingsVolumeMusicDown.Select();
        SetSelectSound(menuSettingsVolumeMusicDown, true);
    }
    public void upVolumeFX()
    {
        SetSelectSound(menuSettingsVolumeFXUp, false);
        setFXVolumeByStep(true);
        menuSettingsVolumeFXUp.interactable = false;
        menuSettingsVolumeFXUp.interactable = true;
        menuSettingsVolumeFXUp.Select();
        SetSelectSound(menuSettingsVolumeFXUp, true);
    }
    public void downVolumeFX()
    {
        SetSelectSound(menuSettingsVolumeFXDown, false);
        setFXVolumeByStep(false);
        menuSettingsVolumeFXDown.interactable = false;
        menuSettingsVolumeFXDown.interactable = true;
        menuSettingsVolumeFXDown.Select();
        SetSelectSound(menuSettingsVolumeFXDown, true);
    }

    public void buttonExit()
    {
        gameObject.SetActive(false);
        GameEvents.instance.returnPauseScene.Invoke();
        menuPauseExitButton.interactable = false;
        menuPauseExitButton.interactable = true;
    }

    private string getVolumeMusicPercent()
    {
        return (volumenBankMusic * 100).ToString("0");
    }
    private string getVolumeFXPercent()
    {
        return (volumenBankMaster * 100).ToString("0");
    }

    private void setMusicVolumeByStep(bool increment)
    {
        setMusicVolume(increment ? volumenBankMusic + scaleIncrement : volumenBankMusic - scaleIncrement);
    }

    private void setMusicVolume(float value)
    {
        volumenBankMusic = setLimitsAudio(value);
        FMODUtils.saveVolumeSettings(volumenBankMusic, volumenBankMaster);
        textMusic.text = getVolumeMusicPercent();
    }
    private void setFXVolumeByStep(bool increment)
    {
        setFXVolume(increment ? volumenBankMaster + scaleIncrement : volumenBankMaster - scaleIncrement);
    }

    private void setFXVolume(float value)
    {
        volumenBankMaster = setLimitsAudio(value);
        FMODUtils.saveVolumeSettings(volumenBankMusic, volumenBankMaster);
        textFX.text = getVolumeFXPercent();
    }

    private float setLimitsAudio(float volume)
    {
        return volume > 1 ? 1 : volume < 0 ? 0 : volume;
    }

    private void SetSelectSound(Button button, bool shouldPlay)
    {
        SoundButton soundButton = button.GetComponent<SoundButton>();
        if (soundButton != null)
        {
            soundButton.SetShouldPlaySelectSound(shouldPlay);
        }
    }
}
