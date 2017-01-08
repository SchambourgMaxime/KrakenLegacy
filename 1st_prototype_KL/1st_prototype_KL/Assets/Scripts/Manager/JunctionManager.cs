using UnityEngine;
using System.Collections.Generic;

public class JunctionManager : MonoBehaviour {

    public RoomManager roomManager1;
    public RoomManager roomManager2;

    private BoxCollider entry1;
    private BoxCollider entry2;

    private List<GameObject> existingFishes;
    private List<GameObject> existingConcurentCreatures;
    private int temperature;
    private bool isPlayerIn = false;

    // Use this for initialization
    void Start ()
    {
        existingFishes = new List<GameObject>();
        existingConcurentCreatures = new List<GameObject>();
        ComputeTemperature();
    }

    void Update()
    {
        ComputeTemperature();       
    }

    public void PlayerLocation(GameObject player, int entryID, bool enteringJunction)
    {
        RoomManager[] roomManagers = { roomManager1, roomManager2 };

        if (isPlayerIn && enteringJunction == false)
        {
            Thermometer.SetRoomManager(roomManagers[entryID - 1]);
            isPlayerIn = false;
        }
        else if (!isPlayerIn && enteringJunction == true) 
        {
            Thermometer.SetJunctionManager(this);
            isPlayerIn = true;
        }
    }

    public void RemoveFood(GameObject foodToRemove) 
    {
        if (existingFishes.Contains(foodToRemove))
        {
            existingFishes.Remove(foodToRemove);
            BehaviourScript bs = foodToRemove.GetComponent<BehaviourScript>();
            if (bs)
            {
                RoomManager rm = bs.roomOrigine.GetComponent<RoomManager>();
                if (rm)
                {
                    rm.currentFishRoomOrigine--;
                }
            }
        }

        else if (existingConcurentCreatures.Contains(foodToRemove))
        {
            existingConcurentCreatures.Remove(foodToRemove);
            BehaviourScript bs = foodToRemove.GetComponent<BehaviourScript>();
            if (bs)
            {
                RoomManager rm = bs.roomOrigine.GetComponent<RoomManager>();
                if (rm)
                {
                    rm.currentConcurentCreatureRoomOrigine--;
                }
            }
        }
    }

    public void FishEntryHandler(GameObject enteringFish, int entryID)
    {
        RoomManager[] roomManagers = { roomManager1, roomManager2 };

        bool enteringJunction = false;

        if (roomManagers[entryID - 1].doesThisFishExist(enteringFish))
            enteringJunction = true;

        //if (enteringJunction)
        //    Debug.Log(enteringFish.name + " from " + roomManagers[entryID - 1].name + " to " + gameObject.name);
        //else
        //    Debug.Log(enteringFish.name + "from " + gameObject.name + " to " + roomManagers[entryID - 1].name);

        if (existingFishes.Contains(enteringFish) && enteringJunction == false)
        {
            roomManagers[entryID - 1].AddFish(enteringFish);
            enteringFish.transform.parent = roomManagers[entryID - 1].transform;
            existingFishes.Remove(enteringFish);

            int value = 0;
            if (entryID - 1 == 0)
                value = 1;
            roomManagers[value].FishLeft(enteringFish);
        }
        else if (!existingFishes.Contains(enteringFish) && enteringJunction == true) 
        {
            roomManagers[entryID - 1].RemoveFood(enteringFish);
            enteringFish.transform.parent = transform;
            existingFishes.Add(enteringFish);

            int value = 0;
            if (entryID - 1 == 0)
                value = 1;
            roomManagers[value].FishLeft(enteringFish);
        }
    }

    public void CreatureEntryHandler(GameObject enteringCreature, int entryID) 
    {
        RoomManager[] roomManagers = { roomManager1, roomManager2 };

        bool enteringJunction = false;

        if (roomManagers[entryID - 1].doesThisCreatureExist(enteringCreature))
            enteringJunction = true;

        //if (enteringJunction)
        //    Debug.Log(enteringCreature.name + " from " + roomManagers[entryID - 1].name + " to " + gameObject.name);
        //else
        //    Debug.Log(enteringCreature.name + "from " + gameObject.name + " to " + roomManagers[entryID - 1].name);

        if (existingConcurentCreatures.Contains(enteringCreature) && enteringJunction == false) 
        {
            roomManagers[entryID - 1].AddConcurentCreature(enteringCreature);

            int value = 0;
            if (entryID - 1 == 0)
                value = 1;
            roomManagers[value].CreatureLeft(enteringCreature);

            enteringCreature.transform.parent = roomManagers[entryID - 1].transform;
            existingConcurentCreatures.Remove(enteringCreature);
        } 
        else if (!existingConcurentCreatures.Contains(enteringCreature) && enteringJunction == true)
        {
            roomManagers[entryID - 1].RemoveFood(enteringCreature);
            enteringCreature.transform.parent = transform;
            existingConcurentCreatures.Add(enteringCreature);

            int value = 0;
            if (entryID - 1 == 0)
                value = 1;
            roomManagers[value].CreatureLeft(enteringCreature);
        }
    }

    public bool doesThisFishExist(GameObject fishToInspect) 
    {
        return existingFishes.Contains(fishToInspect);
    }

    public bool doesThisCreatureExist(GameObject creatureToInspect) {
        return existingConcurentCreatures.Contains(creatureToInspect);
    }

    public int GetTemperature()
    {
        return temperature;
    }

    public int GetMaxTemperature()
    {
        return (int)Mathf.Round((roomManager1.GetMaxTemperature() + roomManager2.GetMaxTemperature()) / 2);
    }

    void ComputeTemperature()
    {
        temperature = (int)Mathf.Round((roomManager1.GetTemperature() + roomManager2.GetTemperature()) / 2);
    }
}
