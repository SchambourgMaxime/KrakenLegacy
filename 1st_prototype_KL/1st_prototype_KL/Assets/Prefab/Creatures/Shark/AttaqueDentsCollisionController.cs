using UnityEngine;
using System.Collections;

public class AttaqueDentsCollisionController : MonoBehaviour {

    CreatureController thisCreatureController;

	// Use this for initialization
	void Start () {
        thisCreatureController = gameObject.transform.parent.GetComponent<CreatureController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (!thisCreatureController.IsDead())
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
}
