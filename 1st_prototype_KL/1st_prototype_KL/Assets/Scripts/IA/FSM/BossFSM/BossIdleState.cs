using UnityEngine;
using System.Collections;

public class BossIdleState : I_State {
    private static BossIdleState instance = null;
    private static readonly object padlock = new object();



    private BossIdleState() { }

    public static BossIdleState Instance {
        get {
            lock (padlock) {
                if (instance == null) {
                    instance = new BossIdleState();
                }
                return instance;
            }
        }
    }

    public void Enter(C_BaseGameEntity entity) {
        C_Boss boss = (C_Boss)entity;
        boss.GetComponentInChildren<BehaviourScript>().setBehaviour("Idle");
    }

    public void Execute(C_BaseGameEntity entity) {
        C_Boss boss = (C_Boss)entity;
        //do nothing        
    }

    public void Exit(C_BaseGameEntity entity) {
        //Debug.Log("Exit Chase");
    }
}
