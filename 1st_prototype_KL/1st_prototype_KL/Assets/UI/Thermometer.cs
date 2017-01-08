using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Thermometer : MonoBehaviour {

    public Sprite thermometreExtremeCold;
    public Sprite thermometreMoreCold;
    public Sprite thermometreCold;

    public Sprite thermometreWarm;
    public Sprite thermometreMoreWarm;
    public Sprite thermometreExtremeWarm;

    public Sprite thermometreHot;
    public Sprite thermometreMoreHot;
    public Sprite thermometreExtremeHot;

    public RoomManager firstRoom;

    public static bool isInJunction = false;

    private int maxTemperature;
    private int currentTemperature;
    
    private static RoomManager roomManager;
    private static JunctionManager junctionManager;

    private int limitExtremeCold;
    private int limitMoreCold;
    private int limitCold;

    private int limitWarm;
    private int limitMoreWarm;
    private int limitExtremeWarm;

    private int limitHot;
    private int limitMoreHot;

    // Use this for initialization
    void Start ()
    {
        roomManager = firstRoom;
	    SetTemperatures();
        SetLimits();
        ChangeImage();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        SetTemperatures();
        SetLimits();
        ChangeImage();
    }

    void SetTemperatures()
    {
        if(junctionManager)
        {
            maxTemperature = junctionManager.GetMaxTemperature();
            currentTemperature = junctionManager.GetTemperature();
        }

        else if (roomManager)
        {
            maxTemperature = roomManager.GetMaxTemperature();
            currentTemperature = roomManager.GetTemperature();
            roomManager.ChangeTemperature(currentTemperature);
        }
    }

    public static void SetRoomManager(RoomManager newRoomManager)
    {
        roomManager = newRoomManager;
        junctionManager = null;
    }

    public static void SetJunctionManager(JunctionManager newJunctionManager)
    {
        junctionManager = newJunctionManager;
        roomManager = null;
    }

    void SetLimits()
    {
        limitExtremeCold    = (int)Mathf.Floor((1f * maxTemperature / 9f));
        limitMoreCold       = (int)Mathf.Floor(2f * maxTemperature / 9f);
        limitCold           = (int)Mathf.Floor(3f * maxTemperature / 9f);
        limitWarm           = (int)Mathf.Floor(4f * maxTemperature / 9f);
        limitMoreWarm       = (int)Mathf.Floor(5f * maxTemperature / 9f);
        limitExtremeWarm    = (int)Mathf.Floor(6f * maxTemperature / 9f);
        limitHot            = (int)Mathf.Floor(7f * maxTemperature / 9f);
        limitMoreHot        = (int)Mathf.Floor(8f * maxTemperature / 9f);
    }

    void ChangeImage()
    {
        if (currentTemperature <= limitExtremeCold)
        {
          	GetComponent<Image>().sprite = thermometreExtremeCold;
        }
        if (currentTemperature > limitExtremeCold && currentTemperature <= limitMoreCold)
        {
           	GetComponent<Image>().sprite = thermometreMoreCold;
        }
        if (currentTemperature > limitMoreCold && currentTemperature <= limitCold)
        {
          	GetComponent<Image>().sprite = thermometreCold;
        }
        if (currentTemperature > limitCold && currentTemperature <= limitWarm)
        {
          	GetComponent<Image>().sprite = thermometreWarm;
        }
        if (currentTemperature > limitWarm && currentTemperature <= limitMoreWarm)
        {
            GetComponent<Image>().sprite = thermometreMoreWarm;
        }
        if (currentTemperature > limitMoreWarm && currentTemperature <= limitExtremeWarm)
        {
            GetComponent<Image>().sprite = thermometreExtremeWarm;
        }
        if (currentTemperature > limitExtremeWarm && currentTemperature <= limitHot)
        {
            GetComponent<Image>().sprite = thermometreHot;
        }
        if (currentTemperature > limitHot && currentTemperature <= limitMoreHot)
        {
            GetComponent<Image>().sprite = thermometreMoreHot;
        }
        if (currentTemperature > limitMoreHot)
        {
            GetComponent<Image>().sprite = thermometreExtremeHot; 
        }
                   
    }
}
