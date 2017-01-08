using UnityEngine;
using System.Collections;

public class OrbesMovement : MonoBehaviour {

    public float dispersionForce = 1.0f;
    public float slowDownForce = 1.0f;
    public int mili_LifeTime = 20000;
    public float percentageStats = 5.0f;
    public float ratio = 1;

    private int startTime = -1;

    private Vector3 center = Vector3.zero;

    private GameObject[] orbs = new GameObject[3];
    private Rigidbody[] orbsRB = new Rigidbody[3];

    private CreatureController owner;

    private float creatureSize = 1.0f;

	// Use this for initialization
	void Start () 
    {
        for(int i = 0; i < 3; i++)
        {
            Transform go = transform.GetChild(i);

            if(go.name.Contains("Orbe"))
            {
                center = new Vector3(center.x + go.position.x, center.y + go.position.y, -1.0f);
                orbs[i] = go.gameObject;
                orbsRB[i] = go.GetComponent<Rigidbody>();
            }
        }

        center = new Vector3(center.x / 3, center.y / 3, -1.0f);

        for (int i = 0; i < 3; i++) 
        {
            Vector3 distanceFromCenter = (orbs[i].transform.position - center).normalized;
            orbsRB[i].AddForce(distanceFromCenter * dispersionForce * (1.0f + creatureSize));
        }

        startTime = (int)(Time.realtimeSinceStartup * 1000.0f);
    }
	
	// Update is called once per frame
	void Update () 
    {
        for (int i = 0; i < 3; i++) 
        {
            if(orbs[i])
                orbsRB[i].AddForce(-orbsRB[i].velocity * slowDownForce);
        }

        if(startTime != -1)
        {
            if((int)(Time.realtimeSinceStartup * 1000.0f) - startTime > mili_LifeTime)
            {
                Destroy(gameObject);
            }
        }
	}

    void Init()
    {
        for (int i = 0; i < 3; i++) 
        {
            Transform go = transform.GetChild(i);

            if(go.name.Contains("Orbe"))
                orbs[i] = go.gameObject;

            orbs[i].transform.localScale = owner.transform.localScale * ratio;
            if(orbs[i].GetComponent("Halo"))
            {
                orbs[i].GetComponent("Halo").transform.localScale = owner.transform.localScale * ratio;
            }
        }


            FoodController[] foodControllers = 
        { 
            orbs[0].GetComponent<FoodController>(),
            orbs[1].GetComponent<FoodController>(),
            orbs[2].GetComponent<FoodController>() 
        };

        float statsRatio = percentageStats / 100.0f;

        uint[] statsFood = 
        { 
            (uint)((owner.getStatLevel()[0] * owner.GetStatsCaps()[0]) * statsRatio),
            (uint)((owner.getStatLevel()[1] * owner.GetStatsCaps()[1]) * statsRatio),
            (uint)((owner.getStatLevel()[2] * owner.GetStatsCaps()[2]) * statsRatio)
         };

        if(foodControllers[0])
            foodControllers[0].SetStats(statsFood[0] + 1, 0, 0, 0);

        if (foodControllers[1])
            foodControllers[1].SetStats(0, statsFood[1] + 1, 0, 0);

        if (foodControllers[2])
            foodControllers[2].SetStats(0, 0, statsFood[2] + 1, 0);

        creatureSize = owner.GetSize();
    }

    public void setCreatureSize(float _creatureSize)
    {
        creatureSize = _creatureSize;
    }

    public void DefineOwner(CreatureController ownerCreatureController)
    {
        owner = ownerCreatureController;
        Init();
    }
}
