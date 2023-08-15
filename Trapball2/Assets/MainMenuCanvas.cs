using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenuCanvas : MonoBehaviour
{
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
        if (value.isPressed)
        {
            SceneManager.LoadScene("Level1_develop");
        }
    }
}
