using UnityEngine;
using System.Collections;

public class FogOfWarFading : MonoBehaviour {

    public float duration = 1.0f;
    public bool deadlyFOW;
    public GameObject wall;

    private bool isFogActivated = false;
    private GameObject player;
    private float startTime;

	void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            player = collider.gameObject;
            player.GetComponent<FogOfWar>().SetFirstTimeThroughAFadingTrigger(true);

            if (player.GetComponent<CreatureController>().GetHasLightPower())
            {
                deadlyFOW = false;
                if (wall)
                    Destroy(wall);
            }

            if (!isFogActivated)
            {
                startTime = Time.time;
                isFogActivated = true;
                player.GetComponent<FogOfWar>().SetInLight(false);
            }

        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player")) 
        {
            player = collider.gameObject;

            bool isGoingTheRightWay = transform.FindChild("Direction").GetComponent<DirectionCheck>().GetIsTouching();

            if (!isGoingTheRightWay) 
            {
                if (isFogActivated) 
                {
                    startTime = Time.time;
                    isFogActivated = false;
                    player.GetComponent<FogOfWar>().SetInLight(true);
                }
            }
        }
    }

    void Update()
    {
        if(player)
        {
	        float t = (Time.time - startTime) / duration;
	        if (!isFogActivated)
	        {
                AugmentRadius();
	        }
	        else
	        {
                ReduceRadius();
	        }
        }
    }

    public void AugmentRadius()
    {
        float t = (Time.time - startTime) / duration;
        if(!deadlyFOW)
            player.GetComponent<FogOfWar>().SetCurrentRadiusFOW(Mathf.SmoothStep(player.GetComponent<FogOfWar>().GetCurrentRadiusFOW(), player.GetComponent<FogOfWar>().GetVisionOfThePlayer(), t));
        else if(deadlyFOW)
            player.GetComponent<FogOfWar>().SetCurrentRadiusFOW(Mathf.SmoothStep(player.GetComponent<FogOfWar>().GetRadiusFogOfWar() * 0.001f, player.GetComponent<FogOfWar>().GetVisionOfThePlayer(), t));
    }

    public void ReduceRadius()
    {
        float t = (Time.time - startTime) / duration;
        if(!deadlyFOW)
            player.GetComponent<FogOfWar>().SetCurrentRadiusFOW(Mathf.SmoothStep(player.GetComponent<FogOfWar>().GetCurrentRadiusFOW(), player.GetComponent<FogOfWar>().GetRadiusFogOfWar(), t));
        else if (deadlyFOW)
            player.GetComponent<FogOfWar>().SetCurrentRadiusFOW(Mathf.SmoothStep(player.GetComponent<FogOfWar>().GetCurrentRadiusFOW(), player.GetComponent<FogOfWar>().GetRadiusFogOfWar() * 0.001f, t));
    }
}
