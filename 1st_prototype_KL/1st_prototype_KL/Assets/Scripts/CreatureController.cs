using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;


public class CreatureController : MonoBehaviour {
    
    // constants for arrays
    const int STRENGTH = 0;
    const int DEFENSE  = 1;
    const int SPEED    = 2;

    //feedback
    public GameObject strengthLevelUpEmitter;
    public GameObject defenseLevelUpEmitter;
    public GameObject speedLevelUpEmitter;

    // life
    public uint maxLifePoints;
    public uint currentLifePoints;
    public uint maxLifeIncrement = 1; //how much max health you gain when you gain size

    // true stats
    public uint strength;
    public uint defense;
    public uint speed;

    

    public float knockbackForce = 2000.0f;

    // food related - only rival creatures can gain satiety. The method 'increaseSatiety' is called when a creature eats a fish or a seaweed (see 'FoodController.cs')
    public uint satiety;
    public uint maxSatiety;
    private float satietyTimer = 4;

    // variable for power
    public uint powerStrengthPercentage = 70;
    public uint powerDefensePercentage = 20;
    public uint powerSpeedPercentage = 10;

    // gain in size
    public float sizeGain = 0.1f;
    public uint maxSizeIncrements = 20; //how much times you can gain size in size (your max size = this * sizeGain + your base scale)
    private uint sizeIncrements = 0; //how many times you gained in size

    // recovery variables
    public bool isInvincible = false;


    public float recoveryTime = 1;
    public int recoveryFlash = 6;


    private float recoveryFlashingSpeed;

    //respawn variables
    public float timeToRespawn;
    private Vector3 spawnLocation;
    private CheckPoint lastCheckPoint = null;

    //food variables
    private uint[] foodEaten = { 0, 0, 0 }; //strength/def/speed
    private uint[] statLevel = { 0, 0, 0 };
    private uint[] currentLevelUpThreshold = { 0, 0, 0 };
    private uint[] statStep = { 0, 0, 0 };
    public uint statLevelUpThreshold = 5; //# of food of a type you have to eat to gain 1 of the corresponding stat
    public uint baseStatStep = 3; //the cap goes up separately for each stat when they are > to this
    public uint statsCap = 20;

    public float radiusFogOfWar;

    private float startTime = -1.0f;
    public float attackRange = 10.0f;
    private float power = 0;
    private float gainMultiplier = 1.0f;
    private Quaternion deathRotation;
    private bool isDead;
    private bool hasLightPower = false;
    //private SkinnedMeshRenderer[] meshRenderer;
    private RaycastHit centerHit;

    //progression
    public uint dashUnlockSpdThreshold = 10;
    public uint destrUnlockStrThreshold = 10;

    //savefile path from Application.dataPath (including file name). 
    public static string savePath = "/Saves/Save0.txt";

    public GameObject prefabOrbes;
    private bool orbesSpawn = false;

    private Transform fishMesh;
    private float baseScale;
    private GameObject instantiatedFX;
    private float FXScaleProportion; //your 3  levelUpEmitters should have the same base scale for this to work properly !
    private float FXBaseScale;

    public Texture redTexture;
    private Texture normalTexture;

    public float bonusSpeedMaxWanted = 10.0f;
    private float ratioBonusSpeed = 5000f;
    public float ratioBonusAcceleration = 5.0f;

    public GameObject tmpOrigine;



