using UnityEngine;
using System.Collections;

public class RockMove : MonoBehaviour {

    private bool block = false;

    void Update()
    {
        if(block)
        {
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, new Vector3(99.3f, -52.6f, 2.5f), 0.1f);

            if(Vector3.Distance(gameObject.transform.position , new Vector3(99.3f, -52.6f, 2.5f)) < 0.5f)
            {
                Destroy(gameObject.GetComponent<BoxCollider>());
                Destroy(gameObject.GetComponent<RockMove>());
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player" && other.transform.GetComponent<CreatureController>().speed >= 3)
        {
            block = true;
        }
    }
}
