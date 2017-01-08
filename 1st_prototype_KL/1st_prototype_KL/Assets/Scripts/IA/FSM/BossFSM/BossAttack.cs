using UnityEngine;
using System.Collections;

public class BossAttack : MonoBehaviour {

    private float dashCharge;
    private bool isCharging = false;
    private bool isDashing = false;
    private uint damageDealt; //we get this from playerMaxHealth/numberOfHits
    private CreatureController playerCreatureRef = null;
    private uint dashChargeReduction; //don't forget to recalculate dash CD compensation (in controller) if you use this
    private uint baseDashChargeTime;

    public uint dashChargeTime = 2500;
    public float dashForce = 2000.0f;
    public float dashRecoilForce = 150.0f;
    public int numberOfHitsToKillPlayer = 4;
    public float dashPlayerKnockback = 10F;
    public float baseKnockback = 5F;
    public uint baseDamage = 1;

    public AudioClip bossDash;
    //
    // Summary:
    //     damage the player takes when colliding with the outer trigger when the boss is not dashing (affected by defense)


    // Use this for initialization
    void Start ()
    {
        baseDashChargeTime = dashChargeTime;
        dashChargeReduction = (uint)(dashChargeTime / GetComponent<C_Boss>().maxHealth);
        ComputeDamageDealt();
        isCharging = false;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        //the dash itself occurs here
        if ((((int)(Time.realtimeSinceStartup * 1000) - dashCharge) >= dashChargeTime) && (isCharging))
        {
            Animator anim = GetComponentInChildren<Animator>();
            if (anim != null)
            {
                anim.SetTrigger("Attack");
            }
            isDashing = true;
            isCharging = false;
            GetComponentInChildren<MoveController>().instantStop();
            GetComponent<Rigidbody>().AddForce(dashForce * GetComponent<Rigidbody>().transform.up);
        }
	}

    public void ChargeDash()
        
    {
        GetComponent<AudioSource>().clip = bossDash;
        GetComponent<AudioSource>().volume = 1.0f;
        GetComponent<AudioSource>().Play();

        isCharging = true;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(-dashRecoilForce * rb.transform.up);

        dashCharge = (int)(Time.realtimeSinceStartup * 1000);
    }

    void OnCollisionEnter(Collision other) //should only collide with environment
    {
        if (isDashing)
        {
            if (!(other.gameObject.CompareTag("Player")) && !(other.gameObject.CompareTag("DestroyedSpike")))
            {
                isDashing = false;
                if(other.gameObject.CompareTag("BossSpikes"))
                {
                    dashChargeTime -= dashChargeReduction;
                    GetComponent<C_Boss>().TakeDamage();
                    GetComponent<C_Boss>().Recover(true);
                }
                else //collision with something else
                {
                    GetComponent<C_Boss>().Recover(false);
                }
            }
            else
            {
                isDashing = false;
                GetComponent<C_Boss>().ReturnToChase();
                Debug.Log("boss collided with player !");

            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && isDashing)
        {
            //randomize knockback direction
            int knockbackDirection = Random.Range(0,1);
            if(knockbackDirection == 0)
            {
                playerCreatureRef.GetComponent<MoveController>().Knockback(dashPlayerKnockback, transform.position);
            }
            else
            {
                playerCreatureRef.GetComponent<MoveController>().Knockback(dashPlayerKnockback, transform.position);
            }

            playerCreatureRef.TakeDamage(damageDealt, true);
            isDashing = false;
            GetComponent<C_Boss>().ReturnToChase();
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            playerCreatureRef.TakeDamage(baseDamage, true);
            playerCreatureRef.GetComponent<MoveController>().Knockback(baseKnockback, this.transform.position);
        }
    }

    public void ComputeDamageDealt()
    {
        playerCreatureRef = GameObject.FindGameObjectWithTag("Player").GetComponent<CreatureController>();
        if (playerCreatureRef)
        {
            damageDealt = (uint)(playerCreatureRef.maxLifePoints / numberOfHitsToKillPlayer);
        }
    }

    public void ResetDashChargeTime()
    {
        dashChargeTime = baseDashChargeTime;
    }

    public void ResetBools()
    {
        isCharging = false;
        isDashing = false;
    }
}
