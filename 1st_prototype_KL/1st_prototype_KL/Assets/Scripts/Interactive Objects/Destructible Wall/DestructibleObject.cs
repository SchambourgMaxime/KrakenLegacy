using UnityEngine;
using System.Collections.Generic;

public class DestructibleObject : MonoBehaviour {

    public List<GameObject> rocks;
    MoveController moveController;
    CreatureController creatureController;

    public uint powerRequiredToDestroy;

    public float projectionSpeed;


    void Start () 
        {
            
            for(int i=0; i < transform.GetChildCount(); i++ )
            {
                GameObject current = transform.GetChild(i).gameObject;
                if(current.name.Contains("DestructibleRock"))
                {
                    rocks.Add(current);
                }

            }
            powerRequiredToDestroy = GameObject.FindGameObjectWithTag("Player").GetComponent<CreatureController>().destrUnlockStrThreshold;
            
        }
    void OnCollisionStay(Collision collision)
    {

        moveController = collision.gameObject.GetComponent<MoveController>();
        creatureController = collision.gameObject.GetComponent<CreatureController>();

        if (collision.gameObject.CompareTag("Player"))
        {
            if ((moveController.IsDashing() || moveController.IsDecelerating()) && (creatureController.getStatLevel()[0] >= powerRequiredToDestroy)) 
            {
                collision.collider.gameObject.GetComponent<AudioController>().PlayWallDestruction();
                Vector3[] positionDiff = new Vector3[rocks.Count];
                float[] forcePush = new float[rocks.Count];

                float maxPush = 0.0f;

                for (int i = 0; i < rocks.Count; i++) {
                    positionDiff[i] = rocks[i].transform.position - collision.transform.position;

                    if (positionDiff[i].magnitude > maxPush)
                        maxPush = positionDiff[i].magnitude;
                }

                for (int i = 0; i < rocks.Count; i++) {
                    Rigidbody rb = rocks[i].GetComponent<Rigidbody>();

                    float speedBoost = 0.0f;

                    if (rb) {
                        speedBoost = positionDiff[i].magnitude / maxPush;

                        rb.AddForce((positionDiff[i].normalized * speedBoost) * projectionSpeed);
                    }

                    SphereCollider sphereCollider = rocks[i].GetComponent<SphereCollider>();

                    if(sphereCollider)
                    {
                        sphereCollider.enabled = true;
                    }

                    RockHandler rockHandler = rocks[i].GetComponent<RockHandler>();

                    if(rockHandler)
                    {
                        rockHandler.SetDestroyCountdown();
                    }
                }

                Destroy(GetComponent<BoxCollider>());
            }
            else if ((moveController.IsDashing() || moveController.IsDecelerating()) || (creatureController.getStatLevel()[0] < powerRequiredToDestroy))
            {
                moveController.Knockback(5f, transform.position);
            }
        }
    }
}
