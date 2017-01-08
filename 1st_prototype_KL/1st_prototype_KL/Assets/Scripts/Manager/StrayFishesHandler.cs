using UnityEngine;
using System.Collections;

public class StrayFishesHandler : MonoBehaviour {

    //private RoomManager roomManager;
    //private JunctionManager junctionManager;

	// Use this for initialization
	void Start () 
    {

	}
	
    void OnTriggerExit(Collider other)
    {
        RoomManager roomManager = GetComponentInParent<RoomManager>();
        JunctionManager junctionManager = GetComponentInParent<JunctionManager>();

        if (roomManager) 
        {
            if (roomManager.doesThisFishExist(other.gameObject) || roomManager.doesThisCreatureExist(other.gameObject)) 
            {
                roomManager.RemoveFood(other.gameObject, true);
                Destroy(other.gameObject);
            }
        }

        if (junctionManager) 
        {
            if (junctionManager.doesThisFishExist(other.gameObject) || junctionManager.doesThisCreatureExist(other.gameObject)) 
            {
                junctionManager.RemoveFood(other.gameObject);
                Destroy(other.gameObject);
            }
        }
    }

    ////Manage the thermometer
    //void OnTriggerEnter(Collider collider)
    //{
    //    if(collider.CompareTag("Player"))
    //    {
    //        if (roomManager)
    //        {
    //            Thermometer.SetRoomManager(roomManager);
    //            Thermometer.isInJunction = false;
    //        }
    //        if (junctionManager)
    //        {
    //            Thermometer.SetJunctionManager(junctionManager);
    //            Thermometer.isInJunction = true;
    //        }
    //    }
    //}
}
