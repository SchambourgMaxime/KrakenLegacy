using UnityEngine;
using System.IO;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public GameObject buttonContinue;

	void Start ()
    {
        if (File.Exists(Application.dataPath + CreatureController.savePath))
            buttonContinue.SetActive(true);
        else
            buttonContinue.SetActive(false);
    }
	
    public void QuitGame()
    {
        Application.Quit();
    }

    public void CreateNewGame()
    {
        if (File.Exists(Application.dataPath+CreatureController.savePath))
            File.Delete(Application.dataPath+CreatureController.savePath);
        SceneManager.LoadScene("TutoControls");
    }

    public void ContinueGame()
    {
        if(buttonContinue.activeInHierarchy)
            SceneManager.LoadScene("SampleScene");
    }

    public void Credits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }
}
