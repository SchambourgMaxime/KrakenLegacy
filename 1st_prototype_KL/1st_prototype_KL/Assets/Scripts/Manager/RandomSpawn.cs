using UnityEngine;

using System.Collections;

public class RandomSpawn : MonoBehaviour
{
    // Gameobject à instancier 
    public GameObject spawnObj;
    private RaycastHit hit;

    public bool YoloSpawn = false;

    public int maxFishes = 45;

    private int fishesSpawned = 0;

    // Coordonnées de la salle
    public float RoomMinX = 0;
    public float RoomMaxX = 200;
    public float RoomMinY = 0;
    public float RoomMaxY = 200;

    // Coordonnées de la zone de spawn aléatoire
    public float HalfSpawnAreaWidth = 10;
    public float HalfSpawnAreaHeight = 10;
    private float SpawnMinX = 0;
    private float SpawnMaxX = 0;
    private float SpawnMinY = 0;
    private float SpawnMaxY = 0;

    // Coordonnées des zones de spawn fixes
    public float[,] coordinates;

    public float SpawnAMinX = 0;
    public float SpawnAMinY = 0;
    public float SpawnASize = 0;
    private float SpawnAMaxX = 0;
    private float SpawnAMaxY = 0;

    public float SpawnBMinX = 0;
    public float SpawnBMinY = 0;
    public float SpawnBSize = 0;
    private float SpawnBMaxX = 0;
    private float SpawnBMaxY = 0;

    public float SpawnCMinX = 0;
    public float SpawnCMinY = 0;
    public float SpawnCSize = 0;
    private float SpawnCMaxX = 0;
    private float SpawnCMaxY = 0;

    void Start()
    {
        SpawnAMaxX = SpawnAMinX + SpawnASize;
        SpawnBMaxX = SpawnBMinX + SpawnBSize;
        SpawnCMaxX = SpawnCMinX + SpawnCSize;
        SpawnAMaxY = SpawnAMinY + SpawnASize;
        SpawnBMaxY = SpawnBMinY + SpawnBSize;
        SpawnCMaxY = SpawnCMinY + SpawnCSize;

        coordinates = new float[,] { { SpawnAMinX, SpawnAMaxX, SpawnAMinY, SpawnAMaxY }, { SpawnBMinX, SpawnBMaxX, SpawnBMinY, SpawnBMaxY }, { SpawnCMinX, SpawnCMaxX, SpawnCMinY, SpawnCMaxY } };
    }

    void Update()
    {
        //while (GameObject.FindGameObjectsWithTag("Fish").Length <= maxFishes && spawnAttempts < 15)
        //{
        //    if (!YoloSpawn)
        //    {
        //        SelectRandomSpawnArea();
        //    }
        //    else
        //    {
        //        SelectSpawnArea();
        //    }
        //    spawnAttempts++;
        //}
        //if(spawnAttempts == 16) { Debug.Log("aucune zone viable"); }
    }

    void SelectSpawnArea()
    {
        Vector3 spawnCenter = pointsRandomGeneration(RoomMinX, RoomMaxX, RoomMinY, RoomMaxY);

        // Création des objets
        if (Physics.OverlapBox(spawnCenter, new Vector3(10, 5, 10), Quaternion.identity, 0, QueryTriggerInteraction.Ignore).Length == 0)
        {
            SpawnMinX = spawnCenter.x - HalfSpawnAreaWidth;
            SpawnMaxX = spawnCenter.x + HalfSpawnAreaWidth;
            SpawnMinY = spawnCenter.y - HalfSpawnAreaHeight;
            SpawnMaxY = spawnCenter.y + HalfSpawnAreaHeight;

            while (GameObject.FindGameObjectsWithTag("Fish").Length <= maxFishes && fishesSpawned <= 15)
            {
                float x = Random.Range(SpawnMinX, SpawnMaxX);
                float y = Random.Range(SpawnMinY, SpawnMaxY);
                float z = 0.5f;

                Vector3 spawnPoint = new Vector3(x, y, z);

                if (!Physics.SphereCast(spawnPoint, 1, Vector3.forward, out hit, 1))
                {
                    Instantiate(spawnObj, spawnPoint, Quaternion.identity);
                }
                fishesSpawned++;
            }
            if(fishesSpawned == 16) { Debug.Log("nombre max de poissons pour 1 spawn"); }
            fishesSpawned = 0;
        }
    }

    private void SelectRandomSpawnArea()
    {
        int areaSelector = Random.Range(0, 3);

        while (GameObject.FindGameObjectsWithTag("Fish").Length <= maxFishes && fishesSpawned <= 15)
        {
            Vector3 spawnPoint = pointsRandomGeneration(coordinates[areaSelector, 0], coordinates[areaSelector, 1], coordinates[areaSelector, 2], coordinates[areaSelector, 3]);

            if (!Physics.SphereCast(spawnPoint, 1, Vector3.forward, out hit, 1))
            {
                Instantiate(spawnObj, spawnPoint, Quaternion.identity);
                fishesSpawned++;
            }
            if (fishesSpawned == 16) { Debug.Log("nombre max de poissons pour 1 spawn"); }
        }
        fishesSpawned = 0;
    }

    // Génère un vecteur position situé en dehors deu champ de vision de la caméra
    public Vector3 pointsRandomGeneration(float minX, float maxX, float minY, float maxY)
    {
        // Variables pour les coordonnées de spawn
        float x = 0;
        float y = 0;
        float z = -1f;

        // Projection des points limites de la caméra dans le repère du monde
        Vector3 CamXWorld = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.transform.position.z));
        Vector3 CamYWorld = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.transform.position.z));

        // Coordonnées du rectangle à exclure de la génération aléatoire = vue de la caméra
        float minExcludedX = Mathf.Min(CamXWorld.x, CamYWorld.x) - HalfSpawnAreaWidth;
        float maxExcludedX = Mathf.Max(CamXWorld.x, CamYWorld.x) + HalfSpawnAreaWidth;
        float minExcludedY = Mathf.Min(CamXWorld.y, CamYWorld.y) - HalfSpawnAreaHeight;
        float maxExcludedY = Mathf.Max(CamXWorld.y, CamYWorld.y) + HalfSpawnAreaHeight;

        int randX = Random.Range(0, 2);

        // Génération aléatoire de coordonnées
        x = Random.Range(minX, maxX);

        if (x >= minExcludedX && x <= maxExcludedX)
        {
            if (minExcludedY < minY && maxExcludedY < maxY)
            {
                z = Random.Range(maxExcludedY, maxY);
            }
            else if (maxExcludedY > maxY && minExcludedY > minY)
            {
                z = Random.Range(minY, minExcludedY);
            }
            else
            {
                // Chiffre aléatoire pour décider de se placer dans l'intervale inférieur ou supérieur
                int randY = Random.Range(0, 2);

                if (randY == 0)
                {
                    y = Random.Range(minY, minExcludedY);
                }
                else
                {
                    y = Random.Range(maxExcludedY, maxY);
                }
            }
        }
        else
        {
            y = Random.Range(minY, maxY);
        }

        return new Vector3(x, y, z);
    }
}