using UnityEngine;
using System.Collections;

public class FloatingTextScript : MonoBehaviour
{

    public float yDistanceTraveled = 2000.0f;
    public float duration = 10.0f;
    public float persistence = 0.5f; // for how much times the text stays at its end position before disappearing

    private float startTime;
    private Vector3 startPos;
    private Vector3 endPos;
    private bool isDestroyed = false;

	// Use this for initialization
	void Start ()
    {
        startTime = Time.time;
        startPos = this.transform.position;
        endPos = startPos + Vector3.up * yDistanceTraveled;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if(this.transform.position.y < (endPos.y - 0.1f))
        {
            this.transform.position = Vector3.Lerp(startPos, endPos, (Time.time - startTime) / duration);
        }
        else
        {
            if (!isDestroyed)
            {
                isDestroyed = true;
                Destroy(this.gameObject, persistence);
            }
        }
	}
}
