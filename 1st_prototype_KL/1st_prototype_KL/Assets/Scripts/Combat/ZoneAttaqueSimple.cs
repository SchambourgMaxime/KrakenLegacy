using UnityEngine;
using System.Collections;

public class ZoneAttaqueSimple : MonoBehaviour {

    public GameObject objectToSpawn;
    public GameObject owner;

    public float sizeSoftener;

    private float timeLeft = 1.0f;

    private GameObject objectSpawned;
    private CreatureController creatureController;

    private ArrayList tabHit = new ArrayList();

    void Start() {
        transform.Rotate(new Vector3(0, 0, 1), 180);

        creatureController = owner.GetComponent<CreatureController>();

        SphereCollider sphereCollider = GetComponent<SphereCollider>();

        if(sphereCollider)
            sphereCollider.radius = owner.transform.localScale.x * sizeSoftener;

        objectSpawned = (GameObject)Instantiate(objectToSpawn, transform.position, transform.rotation);

        objectSpawned.transform.parent = transform;

        objectSpawned.GetComponent<ParticleSystem>().startSize = owner.transform.localScale.x;

        //player = transform.parent.gameObject;
        //ecart = transform.position - player.transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("Fish") || other.tag.Contains("RivalCreature"))
        {
            CreatureController otherCreatureController = other.GetComponentInParent<CreatureController>();
            MoveController otherMoveController = other.GetComponent<MoveController>();

            CreatureController thisCreatureController = owner.GetComponent<CreatureController>();
            C_RCDetectionTrigger zoneDetection = null;

            if (other.gameObject.tag == "Turtle_RivalCreature")
            {
                zoneDetection = other.GetComponentInParent<C_RCDetectionTrigger_Turtle>();
            }
            else if(other.gameObject.tag == "Shark_RivalCreature")
            {
                zoneDetection = other.GetComponentInParent<C_RCDetectionTrigger_Shark>();
            }

            if (otherCreatureController && thisCreatureController && !zoneDetection) 
            {
                if (!tabHit.Contains(otherCreatureController)) 
                {
                    tabHit.Add(otherCreatureController);
                    otherCreatureController.TakeDamage(thisCreatureController.strength);
                    otherMoveController.Knockback(thisCreatureController.knockbackForce, transform.position);
                }
            } 
            else 
            {
                C_Fish fish = other.GetComponentInParent<C_Fish>();
                if (fish) 
                {
                    fish.die();
                }
            }
        }
        else if (other.tag.Contains("Meduse"))
        {
            MeduseContact meduse = other.GetComponentInParent<MeduseContact>();
            meduse.takeDamage();
        }
    }

    public void setTimer(float timer)
    {
        timeLeft = timer;
    }

    void Update () {
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0)
        {
            Destroy(gameObject);
        }
    }
}
