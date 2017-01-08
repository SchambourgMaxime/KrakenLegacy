using UnityEngine;
using System.Collections;


public class TutorialTextBox : MonoBehaviour {

    public bool isTutoStats = false;
    public bool isTutoDash = false;
    public bool isTutoDestruction = false;
    public bool isTutoLight = false;
    public bool isTutoTemperature = false;
    public bool isTutoAttack = false;

    public Texture textureStats;
    public Texture textureNoDash;
    public Texture textureDash;
    public Texture textureNoDestruction;
    public Texture textureDestruction;
    public Texture textureNoLight;
    public Texture textureLight;
    public Texture textureTemperature;
    public Texture textureAttack;

    public GameObject planeForTutorialImage;

    private CreatureController creatureController;
    private MoveController moveController;
    private bool guiEnabled = false;
    private bool hasEnoughStrength = false;
    private bool hasEnoughSpeed = false;
    private bool hasPowerOfLight = false;
 
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        moveController = player.GetComponent<MoveController>();
        creatureController = player.GetComponent<CreatureController>();
        GetComponentInParent<AudioSource>().pitch = Random.Range(0.5f, 1.5f);

        if (isTutoStats)
            planeForTutorialImage.GetComponent<Renderer>().material.mainTexture = textureStats;
        if (isTutoDash)
            planeForTutorialImage.GetComponent<Renderer>().material.mainTexture = textureNoDash;
        if (isTutoDestruction)
            planeForTutorialImage.GetComponent<Renderer>().material.mainTexture = textureNoDestruction;
        if (isTutoLight)
            planeForTutorialImage.GetComponent<Renderer>().material.mainTexture = textureNoLight;
        if (isTutoTemperature)
            planeForTutorialImage.GetComponent<Renderer>().material.mainTexture = textureTemperature;
        if (isTutoAttack)
            planeForTutorialImage.GetComponent<Renderer>().material.mainTexture = textureAttack;
    }

    void Update()
    {
        if(isTutoDash && hasEnoughSpeed)
        {
            planeForTutorialImage.GetComponent<Renderer>().material.mainTexture = textureDash;
        }

        if(isTutoDestruction && hasEnoughStrength)
        {
            planeForTutorialImage.GetComponent<Renderer>().material.mainTexture = textureDestruction;
        }

        if(isTutoLight && hasPowerOfLight)
        {
            planeForTutorialImage.GetComponent<Renderer>().material.mainTexture = textureLight;
        }
    }

    void OnGUI()
    {
        if (guiEnabled)
        {
            planeForTutorialImage.SetActive(true);
        }

        if (!guiEnabled)
        {
            planeForTutorialImage.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Player"))
        {
            hasEnoughSpeed = moveController.GetDashUnlocked();
            hasEnoughStrength = creatureController.CanBreakWalls();
            hasPowerOfLight = creatureController.GetHasLightPower();
	        guiEnabled = true;
            GetComponentInParent<AudioSource>().Play();
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
	        guiEnabled = false;
        }
    }
}
