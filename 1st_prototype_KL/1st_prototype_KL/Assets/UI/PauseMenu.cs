
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    public GameObject pauseMenu;
    private bool isEnabled = false;


    void Update()
    {
        // Enable pause menu

        if (Input.GetKeyDown(KeyCode.Joystick1Button7) || Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape) || (Input.GetKeyDown(KeyCode.Joystick1Button1) && isEnabled))
        {
            if (!isEnabled)
            {
                pauseMenu.SetActive(true);
                isEnabled = true;
                Time.timeScale = 0f;
            }
            else 
            {
                ContinueGame();
            }
        }

        // disable pause menu
    }

    /*void Update ()
    {
	    if(Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            ContinueGame();
        }
	}
    */
    public void ContinueGame()
    {
        pauseMenu.SetActive(false);
        isEnabled = false;
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("StartMenu");
        Time.timeScale = 1f;
    }
}
