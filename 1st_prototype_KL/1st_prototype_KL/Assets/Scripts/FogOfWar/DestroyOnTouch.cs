using UnityEngine;
using System.Collections;

public class DestroyOnTouch : MonoBehaviour {

	void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
            Destroy(gameObject);
           // Debug.Log("tzrnynny");
    }
}
