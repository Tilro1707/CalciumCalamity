using UnityEngine;

public class SkeletonSword : SkeletonBase
{
    public float churchProtectionRadius = 10f; // Radar 1: Kirche gefährdet
    public float selfDetectionRadius = 5f;     // Radar 2: Mir selbst zu nah

    public float patrolRadius = 10f;           // Ring um die Kirche (9-11m)
    public float patrolSpeed = 1.5f;

    public override void ApplyElementMutation(string elementType)
    {
        // 1. Erst die Werte-Änderung aus deiner SkeletonBase ausführen (z.B. Erde HP-Bonus/Tempo-Malus)
        base.ApplyElementMutation(elementType);

    }

    public override void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        churchScript = FindFirstObjectByType<Church>();

        if (churchScript == null) return; // Sicherheitshalber abbrechen, falls keine Kirche da ist

        float minDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            // 1. Prüfe, ob der Gegner überhaupt im Schutzradius der Kirche ist
            float distanceToChurch = Vector2.Distance(churchScript.transform.position, enemy.transform.position);
            float distanceToMe = Vector2.Distance(transform.position, enemy.transform.position);

            if (distanceToChurch <= churchProtectionRadius || distanceToMe <= selfDetectionRadius)
            {
                if (distanceToMe < minDistance)
                {
                    minDistance = distanceToMe;
                    closestEnemy = enemy.transform;
                }

            }
        }
        targetEnemy = closestEnemy;
    }

    public override void NoTargetBehavior()
    {
        if (churchScript == null) return;

        // 1. Berechne den aktuellen Winkel basierend auf der Zeit und dem Tempo
        float angle = (Time.time * patrolSpeed) + transform.GetInstanceID(); ;

        // 2. Erstelle den Kreis-Offset (Nutze Mathf.Cos(angle) für X und Mathf.Sin(angle) für Y)
        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * patrolRadius;

        // 3. Berechne den finalen Zielpunkt im Raum (Kirchenposition + Offset)
        Vector3 targetPosition = churchScript.transform.position + offset;

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        if (anim != null) anim.SetBool("isWalking", true);
    }
}
