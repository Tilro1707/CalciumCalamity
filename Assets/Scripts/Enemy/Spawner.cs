using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject spamPrefab;
    public GameObject tankPrefab;
    public GameObject rangerPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 3f;

    private float spawnTimer;
    private float gameStageTimer;

    void Update()
    {
        gameStageTimer += Time.deltaTime;
        spawnTimer += Time.deltaTime;

        if (gameStageTimer >= 300) spawnInterval = 1f;
        else if (gameStageTimer >= 270) spawnInterval = 1.5f;
        else if (gameStageTimer >= 210) spawnInterval = 1.75f;
        else if (gameStageTimer >= 150) spawnInterval = 2f;
        else if (gameStageTimer >= 90) spawnInterval = 2.25f;
        else if (gameStageTimer >= 30) spawnInterval = 2.5f;
        else spawnInterval = 3f;

        if (spawnTimer >= spawnInterval)
        {
            // 3. Wenn ja, übergeben wir die passenden Prozent-Raten je nach Zeitstufe!
            if (gameStageTimer >= 300)
            {
                SpawnEnemy(50, 30, 20); // 50% Tank, 30% Ranger, 20% Spam
            }
            else if (gameStageTimer >= 240)
            {
                SpawnEnemy(25, 25, 50);  // 0% Tank, 10% Ranger, 90% Spam
            }
            else if (gameStageTimer >= 180)
            {
                SpawnEnemy(10, 20, 70);  // 0% Tank, 10% Ranger, 90% Spam
            }
            else if (gameStageTimer >= 120)
            {
                SpawnEnemy(5, 15, 80);  // 0% Tank, 10% Ranger, 90% Spam
            }
            else if (gameStageTimer >= 60)
            {
                SpawnEnemy(0, 10, 90);  // 0% Tank, 10% Ranger, 90% Spam
            }
            else if (gameStageTimer >= 30)
            {
                SpawnEnemy(0, 5, 95);   // 0% Tank, 5% Ranger, 95% Spam
            }
            else
            {
                SpawnEnemy(0, 0, 100);  // Ganz am Anfang: 100% nur Spam-Gegner!
            }

            spawnTimer = 0f; // Timer zurücksetzen nicht vergessen
        }
    }

    // Wir sagen der Funktion jetzt beim Aufrufen, wie hoch die Chancen GERADE sind
    void SpawnEnemy(int tankChance, int rangerChance, int SpamChance)
    {
        if (spawnPoints.Length == 0) return;

        int randomIndex = Random.Range(0, spawnPoints.Length);
        Vector3 spawnPos = spawnPoints[randomIndex].position;

        GameObject prefabToSpawn = spamPrefab; // Standard-Rückfalloption

        int chance = Random.Range(0, 100); // Würfelt von 0 bis 99

        // Das Glücksrad verteilt sich dynamisch nach deinen Variablen!
        if (chance < tankChance)
        {
            prefabToSpawn = tankPrefab;
        }
        // Wenn die gewürfelte Zahl hinter der Tank-Chance, aber noch im Ranger-Bereich liegt:
        else if (chance < (tankChance + rangerChance))
        {
            prefabToSpawn = rangerPrefab;
        }
        else
        {
            prefabToSpawn = spamPrefab;
        }

        Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
    }
}