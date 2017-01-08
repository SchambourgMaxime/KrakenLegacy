using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class destruction : MonoBehaviour {

    public float duration = 2f;

    private float startTime;
    private Vector3 initialPosition;

    private bool isRed = false;
    private bool isBlue = false;
    private bool isYellow = false;
    private bool isGreen = false;
    private bool hasAffectedPlayer = false;

    private uint strengthGain = 0;
    private uint defenseGain = 0;
    private uint speedGain = 0;
    private uint healthGain = 0;

    private Slider currentSlider;

    private GameObject player;

    // Use this for initialization
    void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        startTime = Time.time;
        initialPosition = gameObject.transform.position;
        if(isRed)
        {
            currentSlider = player.GetComponent<HUDController>().GetStrengthSlider();
        }
        if (isBlue)
        {
            currentSlider = player.GetComponent<HUDController>().GetDefenseSlider();
        }
        if (isYellow)
        {
            currentSlider = player.GetComponent<HUDController>().GetSpeedSlider();
        }
        if (isGreen)
        {
            currentSlider = player.GetComponent<HUDController>().GetHealthSlider();
        }        
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {           
	    if(Time.time - startTime < duration)
        {
            Vector3 debugBubblePos = initialPosition;
            Vector3 debugSliderPos;
            if (isGreen)
                debugSliderPos = Camera.main.ScreenPointToRay(currentSlider.transform.position).GetPoint((Camera.main.transform.position - player.transform.position).magnitude);
            else
                debugSliderPos = Camera.main.ScreenPointToRay(currentSlider.GetComponentsInChildren<Image>()[2].transform.position).GetPoint((Camera.main.transform.position - player.transform.position).magnitude);
            transform.position = Vector3.Lerp(debugBubblePos, debugSliderPos, (Time.time - startTime) / duration);
        }
        else if(Time.time - startTime >= duration)
        {
            if (!hasAffectedPlayer)
            {
                if (isRed)
                {
                    player.GetComponent<CreatureController>().eatFood(strengthGain, defenseGain, speedGain);
                    player.GetComponent<HUDController>().UpdateStrengthSlider();
                    hasAffectedPlayer = true;
                }
                if (isBlue)
                {
                    player.GetComponent<CreatureController>().eatFood(strengthGain, defenseGain, speedGain);
                    player.GetComponent<HUDController>().UpdateDefenseSlider();
                    hasAffectedPlayer = true;
                }
                if (isYellow)
                {
                    player.GetComponent<CreatureController>().eatFood(strengthGain, defenseGain, speedGain);
                    player.GetComponent<HUDController>().UpdateSpeedSlider();
                    hasAffectedPlayer = true;
                }
                if (isGreen)
                {
                    player.GetComponent<CreatureController>().Healing(healthGain);
                    player.GetComponent<HUDController>().UpdateHealthSlider();
                    hasAffectedPlayer = true;
                }
            }
            GetComponent<AudioSource>().Play();
            Destroy(gameObject, GetComponent<AudioSource>().clip.length);
        }
	}

    public void SetIsRed(bool newIsRed)
    {
        isRed = newIsRed;
        isBlue = !newIsRed;
        isYellow = !newIsRed;
        isGreen = !newIsRed;
    }

    public void SetIsBlue(bool newIsBlue)
    {
        isRed = !newIsBlue;
        isBlue = newIsBlue;
        isYellow = !newIsBlue;
        isGreen = !newIsBlue;
    }

    public void SetIsYellow(bool newIsYellow)
    {
        isRed = !newIsYellow;
        isBlue = !newIsYellow;
        isYellow = newIsYellow;
        isGreen = !newIsYellow;
    }

    public void SetIsGreen(bool newIsGreen)
    {
        isRed = !newIsGreen;
        isBlue = !newIsGreen;
        isYellow = !newIsGreen;
        isGreen = newIsGreen;
    }

    public void SetStrengthGain(uint strengthGainValue)
    {
        strengthGain = strengthGainValue;
    }

    public void SetDefenseGain(uint defenseGainValue)
    {
        defenseGain = defenseGainValue;
    }

    public void SetSpeedGain(uint speedGainValue)
    {
        speedGain = speedGainValue;
    }

    public void SetHealthGain(uint healthGainValue)
    {
        healthGain = healthGainValue;
    }
}
