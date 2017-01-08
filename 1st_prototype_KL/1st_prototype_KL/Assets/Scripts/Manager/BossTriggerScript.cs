using UnityEngine;
using System.Collections;

public class BossTriggerScript : MonoBehaviour {

    public GameObject bossToTrigger;
    public GameObject wall;
    public float yOffset = 10.0f;

    private C_Boss bossRef;
    private GameObject spawnedWallRef;
    private bool wallExists = false;

	// Use this for initialization
	void Start ()
    {
        bossRef = bossToTrigger.GetComponent<C_Boss>();
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && (!wallExists) && (bossToTrigger))
        {
            GetComponent<AudioSource>().Play();
            wallExists = true;
            bossRef.BeginFight(other.gameObject, this.gameObject);
            Invoke("SpawnWall", 0.5f);
        }
    }

    private void SpawnWall()
    {
        spawnedWallRef = (GameObject)(Instantiate(wall, this.transform.position + this.transform.up * yOffset, wall.transform.rotation));
        spawnedWallRef.transform.parent = this.transform;
    }

    public void resetWall()
    {
        Destroy(spawnedWallRef);
        wallExists = false;
        //deactivate wall here
    }
}
