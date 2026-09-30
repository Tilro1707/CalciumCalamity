using UnityEngine;

public class EnemyRanger : EnemyBase
{
    [Header("Ranger Einstellungen")]
    public float detectionRadius = 8f;
    public float shootRange = 6f;
    public GameObject enemyArrowPrefab; // Hier ziehst du dein Projektil-Prefab rein

    public override void ExecuteBehavior()
    {
        currentTarget = GetClosestTarget(detectionRadius);

        if (currentTarget == null) return;

        float distanceToTarget = Vector2.Distance(transform.position, currentTarget.position);

        // Wenn er noch zu weit weg für einen Schuss ist, läuft er weiter ran
        if (distanceToTarget > shootRange)
        {
            MoveTowardsTarget(currentTarget);
        }
        else // In Schussreichweite -> Stehen bleiben und ballern!
        {
            if (Time.time >= nextAttackTime)
            {
                Shoot();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void Shoot()
    {
        if (currentTarget == null || enemyArrowPrefab == null) return;

        // Erstellt die Kugel/den Pfeil an der Position des Rangers
        GameObject projectile = Instantiate(enemyArrowPrefab, transform.position, Quaternion.identity);

        // Sucht nach deinem neuen, allgemeinen Projectile-Skript
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            // Übergibt das Ziel (Skelett oder Kirche) und den Schaden an das Projektil
            projectileScript.Setup(currentTarget, damage);
        }
    }
}