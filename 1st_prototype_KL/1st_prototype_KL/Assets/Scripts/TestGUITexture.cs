using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TestGUITexture : MonoBehaviour {

    public Material[] frames;
    public float framesPerSecond = 10.0f;

    public void Update()
    {
        Debug.Log(GetComponent<Image>());
        int index = (int)(Time.time * framesPerSecond) % frames.Length;
        GetComponent<Image>().material = frames[index];
    }
}
