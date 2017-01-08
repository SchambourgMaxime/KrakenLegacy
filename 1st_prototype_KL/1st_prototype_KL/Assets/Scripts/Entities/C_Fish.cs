using UnityEngine;
using System.Collections;
using System;

public class C_Fish : C_BaseGameEntity
{
    private C_FiniteStateMachine FSM;
    private I_State defaultState;
    private bool isHidden;
    private bool isDead;
    private Quaternion deathRotation;
    private Transform fishMesh;

    void Start()
    {
        FSM = new C_FiniteStateMachine();
        defaultState = FishWanderState.Instance;
        FSM.Init(this, defaultState);
        fishMesh = this.gameObject.transform.GetChild(0);
    }

    private void Update()
    {

        if (!isDead)
        {
            if (FSM != null)
            {
                FSM.Update();
            }
        }
        else
        {
            if (fishMesh)
            {
                if (fishMesh.rotation != deathRotation)
                {
                    fishMesh.rotation = Quaternion.Slerp(fishMesh.rotation, deathRotation, 0.01f);
                }
            }
        }
    }

    public C_FiniteStateMachine getFSM()
    {
        return FSM;
    }

    public void setIsHidden(bool hide)
    {
        isHidden = hide;
    }

    public bool getIsHidden()
    {
        return isHidden;
    }

    public void die()
    {
        if (!isHidden)
        {
            if (!isDead)
            {
                Destroy(GetComponent<BehaviourScript>());
                Destroy(GetComponentInChildren<Animation>());
                Destroy(GetComponentInChildren<SphereCollider>());
                isDead = true;

                MoveController moveController = GetComponent<MoveController>();
                if (moveController)
                {
                    moveController.instantStop();
                    Transform fishMesh = transform.FindChild("Fish_Mesh");
                    if (fishMesh)
                    {
                        deathRotation = fishMesh.rotation * Quaternion.Euler(0, 0, 180);
                    }
                }
                //Animation de mort à faire
                FoodController edible = GetComponent<FoodController>();
                edible.setIsEdible(true);

                //Destroy(GetComponent<C_Fish>());
            }
        }
    }
}