using UnityEngine;
using System.Collections;

public class SeaweedAnimation : MonoBehaviour
{


    public float forceMouvement = 0.0001f;

    public float forceTransmission = 0.4f;

    private float ratioLimitJoint = 10f;


    public Transform extremite;



    void Start()
    {
        ConfigurableJoint myJoint = GetComponent<ConfigurableJoint>();
        SoftJointLimit jointScaled = myJoint.linearLimit;
        //jointScaled.limit = ratioLimitJoint * transform.root.localScale.y; //avec ratio a 0.05, version pas utilisable pour un prefab comprenant plusieurs algues
        jointScaled.limit = transform.lossyScale.x / ratioLimitJoint;
       // Debug.Log(jointScaled.limit);
        GetComponent<ConfigurableJoint>().linearLimit = jointScaled;
    }

    //Debug
    /*
    void Update()
    {
        if (extremite)
        {
            Vector3 test = Vector3.Cross(-extremite.right, new Vector3(0, 0, 1));
            test.Normalize();

            Debug.DrawLine(extremite.position, -extremite.right * 3 + extremite.position, Color.yellow);
            Debug.DrawLine(extremite.position, test * 3 + extremite.position, Color.black);
        }
    }
    */

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player") /*|| other.transform.tag.Contains("Fish") || other.transform.tag.Contains("_RivalCreature")*/)
        {
            Vector3 velo = other.transform.GetComponent<Rigidbody>().velocity;
            Vector3 directionForce = Vector3.zero;

            if (extremite)
            {
                if (Vector3.Dot(-extremite.right, (other.transform.position - transform.position).normalized) > 0) //Cas special, on arrive par le haut
                {
                    Vector3 normalVector = Vector3.Cross(-extremite.right, new Vector3(0, 0, 1));
                    normalVector.Normalize();
                    //Debug.Log(normalVector);

                    if (Vector3.Dot(normalVector, (other.transform.position - transform.position).normalized) < 0)
                    { // on vient de la gauche
                      //sens de normalVector
                       //Debug.Log("gauche");
                        directionForce = normalVector;
                    }
                    else
                    { // on vient de la droite
                        //sens opposé de normalVector
                        //Debug.Log("droite");
                        directionForce = -normalVector;
                    }
                    //if (Mathf.Abs(Vector3.Dot(normalVector, velo.normalized)) < 0.05f)
                    //{
                    //    Vector3 t = (-(other.transform.position - transform.position)).normalized;
                    //    float inverseSigne = Vector3.Dot(t, normalVector);

                    //    //if (inverseSigne < 0)
                    //    //    directionForce = -directionForce;
                    //    //Debug.Log(Vector3.Dot(t, normalVector));
                    //}
                }
                else
                    directionForce = velo; // on arrive par le coté, pas de probleme
            }
            else
                directionForce = velo; //valeur par defaut

            float percentForce = velo.magnitude / other.GetComponent<MoveController>().BaseMaxSpeedInitiale;
            percentForce = Mathf.Min(1, percentForce);
            //Debug.Log(percentForce);


            transform.GetComponent<Rigidbody>().AddForce(directionForce.normalized * forceMouvement * percentForce);
            foreach (Rigidbody r in transform.GetComponentsInChildren<Rigidbody>())
            {
                if (r.transform.tag != "SeaweedCollisions")
                    r.AddForce(directionForce.normalized * forceMouvement * percentForce * forceTransmission);
            }
        }
    }

}
