using UnityEngine;
using System.Collections;

public class AttaqueCCRequin : MonoBehaviour
{

    public float initialChaseTimer;
    public float initialRestTimer;

    private float chaseTimer;
    private float restTimer;
    public bool attackAllowed;
    private MoveController mc;
    private float initialMaxSpeed;
    private GameObject teeth;
    private Vector3 selfPos;
    private Vector3 targetPos;
    private float attackDistance;
    protected CreatureController thisCreatureController;
    private Animator anim;

    // Use this for initialization
    void Awake () {
        attackAllowed = false;
        mc = gameObject.GetComponent<MoveController>();
        initialMaxSpeed = mc.maxSpeed;
        teeth = gameObject.transform.FindChild("AttaqueDents").gameObject;
        attackDistance = gameObject.GetComponent<C_RivalCreature_Shark>().attackDistance;
        thisCreatureController = gameObject.GetComponent<CreatureController>();
        Transform fishMesh = transform.Find("Fish_Mesh");
        if (fishMesh)
        {
            GameObject sharkMesh = fishMesh.Find("shark").gameObject;
            if (sharkMesh)
            {
                anim = sharkMesh.GetComponent<Animator>();
            }
        }
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        selfPos = gameObject.transform.position;
        if (gameObject.GetComponent<C_RivalCreature_Shark>() != null)
        {
            targetPos = gameObject.GetComponent<C_RivalCreature_Shark>().getTargetPosition();
        }
        
        float distance = Vector3.Distance(selfPos, targetPos);

        if (attackAllowed)
        {
            chase();

            if(distance < attackDistance  && !teeth.activeSelf)
            {
                teeth.SetActive(true);
            }
            if(distance > attackDistance  && teeth.activeSelf)
            {
                teeth.SetActive(false);
            }
        }
    }

    public void attack()
    {
        resetTimers();
        attackAllowed = true;
    }

    private void chase()
    {
        chaseTimer -= Time.deltaTime;
        mc.SetMaxSpeed(initialMaxSpeed * 1.5f);
        if(gameObject.GetComponent<BehaviourScript>() != null)
        {
            gameObject.GetComponent<BehaviourScript>().behaviour = "Seek";

            // CHANGER L'ANIMATION
            startChaseAnim();

            if(chaseTimer <= 0)
            {
                mc.SetMaxSpeed(mc.maxSpeed * 0.66f);

                // CHANGER L'ANIMATION
                stopChaseAnim();

                gameObject.GetComponent<BehaviourScript>().behaviour = "Idle";
                gameObject.GetComponent<MoveController>().instantStop();

                restTimer -= Time.deltaTime;
                if(restTimer <= 0)
                {
                    //gameObject.GetComponent<BehaviourScript>().behaviour = "Seek";
                    //resetTimers();
                    teeth.SetActive(false);
                    attackAllowed = false;
                }
            }
        }
    }

    public void resetTimers()
    {
        chaseTimer = initialChaseTimer;
        restTimer = initialRestTimer;
    }

    void OnCollisionEnter(Collision other)
    {
        if (thisCreatureController && !thisCreatureController.IsDead())
        {
            CreatureController creatureController = other.gameObject.GetComponent<CreatureController>();
            if (creatureController && thisCreatureController && (gameObject.tag != other.gameObject.tag))
            {
                creatureController.TakeDamage(thisCreatureController.strength);
            }

            MoveController moveController = other.gameObject.GetComponent<MoveController>();
            if (moveController && thisCreatureController)
            {
                moveController.Knockback(thisCreatureController.knockbackForce, transform.position);
            }
        }
    }
    public bool getAttackAllowed()
    {
        return attackAllowed;
    }

    public void setAttackAllowed(bool value)
    {
        attackAllowed = value;
    }

    public void startChaseAnim()
    {
        anim.SetBool("IsChasing", true);
    }

    public void stopChaseAnim()
    {
        anim.SetBool("IsChasing", false);
    }

}
