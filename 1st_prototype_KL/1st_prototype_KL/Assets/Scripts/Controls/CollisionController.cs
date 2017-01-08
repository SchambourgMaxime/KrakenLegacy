using UnityEngine;
using System.Collections;


//Bug qui traine avec les collisions qui fait que la vitesse et l'accélération sont pas réduits à 0 mais continuent detre diminués vers 0 à l'infini = calculs useless
public class CollisionController : MonoBehaviour
{

    private Rigidbody rb;
    private MoveController moveController;

    public float angleMin = 45.0f; //inférieur a 90
    public float angleMax = 175.0f; // supérieur à 90

    //public float angled = 0; 


    // Use this for initialization
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        moveController = GetComponent<MoveController>();
    }
    
    void OnCollisionStay(Collision col)
    {

        Vector3 ratioCollide = Vector3.zero;

        foreach (ContactPoint contact in col.contacts)
        {
            float angle = Vector3.Angle(contact.normal, transform.up);

            if(angle > angleMax || angle < angleMin)
            {
                moveController.cancelDeviationCollision = true;
            }
            else
            {
                moveController.cancelDeviationCollision = false;
            }

            if (angle > angleMax)
            {
                moveController.cancelAccelCollision = true;
                StartCoroutine(UnlockCollisionBug(col, 0.2f));
            }
            else
                moveController.cancelAccelCollision = false;

            if (!moveController.impactAccelLoss)
            {
                moveController.percentAccelLost = Mathf.Abs(100.0f - 100.0f * (180.0f - angle) / 90.0f);
            }


            //Debug.DrawLine(contact.point, transform.position, Color.blue);


            ratioCollide = new Vector3(contact.normal.x, contact.normal.y, 0);
            ratioCollide.Normalize(); //pour être sur


            // angled = angle;
        }


        moveController.ratioColliding = ratioCollide;
    }

    IEnumerator UnlockCollisionBug(Collision col, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        if (moveController.cancelAccelCollision)
        {

            if (!col.rigidbody)
                moveController.cancelAccelCollision = false;
            else
            {
                UnlockCollisionBug(col, delayTime);
            }
        }
    }


    void OnCollisionExit(Collision col)
    {
        moveController.ratioColliding = Vector3.zero;
        moveController.percentAccelLost = 0f;
        moveController.impactAccelLoss = false;
        moveController.cancelDeviationCollision = false;
        moveController.cancelAccelCollision = false;
    }


}

