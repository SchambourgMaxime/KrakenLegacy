using UnityEngine;
using System.Collections;
using System;

public class C_RivalCreatures_FightState_Shark : I_State
{
    private static C_RivalCreatures_FightState_Shark instance = null;
    private static readonly object padlock = new object();
    private float timer;
    private GameObject teeth;

    public float standByTimer = 5;

    private C_RivalCreatures_FightState_Shark() { }

    public static C_RivalCreatures_FightState_Shark Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new C_RivalCreatures_FightState_Shark();
                }
                return instance;
            }
        }
    }

    public void Enter(C_BaseGameEntity entity)
    {
        C_RivalCreature_Shark rivalCreature = (C_RivalCreature_Shark)entity;
        timer = standByTimer;
        teeth = rivalCreature.gameObject.transform.Find("AttaqueDents").gameObject;

        Vector3 targetPosition = rivalCreature.getTargetPosition();
        Vector3 selfPosition = rivalCreature.gameObject.transform.position;

        Vector3 direction = targetPosition - selfPosition;

        Quaternion q = Quaternion.FromToRotation(rivalCreature.transform.up, direction);

        rivalCreature.gameObject.transform.rotation *= q;
    }

    public void Execute(C_BaseGameEntity entity)
    {
        C_RivalCreature_Shark rivalCreature = (C_RivalCreature_Shark)entity;
        AttaqueCCRequin attaqueCC = rivalCreature.gameObject.GetComponent<AttaqueCCRequin>();
        if (rivalCreature.getTarget() != null && (rivalCreature.getTarget().tag.Contains("_RivalCreature") || rivalCreature.getTarget().tag == "Player"))
        {
            Vector3 targetPosition = rivalCreature.getTargetPosition();
            Vector3 selfPosition = rivalCreature.gameObject.transform.position;

            float attackDistance = rivalCreature.attackDistance;
            float currentDistance = Vector3.Distance(selfPosition, targetPosition);

            float speed = rivalCreature.turnSpeed;

            if (currentDistance <= attackDistance)
            {
                if (!attaqueCC.getAttackAllowed())
                {
                    Vector3 direction = targetPosition - selfPosition;

                    Quaternion q = Quaternion.FromToRotation(rivalCreature.transform.up, direction);

                    rivalCreature.gameObject.transform.rotation *= q;

                    attaqueCC.attack();
                }
            }
            else
            {
                rivalCreature.setTarget(null);
                rivalCreature.gameObject.GetComponent<AttaqueCCRequin>().stopChaseAnim();
                teeth.SetActive(false);
                rivalCreature.getFSM().ChangeState(C_RivalCreatures_WanderState_Shark.Instance);
            }
        }
        else
        {
            rivalCreature.setTarget(null);
            rivalCreature.gameObject.GetComponent<AttaqueCCRequin>().stopChaseAnim();
            teeth.SetActive(false);
            rivalCreature.getFSM().ChangeState(C_RivalCreatures_WanderState_Shark.Instance);
        }
        
    }
    public void Exit(C_BaseGameEntity entity)
    {

    }
}
