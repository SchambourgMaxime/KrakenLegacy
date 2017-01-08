using UnityEngine;
using System.Collections;

public class SeaweedCollision : MonoBehaviour {


   private FoodController fc;

    public void Start()
    {
    }

    void OnTriggerEnter(Collider collision)
    {
        fc = transform.GetComponentInParent<FoodController>();
        if (fc == null)
            fc = transform.GetComponent<FoodController>();

        fc.OnTriggerEnter(collision);
    }

}
