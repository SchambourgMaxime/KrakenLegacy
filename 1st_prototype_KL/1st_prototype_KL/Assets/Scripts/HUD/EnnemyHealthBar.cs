using UnityEngine;
using System.Collections;

public class EnnemyHealthBar : MonoBehaviour {

    public float adjustment = 2.3f;
    public Font textFont;
    public GUISkin sliderStyle;

    private Vector3 worldPosition = new Vector3();
    private Vector3 screenPosition = new Vector3();
    private Transform myTransform;
    private Camera myCamera;
    private float healthBarHeight = 3.5f;
    private int healthBarLeft = 110;
    private int barTop = 1;
    private GUIStyle myStyle = new GUIStyle();

    private int currentHealth;
    private int maxHealth;

    private GameObject myCam;
    private GUIStyle style;
   
    // Use this for initialization
    void Start ()
    {
        myCam = GameObject.FindGameObjectWithTag("MainCamera");
        myTransform = transform;
        myCamera = Camera.main;
        currentHealth = (int)GetComponent<CreatureController>().currentLifePoints;
        maxHealth = (int)GetComponent<CreatureController>().maxLifePoints;
        style = new GUIStyle();
        style.font = textFont;
        style.fontSize = 8;
        style.normal.textColor = Color.white;
    }

    void OnGUI()
    {
        currentHealth = (int)GetComponent<CreatureController>().currentLifePoints;
        maxHealth = (int)GetComponent<CreatureController>().maxLifePoints;
        worldPosition = new Vector3(myTransform.position.x, myTransform.position.y + adjustment, myTransform.position.z);
        screenPosition = myCamera.WorldToScreenPoint(worldPosition);

        //creating a ray that will travel forward from the camera's position   
        RaycastHit hit;
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        float distance = Vector3.Distance(myCam.transform.position, transform.position); //gets the distance between the camera, and the intended target we want to raycast to

        //GUI.contentColor = Color.white;
        //GUI.color = Color.red;
        GUI.backgroundColor = Color.green;
        GUI.HorizontalScrollbar(new Rect(screenPosition.x - healthBarLeft / 2, Screen.height - screenPosition.y - barTop, 120, 300), 0, currentHealth, 0, maxHealth); //displays a healthbar
 
        GUI.color = Color.white;
        GUI.contentColor = Color.white;       
        GUI.Label(new Rect(screenPosition.x - healthBarLeft / 2 + 40, Screen.height - screenPosition.y + 3, 100, 100), currentHealth + " / " + maxHealth, style); //displays health in text format
    }
}