    // Use this for initialization
    void Start()
    {
        if (bonusSpeedMaxWanted <= 0)
            ratioBonusSpeed = 0;
        else
            ratioBonusSpeed = 210 / bonusSpeedMaxWanted; // 210 est une valeur a ne pas changer, trouvé de maniere archaique mais qui devrait marcher si le dieu des maths est généreux



        spawnLocation = this.transform.position;
        baseScale = this.transform.localScale.x;

        normalTexture = GetComponentInChildren<Renderer>().material.mainTexture;


        if (recoveryFlash < 1)
            recoveryFlash = 1;

        recoveryFlashingSpeed = recoveryTime / recoveryFlash;

        if (strengthLevelUpEmitter != null) {
            FXBaseScale = strengthLevelUpEmitter.transform.localScale.x;
            FXScaleProportion = sizeGain / baseScale;
        }

        //meshRenderer = GetComponentsInChildren<SkinnedMeshRenderer>();

        currentLevelUpThreshold[0] = statLevelUpThreshold;
        currentLevelUpThreshold[1] = statLevelUpThreshold;
        currentLevelUpThreshold[2] = statLevelUpThreshold;

        statStep[0] = baseStatStep;
        statStep[1] = baseStatStep;
        statStep[2] = baseStatStep;

        eatFood(strength, defense, speed);

        if(gameObject.CompareTag("Player"))
            Load();

        UpdatePower();

        fishMesh = transform.Find("Fish_Mesh");

        if (!fishMesh)
        {
            Transform child = transform.Find("Textures");

            if (child) 
            {
                fishMesh = child.Find("Fish_Mesh");
            }
        }

        if(tag.Contains("RivalCreature"))
        {
            EquilibrateCreature();
        }
    }

    void EquilibrateCreature()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        CreatureController playerCreatureController = player.GetComponent<CreatureController>();

        uint[] trueStats = { strength, defense, speed };

        do 
        {
            int randomIndex = Random.Range(0, statLevel.Length);
            statLevel[randomIndex]++;
            switch(randomIndex)
            {
                case STRENGTH:
                    strength++;
                    break;
                case DEFENSE:
                    defense++;
                    break;
                case SPEED:
                    speed++;
                    break;
            }
            UpdatePower();
        }
        while (power < playerCreatureController.GetPower());

        currentLifePoints = maxLifePoints;

