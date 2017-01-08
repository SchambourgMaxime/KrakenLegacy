using UnityEngine;
using System.Collections;

public class CheckpointWall : MonoBehaviour {

    private string[] tag = new string[] { "Player" };
    BoxCollider parentColliderBox;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) 
        {
            transform.parent.GetComponent<Collider>().isTrigger = true;
        }

    }
    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) 
        {
            transform.parent.GetComponent<Collider>().isTrigger = false;
        }

    }

}
