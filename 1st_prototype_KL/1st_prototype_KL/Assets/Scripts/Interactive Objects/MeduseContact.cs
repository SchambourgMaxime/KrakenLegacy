using UnityEngine;
using System.Collections;

public class MeduseContact : MonoBehaviour {

    public uint damage;
    public uint knockbackForce = 3;
    private int hitCount = 3;
    public float recoveryTime = 1;
    public int recoveryFlash = 6;
    private float recoveryFlashingSpeed;
    public Texture redTexture;
    private Texture normalTexture;

   void Start()
    {
        recoveryFlashingSpeed = recoveryTime / recoveryFlash;
        normalTexture = GetComponentInChildren<Renderer>().material.mainTexture;
    }

    public void OnTriggerEnter(Collider collision)
    {
        if(collision.tag.Contains("Player") || collision.tag.Contains("_RivalCreature"))
        {
            CreatureController otherCreatureController = collision.GetComponentInParent<CreatureController>();
            MoveController otherMoveController = collision.GetComponent<MoveController>();
            otherCreatureController.TakeDamage(damage);
            otherMoveController.Knockback(knockbackForce, transform.position);
        }
    }

    public void takeDamage()
    {
        hitCount--;
        StartCoroutine(ClignoteRouge(recoveryFlashingSpeed, recoveryFlash * 2, false));
        if(hitCount <= 0)
        {
            GetComponent<AudioSource>().Play();
            gameObject.GetComponent<BehaviourScript>().behaviour = "Idle";
            gameObject.GetComponent<MoveController>().instantStop();
            Invoke("cdDestroy", 3);           
        }
    }

    IEnumerator ClignoteRouge(float delayTime, int nbTime, bool red)
    {
        //  Debug.Log("iteration");
        yield return new WaitForSeconds(delayTime);


        //Texture t = GetComponentInChildren<Renderer>().material.mainTexture;

        if (hitCount > 0)
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

    public void cdDestroy()
    {
        Destroy(gameObject);
    }


    void OnDestroy()
    {
        RoomManager roomManager = GetComponentInParent<RoomManager>();

        if (roomManager)
        {
            roomManager.RemoveFood(gameObject, true);
            return;
        }

        JunctionManager junctionManager = GetComponentInParent<JunctionManager>();

        if (junctionManager)
        {
            junctionManager.RemoveFood(gameObject);
            return;
        }
    }

}