        eatFood(0u, 0u, 0u);
    }

    void OnDestroy()
    {
        RoomManager roomManager = GetComponentInParent<RoomManager>();

        if(roomManager)
        {
            roomManager.RemoveFood(gameObject, true);
            return;
        }

        JunctionManager junctionManager = GetComponentInParent<JunctionManager>();

        if (junctionManager) 
        {
            junctionManager.RemoveFood(gameObject);
            return;
        }
    }

    void UpdatePower() 
    {

        float newPower = 0;
        newPower = 0;

        newPower += (powerStrengthPercentage/100.0f) * (float)strength;
        newPower += (powerDefensePercentage/100.0f) * (float)defense;
        newPower += (powerSpeedPercentage/100.0f) * (float)speed;

        newPower *= (currentLifePoints / maxLifePoints);

        power = newPower;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if(startTime != -1.0f)
        //{
        //    float delta = Time.realtimeSinceStartup - startTime;

        //    if (delta > recoveryTime) 
        //    {
        //        startTime = -1.0f;
        //        isInvincible = false;
        //        //foreach(SkinnedMeshRenderer smr in meshRenderer)
        //        //    smr.enabled = true;
        //    }
        //    else
        //    {
        //        int interval = Mathf.FloorToInt((Mathf.FloorToInt(delta*1000) % (recoveryFlashingSpeed * 2 * 1000)) / (recoveryFlashingSpeed * 1000));
        //        if (interval == 0) 
        //        {
        //            //foreach (SkinnedMeshRenderer smr in meshRenderer)
        //            //    smr.enabled = true;
        //        }
        //        else if (interval == 1)
        //        {
        //            //foreach (SkinnedMeshRenderer smr in meshRenderer)
        //            //    smr.enabled = false;
        //        }

        //    }
        //}

        // Réduction de la satiété de 10 points toutes les 4 secondes
        if(satiety >= 10)
        {
            satietyTimer -= Time.deltaTime;
            if (satietyTimer <= 0)
            {
                satietyTimer = 4;
                satiety -= 10;
            }
        }

        

        if (isDead)
        {
            if (fishMesh)
            {
                if (fishMesh.rotation != deathRotation) 
                {
                    fishMesh.rotation = Quaternion.Slerp(fishMesh.rotation, deathRotation, 0.01f);
                }
            }
        }
    }

    public void TakeDamage(uint damageAmount, bool trueDamage = false) {
        if (isInvincible == false) {

            int pointsLost = 0;


            if (trueDamage)
                pointsLost = (int)(damageAmount);
            else
                pointsLost = (int)(ArmorFormula(damageAmount, this.defense));
            

            if ((currentLifePoints - pointsLost) <= 0)
            {
                currentLifePoints = 0;
                if (CompareTag("Player"))
                    GetComponent<HUDController>().UpdateHealthSlider();
                OnDeath();
                return;
            }

            if (pointsLost > 0)
            {
                currentLifePoints -= (uint)pointsLost;
                StartCoroutine(ClignoteRouge(recoveryFlashingSpeed, recoveryFlash * 2, false)); // fois 2 pour correspondre au nombre de passage a rouge
                isInvincible = true;
            }

            
            if (CompareTag("Player"))
            {
	            GetComponent<HUDController>().UpdateHealthSlider();
                GetComponent<AudioController>().PlayTakeDamage();
            }
            else
            {
                GetComponent<AudioSource>().Play();
            }

            //startTime = Time.realtimeSinceStartup;

        }
    }

    IEnumerator ClignoteRouge(float delayTime, int nbTime, bool red)
    {
      //  Debug.Log("iteration");
        yield return new WaitForSeconds(delayTime);


        //Texture t = GetComponentInChildren<Renderer>().material.mainTexture;

        if (!isDead)
        {
            if (nbTime > 0)
            {
                if (red)
                    GetComponentInChildren<Renderer>().material.mainTexture = normalTexture;
                else
                    GetComponentInChildren<Renderer>().material.mainTexture = redTexture;

                StartCoroutine(ClignoteRouge(delayTime, --nbTime, !red));
            }
            else
            {
                if (red)
                    GetComponentInChildren<Renderer>().material.mainTexture = normalTexture;

                isInvincible = false;
            }
        }
        else
        {
            //GetComponentInChildren<Renderer>().material.mainTexture = normalTexture;

            GetComponentInChildren<Renderer>().material.mainTexture = redTexture;
            isInvincible = false; //utile ?
        }
    }

    void OnDeath() 
    {
        if(CompareTag("Player"))
            GetComponent<AudioController>().PlayDeath();

        EnableMovement(false);

        MoveController moveController = GetComponent<MoveController>();

        GetComponentInChildren<Renderer>().material.mainTexture = redTexture;

        //On pourrait mettre la bestiole en rouge ici

        if (moveController)
        {
            moveController.setcanBeKnockedBack(false);
        }

        if (CompareTag("Player"))
            Invoke("Respawn", timeToRespawn);
        else
        {
            // GetComponent

            BehaviourScript bs = GetComponent<BehaviourScript>();
            if (bs)
                tmpOrigine = GetComponent<BehaviourScript>().roomOrigine;

            Destroy(GetComponent<BehaviourScript>());
            Destroy(GetComponentInChildren<Animation>());
            Destroy(GetComponentInChildren<SphereCollider>());
            if(gameObject.tag == "Turtle_RivalCreature")
            {
                Destroy(GetComponent<C_RivalCreature_Turtle>());
            }
            else if(gameObject.tag == "Shark_RivalCreature")
            {
                Destroy(GetComponent<C_RivalCreature_Shark>());
            }
            

            isDead = true;

            if (moveController) 
            {
                moveController.instantStop();
                if (fishMesh) 
                {
                    deathRotation = fishMesh.rotation * Quaternion.Euler(0, 0, 180);
                }

                if (tag.Contains("_RivalCreature"))
                {
                    if (prefabOrbes)
                        if (!orbesSpawn)
                        {
                            GameObject orb = (GameObject)Instantiate(prefabOrbes, transform.position, transform.rotation);
                            orb.transform.parent = transform.parent;

                            OrbesMovement orbsMovement = orb.GetComponent<OrbesMovement>();
                            if(orbsMovement)
                            {
                                orbsMovement.DefineOwner(this);
                            }

                            orbesSpawn = true;
                        }
                }

            }

            Invoke("Dead", timeToRespawn);
        }
    }

    void Dead()
    {
        RoomManager roomManager = tmpOrigine.GetComponent<RoomManager>();
        if(roomManager)
        {
            roomManager.RemoveFood(gameObject,true);
            if(!gameObject.name.Contains("Meduse"))
                roomManager.RemoveFoodCreatureBricolage(roomManager);
        }
        Destroy(gameObject);
    }

    public void Respawn() {

        isInvincible = false;
        GetComponent<MoveController>().instantStop();
        GetComponent<MoveController>().setcanBeKnockedBack(true);
        GetComponentInChildren<Renderer>().material.mainTexture = normalTexture;
        EnableMovement(true);
        EnableBox(true);
        this.transform.position = spawnLocation;
        if(lastCheckPoint)
        {
            strength = lastCheckPoint.savedStats.strength;
            defense = lastCheckPoint.savedStats.defense;
            speed = lastCheckPoint.savedStats.speed;
            GetComponent<MoveController>().inputAcceleration = lastCheckPoint.savedStats.inputAccel;
            maxLifePoints = lastCheckPoint.savedStats.maxLife;
            currentLifePoints = maxLifePoints;
            sizeIncrements = lastCheckPoint.savedStats.sizeIncrements;
            attackRange = lastCheckPoint.savedStats.attackRange;
            power = lastCheckPoint.savedStats.power;
            currentLevelUpThreshold = lastCheckPoint.savedStats.currentLevelUpThreshold;
            statStep = lastCheckPoint.savedStats.statStep;
            foodEaten = lastCheckPoint.savedStats.foodEaten;
            statLevel = lastCheckPoint.savedStats.statLevel;
            GetComponent<MoveController>().baseMaxSpeed = lastCheckPoint.savedStats.baseMaxSpeed;
            GetComponent<MoveController>().maxSpeed = lastCheckPoint.savedStats.baseMaxSpeed;
            GetComponent<MoveController>().dashMaxSpeed = lastCheckPoint.savedStats.dashMaxSpeed;
            hasLightPower = lastCheckPoint.savedStats.hasLight;
        }
        currentLifePoints = maxLifePoints;
        if (CompareTag("Player"))
        {
            GetComponent<HUDController>().UpdateSlider();
            gameObject.GetComponent<FogOfWar>().ResetRadius();
            if(speed < dashUnlockSpdThreshold)
            {
                GetComponent<MoveController>().isDashUnlocked = false;
            }
        }

    }

    private void Load()
   { 
        if (File.Exists(Application.dataPath + savePath))
        {
            StreamReader fd = new StreamReader(Application.dataPath + savePath);

            lastCheckPoint = GameObject.Find(fd.ReadLine()).GetComponent<CheckPoint>();
            strength = uint.Parse(fd.ReadLine());
            defense = uint.Parse(fd.ReadLine());
            speed = uint.Parse(fd.ReadLine());
            GetComponent<MoveController>().inputAcceleration = float.Parse(fd.ReadLine());
            maxLifePoints = uint.Parse(fd.ReadLine());
            currentLifePoints = uint.Parse(fd.ReadLine());
            sizeIncrements = uint.Parse(fd.ReadLine());
            attackRange = float.Parse(fd.ReadLine());
            power = float.Parse(fd.ReadLine());
            for (int i = 0; i < 3; i++)
            {
                foodEaten[i] = uint.Parse(fd.ReadLine());
            }
            for (int i = 0; i < 3; i++)
            {
                statLevel[i] = uint.Parse(fd.ReadLine());
            }
            for (int i = 0; i < 3; i++)
            {
                statStep[i] = uint.Parse(fd.ReadLine());
            }
            for (int i = 0; i < 3; i++)
            {
                currentLevelUpThreshold[i] = uint.Parse(fd.ReadLine());
            }
            GetComponent<MoveController>().baseMaxSpeed = float.Parse(fd.ReadLine());
            GetComponent<MoveController>().maxSpeed = GetComponent<MoveController>().baseMaxSpeed;
            GetComponent<MoveController>().dashMaxSpeed = float.Parse(fd.ReadLine());
            hasLightPower = bool.Parse(fd.ReadLine());

            //avoir displaying useless messages
            GetComponent<HUDController>().SetIsSpeedSkillUnlocked(true); 
            GetComponent<HUDController>().SetIsStrengthSkillUnlocked(true);

            spawnLocation = lastCheckPoint.spawnPoint.position;
            this.transform.position = spawnLocation;

            //restore scale
            transform.localScale = new Vector3(baseScale + sizeGain * sizeIncrements, baseScale + sizeGain * sizeIncrements, baseScale + sizeGain * sizeIncrements);

            if (GetComponent<MoveController>().GetDashUnlocked() != true && speed >= dashUnlockSpdThreshold)
                EnableDash();
            SetCheckPointValues(); //to avoid unexpected spawn values if the player dies after loading but before getting to a checkpoint

            GetComponent<HUDController>().UpdateSlider();

            fd.Close();

        }
        else
            Debug.Log("No saveFile at" + Application.dataPath + savePath);
    }

    void updateSpeed() 
    {
        GetComponent<MoveController>().maxSpeed += speed / ratioBonusSpeed;
        GetComponent<MoveController>().baseMaxSpeed += speed / ratioBonusSpeed;
        GetComponent<MoveController>().dashMaxSpeed += speed / ratioBonusSpeed;
        GetComponent<MoveController>().inputAcceleration += speed / ratioBonusSpeed * ratioBonusAcceleration;
    }

    public void eatFood(uint strEaten, uint defEaten, uint spdEaten)
    {

        if (statLevel[STRENGTH] < statsCap)
        {
            foodEaten[STRENGTH] += (uint)(gainMultiplier * (float)strEaten);
        }
        if (statLevel[DEFENSE] < statsCap)
        {
        foodEaten[DEFENSE]  += (uint)(gainMultiplier * (float)defEaten);
        }
        if (statLevel[SPEED] < statsCap)
        {
            foodEaten[SPEED] += (uint)(gainMultiplier * (float)spdEaten);
        }

        updateStats(STRENGTH);
        updateStats(DEFENSE);
        updateStats(SPEED);

        //to make food have diminishing returns
        for (int i = 0; i < 3; i++)
        {
            if ((statLevel[i]) >= statStep[i])
            {
                currentLevelUpThreshold[i]++;
                statStep[i]++;
            }
        }

        while (Mathf.Min(statLevel[0], statLevel[1], statLevel[2]) > sizeIncrements)
        {
            if((sizeIncrements +1) <= maxSizeIncrements)
            {
                UpgradeMaxHealth(maxLifeIncrement);
                sizeIncrements++;
                transform.localScale = new Vector3(transform.localScale.x + sizeGain, transform.localScale.y +sizeGain, transform.localScale.z + sizeGain);
                attackRange += sizeGain;
                if (CompareTag("Player"))
                    GetComponent<HUDController>().UpdateHealthSlider();
            }
            else
            { 
                break;
            }
         }   
        

        //Debug.Log("stats:"+statLevel[0]+","+statLevel[1]+","+statLevel[2]);
        //Debug.Log("food:" + foodEaten[0] + "," + foodEaten[1] + "," + foodEaten[2]);

        

        UpdatePower();
    }

    void updateStats(int statIndex)
    {
        //stats level up occurs here
        if ((foodEaten[statIndex] >= currentLevelUpThreshold[statIndex]) && (statLevel[statIndex] < statsCap))
        {
            if (statLevel[statIndex] + 1 <= statsCap)
            {
                statLevel[statIndex] += foodEaten[statIndex] / currentLevelUpThreshold[statIndex];
            }
            if (statLevel[statIndex] < statsCap)
            {
                foodEaten[statIndex] = foodEaten[statIndex] % currentLevelUpThreshold[statIndex];
            }
            switch (statIndex)
            {
                case STRENGTH:
                    strength = statLevel[statIndex];

                    if (this.CompareTag("Player"))
                    {
                        instantiatedFX = (GameObject)Instantiate(strengthLevelUpEmitter, transform.position, Quaternion.identity);
                        instantiatedFX.transform.localScale = new Vector3(FXBaseScale + sizeIncrements * FXScaleProportion, FXBaseScale + sizeIncrements * FXScaleProportion, FXBaseScale + sizeIncrements * FXScaleProportion);
                        instantiatedFX.transform.parent = this.transform;
                        GetComponent<AudioController>().PlayFullBarSound(1.0f);
                    }

                    break;
                case DEFENSE:
                    defense = statLevel[statIndex];

                    if (this.CompareTag("Player"))
                    {
                        instantiatedFX = (GameObject)Instantiate(defenseLevelUpEmitter, transform.position, Quaternion.identity);
                        instantiatedFX.transform.localScale = new Vector3(FXBaseScale + sizeIncrements * FXScaleProportion, FXBaseScale + sizeIncrements * FXScaleProportion, FXBaseScale + sizeIncrements * FXScaleProportion);
                        instantiatedFX.transform.parent = this.transform;
                        GetComponent<AudioController>().PlayFullBarSound(2.0f);
                    }

                    UpgradeMaxHealth(1);
                    if(CompareTag("Player"))
                        GetComponent<HUDController>().UpdateHealthSlider(); //update the slider because we gained some max health

                    break;
                case SPEED:
                    speed = statLevel[statIndex];

                    if (this.CompareTag("Player"))
                    {
                        instantiatedFX = (GameObject)Instantiate(speedLevelUpEmitter, transform.position, Quaternion.identity);
                        instantiatedFX.transform.localScale = new Vector3(FXBaseScale + sizeIncrements * FXScaleProportion, FXBaseScale + sizeIncrements * FXScaleProportion, FXBaseScale + sizeIncrements * FXScaleProportion);
                        instantiatedFX.transform.parent = this.transform;
                        GetComponent<AudioController>().PlayFullBarSound(3.0f);
                    }
                    updateSpeed();

                    if (GetComponent<MoveController>().GetDashUnlocked() != true && speed >= dashUnlockSpdThreshold)
                        EnableDash();
                    break;
            }
            
        }
    }

    public uint[] getFoodEaten()
    {
        return foodEaten;
    }

    public uint[] getStatLevel()
    {
        return statLevel;
    }

    public void Healing(uint healthRecovery)
    {
        // currentLifePoints = currentLifePoints + healthRecovery > maxLifePoints ? maxLifePoints : currentLifePoints + healthRecovery;

        if (currentLifePoints + healthRecovery > maxLifePoints)
            currentLifePoints = maxLifePoints;
        else
            currentLifePoints += healthRecovery;
    }

    public void SetSpawnLocation(Vector3 newLocation)
    {
        spawnLocation = newLocation;
    }

    public bool IsDead()
    {
        return currentLifePoints == 0;
    }

    public void EnableMovement(bool isEnabled) 
    {
        MoveController movecontroller = GetComponent<MoveController>();
        PlayerController playerController = GetComponent<PlayerController>();
        BehaviourScript behaviourScript = GetComponent<BehaviourScript>();
        AttaqueSimple attaqueSimple = GetComponent<AttaqueSimple>();

        if (movecontroller)
            movecontroller.enabled = isEnabled;
        if (playerController)
            playerController.enabled = isEnabled;
        if (behaviourScript)
            behaviourScript.enabled = isEnabled;
        if (attaqueSimple)
            attaqueSimple.enabled = isEnabled;
    }

    public void EnableBox(bool isEnabled) 
    {
        Collider col = GetComponent<Collider>();

        if (col)
            col.enabled = isEnabled;
    }

    public void EnableDash() 
    {
        if (speed >= dashUnlockSpdThreshold)
            GetComponent<MoveController>().SetDashUnlocked();
    }

    public uint GetHealth()
    {
        return currentLifePoints;
    } 

    public float GetPower()
    {
        return power;
    }

    public uint ArmorFormula(uint damage, uint armor) 
    {
        float hundred = 100.0f;
        return (uint)(hundred / (hundred + armor) * damage);
    }

    public void increaseSatiety(uint satietyValue)
    {
        satiety += satietyValue;
    }

    public void setGainMultiplier(float newGainMultiplier)
    {
        gainMultiplier = newGainMultiplier;
    }

    public float getGainMultiplier()
    {
        return gainMultiplier;
    }

    public uint GetSize()
    {
        return sizeIncrements;
    }

    public uint[] GetFood()
    {
        return foodEaten;
    }

    public uint[] GetStats()
    {
        return statLevel;
    }

    public uint[] GetStatsStep()
    {
        return statStep;
    }

    public uint[] GetStatsCaps()
    {
        return currentLevelUpThreshold;
    }

    public void OnCheckPoint(CheckPoint checkPointPassed)
    {
        lastCheckPoint = checkPointPassed;
    }

    public void setStatietyTimer(int duration)
    {
        satietyTimer = duration;
    }

    public float getSatietyTimer()
    {
        return satietyTimer;
    }

    public bool CanBreakWalls()
    {
        return (strength >= destrUnlockStrThreshold);
    }

    public float GetBaseScale()
    {
        return baseScale;
    }

    public bool GetHasLightPower()
    {
        return hasLightPower;
    }

    public void SetHasLightPower(bool newHasLightPower)
    {
        hasLightPower = newHasLightPower;
    }

    private void SetCheckPointValues() //only use this when loading the game
    {
        if (lastCheckPoint)
        {
            lastCheckPoint.savedStats.strength = strength;
            lastCheckPoint.savedStats.defense = defense;
            lastCheckPoint.savedStats.speed = speed;
            lastCheckPoint.savedStats.inputAccel = GetComponent<MoveController>().inputAcceleration;
            lastCheckPoint.savedStats.maxLife = maxLifePoints;
            lastCheckPoint.savedStats.currentLifePoints = currentLifePoints;
            lastCheckPoint.savedStats.sizeIncrements = sizeIncrements;
            lastCheckPoint.savedStats.attackRange = attackRange;
            lastCheckPoint.savedStats.power = power;
            lastCheckPoint.savedStats.currentLevelUpThreshold = currentLevelUpThreshold;
            lastCheckPoint.savedStats.statStep = statStep;
            lastCheckPoint.savedStats.foodEaten = foodEaten;
            lastCheckPoint.savedStats.statLevel = statLevel;
            lastCheckPoint.savedStats.baseMaxSpeed = GetComponent<MoveController>().baseMaxSpeed;
            lastCheckPoint.savedStats.dashMaxSpeed = GetComponent<MoveController>().dashMaxSpeed;
            lastCheckPoint.savedStats.hasLight = hasLightPower;
            lastCheckPoint.savedStats.spawnPos = lastCheckPoint.transform.position;

            lastCheckPoint.EnableCheckpoint();
        }
    }

    private void UpgradeMaxHealth(uint lifeGain)
    {
        maxLifePoints += lifeGain;
        currentLifePoints += lifeGain;
    }

    public CheckPoint GetLastCheckPoint()
    {
        return lastCheckPoint;
    }

    public void SetLastCheckPoint(CheckPoint newCheckPoint)
    {
        lastCheckPoint = newCheckPoint;
    }
}
