using UnityEngine;
using System.Collections;

public class FogOfWar : MonoBehaviour
{

    public GameObject FogOfWarPlane;
    public float radiusFogOfWar;
    public uint numForShader;

    private GameObject mainCamera;
    private GameObject player;
    private float visionOfThePlayer;
    private bool inLight = true;
    private float currentRadiusFOW;
    private Vector3 borderPos;
    private bool destroyed = false;
    private bool firstTimeThroughAFadingTrigger = false;

    // Use this for initialization
    void Start()
    {
        if(FogOfWarPlane)
        {
	        //FogOfWarPlane.SetActive(true);
	        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
	        player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    void Update()
    {
        if(FogOfWarPlane)
        {
	        if (gameObject.CompareTag("Player"))
	        {
                if (!firstTimeThroughAFadingTrigger)
                    currentRadiusFOW = GetVisionOfThePlayer();

                Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
	            Ray rayToPlayerPos = Camera.main.ScreenPointToRay(screenPos);

                RaycastHit hit;
	            if (Physics.Raycast(rayToPlayerPos, out hit, 1000))
	            {
	                FogOfWarPlane.GetComponent<Renderer>().sharedMaterial.SetVector("_Player_Pos", hit.point);
	                FogOfWarPlane.GetComponent<Renderer>().sharedMaterial.SetFloat("_FogRadius", currentRadiusFOW);
	            }
	        }
	
            else if(gameObject.CompareTag("Special"))
            {
                FogOfWarPlane.GetComponent<Renderer>().sharedMaterial.SetVector("_SpecialObjectPos", transform.position);
                FogOfWarPlane.GetComponent<Renderer>().sharedMaterial.SetFloat("_SpecialObjectRadius", radiusFogOfWar);
            }

	        else
	        {
                if (!destroyed)
                {
                    FogOfWarPlane.GetComponent<Renderer>().sharedMaterial.SetVector("_Points" + numForShader.ToString(), transform.position);
                    FogOfWarPlane.GetComponent<Renderer>().sharedMaterial.SetFloat("_ObjectFogRadius", radiusFogOfWar);
                }
	        }
        }
    }

    void OnDestroy()
    {
        if (gameObject.CompareTag("Special"))
        {
            FogOfWarPlane.GetComponent<Renderer>().sharedMaterial.SetFloat("_SpecialObjectRadius", 0.0001f);
        }
        else if (FogOfWarPlane)
        {
            FogOfWarPlane.GetComponent<Renderer>().sharedMaterial.SetFloat("_ObjectFogRadius" + numForShader.ToString(), 0.0001f);
            destroyed = true;
        }
    }

    public void SetInLight(bool newInLight)
    {
        inLight = newInLight;
    }

    public bool GetInLight()
    {
        return inLight;
    }

    public float GetVisionOfThePlayer()
    {
        ComputeVisionOfThePlayer();
        return visionOfThePlayer;
    }

    public void SetVisionOfThePlayer(float newVisionOfThePlayer)
    {
        visionOfThePlayer = newVisionOfThePlayer;
    }

    public void ComputeVisionOfThePlayer()
    {
        borderPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        visionOfThePlayer = (borderPos - transform.position).magnitude * 1.25f;
    }

    public float GetCurrentRadiusFOW()
    {
        return currentRadiusFOW;
    }

    public void SetCurrentRadiusFOW(float newRadiusFOW)
    {
        currentRadiusFOW = newRadiusFOW;
    }

    public void SetFirstTimeThroughAFadingTrigger(bool newFirstTimeThroughAFadingTrigger)
    {
        firstTimeThroughAFadingTrigger = newFirstTimeThroughAFadingTrigger;
    }

    public float GetRadiusFogOfWar()
    {
        return radiusFogOfWar + ((player.GetComponent<CreatureController>().sizeGain * player.GetComponent<CreatureController>().GetSize()) * 2f);
    }

    public void ResetRadius()
    {
        firstTimeThroughAFadingTrigger = false;
        currentRadiusFOW = GetVisionOfThePlayer();
    }
}
