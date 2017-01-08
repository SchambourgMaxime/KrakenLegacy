using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour {

    public int temperature = 0;

    public GameObject[] possiblePlants;
    public GameObject[] possibleFishes;
    public GameObject[] possibleConcurentCreatures;

    public int maxPlantsLimit;
    public int maxFishesLimit;
    public int maxConcurentCreaturesLimit;


    public int minPlantsLimit;
    public int minFishesLimit;
    public int minConcurentCreaturesLimit;


    public int maxFishesLimitSpawned;
    public int maxConcurentCreaturesLimitSpawned;
    public int minFishesLimitSpawned;
    public int minConcurentCreaturesLimitSpawned;
    public int currentFishRoomOrigine = 0;
    public int currentConcurentCreatureRoomOrigine = 0;

    public int currentFishInsideRoom = 0;

    public int currentCreatureInsideRoom = 0;



    public float percentageSizePlayer = 50.0f;

    public int limitTestSpawn;
    public float baseSpawnInterval = 1.0f; //in s
    public float minRandTime = 0.0f;
    public float maxRandTime = 2.5f;

    private List<GameObject> existingPlants;
    private List<GameObject> existingFishes;
    private List<GameObject> existingConcurentCreatures;

    private BoxCollider generalBoxCollider;
    private BoxCollider FishBoxCollider;
    private RandomSpawn randomSpawn;

    private bool isPlayerIn = false;
    private int plantsToSpawn = 0;
    private int fishesToSpawn = 0;
    private int concurrentCreaturesToSpawn = 0;

    // Use this for initialization
    void Start() 
    {
        existingPlants = new List<GameObject>();
        existingFishes = new List<GameObject>();
        existingConcurentCreatures = new List<GameObject>();

        ComputeTemperature();

        generalBoxCollider = GetComponent<BoxCollider>();
        FishBoxCollider = transform.FindChild("FishSpawnZone").GetComponent<BoxCollider>();
        randomSpawn = GetComponentInChildren<RandomSpawn>();

        SpawnProcess(possibleFishes, existingFishes, minFishesLimitSpawned + Random.Range(0, maxFishesLimitSpawned - minFishesLimitSpawned), FishBoxCollider);

        SpawnProcess(possiblePlants, existingPlants, minPlantsLimit + Random.Range(0, maxPlantsLimit - minPlantsLimit), generalBoxCollider);

        SpawnProcess(possibleConcurentCreatures, existingConcurentCreatures, minConcurentCreaturesLimitSpawned + Random.Range(0, maxConcurentCreaturesLimitSpawned - minConcurentCreaturesLimitSpawned), FishBoxCollider);




        CheckSpawn();
        //SpawnRandom(possibleFishes[Random.Range(0, possibleFishes.Length)]);
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        ComputeTemperature();
        currentFishInsideRoom = existingFishes.Count;
        currentCreatureInsideRoom = existingConcurentCreatures.Count;
        ChangeSizes();
	}

    void UpdateCreatures()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        CreatureController playerCreatureController = player.GetComponent<CreatureController>();

        foreach (GameObject ConcurrentCreatures in possibleConcurentCreatures) 
        {
            CreatureController thisCreatureController = ConcurrentCreatures.GetComponent<CreatureController>();

            if(thisCreatureController && playerCreatureController)
            {
                float newSize = thisCreatureController.GetBaseScale() + (thisCreatureController.sizeGain * (float)(playerCreatureController.GetSize()));
                ConcurrentCreatures.transform.localScale = new Vector3(newSize, newSize, newSize);

                //thisCreatureController.set
            }
        }
    }

    void ChangeSizes() 
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player) 
        {
            for (int i = 0; i < existingPlants.Count; i++) 
            {
                existingPlants[i].transform.localScale = player.transform.localScale * (percentageSizePlayer / 100.0f);
            }

            for (int i = 0; i < existingFishes.Count; i++) 
            {
                if (existingFishes[i])
                    existingFishes[i].transform.localScale = player.transform.localScale * (percentageSizePlayer / 100.0f);
                else
                    existingFishes.RemoveAt(i);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerIn = false;
        }
    }

    void SpawnProcess(GameObject[] tabToSpawn, List<GameObject> listToStore, int iterations, BoxCollider zoneToSpawn)
    {
        for (int i = 0; i < iterations; i++) 
        {
            GameObject spawnedObject = SpawnRandom(tabToSpawn[Random.Range(0, tabToSpawn.Length)], zoneToSpawn);
            if (spawnedObject)
            {
                listToStore.Add(spawnedObject);
                if (spawnedObject.transform.GetComponent<SeaweedController>()) 
                {
                    //spawnedObject.transform.GetComponent<SeaweedController>().ChangeTemperature(temperature);
                } 
                else 
                {
                    if (spawnedObject.GetComponent<MoveController>())
                    {
                        spawnedObject.GetComponent<MoveController>().init();

                        BehaviourScript bs = spawnedObject.GetComponent<BehaviourScript>();
                        if (bs)
                            bs.roomOrigine = gameObject;
                    }
                }
            }
        }
    }
    
    GameObject SpawnRandom(GameObject objectToSpawn, BoxCollider zoneToSpawn)
    {
        int i = 0;
        Vector3 spawnPosition;

        do {
            i++;
            //if (!isPlayerIn || !randomSpawn) {
            //    float x = Random.Range(0.0f, zoneToSpawn.size.x) - zoneToSpawn.size.x / 2;
            //    float y = Random.Range(0.0f, zoneToSpawn.size.y) - zoneToSpawn.size.y / 2;

            //    spawnPosition = new Vector3(x, y, -1.0f) + zoneToSpawn.transform.position + zoneToSpawn.center;
            //} else {
                float x = zoneToSpawn.gameObject.transform.position.x + zoneToSpawn.center.x;
                float y = zoneToSpawn.gameObject.transform.position.y + zoneToSpawn.center.y;

            float HalfXSize = zoneToSpawn.size.x / 2;
            float HalfYSize = zoneToSpawn.size.y / 2;

            spawnPosition = randomSpawn.pointsRandomGeneration(x - HalfXSize, x + HalfXSize, y - HalfYSize, y + HalfYSize);

                spawnPosition = new Vector3(spawnPosition.x, spawnPosition.y, -1.0f);
            //}

            if(i > limitTestSpawn)
                return null;

        } while (!IsPositionEmpty(spawnPosition));

        GameObject spawnedObject = null;

        if (objectToSpawn.CompareTag("Plant"))
        {
            i = 0;
            RaycastHit hitStruct;
            int xDir;
            int yDir;
            Vector3 origin = spawnPosition;
            Vector3 direction = Vector3.zero;
            //do {
            //    i++;
                

            //    do
            //    {
            //        xDir = Random.Range(0, 3) - 1;
            //        yDir = Random.Range(0, 3) - 1;
            //    }
            //    while (xDir == 0 && yDir == 0);

            //    direction = new Vector3(xDir, yDir, 0.0f);
            //    direction.Normalize();
            //    Debug.Log(direction.ToString());

            //    Debug.DrawRay(origin, direction * 5, Color.magenta, 1000);
            //    Physics.Raycast(origin, direction, out hitStruct, zoneToSpawn.size.y);

            //    if (i > limitTestSpawn)
            //        return null;

            //    if (!hitStruct.collider)
            //        continue;

            //}
            //while (!hitStruct.collider.CompareTag("Environment"));


            bool byebye = false;

            while(!byebye)
            {
                i++;
                do
                {
                    xDir = Random.Range(0, 3) - 1;
                    yDir = Random.Range(0, 3) - 1;
                }
                while (xDir == 0 && yDir == 0);

                direction = new Vector3(xDir, yDir, 0.0f);
                direction.Normalize();

                //Debug.DrawRay(origin, direction * 5, Color.magenta, 1000);
                Physics.Raycast(origin, direction, out hitStruct, zoneToSpawn.size.y);

                if (i > limitTestSpawn)
                    return null;

                if (hitStruct.collider)
                {
                    if (hitStruct.collider.CompareTag("Environment"))
                    {
                        spawnPosition = hitStruct.point;

                        //Debug.DrawRay(hitStruct.point, hitStruct.normal, Color.blue, 1000);
                        Quaternion rotationFinale = Quaternion.FromToRotation(new Vector3(0.0f, 1.0f, 0.0f), new Vector3(hitStruct.normal.x, hitStruct.normal.y, 0.0f)) * objectToSpawn.transform.rotation;
                        spawnedObject = (GameObject)Instantiate(objectToSpawn, spawnPosition, rotationFinale);
                        spawnedObject.transform.parent = this.transform;
                        byebye = true;
                        return spawnedObject;
                    }
                }
            }
            

        }
        else
        {
            spawnedObject = (GameObject)Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
            spawnedObject.transform.parent = this.transform;

            if (spawnedObject.tag.Contains("Fish"))
            {
                currentFishRoomOrigine++;

            }
            else if (spawnedObject.tag.Contains("_Rival") || spawnedObject.name.Contains("Meduse"))
            {
                currentConcurentCreatureRoomOrigine++;
            }

        }
        // should be commented
        //spawnedObject.GetComponent<MoveController>().maxSpeed = 0;
        return spawnedObject;

    }

    bool IsPositionEmpty(Vector3 pos)
    {
        RaycastHit hitStruct;

        Vector3 origin = new Vector3(pos.x, pos.y, -2.0f);
        Vector3 direction = new Vector3(0.0f, 0.0f, 1.0f);

        Debug.DrawRay(origin, direction * 15f, Color.green, 1000);

        return !Physics.Raycast(origin, direction, out hitStruct, 1.5f);
    }

    private void CheckSpawn() 
    {
        if (existingPlants.Count < minPlantsLimit)
            plantsToSpawn = minPlantsLimit - existingPlants.Count + Random.Range(0, minPlantsLimit);
        else if (existingPlants.Count >= minPlantsLimit && existingPlants.Count < maxPlantsLimit)
            plantsToSpawn = Random.Range(0, (maxPlantsLimit - existingPlants.Count) + 1);



        //if (existingFishes.Count < minFishesLimit)
        //    fishesToSpawn = minFishesLimit - existingFishes.Count + Random.Range(0, minFishesLimit);
        //else if (existingFishes.Count >= minFishesLimit && existingFishes.Count < maxFishesLimit)
        //    fishesToSpawn = Random.Range(0, (maxFishesLimit - existingFishes.Count) + 1);

        //if (existingConcurentCreatures.Count < minConcurentCreaturesLimit)
        //    concurrentCreaturesToSpawn = minConcurentCreaturesLimit - existingConcurentCreatures.Count + Random.Range(0, minConcurentCreaturesLimit);
        //else if (existingConcurentCreatures.Count >= minConcurentCreaturesLimit && existingConcurentCreatures.Count < maxConcurentCreaturesLimit)
        //    concurrentCreaturesToSpawn = Random.Range(0, (maxConcurentCreaturesLimit - existingConcurentCreatures.Count) + 1);


        if (currentFishRoomOrigine < minFishesLimitSpawned)
        {
            fishesToSpawn = minFishesLimitSpawned - currentFishRoomOrigine + Random.Range(0, minFishesLimitSpawned);
            if (currentFishRoomOrigine + fishesToSpawn > maxFishesLimitSpawned)
                fishesToSpawn = maxFishesLimitSpawned - currentFishRoomOrigine;
        }
        else if (currentFishRoomOrigine >= minFishesLimitSpawned && currentFishRoomOrigine < maxFishesLimitSpawned)
        {
            fishesToSpawn = Random.Range(0, (maxFishesLimitSpawned - currentFishRoomOrigine) + 1);
            if (currentFishRoomOrigine + fishesToSpawn > maxFishesLimitSpawned)
                fishesToSpawn = maxFishesLimitSpawned - currentFishRoomOrigine;
        }

        if (currentConcurentCreatureRoomOrigine < minConcurentCreaturesLimitSpawned)
        {
            concurrentCreaturesToSpawn = minConcurentCreaturesLimitSpawned - currentConcurentCreatureRoomOrigine + Random.Range(0, minConcurentCreaturesLimitSpawned);
            if (currentConcurentCreatureRoomOrigine + concurrentCreaturesToSpawn > maxConcurentCreaturesLimitSpawned)
                concurrentCreaturesToSpawn = maxConcurentCreaturesLimitSpawned - currentConcurentCreatureRoomOrigine;
        }
        else if (currentConcurentCreatureRoomOrigine >= minConcurentCreaturesLimitSpawned && currentConcurentCreatureRoomOrigine < maxConcurentCreaturesLimitSpawned)
        {
            concurrentCreaturesToSpawn = Random.Range(0, (maxConcurentCreaturesLimitSpawned - currentConcurentCreatureRoomOrigine) + 1);
            if (currentConcurentCreatureRoomOrigine + concurrentCreaturesToSpawn > maxConcurentCreaturesLimitSpawned)
                concurrentCreaturesToSpawn = maxConcurentCreaturesLimitSpawned - currentConcurentCreatureRoomOrigine;
        }




        



        Invoke("Spawn", baseSpawnInterval + Random.Range(minRandTime, maxRandTime));
    }

    private void Spawn()
    {
        if (plantsToSpawn > 0)
        {
            SpawnProcess(possiblePlants, existingPlants, plantsToSpawn, generalBoxCollider);
            plantsToSpawn = 0; //reset
        }
        
        if (fishesToSpawn > 0)
        {
            SpawnProcess(possibleFishes, existingFishes, fishesToSpawn, FishBoxCollider);
            fishesToSpawn = 0; //reset
        }

        if(concurrentCreaturesToSpawn > 0)
        {
            SpawnProcess(possibleConcurentCreatures, existingConcurentCreatures, concurrentCreaturesToSpawn, FishBoxCollider);
            concurrentCreaturesToSpawn = 0; //reset
        }

        Invoke("CheckSpawn", baseSpawnInterval + Random.Range(minRandTime, maxRandTime));
    }


    public void RemoveFoodCreatureBricolage(RoomManager rm) 
    {
        if (rm)
        {
            rm.currentConcurentCreatureRoomOrigine--;
        }
    }
    

    public void RemoveFood(GameObject foodToRemove, bool origine = false)
    {
        if (existingPlants.Contains(foodToRemove))
            existingPlants.Remove(foodToRemove);

        else if (existingFishes.Contains(foodToRemove))
        {
            existingFishes.Remove(foodToRemove);
            if (origine)
            {
                BehaviourScript bs = foodToRemove.GetComponent<BehaviourScript>();
                if (bs)
                {
                    RoomManager rm = bs.roomOrigine.GetComponent<RoomManager>();
                    if (rm)
                    {
                        rm.currentFishRoomOrigine--;
                    }
                }
            }
        }

        else if (existingConcurentCreatures.Contains(foodToRemove))
        {
            existingConcurentCreatures.Remove(foodToRemove);
            if (origine)
            {
                BehaviourScript bs = foodToRemove.GetComponent<BehaviourScript>();
                if (bs)
                {
                    RoomManager rm = bs.roomOrigine.GetComponent<RoomManager>();
                    if (rm)
                    {
                        rm.currentConcurentCreatureRoomOrigine--;
                    }
                }
            }
        }
    }

    public void AddFish(GameObject fishToAdd)
    {
        existingFishes.Add(fishToAdd);
    }

    public void AddConcurentCreature(GameObject concurrentCreatureToAdd)
    {
        existingConcurentCreatures.Add(concurrentCreatureToAdd);
    }

    public bool doesThisFishExist(GameObject fishToInspect)
    {
        return existingFishes.Contains(fishToInspect);
    }

    public bool doesThisCreatureExist(GameObject creatureToInspect) 
    {
        return existingConcurentCreatures.Contains(creatureToInspect);
    }

    public int GetTemperature()
    {
        return temperature;
    }

    public int GetMaxTemperature()
    {
        return maxFishesLimit + maxConcurentCreaturesLimit;
    }

    void ComputeTemperature()
    {
        temperature = existingFishes.Count + existingConcurentCreatures.Count;
        ChangeTemperature(temperature);
    }

    public void ChangeTemperature(int t)
    {
        temperature = t;

        if(existingPlants != null)
            foreach(GameObject g in existingPlants)
            {
                SeaweedController sc = g.GetComponent<SeaweedController>();
                if(sc)
                {
                    sc.TemperatureColdToWarm = (int)Mathf.Floor(3f * GetMaxTemperature() / 9f);               
                    sc.TemperatureWarmToHot = (int)Mathf.Floor(6f * GetMaxTemperature() / 9f);
                    sc.ChangeTemperature(t);
                }
            }
    }

    public List<GameObject> GetFishesInRoom()
    {
        return this.existingFishes;
    }

    public List<GameObject> GetEnnemiesInRoom()
    {
        return this.existingConcurentCreatures;
    }


    public void FishLeft(GameObject g)
    {
        existingFishes.Remove(g);
    }

    public void CreatureLeft(GameObject g)
    {
        existingConcurentCreatures.Remove(g);
    }

}
