using UnityEngine;
using System.Collections;

public class BossDashState : I_State {
    private static BossDashState instance = null;
    private static readonly object padlock = new object();

    //private int dashCharge = -1; //-1 = not charging
    private BossDashState() { }

    public static BossDashState Instance {
        get {
            lock (padlock) {
                if (instance == null) {
                    instance = new BossDashState();
                }
                return instance;
            }
        }
    }

    public void Enter(C_BaseGameEntity entity) {
        C_Boss boss = (C_Boss)entity;
        boss.GetComponentInChildren<BehaviourScript>().setBehaviour("Idle"); /*so he doesnt move. We don't care that he stops
                                                                              * tracking his target because when dashing, he is
                                                                              * supposed to dash at the player's position when
                                                                              * he started charging */
        boss.gameObject.GetComponent<BossAttack>().ChargeDash(); // declenche le dash
        //dashCharge = (int)(Time.realtimeSinceStartup * 1000);
    }

    public void Execute(C_BaseGameEntity entity) {
        C_Boss boss = (C_Boss)entity;
       
    }

    public void Exit(C_BaseGameEntity entity) {
        C_Boss boss = (C_Boss)entity;
    }
}