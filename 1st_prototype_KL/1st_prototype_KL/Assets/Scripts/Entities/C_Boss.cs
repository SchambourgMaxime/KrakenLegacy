using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class C_Boss : C_BaseGameEntity {
    private C_FiniteStateMachine FSM;
    private I_State defaultState;


    //timers
    private int dashTimer = 0; //it should be at 0 when the game starts, then it gets reset everytime the player dashes
    private int recoveryTimer;
    private int decelerationTimer;
    private int dashCDCompensation; //this value is equal to the time it takes to charge a dash + the recovery time, so we can compensate for it when calculating the dash CD
    private int deathTimer;
    private float timeToReset;

    //bools
    private bool isRecovering;
    private bool isDecelerating = false;
    private bool isActive;
    private bool isDying;
    private bool isDead = false;
    private bool isResetting = false;

    private Vector3 dashTarget;
    private Vector3 basePosition;
    private int baseRunningSpeed;
    private float recoveryCompensation; //so the recoil force diminishes over time based on recovery time and recovery force
    private float baseTurnRate;
    private GameObject[] crystals;
    private GameObject bossTriggerRef;
    private GameObject currentTarget = null;

    public int dashCDDecrease = 2000; //if we want the boss' dash CD to decrease at some point (a certain fight phase, etc). Only occurs once for now (at half health)
    public int recoveryTime = 750;
    public int dashCD = 10000;
    public int health;
    public int maxHealth = 4;
    public float recoveryForce = 500.0f;
    public int decelerationTime = 1500;
    public GameObject dropPrefab;
    public int bossDeathAnimTime = 1500;

    public AudioClip crystalBreak;

    public float recoveryTimeDamage = 1;
    public int recoveryFlash = 6;
    private float recoveryFlashingSpeed;
    public Texture redTexture;
    private Texture normalTexture;

    void Start()
    {
        isActive = false;
        FSM = new C_FiniteStateMachine();
        defaultState = BossIdleState.Instance;
        FSM.Init(this, defaultState);
        health = maxHealth;
        isRecovering = false;
        recoveryCompensation = recoveryForce / recoveryTime; //so the recoil force diminishes over time bases on recovery time and recovery force
        ComputeDashCDCompensation();
        baseRunningSpeed = GetComponent<BehaviourScript>().maxRunningSpeed;
        baseTurnRate = GetComponent<MoveController>().turnRate;
        basePosition = this.transform.position;
        crystals = GameObject.FindGameObjectsWithTag("BossSpikes");
        timeToReset = GameObject.FindGameObjectWithTag("Player").GetComponent<CreatureController>().timeToRespawn - 0.1f;

        recoveryFlashingSpeed = recoveryTimeDamage / recoveryFlash;
        normalTexture = GetComponentInChildren<Renderer>().material.mainTexture;
    }

    private void FixedUpdate()
    {
        if (isActive)
        {
            Transform fishMesh = this.gameObject.transform.GetChild(0);

            if (!isDead)
            {
                FSM.Update();
            }


            if (currentTarget != null)
            {
                if (currentTarget.GetComponent<CreatureController>().IsDead())
                {
                    health = maxHealth;
                    FSM.ChangeState(BossIdleState.Instance);
                    Debug.Log("TARGET TERMINATED");
                    if (!isResetting) //to avoid stackoverflow
                    {
                        isResetting = true;
                        Invoke("ResetRoom", timeToReset);
                    }
                }

                GetComponentInChildren<BehaviourScript>().targetPosition = currentTarget.transform.position;

                //dash every X seconds (pattern is chase player for some time -> stop -> dash -> repeat)
                if (((int)(Time.realtimeSinceStartup * 1000) - dashTimer) >= (dashCD + dashCDCompensation))
                {
                    dashTarget = currentTarget.transform.position;
                    GetComponentInChildren<MoveController>().instantStop();
                    resetDashTimer();

                    FSM.ChangeState(BossDashState.Instance);
                }
            }

            if (isRecovering)
            {
                if ((int)(Time.realtimeSinceStartup * 1000) - recoveryTimer >= recoveryTime)
                {
                    FSM.ChangeState(BossChaseState.Instance);
                    isRecovering = false;

                    GetComponent<Collider>().enabled = true;
                }
                else //decelerate the negative recovery speed
                {
                    GetComponent<Rigidbody>().AddForce((recoveryCompensation) * GetComponent<Rigidbody>().transform.up);
                }
            }
            else if (isDecelerating)
            {
                if ((int)(Time.realtimeSinceStartup * 1000) - decelerationTimer >= decelerationTime)
                {
                    GetComponent<MoveController>().turnRate = baseTurnRate;
                    FSM.ChangeState(BossChaseState.Instance);
                    GetComponent<BehaviourScript>().maxRunningSpeed = baseRunningSpeed;
                    isDecelerating = false;
                }
            }
            else if(isDying)
            {
                 if (((int)(Time.realtimeSinceStartup * 1000) - deathTimer) >= bossDeathAnimTime)
                 {
                     Destroy(this.gameObject);
                 }
            }
        }
    }

    public void TargetPlayer(GameObject playerGO)
    {
        currentTarget = playerGO;
    }

    public C_FiniteStateMachine getFSM()
    {
        return FSM;
    }

    public void die()
    {
        if (!isDead)
        {
            GetComponentInChildren<MoveController>().instantStop();
            isDead = true;
            currentTarget = null;
            if (dropPrefab != null)
            {
                Instantiate(dropPrefab, transform.position, dropPrefab.transform.rotation);
            }
        }
    }

    public void BeginFight(GameObject playerGO, GameObject triggerRef)
    {
        if (playerGO != null)
        {
            isActive = true;
            bossTriggerRef = triggerRef;
            TargetPlayer(playerGO);
            GetComponentInChildren<BehaviourScript>().SetTarget(currentTarget.transform.position);
            resetDashTimer();
            GetComponent<BossAttack>().ComputeDamageDealt();
            FSM.ChangeState(BossChaseState.Instance);
        }
        else
            Debug.Log("PLAYERGO IS NULL");
    }

    public void Recover(bool recoverFromSpikes)
    {
        FSM.ChangeState(BossIdleState.Instance);
        GetComponent<MoveController>().instantStop();
        GetComponent<Rigidbody>().AddForce(-recoveryForce * GetComponent<Rigidbody>().transform.up);
        recoveryTimer = (int)(Time.realtimeSinceStartup * 1000);
        isRecovering = true;
        if (recoverFromSpikes)
        {
            GetComponent<Collider>().enabled = false;
        }
    }

    public void ReturnToChase()
    {
        GetComponent<MoveController>().instantStop();
        GetComponent<MoveController>().turnRate = baseTurnRate/7;
        decelerationTimer = (int)(Time.realtimeSinceStartup * 1000);
        FSM.ChangeState(BossChaseState.Instance);
        GetComponent<BehaviourScript>().maxRunningSpeed = baseRunningSpeed/5; //reset it to base after timer is over
        isDecelerating = true;
    }

    public void TakeDamage()
    {
        health--;

        GetComponent<AudioSource>().clip = crystalBreak;
        GetComponent<AudioSource>().volume = 0.7f;
        GetComponent<AudioSource>().Play();

        StartCoroutine(ClignoteRouge(recoveryFlashingSpeed, recoveryFlash * 2, false));

        if(health == health/2) //at half health, decrease the CD
        {
            dashCD -= dashCDDecrease;
        }

        ComputeDashCDCompensation();

        if(health <= 0)
        {
            isRecovering = false;
            isDecelerating = false;
            isResetting = false;
            GetComponent<BossAttack>().ResetBools();
            isDying = true;
            deathTimer = (int)(Time.realtimeSinceStartup * 1000);
            die();
            bossTriggerRef.GetComponent<BossTriggerScript>().resetWall();
        }
    }

    public void ResetRoom() 
    {
        currentTarget = null; //reset aggro
        this.transform.position = basePosition;
        for (int i = 0; i < crystals.Length; i++)
        {
            if (!(crystals[i].activeInHierarchy))
            {
                crystals[i].SetActive(true);
            }
        }
        isResetting = false;
        GetComponent<Collider>().enabled = true;
        GameObject.FindGameObjectWithTag("CameraHook").GetComponent<CameraHooking>().Unhooking();
        GetComponentInChildren<BossAttack>().ResetDashChargeTime();
        ComputeDashCDCompensation();
        bossTriggerRef.GetComponent<BossTriggerScript>().resetWall();
        isActive = false;
    }

    public void setTarget(GameObject target)
    {
        currentTarget = target;
    }

    public void resetDashTimer()
    {
        dashTimer = (int)(Time.realtimeSinceStartup*1000);
    }

    private void ComputeDashCDCompensation()
    {
        dashCDCompensation = ((int)(GetComponentInChildren<BossAttack>().dashChargeTime) + recoveryTime);
    }

    IEnumerator ClignoteRouge(float delayTime, int nbTime, bool red)
    {
        //  Debug.Log("iteration");
        yield return new WaitForSeconds(delayTime);


        //Texture t = GetComponentInChildren<Renderer>().material.mainTexture;

        if (health > 0)
        {
            if (nbTime > 0)
            {
                if (red)
                    GetComponentInChildren<Renderer>().material.mainTexture = normalTexture;
                else
                    GetComponentInChildren<Renderer>().material.mainTexture = redTexture;

                StartCoroutine(ClignoteRouge(delayTime, --nbTime, !red));
            }
            else
            {
                if (red)
                    GetComponentInChildren<Renderer>().material.mainTexture = normalTexture;
            }
        }
        else
        {
            //GetComponentInChildren<Renderer>().material.mainTexture = normalTexture;

            GetComponentInChildren<Renderer>().material.mainTexture = redTexture;
        }
    }
}