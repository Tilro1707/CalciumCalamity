using UnityEngine;

public class EnemySpam : EnemyBase
{
    [Header("Spam Einstellungen")]
    public float detectionRadius = 4f;
    public float meleeAttackRange = 1.2f;

    public override void ExecuteBehavior()
    {
        currentTarget = GetClosestTarget(detectionRadius);

        if (currentTarget == null) return;

        float distanceToTarget = Vector2.Distance(transform.position, currentTarget.position);

        // Wenn noch zu weit weg, laufen
        if (distanceToTarget > meleeAttackRange)
        {
            MoveTowardsTarget(currentTarget);
        }
        else // Im Nahkampfbereich angekommen -> Angreifen!
        {
            if (Time.time >= nextAttackTime)
            {
                AttackTarget();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void AttackTarget()
    {
        if (currentTarget == null) return;

        // Falls das Ziel ein Skelett ist
        if (currentTarget.CompareTag("Skeleton"))
        {
            SkeletonBase skeleton = currentTarget.GetComponent<SkeletonBase>();
            if (skeleton != null) skeleton.TakeDamage(damage);
        }
        // Falls das Ziel die Kirche ist
        else if (currentTarget.CompareTag("Church"))
        {
            if (churchScript != null) churchScript.TakeDamage(damage);
        }
    }
}