using UnityEngine;
using System.Collections;

public class Cheating : MonoBehaviour {


    public GameObject player;
    public GameObject orbeStrength;
    public GameObject orbeDefense;
    public GameObject orbeSpeed;

    public float forceSpawn = 0f;



    void Update ()
    {
        if(player)
        {
            if (orbeStrength)
            {
                if (Input.GetKeyDown(KeyCode.Keypad1))
                {
                    GameObject orbe = (GameObject) Instantiate(orbeStrength, player.transform.position + player.transform.up * 10, player.transform.rotation);
                    orbe.GetComponent<Rigidbody>().AddForce(player.transform.up * forceSpawn);
                    orbe.GetComponent<FoodController>().satietyGain = 0;

                }
            }

            if (orbeDefense)
            {
                if (Input.GetKeyDown(KeyCode.Keypad2))
                {
                    GameObject orbe = (GameObject)Instantiate(orbeDefense, player.transform.position + player.transform.up * 10, player.transform.rotation);
                    orbe.GetComponent<Rigidbody>().AddForce(player.transform.up * forceSpawn);
                    orbe.GetComponent<FoodController>().satietyGain = 0;
                }
            }

            if (orbeSpeed)
            {
                if (Input.GetKeyDown(KeyCode.Keypad3))
                {
                    GameObject orbe = (GameObject)Instantiate(orbeSpeed, player.transform.position + player.transform.up * 10, player.transform.rotation);
                    orbe.GetComponent<Rigidbody>().AddForce(player.transform.up * forceSpawn);
                    orbe.GetComponent<FoodController>().satietyGain = 0;
                }
            }
        }
        
	
	}
}
