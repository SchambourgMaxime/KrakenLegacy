using UnityEngine;
using System.Collections;

public class GateWatcher : MonoBehaviour {

    JunctionManager junctionManager;

    private bool firstIterationWhenLaunched = true;

	// Use this for initialization
	void Start () 
    {
        junctionManager = GetComponentInParent<JunctionManager>();
	}

    //void OnTriggerEnter(Collider other) 
    //{
    //    bool isGoingTheRightWay = transform.FindChild("Direction").GetComponent<DirectionCheck>().GetIsTouching();

    //    if (other.tag.Contains("Fish"))
    //    {
    //        if(name.Contains("1"))
    //            junctionManager.FishEntryHandler(other.gameObject, 1, isGoingTheRightWay);
    //        else if (name.Contains("2"))
    //            junctionManager.FishEntryHandler(other.gameObject, 2, isGoingTheRightWay);
    //    }
    //    else if (other.tag.Contains("RivalCreature"))
    //    {
    //        if (name.Contains("1"))
    //            junctionManager.CreatureEntryHandler(other.gameObject, 1, isGoingTheRightWay);
    //        else if (name.Contains("2"))
    //            junctionManager.CreatureEntryHandler(other.gameObject, 2, isGoingTheRightWay);
    //    }
    //    else if(other.CompareTag("Player"))
    //    {
    //        if (name.Contains("1"))
    //            junctionManager.PlayerLocation(other.gameObject, 1, isGoingTheRightWay);
    //        else if (name.Contains("2"))
    //            junctionManager.PlayerLocation(other.gameObject, 2, isGoingTheRightWay);
    //    }
    //}

    void OnTriggerExit(Collider other) {
        bool isGoingTheRightWay = transform.FindChild("Direction").GetComponent<DirectionCheck>().GetIsTouching();

        if (other.tag.Contains("Fish")) {
            if (name.Contains("1"))
            {
                junctionManager.FishEntryHandler(other.gameObject, 1);
            }
            else if (name.Contains("2"))
            {
                junctionManager.FishEntryHandler(other.gameObject, 2);
            }
        } else if (other.tag.Contains("RivalCreature") || other.gameObject.name.Contains("Meduse")) {
            if (name.Contains("1"))
            {
                junctionManager.CreatureEntryHandler(other.gameObject, 1);
            }
            else if (name.Contains("2"))
            {
                junctionManager.CreatureEntryHandler(other.gameObject, 2);
            }
        } else if (other.CompareTag("Player")) {
            if (name.Contains("1"))
            {
                junctionManager.PlayerLocation(other.gameObject, 1, isGoingTheRightWay);
            }
            else if (name.Contains("2"))
            {
                junctionManager.PlayerLocation(other.gameObject, 2, isGoingTheRightWay);
            }
        }
    }

}
