using UnityEngine;
using System.Collections;

public class HazardController : MonoBehaviour {

    private float damageDealt;
    public float knockback = 5;

	// Use this for initialization
	void Start () 
    {
    }
	
	// Update is called once per frame
	void Update ()   
    {
	
	}

    void OnCollisionEnter(Collision collision)
    {
        CreatureController otherCreatureController = (collision.gameObject).GetComponent<CreatureController>();
        MoveController otherMoveController = (collision.gameObject).GetComponent<MoveController>();

        if (otherCreatureController) 
        {
            damageDealt =  otherCreatureController.maxLifePoints * 0.2f;
            otherCreatureController.TakeDamage((uint) damageDealt, true);


            if (otherMoveController && !otherCreatureController.IsDead()) {
                otherMoveController.Knockback(knockback, transform.position);
            }
        }
    }
}
