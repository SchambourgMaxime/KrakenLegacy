using UnityEngine;
using System.Collections;

public class DestroyCrystals : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("DestroyedSpike"))
        {
            Destroy(other.gameObject);
        }
    }
}
