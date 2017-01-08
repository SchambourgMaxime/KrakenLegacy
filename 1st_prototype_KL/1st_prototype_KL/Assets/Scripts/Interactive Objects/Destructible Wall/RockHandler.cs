using UnityEngine;
using System.Collections;

public class RockHandler : MonoBehaviour {

    public int mili_timeToDie;
    
    private int startTime = -1;

	// Use this for initialization
	void FixedUpdate () 
    {
	    if(startTime != -1)
        {
            if((int)(Time.realtimeSinceStartup * 1000.0f) - startTime >= mili_timeToDie)
            {
                Destroy(gameObject);
            }
        }
	}
	
    public void SetDestroyCountdown()
    {
        startTime = (int)(Time.realtimeSinceStartup * 1000.0f);
    }

}
