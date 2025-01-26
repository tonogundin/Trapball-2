using UnityEngine;
public class SceneLoaderManager : MonoBehaviour
{

    public static string nextScene;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public static void prepareSceneLoad(SCENE sceneToLoad)
    {
        nextScene = FMODUtils.GetStringValue(sceneToLoad);
    }
}


public enum SCENE
{
    [FMODUtils.StringValue("Menu")]
    MENU,
    [FMODUtils.StringValue("Intro")]
    INTRO,
    [FMODUtils.StringValue("Loading")]
    LOADING,
    [FMODUtils.StringValue("Level1")]
    LEVEL1,
    [FMODUtils.StringValue("Level2")]
    LEVEL2
}
