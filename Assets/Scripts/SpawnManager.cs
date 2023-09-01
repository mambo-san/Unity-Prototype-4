using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject [] enemyPrefabs;
    public GameObject [] powerupPrefabs;
    public GameObject bossPrefab;

    private float spawnRange = 9;
    private int enemyCount = 0;
    private int waveNumber = 1;

    // Start is called before the first frame update
    void Start()
    {
        SpawnEnemyWave(waveNumber++);
        SpawnPowerup();
    }

    // Update is called once per frame
    void Update()
    {
        enemyCount = FindObjectsOfType<Enemy>().Length;
        if (enemyCount == 0)
        {
            if (waveNumber % 3 == 0)
            {
                SpawnBossWave(waveNumber++);
            }
            else 
            {
                SpawnEnemyWave(waveNumber++);
                SpawnPowerup();
            }
            
        }
    }

    void SpawnBossWave (int enemiesToSpawn)
    {
        Instantiate(bossPrefab, GenerateSpawnPos(), bossPrefab.transform.rotation);
    }

    void SpawnEnemyWave(int enemiesToSpawn)
    {
        for (int i = 0; i < enemiesToSpawn; i++) 
        {
            int index = Random.Range(0, enemyPrefabs.Length);
            Instantiate(enemyPrefabs[index], GenerateSpawnPos(), enemyPrefabs[index].transform.rotation);
        }
    }

    void SpawnPowerup()
    {
        int index = Random.Range(0, powerupPrefabs.Length);
        Instantiate(powerupPrefabs[index], GenerateSpawnPos(), powerupPrefabs[index].transform.rotation);
    }

    private Vector3 GenerateSpawnPos()
    {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);

        return new Vector3(spawnPosX, 0, spawnPosZ);
    }
}
