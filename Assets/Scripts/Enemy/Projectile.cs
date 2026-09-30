using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 12f;
    private int damage;
    private Transform target;

    public void Setup(Transform enemyTarget, int rangerDamage)
    {
        target = enemyTarget;
        damage = rangerDamage;
        Destroy(gameObject, 4f);
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < 0.7f)
        {
            // Fall A: Kugel trifft einen Gegner
            EnemyBase enemyScript = target.GetComponent<EnemyBase>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }

            // Fall B: Kugel trifft ein Skelett
            SkeletonBase skeletonScript = target.GetComponent<SkeletonBase>();
            if (skeletonScript != null)
            {
                skeletonScript.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }

            // Fall C: Kugel trifft die Kirche (falls gegnerische Ranger darauf schieﬂen)
            Church churchScript = target.GetComponent<Church>();
            if (churchScript != null)
            {
                churchScript.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }

            Destroy(gameObject);
        }
    }
}
