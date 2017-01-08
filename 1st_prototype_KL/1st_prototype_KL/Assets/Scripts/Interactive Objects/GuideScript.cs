using UnityEngine;
using System.Collections;

public class GuideScript : MonoBehaviour {

    private bool one = true;

	// Use this for initialization
	void Start () {
        Invoke("destructionGuide", 10);

        GameObject player = GameObject.Find("Player 1");
        GameObject mur = GameObject.Find("DestructibleWall 1");

        if(mur && player && player.GetComponent<CreatureController>().strength < mur.GetComponent<DestructibleObject>().powerRequiredToDestroy)
        {
            Vector3[] path = { new Vector3(46.7f, 117.1f, -1), new Vector3(46.7f, 27.4f, -1), new Vector3(70.7f, 27.4f, -1), new Vector3(70.7f, -22.2f, -1), new Vector3(74.4f, -69.5f, -1), new Vector3(103.7f, -69.5f, -1), new Vector3(111.97f, -55.09f, -1), new Vector3(117.5f, -39.5f, -1), new Vector3(175f, -39.5f, -1) };
            gameObject.GetComponent<BehaviourScript>().setPath(path);
        }
        else if (GameObject.Find("Angler_Fish"))
        {
            Vector3[] path = { new Vector3(174.2f, -64.4f, -1), new Vector3(174.2f, -10f, -1), new Vector3(174.2f, 12f, -1), new Vector3(174.2f, 50f, -1), new Vector3(143.8f, 114.7f, -1), new Vector3(67f, 114.7f, -1), new Vector3(31.2f, 86.2f, -1), new Vector3(-8.9f, 72.3f, -1), new Vector3(-14.5f, 25.6f, -1), new Vector3(-40.5f, 25.6f, -1), new Vector3(-119f, 25.6f, -1), new Vector3(-203f, 25.6f, -1), new Vector3(-203f, -71f, -1) };
            gameObject.GetComponent<BehaviourScript>().setPath(path);
        }
        else
        {
            Vector3[] path = { new Vector3(-200f, 21.8f, -1), new Vector3(-117.7f, 21.8f, -1), new Vector3(-40.5f, 25.6f, -1), new Vector3(-14.5f, 25.6f, -1), new Vector3(-13.9f, 80f, -1), new Vector3(-8.9f, 72.3f, -1), new Vector3(31.2f, 86.2f, -1), new Vector3(67f, 114.7f, -1), new Vector3(143.8f, 114.7f, -1), new Vector3(174.2f, 50f, -1), new Vector3(174.2f, 12f, -1), new Vector3(300f, 12f, -1), new Vector3(400f, 12f, -1), new Vector3(500f, 12f, -1), new Vector3(600, 12f, -1) };
            gameObject.GetComponent<BehaviourScript>().setPath(path);
        }
    }
	
    public void destructionGuide()
    {
        Destroy(gameObject);
    }
}
