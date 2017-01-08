using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class CheckPoint : MonoBehaviour {

    public struct StatsSave {
        public uint strength;
        public uint defense;
        public uint speed;
        public float inputAccel;
        public uint sizeIncrements;
        public uint maxLife;
        public uint currentLifePoints;
        public float attackRange;
        public float power;
        public uint[] foodEaten; //strength/def/speed
        public uint[] statLevel;
        public uint[] statStep;
        public uint[] currentLevelUpThreshold;
        public float baseMaxSpeed;
        public float dashMaxSpeed;
        public bool hasLight;
        public Vector3 spawnPos;
        

        public StatsSave(uint _strength, uint _defense, uint _speed, float _inputAcceleration, uint _maxLife,  uint _currentLifePoints, uint _sizeIncrements, float _attackRange,
                         float _power, uint[] _foodEaten, uint[] _statLevel, uint[] _statStep, uint[] _statsCap, float _baseMaxSpeed, float _dashMaxSpeed, bool _hasLight, Vector3 _spawnPos)
        {
            foodEaten = new uint[3];
            statLevel = new uint[3];
            statStep = new uint[3];
            currentLevelUpThreshold = new uint[3];

            strength = _strength;
            defense = _defense;
            speed = _speed;
            inputAccel = _inputAcceleration;
            sizeIncrements = _sizeIncrements;
            maxLife = _maxLife;
            currentLifePoints = _currentLifePoints;
            attackRange = _attackRange;
            power = _power;
            for (int i = 0; i < 3; i++)
            {
                foodEaten[i] = _foodEaten[i];
                statLevel[i] = _statLevel[i];
                statStep[i] = _statStep[i];
                currentLevelUpThreshold[i] = _statsCap[i];
            }
            baseMaxSpeed = _baseMaxSpeed;
            dashMaxSpeed = _dashMaxSpeed;
            hasLight = _hasLight;
            spawnPos = _spawnPos;
        }
    };

    public StatsSave savedStats;
    public Transform spawnPoint;
    public static string fileName = "Save0.txt";
    public static string saveDirectoryPath = "/Saves/";
    public GameObject floatingText;

    private bool isEnabled = false;
    private GameObject spawnedText;

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            if (collision.GetComponent<CreatureController>().GetLastCheckPoint())
            {
                collision.GetComponent<CreatureController>().GetLastCheckPoint().DisableCheckpoint();
            }

            collision.GetComponent<CreatureController>().SetSpawnLocation(spawnPoint.position);
            collision.GetComponent<CreatureController>().OnCheckPoint(this.gameObject.GetComponent<CheckPoint>());
            savedStats = Save(collision.GetComponent<CreatureController>());

            EnableCheckpoint();

        }
    }

    private StatsSave Save(CreatureController creatureControllerRef)
    {
        StatsSave tempSave = new StatsSave(creatureControllerRef.strength, creatureControllerRef.defense, creatureControllerRef.speed, creatureControllerRef.GetComponent<MoveController>().inputAcceleration,
                                 creatureControllerRef.maxLifePoints, creatureControllerRef.GetHealth(), creatureControllerRef.GetSize(), creatureControllerRef.attackRange,
                                 creatureControllerRef.GetPower(), creatureControllerRef.GetFood(), creatureControllerRef.GetStats(),
                                 creatureControllerRef.GetStatsStep(), creatureControllerRef.GetStatsCaps(), creatureControllerRef.GetComponent<MoveController>().baseMaxSpeed,
                                 creatureControllerRef.GetComponent<MoveController>().dashMaxSpeed, creatureControllerRef.GetHasLightPower(), spawnPoint.position);
        //SaveToFile(tempSave);
        Debug.Log("SAVED");

        return tempSave;
    }

    private void SaveToFile(StatsSave statsToSave)
    {
        if (CheckForFile(saveDirectoryPath, fileName)) {
            Debug.Log("Overwriting file '" + fileName + "' at " + Application.dataPath+saveDirectoryPath);
            TextWriter fd = new StreamWriter(Application.dataPath+saveDirectoryPath + fileName);
            
            fd.WriteLine(this.gameObject.name);
            fd.WriteLine(statsToSave.strength);
            fd.WriteLine(statsToSave.defense);
            fd.WriteLine(statsToSave.speed);
            fd.WriteLine(statsToSave.inputAccel);
            fd.WriteLine(statsToSave.maxLife);
            fd.WriteLine(statsToSave.currentLifePoints);
            fd.WriteLine(statsToSave.sizeIncrements);
            fd.WriteLine(statsToSave.attackRange);
            fd.WriteLine(statsToSave.power);
            for (int i = 0; i < 3; i++)
            {
                fd.WriteLine(statsToSave.foodEaten[i]);
            }
            for (int i = 0; i < 3; i++)
            {
                fd.WriteLine(statsToSave.statLevel[i]);
            }
            for (int i = 0; i < 3; i++)
            {
                fd.WriteLine(statsToSave.statStep[i]);
            }
            for (int i = 0; i < 3; i++)
            {
                fd.WriteLine(statsToSave.currentLevelUpThreshold[i]);
            }
            fd.WriteLine(statsToSave.baseMaxSpeed);
            fd.WriteLine(statsToSave.dashMaxSpeed);
            fd.WriteLine(statsToSave.hasLight);
            fd.WriteLine(statsToSave.spawnPos);

            fd.Close();
        }
        else
            Debug.Log("Error when trying to save to file " + fileName + " at " + Application.dataPath+saveDirectoryPath);
        
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            SaveToFile(savedStats);
            Debug.Log("Saved to file");
        }
    }

    //checks for save files and directories and creates them if needed
    private bool CheckForFile(string filePath, string name)
    {
        if (Directory.Exists(Application.dataPath+saveDirectoryPath))
        {
            if (File.Exists(Application.dataPath+saveDirectoryPath + fileName))
            {
                return true;
            }
            else
            {
                Debug.Log("Creating file '" + fileName + "' at " + Application.dataPath+saveDirectoryPath);
                FileStream fd = File.Create(Application.dataPath+saveDirectoryPath + fileName);
                fd.Close();
                return true;
            }
        }
        else
        {
            Directory.CreateDirectory(Application.dataPath+saveDirectoryPath);

            Debug.Log("Creating directory " + saveDirectoryPath + " and creating file " + Application.dataPath+fileName);
            FileStream fd = File.Create(Application.dataPath+saveDirectoryPath + fileName); //we have to recover it to be able to close it immediately
            fd.Close();
            return true;
        }
        return false; //precaution if this code gets modified
    }

    public void EnableCheckpoint ()
    {
        Animator animatorRef = GetComponentInChildren<Animator>();
        if (animatorRef)
        {
            animatorRef.SetBool("IsEnabled", true);
        }

        if (floatingText != null)
        {
            if (!spawnedText)
            {
                spawnedText = (GameObject)Instantiate(floatingText, this.transform.position, Quaternion.identity);
                spawnedText.transform.transform.parent = this.transform;
            }
        }

        Light checkpointLight = this.GetComponentInChildren<Light>();
        if (checkpointLight) {
            checkpointLight.enabled = true;
        }
        isEnabled = true;
    }

    public void DisableCheckpoint()
    {
        Animator animatorRef = GetComponentInChildren<Animator>();
        if(animatorRef)
        {
            animatorRef.SetBool("IsEnabled", false);
        }
        Light checkpointLight = this.GetComponentInChildren<Light>();
        if (checkpointLight)
        {
            checkpointLight.enabled = false;
        }
        isEnabled = false;
    }
}