using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SoundButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, ISelectHandler, ISubmitHandler
{

    private FMOD.Studio.EventInstance buttonClickSound;
    private FMOD.Studio.EventInstance buttonSelectSound;
    private Button button;
    private bool shouldPlaySelectSound = true;


    void Start()
    {

        buttonClickSound = FMODUtils.createInstance(FMODConstants.HUD.BUTTON_CLICK);
        buttonSelectSound = FMODUtils.createInstance(FMODConstants.HUD.BUTTON_SELECT);
        button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EventSystem.current.currentSelectedGameObject != gameObject)
        {
            button.Select();
        }

    }
    public void OnSelect(BaseEventData eventData)
    {
        PlayHoverSound();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayClickSound();
    }
    public void OnSubmit(BaseEventData eventData)
    {
        PlayClickSound();
    }
    private void PlayHoverSound()
    {
        if (shouldPlaySelectSound)
        {
            buttonSelectSound.start();
        }
    }

    private void PlayClickSound()
    {
        buttonClickSound.start();
    }

    public void SetShouldPlaySelectSound(bool shouldPlay)
    {
        shouldPlaySelectSound = shouldPlay;
    }
}
