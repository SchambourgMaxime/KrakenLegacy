using UnityEngine;
using System.Collections;

public class HidingSpotController : MonoBehaviour {

    private string[] tag = new string[] { "BWFish", "Y1Fish", "Y2Fish", "Y3Fish", "Y4Fish", "Y5Fish", "Y6Fish", "Y7Fish", "Y8Fish", "Y9Fish", "Y10Fish", "Y11Fish", "VFish", "RFish", "PFish", "CFish", "B1Fish", "B2Fish", "B3Fish", "B4Fish", "B5Fish", "B6Fish", "B7Fish", "B8Fish", "B9Fish" };

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        foreach(var texte in tag)
        {
            if (other.CompareTag(texte))
            {
                C_Fish fish = other.GetComponent<C_Fish>();
                if (fish)
                {
                    fish.GetComponent<FoodController>().isEdible = false;
                    fish.transform.position = new Vector3(fish.transform.position.x, fish.transform.position.y, 3);
                    fish.setIsHidden(true);
                }
                break;
            }
        }        
    }

    void OnTriggerExit(Collider other)
    {
        foreach (var texte in tag)
        {
            if (other.CompareTag(texte))
            {
                C_Fish fish = other.GetComponent<C_Fish>();
                if (fish)
                {
                    fish.GetComponent<FoodController>().isEdible = true;
                    fish.transform.position = new Vector3(fish.transform.position.x, fish.transform.position.y, -1);
                    fish.setIsHidden(false);
                }
                break;
            }
        }
    }
}
