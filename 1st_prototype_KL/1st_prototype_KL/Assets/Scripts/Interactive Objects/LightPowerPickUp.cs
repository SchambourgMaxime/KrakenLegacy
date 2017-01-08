using UnityEngine;
using System.Collections;

public class LightPowerPickUp : MonoBehaviour {

    public float newRadiusFOW = 15f;

    void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Player"))
        {
            collision.collider.GetComponent<FogOfWar>().radiusFogOfWar = newRadiusFOW;
            collision.collider.GetComponent<FogOfWar>().SetCurrentRadiusFOW(newRadiusFOW);
            collision.collider.GetComponent<CreatureController>().SetHasLightPower(true);
            Destroy(gameObject);
        }
    }
}
