using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;



public class FishFleeState : I_State
{
    private static FishFleeState instance = null;
    private static readonly object padlock = new object();
    private FishFleeState() { }
    private Transform fishMesh;

    public static FishFleeState Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new FishFleeState();
                }
                return instance;
            }
        }
    }

    public void Enter(C_BaseGameEntity entity)
    {
        C_Fish fish = (C_Fish)entity;
        fishMesh = fish.transform.GetChild(0);
    }

    public void Execute(C_BaseGameEntity entity)
    {
        C_Fish fish = (C_Fish)entity;
        Vector3 averagePosition = Vector3.zero;
        Vector3 hidePosition = Vector3.zero;
        float shortestDistance = 999999999;
        float distance;
        int ennemiesCount = 0;

        RoomManager room = fish.GetComponentInParent<RoomManager>();

        if (room)
        {
            List<GameObject> ennemies = room.GetEnnemiesInRoom();
            foreach (var ennemie in ennemies)
            {
                if (ennemie) 
                {
                    distance = Vector3.Distance(fish.transform.position, ennemie.transform.position);
                    if (distance < entity.detectionRadius) 
                    {
                        averagePosition += (Vector3)ennemie.transform.position;
                        ennemiesCount++;
                    }
                }
            }

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player)
            {
                distance = Vector3.Distance(fish.transform.position, player.transform.position);
                if (distance < entity.detectionRadius)
                {
                    averagePosition += (Vector3)player.transform.position;
                    ennemiesCount++;
                }
            }

            if (ennemiesCount == 0)
            {
                fish.getFSM().ChangeState(FishWanderState.Instance);
            }
            else
            {
                averagePosition /= ennemiesCount;
                GameObject[] hideSpot = GameObject.FindGameObjectsWithTag("HidingSpot");
                for (int i = 0; i < hideSpot.Length; i++)
                {
                    if (shortestDistance > Vector3.Distance(hideSpot[i].transform.position, fish.transform.position) && Vector3.Distance(hideSpot[i].transform.position, fish.transform.position) < entity.hideDistance)
                    {
                        if (Vector3.Dot((averagePosition - fish.transform.position), (hideSpot[i].transform.position - fish.transform.position)) < 0)
                        {
                            shortestDistance = Vector3.Distance(hideSpot[i].transform.position, fish.transform.position);
                            hidePosition = hideSpot[i].transform.position;
                        }
                    }
                }

                if (!fish.GetComponent<C_Fish>().getIsHidden())
                {
                    if (hidePosition == Vector3.zero)
                    {
                        if (fish.transform.GetChildCount() > 0 && Vector3.Distance(averagePosition, fish.transform.position) < 23)
                        {
                            if (fishMesh)
                            {
                                SphereCollider collider = fishMesh.GetComponent<SphereCollider>();
                                if(collider)
                                {
                                    collider.isTrigger = true;
                                }                                
                            }
                            else
                            {
                                fishMesh = fish.transform.GetChild(0);
                            }
                        }
                        else
                        {
                            if (fishMesh)
                            {
                                SphereCollider collider = fishMesh.GetComponent<SphereCollider>();
                                if (collider)
                                {
                                    fishMesh.GetComponent<SphereCollider>().isTrigger = false;
                                }
                            }
                            else
                            {
                                fishMesh = fish.transform.GetChild(0);
                            }
                        }

                        fish.GetComponentInChildren<BehaviourScript>().SetTarget(averagePosition);
                        fish.GetComponentInChildren<BehaviourScript>().setBehaviour("Flee");
                    }
                    else
                    {
                        GameObject spotPb = GameObject.Find("HidingSpot");
                        GameObject spotPb2 = GameObject.Find("HidingSpot (4)");

                        if (Vector3.Distance(spotPb.transform.position, fish.transform.position) < 120 && fish.transform.position.y > hidePosition.y)
                        {
                            Vector3[] path = { new Vector3(-8.45f, 65, -1), new Vector3(-8.45f, 58.5f, -1), hidePosition };
                            fish.GetComponentInChildren<BehaviourScript>().setPath(path);
                            fish.GetComponentInChildren<BehaviourScript>().setBehaviour("Follow Path");
                        }
                        else if (Vector3.Distance(spotPb2.transform.position, fish.transform.position) < 120 && fish.transform.position.y < hidePosition.y)
                        {
                            Vector3[] path = { new Vector3(183, 47, -1), new Vector3(186, 67, -1), hidePosition };
                            fish.GetComponentInChildren<BehaviourScript>().setPath(path);
                            fish.GetComponentInChildren<BehaviourScript>().setBehaviour("Follow Path");
                        }
                        else
                        {
                            fish.GetComponentInChildren<BehaviourScript>().SetTarget(hidePosition);
                            fish.GetComponentInChildren<BehaviourScript>().setBehaviour("Arrive");
                        }
                    }
                }
            } 
        }        
    }

    public void Exit(C_BaseGameEntity entity)
    {
        //Debug.Log("Exit Flee");
    }
}
