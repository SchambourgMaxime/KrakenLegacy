using UnityEngine;
using System.Collections;

public class BubbleLife : MonoBehaviour {

    public float force = 750f;
    public float lifeTime = 1;
    private GameObject owner;
	// Use this for initialization
	void Start () {
        float angle = Random.Range(-10, 10);
        transform.GetComponent<Rigidbody>().AddForce(Quaternion.Euler(0,0, angle) *this.transform.up*force);
	}

    void Update()
    {
        lifeTime -= Time.deltaTime;
        if(lifeTime<0)
        {
            Destroy(gameObject);
        } 
    }

        // Update is called once per frame
        void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Shark_RivalCreature"))
        {
            CreatureController cc = other.gameObject.GetComponent<CreatureController>();
            uint damage = owner.GetComponent<CreatureController>().strength;
            cc.TakeDamage(damage);

        }
        if (other.CompareTag("Player") || other.CompareTag("Shark_RivalCreature") || other.CompareTag("Environment"))
        {
            Destroy(gameObject);
        }
    }

    public void setOwner(GameObject go)
    {
        this.owner = go;
    }
}


/*

    using UnityEngine;
using System.Collections;

public class AttaqueCCTortue : MonoBehaviour
{


    //Timer
    public float halfLerpDuration;
    public float time;
    public float angle;
    public float totalRotation;
    public GameObject bubbleAttak;
    private float timeLeft;
    private Quaternion initialRotation;
    private int numberBubble = 5;
    private float timer = Random.Range(0.1f, 0.3f);
    private bool attaque = false;
    //   private Quaternion finalRotation;
    private Vector3 initialPos;
    private Vector3 currentPos;
    private Vector3 targetPos;
    private float lerpGo;
    private float lerpBack;
    private float distance = 0;
    private Vector3 currentAngle;
    

    //arguments
    public float lerpDistance;
    public float turnRate;

    private Vector3 up;
    private bool attackAllowed;
    private Transform fishMesh;
    private CreatureController thisCreatureController;
    private C_RivalCreature_Turtle rivalCreature;
    private BehaviourScript behaviourScript;


    void Awake () {

        attackAllowed = false;
        fishMesh = gameObject.transform.GetChild(0);
        thisCreatureController = GetComponent<CreatureController>();
        rivalCreature = GetComponent<C_RivalCreature_Turtle>();
        behaviourScript = GetComponent<BehaviourScript>();

        //rotationZ = initialRotationZ;
        //Timer
        timeLeft = time;
        lerpGo = 0.0f;
        lerpBack = 0.0f;
        initialPos = transform.position;


        initialRotation = Quaternion.Euler(270f, 90f, 0);

       // initialRotation = fishMesh.transform.rotation;

        //   initialRotation = getInitialRotation();


        //  finalRotation = Quaternion.Euler(new Vector3(angle, 0, 0));
        currentAngle = fishMesh.transform.eulerAngles;
    }
	

	void Update ()
    { 
        if (attackAllowed)
        {
            currentPos = transform.position;
            attackAllowed = false;
        }
    }

    public void attack()
    {
        timeLeft -= Time.deltaTime;
        if(timeLeft < 0 || attaque)
        {
            attaque = true;
            timeLeft = time;
            timer -= Time.deltaTime;
            if (timer < 0)
            { 
                timer = Random.Range(0.1f, 0.3f);
                Instantiate(bubbleAttak, this.transform.position + this.transform.up, this.transform.rotation);
                numberBubble--;
            }
            if (numberBubble <= 0)
            {
                numberBubble = 5;
                attackAllowed = true;
                attaque = false;
            }
        }
    }

    void spin()
    {
        if(lerpGo < 1)
        {
            lerpGo += Time.deltaTime;
            if (lerpGo > 1)
            {
                lerpGo = 1;
            }
            //Translation
            transform.position = Vector3.Lerp(initialPos, targetPos, lerpGo);
            //Rotation
            fishMesh.transform.Rotate(0, 0, turnRate);

            if (lerpGo == 1)
            {
                fishMesh.transform.localRotation = initialRotation;
               // fishMesh.transform.localRotation = getInitialRotation();
            }
        }
        else if(lerpGo == 1 && lerpBack < 1)
        {
            lerpBack += Time.deltaTime;
            if (lerpBack > 1)
            {
                lerpBack = 1;
            }
            //Translation
            transform.position = Vector3.Lerp(targetPos, initialPos, lerpBack);
            //Rotation
            fishMesh.transform.Rotate(0, 0, -turnRate);

            if (lerpBack == 1)
            {
                Vector3 test = getInitialRotation().eulerAngles;
                Quaternion q = Quaternion.Euler(test);
                fishMesh.transform.localRotation = getInitialRotation();
                attackAllowed = false;
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (thisCreatureController && !thisCreatureController.IsDead() && (gameObject.tag != other.gameObject.tag))
        {
            CreatureController creatureController = other.gameObject.GetComponent<CreatureController>();
            if (creatureController && thisCreatureController)
            {
                creatureController.TakeDamage(thisCreatureController.strength);
            }

            MoveController moveController = other.gameObject.GetComponent<MoveController>();
            if (moveController && thisCreatureController)
            {
                moveController.Knockback(thisCreatureController.knockbackForce, transform.position + transform.right * 5.0f);
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

    public Quaternion getInitialRotation()
    {
        Quaternion q = initialRotation;

        if (behaviourScript.isLeft())
            q = Quaternion.Euler(270f, 270f, 0);
        else
            q = Quaternion.Euler(270f, 90f, 0);


        return initialRotation;
    }


    

}


    */