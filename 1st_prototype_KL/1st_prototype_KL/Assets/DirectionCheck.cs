using UnityEngine;
using System.Collections;

public class DirectionCheck : MonoBehaviour {

    public string[] tagsAccepted;

    private bool isTouching = false;

    void OnTriggerEnter(Collider other)
    {
        if (isTagAccepted(other.tag))
            isTouching = true;
    }

    void OnTriggerExit(Collider other) 
    {
        if (isTagAccepted(other.tag))
            isTouching = false;
    }

    bool isTagAccepted(string tagToTest)
    {
        for(int i = 0; i < tagsAccepted.Length; i++)
            if (tagToTest.Contains(tagsAccepted[i]))
                return true;

        return false;
    }

    public bool GetIsTouching()
    {
        return isTouching;
    }
}
