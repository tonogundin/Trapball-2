using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


public class MainMenuCanvas : MonoBehaviour
{

    public GameObject LogoTrapBall;
    public GameObject Instructions;
    public GameObject pressA;
    private bool firstPress = false;
    private bool secondPress = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnJump(InputValue value)
    {
        if (!value.isPressed)
        {
            if (!firstPress && !secondPress)
            {
                StartCoroutine(delayStep());
                LogoTrapBall.SetActive(false);
                pressA.SetActive(false);
                Instructions.SetActive(true);
            } else if (firstPress && !secondPress)
            {
                secondPress = true;
                SceneManager.LoadSceneAsync("Level1_develop");
            }

        }
    }

    public void OnDetectControllerOrKeyboard(InputValue value)
    {
        if (Cursor.visible)
        {
            Cursor.visible = false;
        }
    }
    public void OnDetectMouse(InputValue value)
    {
        if (!Cursor.visible)
        {
            Cursor.visible = true;
        }
    }
    IEnumerator delayStep()
    {
        yield return new WaitForSeconds(0.5f);
        firstPress = true;
    }

}
