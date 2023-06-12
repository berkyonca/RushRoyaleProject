using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject slimeEnemy, flyingEnemy, bossEnemy;

    [SerializeField]
    private Transform enemyStorage;

    [SerializeField]
    private Transform spawnPosition;

    private int _slimeEnemyCount = 0;
    private int _flyingEnemyCount = 0;

    public int MaxSlimeEnemy, MaxFlyingEnemy;

    public float EnemySpawnRate;

    public List<GameObject> EnemyList = new List<GameObject>(); 

    private void Start()
    {
        StartCoroutine(SpawnEnemy());
    }

    private IEnumerator SpawnEnemy()
    {
        if (_slimeEnemyCount < MaxSlimeEnemy)
        {
            yield return new WaitForSeconds(EnemySpawnRate);
            _slimeEnemyCount++;
           SpawnEnemy(slimeEnemy, spawnPosition.position);
            StartCoroutine(SpawnEnemy());
        }

        else if (_slimeEnemyCount >= MaxSlimeEnemy && _flyingEnemyCount < MaxFlyingEnemy)
        {
            yield return new WaitForSeconds(EnemySpawnRate);
            _flyingEnemyCount++;
            SpawnEnemy(flyingEnemy, spawnPosition.position);
            StartCoroutine(SpawnEnemy());
        }

        else
        {
            SpawnEnemy(bossEnemy, spawnPosition.position);
        }
    }

    private void SpawnEnemy(GameObject obj, Vector3 spawnPosition)
    {
        
        GameObject enemy = Instantiate(obj, spawnPosition, Quaternion.identity, enemyStorage);
        EnemyList.Add(enemy);
    }






}
