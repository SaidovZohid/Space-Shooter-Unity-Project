using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;

    [SerializeField]
    private GameObject _bossPrefab;

    [SerializeField]
    private int _bossWave = 5;

    [SerializeField]
    private GameObject[] powerups;

    private bool _stopSpawning = false;

    private int _currentWave = 1;
    private int _enemiesAlive = 0;

    private UIManager _uiManager;

    public void StartSpawning()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        StartCoroutine(WaveRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
    }

    IEnumerator WaveRoutine()
    {
        yield return new WaitForSeconds(3f);

        while (!_stopSpawning)
        {
            if (_uiManager != null)
            {
                _uiManager.ShowWave(_currentWave);
            }

            yield return new WaitForSeconds(2f);

            if (_currentWave == _bossWave)
            {
                Debug.Log("Wave " + _currentWave + " - Boss Wave!");
                yield return new WaitForSeconds(1f);
                SpawnBoss();
            }
            else
            {
                int enemiesToSpawn = 1 + _currentWave;

                for (int i = 0; i < enemiesToSpawn; i++)
                {
                    SpawnEnemy();
                    yield return new WaitForSeconds(0.5f);
                }
            }

            while (_enemiesAlive > 0)
            {
                yield return new WaitForSeconds(0.5f);
            }

            _currentWave++;
            yield return new WaitForSeconds(3f);
        }
    }

    void SpawnEnemy()
    {
        Vector3 posToSpawn = new Vector3(Random.Range(-9.5f, 9.5f), 7, 0);
        GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);

        int randomMovement = Random.Range(0, 3);
        Enemy enemyScript = newEnemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.SetMovementType(randomMovement);

            int shieldChance = Random.Range(0, 10);
            if (shieldChance == 0)
            {
                enemyScript.EnableShield();
            }

            int avoidChance = Random.Range(0, 10);
            if (avoidChance < 2)
            {
                enemyScript.EnableLaserAvoidance();
            }
        }

        newEnemy.transform.parent = _enemyContainer.transform;

        _enemiesAlive++;
    }

    void SpawnBoss()
    {
        if (_bossPrefab != null)
        {
            Debug.Log("Spawning Boss!");
            Vector3 spawnPos = new Vector3(0, 7, 0);
            GameObject boss = Instantiate(_bossPrefab, spawnPos, Quaternion.identity);
            boss.transform.parent = _enemyContainer.transform;
            _enemiesAlive++;
            Debug.Log("Boss spawned at position: " + spawnPos + ", Enemies Alive: " + _enemiesAlive);
        }
        else
        {
            Debug.LogError("Boss Prefab is NULL!");
        }
    }

    public void EnemyDestroyed()
    {
        _enemiesAlive--;
    }

    IEnumerator SpawnPowerUpRoutine() {
        yield return new WaitForSeconds(3f);
        while (!_stopSpawning)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-9.5f, 9.5f), 7, 0);
            int randomPowerUps = GetWeightedRandomPowerUp();
            Instantiate(powerups[randomPowerUps], posToSpawn, Quaternion.identity);

            yield return new WaitForSeconds(Random.Range(4, 9));
        }
    }

    int GetWeightedRandomPowerUp()
    {
        int randomValue = Random.Range(0, 100);

        if (randomValue < 30)
            return 3;
        else if (randomValue < 48)
            return 0;
        else if (randomValue < 66)
            return 1;
        else if (randomValue < 78)
            return 2;
        else if (randomValue < 88)
            return 6;
        else if (randomValue < 93)
            return 4;
        else if (randomValue < 97)
            return 5;
        else
            return 7;
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                Destroy(enemies[i], 0.01f);
            }
        }
    }
}
