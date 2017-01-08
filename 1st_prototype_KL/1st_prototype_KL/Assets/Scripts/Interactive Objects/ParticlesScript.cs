using UnityEngine;
using System.Collections;

public class ParticlesScript : MonoBehaviour
{

    public float destroyTime = 1.5f;

    private float startTime;
    private bool isDestroyed = false;

	// Use this for initialization
	void Start ()
    {
        startTime = Time.time;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if((Time.time - this.startTime) >= destroyTime)
        {
            if(!isDestroyed)
            {
                Destroy(this.gameObject);
            }
        }
	}
}
