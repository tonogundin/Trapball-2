using UnityEngine;
using UnityEngine.Events;

public class GameEvents : MonoBehaviour
{
    public static GameEvents instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
    }

    // EVENTS IN GAME.
    public UnityEvent initScene = new UnityEvent();
    public UnityEvent pauseScene = new UnityEvent();
    public UnityEvent finishGame = new UnityEvent();
    public UnityEvent returnPauseScene = new UnityEvent();
    public UnityEvent resumeScene = new UnityEvent();
    public UnityEvent onPause = new UnityEvent();
    public UnityEvent onResume = new UnityEvent();
    public UnityEvent newdeath = new UnityEvent();
    public UnityEvent death = new UnityEvent();
    public UnityEvent reload = new UnityEvent();
    public UnityEvent reloadCompleted = new UnityEvent();
    public UnityEvent stageCompleted = new UnityEvent();
    public UnityEvent stageCompletedAnimation = new UnityEvent();
    public UnityEvent countStars = new UnityEvent();
    public UnityEvent showInterstitial = new UnityEvent();
    public UnityEvent destroyWallSpikes = new UnityEvent();

    // EVENTS COMMONS.
    public UnityEvent viewRatingStars = new UnityEvent();
    public UnityEvent startRatingStars = new UnityEvent();
    public UnityEvent hiddenRatingStars = new UnityEvent();

    // EVENTS MAIN MENU.
    public UnityEvent showMainMenu = new UnityEvent();
    public UnityEvent showSelectStage = new UnityEvent();
    public UnityEvent showCredits = new UnityEvent();
    public UnityEvent showHowToPlayMenu = new UnityEvent();
    public UnityEvent soundOn = new UnityEvent();
    public UnityEvent soundOff = new UnityEvent();
    public UnityEvent showLoading = new UnityEvent();
    public UnityEvent showSelectScene = new UnityEvent();

}
