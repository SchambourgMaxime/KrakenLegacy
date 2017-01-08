using UnityEngine;
using System.Collections.Generic;

public class C_RivalCreatures_WanderState_Shark : I_State
{
    private static C_RivalCreatures_WanderState_Shark instance = null;
    private static readonly object padlock = new object();

    private C_RivalCreatures_WanderState_Shark() { }

    public static C_RivalCreatures_WanderState_Shark Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new C_RivalCreatures_WanderState_Shark();
                }
                return instance;
            }
        }
    }

    public void Enter(C_BaseGameEntity entity)
    {
        C_RivalCreature_Shark rivalCreature = (C_RivalCreature_Shark)entity;

        Collider[] detectedEntities = Physics.OverlapSphere(rivalCreature.transform.position, rivalCreature.detectionRadius, 0, QueryTriggerInteraction.UseGlobal);

        List<GameObject> detectedRC = new List<GameObject>();
        List<GameObject> detectedFishes = new List<GameObject>();

        //float satiety = rivalCreature.getSatiety();
        //float maxSatiety = rivalCreature.getMaxSatiety();

        if(detectedEntities.Length != 0)
        {
            float distance = 0;
            string selfTag = rivalCreature.gameObject.tag;

            foreach(Collider collider in detectedEntities)
            {
                if (collider != rivalCreature.gameObject.GetComponent<Collider>())
                {
                    if ((collider.gameObject.tag.Contains("Fish") || collider.gameObject.tag.Contains("Plant") || collider.gameObject.tag == "Orbs") && !collider.gameObject.GetComponent<C_Fish>().getIsHidden())
                    {
                        detectedFishes.Add(collider.gameObject);
                    }
                    else if((collider.gameObject.tag.Contains("_RivalCreature") || collider.gameObject.tag == "Player") && selfTag != collider.gameObject.tag)
                    {
                        detectedRC.Add(collider.gameObject);
                    }
                }
            }

            if (detectedRC.Count != 0)
            {
                if(detectedRC.Count == 1)
                {
                    rivalCreature.setTarget(detectedRC[0]);
                    if (rivalCreature.initiateSharkAttack(rivalCreature.SharkThresholdCalculation(rivalCreature.GetComponentInChildren<CreatureController>().GetPower(), detectedRC[0].GetComponentInChildren<CreatureController>().GetPower())))
                    {
                        rivalCreature.getFSM().ChangeState(C_RivalCreatures_FightState_Shark.Instance);
                    }
                    else
                    {
                        rivalCreature.getFSM().ChangeState(C_RivalCreatures_FleeState_Shark.Instance);
                    }
                }
                else
                {
                    Vector3 fleePoint = rivalCreature.setFleePoint(rivalCreature);
                    rivalCreature.GetComponentInChildren<BehaviourScript>().SetTarget(fleePoint);
                    rivalCreature.getFSM().ChangeState(C_RivalCreatures_FleeState_Shark.Instance);
                }
            }
            else if(detectedFishes.Count != 0/* && satiety < maxSatiety*/)
            {
                foreach(GameObject go in detectedFishes)
                {
                    float newDistance = Vector3.Distance(rivalCreature.gameObject.transform.position, go.transform.position);

                    if (distance == 0)
                    {
                        distance = newDistance;
                    }

                    if (newDistance <= distance)
                    {
                        rivalCreature.setTarget(go);
                    }
                }
                rivalCreature.getFSM().ChangeState(C_RivalCreatures_HuntState_Shark.Instance);
            }
        }
        else
        {
            rivalCreature.GetComponentInChildren<BehaviourScript>().behaviour = "Wander";
        }
    }

    public void Execute(C_BaseGameEntity entity)
    {
        C_RivalCreature_Shark rivalCreature = (C_RivalCreature_Shark)entity;

        rivalCreature.GetComponentInChildren<BehaviourScript>().behaviour = "Wander";
    }

    public void Exit(C_BaseGameEntity entity)
    {

    }
}
