using UnityEngine;
using System.Collections;
using System;



public class BossChaseState : I_State {
    private static BossChaseState instance = null;
    private static readonly object padlock = new object();

    

    private BossChaseState() { }

    public static BossChaseState Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new BossChaseState();
                }
                return instance;
            }
        }
    }

    public void Enter(C_BaseGameEntity entity)
    {
        C_Boss boss = (C_Boss)entity;
        boss.GetComponentInChildren<BehaviourScript>().setBehaviour("Seek");
    }

    public void Execute(C_BaseGameEntity entity)
    {
        C_Boss boss = (C_Boss)entity;
    }

    public void Exit(C_BaseGameEntity entity) {
        //Debug.Log("Exit Chase");
    }
}
