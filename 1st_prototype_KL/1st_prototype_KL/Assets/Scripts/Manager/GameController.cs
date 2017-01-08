using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    public float minRubberBanding = 10.0f;
    public float minExtremeRubberBanding = 20.0f;
    public float maxRubberBanding = 20.0f;
    public float maxExtremeRubberBanding = 30.0f;

    public float foodMultiplier_extremeLowCreature = 3.0f;
    public float foodMultiplier_lowCreature = 2.0f;
    public float foodMultiplier_normalCreature = 1.0f;
    public float foodMultiplier_highCreature = 0.2f;
    public float foodMultiplier_extremeHighCreature = 0.1f;

    public float checkFrequency = 30.0f;

    private RoomManager[] roomManagers;
    private JunctionManager[] junctionManager;

	// Use this for initialization
	void Start () {
        roomManagers = FindObjectsOfType<RoomManager>();
        Invoke("CheckCreatures", checkFrequency);
        Invoke("junctionTemperature", checkFrequency);
	}

    void CheckCreatures() {
        CreatureController[] creatureControllers = FindObjectsOfType<CreatureController>();

        CreatureController player = GameObject.FindGameObjectWithTag("Player").GetComponent<CreatureController>();

        float minPlayerDifference = player.GetPower() - minRubberBanding;
        float maxPlayerDifference = player.GetPower() + maxRubberBanding;

        float minExtremePlayerDifference = player.GetPower() - minExtremeRubberBanding;
        float maxExtremePlayerDifference = player.GetPower() - maxExtremeRubberBanding;

        foreach (CreatureController instance in creatureControllers) 
        {
            float creaturePower = instance.GetPower();

            if (!instance.CompareTag("Player"))
            {
	            if(creaturePower < minPlayerDifference)
	            {        
                    if(creaturePower < minExtremePlayerDifference)
                    {
                        if (creaturePower != foodMultiplier_extremeLowCreature) 
                        {
                            instance.setGainMultiplier(foodMultiplier_extremeLowCreature);
                        }
                    }
                    else
                    {
                        if (creaturePower != foodMultiplier_lowCreature) 
                        {
                            instance.setGainMultiplier(foodMultiplier_lowCreature);
                        }
                    }
                }
                else if(creaturePower > minPlayerDifference && creaturePower < maxPlayerDifference)
                {
                    if (creaturePower != foodMultiplier_normalCreature) 
                    {
                        instance.setGainMultiplier(foodMultiplier_normalCreature);
                    }
                }
                else if (creaturePower > maxPlayerDifference)
                {
                    if (creaturePower > maxExtremePlayerDifference) 
                    {
                        if (creaturePower != foodMultiplier_extremeHighCreature) 
                        {
                            instance.setGainMultiplier(foodMultiplier_extremeHighCreature);
                        }
                    } 
                    else 
                    {
                        if (creaturePower != foodMultiplier_extremeHighCreature) 
                        {
                            instance.setGainMultiplier(foodMultiplier_highCreature);
                        }
                    }
                }
            }
        }

        Invoke("CheckCreatures", checkFrequency);
    }

    void junctionTemperature()
    {
        int difference = 0;
        junctionManager = FindObjectsOfType<JunctionManager>();
        foreach (JunctionManager junction in junctionManager)
        {
            difference = Mathf.Abs(junction.roomManager1.temperature - junction.roomManager2.temperature);
            difference /= 10;
            if(junction.roomManager1.temperature < junction.roomManager2.temperature)
            {
                junction.roomManager1.temperature += difference;
                junction.roomManager2.temperature -= difference;
            }
            else
            {
                junction.roomManager1.temperature -= difference;
                junction.roomManager2.temperature += difference;
            }
        }
        Invoke("junctionTemperature", checkFrequency);
    }
}
