using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class Button3DSelection : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    void Start()
    {        
        GetComponent<Light>().enabled = false;
    }

    public void OnSelect(BaseEventData eventData)
    {
        GetComponent<Light>().enabled = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        GetComponent<Light>().enabled = false;
    }
}
