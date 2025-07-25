using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public Transform[] spawnPoints; // Array of spawn points
    public GameObject enemyPrefab; // Enemy to spawn
    public float timeBetweenWaves = 5f; // Time delay between wave

    public PlayerHealth playerHealth; // Reference to the PlayerHealth script

    private int currentWave = 1; 
    private int enemiesToSpawn; // Number of enemies to spawn in the current wave
    private List<GameObject> activeEnemies = new List<GameObject>(); // List to track active enemies

    void Start()
    {
        // Coroutine to spawn waves of enemies
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        while (true) // Infinite loop for continuous waves
        {
            enemiesToSpawn = GetEnemiesForWave(currentWave); // Get the number of enemies for the current wave
            Debug.Log("Starting Wave " + currentWave);

            
            for (int i = 0; i < enemiesToSpawn; i++) // Spawn all enemies for the current wave
            {
                SpawnNewEnemy();
                yield return new WaitForSeconds(0.5f); // Small delay between enemy spawns
            }

            // Wait until all enemies are destroyed
            yield return new WaitUntil(() => activeEnemies.Count == 0);

            Debug.Log("Wave " + currentWave + " completed. Waiting for next wave...");
            yield return new WaitForSeconds(timeBetweenWaves);

            currentWave++; 
            resetPlayerHealth(); // Reset player health after each wave
        }
    }

    void SpawnNewEnemy()
    {
        // Spawn an enemy at a random spawn point
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
        activeEnemies.Add(newEnemy); // Add the spawned enemy to the activeEnemies list

        // Subscribe to the enemy's death event
        EnemyAi enemyAi = newEnemy.GetComponent<EnemyAi>();
        if (enemyAi != null)
        {
            enemyAi.OnEnemyDeath += HandleEnemyDeath;
        }
    }

    void HandleEnemyDeath(GameObject enemy)
    {
        // Remove the enemy from the activeEnemies list when it dies
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
    }

    int GetEnemiesForWave(int wave)
    {
        // Define the number of enemies for each wave
        if (wave == 1) return 1;
        if (wave == 2) return 2;
        if (wave == 3) return 5;

        // For next waves, just double the number of enemies
        return wave * 2; // Example: Wave 4 spawns 8 enemies, Wave 5 spawns 10, etc.
    }

    void resetPlayerHealth()
    {
        // Reset player health to maximum when all enemies are defeated
        playerHealth.currentHealth = playerHealth.maxHealth;
        
    }
}