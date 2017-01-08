using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;



public class FishWanderState : I_State
{
    private static FishWanderState instance = null;
    private static readonly object padlock = new object();

    private FishWanderState() { }

    public static FishWanderState Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new FishWanderState();
                }
                return instance;
            }
        }
    }

    public void Enter(C_BaseGameEntity entity)
    {
        C_Fish fish = (C_Fish)entity;
        fish.GetComponentInChildren<BehaviourScript>().setBehaviour("Wander");
    }

    public void Execute(C_BaseGameEntity entity)
    {
        C_Fish fish = (C_Fish)entity;
        bool ennemi = false;
        float distance;
        
        RoomManager room = fish.GetComponentInParent<RoomManager>();

        if(room)
        {
            List<GameObject> ennemies = room.GetEnnemiesInRoom();
            foreach(var ennemie in ennemies)
            {
                if (ennemie) 
                {
                    distance = Vector3.Distance(fish.transform.position, ennemie.transform.position);
                    if (distance < entity.detectionRadius) 
                    {
                        ennemi = true;
                        fish.getFSM().ChangeState(FishFleeState.Instance);
                        break;
                    }
                }
            }

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if(player)
            {
                distance = Vector3.Distance(fish.transform.position, player.transform.position);
                if (distance < entity.detectionRadius)
                {
                    ennemi = true;
                    fish.getFSM().ChangeState(FishFleeState.Instance);
                }
            }

            if (!ennemi)
            {
                List<GameObject> fishes = room.GetFishesInRoom();

                foreach (var fishA in fishes)
                {
                    if (fishA && fishA.transform.position != fish.transform.position && Vector3.Distance(fish.transform.position, fishA.transform.position) < entity.detectionRadius)
                    {
                        fish.getFSM().ChangeState(FishFlockState.Instance);
                        break;
                    }
                }
            }
        }
    }

    public void Exit(C_BaseGameEntity entity)
    {
        //Debug.Log("Exit Wander");
    }
}
