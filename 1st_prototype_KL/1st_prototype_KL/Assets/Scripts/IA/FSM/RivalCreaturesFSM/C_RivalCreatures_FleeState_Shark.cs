using UnityEngine;

public class C_RivalCreatures_FleeState_Shark : I_State
{
    private static C_RivalCreatures_FleeState_Shark instance = null;
    private static readonly object padlock = new object();
    private BehaviourScript behaviourScript;
    private Vector3 fleePoint;

    private C_RivalCreatures_FleeState_Shark() { }

    public static C_RivalCreatures_FleeState_Shark Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new C_RivalCreatures_FleeState_Shark();
                }
                return instance;
            }
        }
    }

    public void Enter(C_BaseGameEntity entity)
    {
        C_RivalCreature_Shark rivalCreature = (C_RivalCreature_Shark)entity;
        behaviourScript = rivalCreature.gameObject.GetComponentInChildren<BehaviourScript>();

        fleePoint = rivalCreature.setFleePoint(rivalCreature);
        behaviourScript.SetTarget(fleePoint);
        behaviourScript.behaviour = "Flee";
    }

    public void Execute(C_BaseGameEntity entity)
    {
        C_RivalCreature_Shark rivalCreature = (C_RivalCreature_Shark)entity;

        fleePoint = rivalCreature.setFleePoint(rivalCreature);
        behaviourScript.SetTarget(fleePoint);

        float distance = Vector3.Distance(rivalCreature.gameObject.transform.position, rivalCreature.GetComponentInChildren<BehaviourScript>().targetPosition);
        float maxRange = rivalCreature.detectionRadius;

        if(distance > maxRange)
        {
            rivalCreature.getFSM().ChangeState(C_RivalCreatures_WanderState_Shark.Instance);
        }
    }

    public void Exit(C_BaseGameEntity entity)
    {
    }
}
