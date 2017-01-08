using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour {

    private MoveController moveController;
    private CreatureController creatureController;
    private Animator animator;

    private float speed;
    private bool isDashing = false;
    private bool isDead = false;

    // Use this for initialization
    void Start () 
    {
        moveController = GetComponent<MoveController>();
        creatureController = GetComponent<CreatureController>();
        animator = GetComponentInChildren<Animator>();
    }
	
	// Update is called once per frame
	void Update () 
   {
        if (moveController) 
        {
            float velocityMagnitude = moveController.GetVelocity().magnitude;

            if (!Mathf.Approximately(speed, velocityMagnitude))
            {
                animator.SetFloat("Speed", velocityMagnitude);
                speed = velocityMagnitude;
            }

            //GameObject mesh = transform.FindChild("Octopus@Animations").gameObject;

            //if (mesh) 
            //{
            //    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Swimming")) 
            //    {
            //        mesh.transform.localEulerAngles = new Vector3(270.0f, 90.0f, 0.0f);
            //        mesh.transform.localPosition = new Vector3(0.0f + 0.5f, 0.0f, 0.0f);
            //    } 
            //    else if(animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            //    {
            //        mesh.transform.localEulerAngles = new Vector3(0.0f, 90.0f, 0.0f);
            //        mesh.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            //    }
            //}

            bool importIsDashing = moveController.IsDashing();

            if (isDashing != importIsDashing) 
            {
                animator.SetBool("IsDashing", importIsDashing);
                isDashing = importIsDashing;
            }
        }

        if(creatureController) 
        {
            bool importIsDead = creatureController.IsDead();

            if (importIsDead != isDead)
            {
                animator.SetBool("IsDead", importIsDead);
                isDead = importIsDead;
            }
        }
	}
}
