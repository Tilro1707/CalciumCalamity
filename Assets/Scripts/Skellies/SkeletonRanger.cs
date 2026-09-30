using UnityEngine;

public class SkeletonRanger : SkeletonBase
{
    public float rangerDetectionRadius = 12f;
    public float patrolRadius = 6f; // Engerer Kreis als das Schwert
    public float patrolSpeed = 1f;

    [Header("Ranger Setup")]
    public GameObject arrowPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        attackRange = rangerDetectionRadius;
    }

    public override void FindClosestEnemy()
    {
        GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");

        churchScript = FindFirstObjectByType<Church>();

        float minDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        foreach (GameObject enemy in enemys)
        {
            float currentDistance = Vector2.Distance(transform.position, enemy.transform.position);
            if (currentDistance < minDistance)
            {
                closestEnemy = enemy;
                minDistance = currentDistance;
            }
        }

        if (closestEnemy != null && minDistance <= rangerDetectionRadius)
        {
            targetEnemy = closestEnemy.transform;
        }
        else
        {
            targetEnemy = null;
        }
    }

    public override void NoTargetBehavior()
    {
        if (churchScript == null) return;

        // 1. Berechne den aktuellen Winkel basierend auf der Zeit und dem Tempo
        float angle = (Time.time * patrolSpeed) + transform.GetInstanceID();

        // 2. Erstelle den Kreis-Offset (Nutze Mathf.Cos(angle) für X und Mathf.Sin(angle) für Y)
        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * patrolRadius;

        // 3. Berechne den finalen Zielpunkt im Raum (Kirchenposition + Offset)
        Vector3 targetPosition = churchScript.transform.position + offset;

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    public override void Attack()
    {
        if (anim != null) anim.SetTrigger("Attack");
        if (targetEnemy == null || arrowPrefab == null) return;

        GameObject projectile = Instantiate(arrowPrefab, transform.position, Quaternion.identity);

        Arrow arrowScript = projectile.GetComponent<Arrow>();

        if (arrowScript != null)
        {
            arrowScript.Setup(targetEnemy, damage);
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayRandomArrowSound();
            }
        }
    }
}