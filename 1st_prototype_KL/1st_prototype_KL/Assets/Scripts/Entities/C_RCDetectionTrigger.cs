using UnityEngine;
using System.Collections;

public abstract class C_RCDetectionTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public abstract void OnTriggerEnter(Collider other);
}
