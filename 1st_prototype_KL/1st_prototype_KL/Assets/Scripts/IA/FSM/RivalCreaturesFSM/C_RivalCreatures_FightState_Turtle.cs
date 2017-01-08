using UnityEngine;
using System.Collections;
using System;

public class C_RivalCreatures_FightState_Turtle : I_State
{
    private static C_RivalCreatures_FightState_Turtle instance = null;
    private static readonly object padlock = new object();
    private float timer;

    public float standByTimer = 5;

    

    private C_RivalCreatures_FightState_Turtle() { }

    public static C_RivalCreatures_FightState_Turtle Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new C_RivalCreatures_FightState_Turtle();
                }
                return instance;
            }
        }
    }

    public void Enter(C_BaseGameEntity entity)
    {
        C_RivalCreature_Turtle rivalCreature = (C_RivalCreature_Turtle)entity;
        
        timer = standByTimer;

        BehaviourScript bs = rivalCreature.gameObject.GetComponent<BehaviourScript>();
        bs.SetRotationLocked(true);

        Vector3 targetPosition = rivalCreature.getTargetPosition();
        Vector3 selfPosition = rivalCreature.gameObject.transform.position;

        Vector3 direction = targetPosition - selfPosition;

        Quaternion q = Quaternion.FromToRotation(rivalCreature.transform.up, direction);

        rivalCreature.gameObject.transform.rotation *= q;
        rivalCreature.gameObject.transform.rotation = Quaternion.Euler(0, 0, rivalCreature.gameObject.transform.rotation.eulerAngles.z);
    }

    public void Execute(C_BaseGameEntity entity)
    {
        C_RivalCreature_Turtle rivalCreature = (C_RivalCreature_Turtle)entity;
        AttaqueCCTortue attaqueCC = rivalCreature.gameObject.GetComponent<AttaqueCCTortue>();
        if (rivalCreature.getTarget() != null && (rivalCreature.getTarget().tag.Contains("_RivalCreature") || rivalCreature.getTarget().tag == "Player"))
        {
            Vector3 targetPosition = rivalCreature.getTargetPosition();
            Vector3 selfPosition = rivalCreature.gameObject.transform.position;

            float attackDistance = rivalCreature.attackDistance;
            float currentDistance = Vector3.Distance(selfPosition, targetPosition);

            float speed = rivalCreature.turnSpeed;

            if (currentDistance <= attackDistance)
            {
                //rivalCreature.GetComponentInChildren<BehaviourScript>().behaviour = "Idle";
                rivalCreature.GetComponentInChildren<MoveController>().instantStop();
                if (!rivalCreature.AttackStarted)
                {
                    rivalCreature.gameObject.transform.GetChild(0).transform.localRotation = rivalCreature.gameObject.GetComponent<AttaqueCCTortue>().getInitialRotation();
                    rivalCreature.AttackStarted = true;
                }

                if (!attaqueCC.getAttackAllowed())
                {
                    Vector3 direction = targetPosition - selfPosition;

                    Quaternion q = Quaternion.FromToRotation(rivalCreature.transform.up, direction);

                    rivalCreature.gameObject.transform.rotation *= q;
                    rivalCreature.gameObject.transform.rotation = Quaternion.Euler(0, 0, rivalCreature.gameObject.transform.rotation.eulerAngles.z);

                    attaqueCC.attack();
                }
            }
            else
            {
                rivalCreature.setTarget(null);
                attaqueCC.setAttackAllowed(true);
                rivalCreature.gameObject.transform.GetChild(0).transform.localRotation = rivalCreature.gameObject.GetComponent<AttaqueCCTortue>().getInitialRotation();
                rivalCreature.getFSM().ChangeState(C_RivalCreatures_WanderState_Turtle.Instance);
            }
        }
        else
        {
            rivalCreature.setTarget(null);
            attaqueCC.setAttackAllowed(true);
            rivalCreature.gameObject.transform.GetChild(0).transform.localRotation = rivalCreature.gameObject.GetComponent<AttaqueCCTortue>().getInitialRotation();
            rivalCreature.getFSM().ChangeState(C_RivalCreatures_WanderState_Turtle.Instance);
        }

    }
    public void Exit(C_BaseGameEntity entity)
    {
        C_RivalCreature_Turtle rivalCreature = (C_RivalCreature_Turtle)entity;
        AttaqueCCTortue attaqueCC = rivalCreature.gameObject.GetComponent<AttaqueCCTortue>();
        rivalCreature.AttackStarted = false;
        attaqueCC.setAttackAllowed(true);

        rivalCreature.gameObject.transform.GetChild(0).transform.localRotation = rivalCreature.gameObject.GetComponent<AttaqueCCTortue>().getInitialRotation();  

        BehaviourScript bs = rivalCreature.gameObject.GetComponent<BehaviourScript>();
        bs.SetRotationLocked(false);

    }
}
