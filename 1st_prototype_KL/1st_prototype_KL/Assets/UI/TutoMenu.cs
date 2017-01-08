using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class TutoMenu : MonoBehaviour {

    public Sprite firstTuto;
    public Sprite secondTuto;

    private Image canvasImage;

    private int actionID; //1 is next image, 2 is begin game

    void Start()
    {
        actionID = 1;
        canvasImage = GetComponentInChildren<Image>();
        canvasImage.sprite = firstTuto;
    }

    public void Next()
    {
        if(actionID == 1)
        {
            canvasImage.sprite = secondTuto;
        }
        else if(actionID == 2)
        {
            SceneManager.LoadScene("SampleScene");
        }
        actionID++;
    }
}
