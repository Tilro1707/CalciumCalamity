using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 12f;
    private int damage;
    private Transform target;

    // Wird vom Ranger aufgerufen, wenn er den Pfeil instanziiert
    public void Setup(Transform enemyTarget, int rangerDamage)
    {
        target = enemyTarget;
        damage = rangerDamage;
        Destroy(gameObject, 4f); // Sicherheitshalber nach 4 Sek zerstören
    }

    void Update()
    {
        if(target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 richtung = (target.position - transform.position).normalized;
        float winkel = Mathf.Atan2(richtung.y, richtung.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, winkel);

        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if(Vector2.Distance(transform.position, target.position) < 0.7f)
        {
            EnemyBase enemyScript = target.GetComponent<EnemyBase>();
            if(enemyScript != null)
            {
                enemyScript.TakeDamage(damage);
                Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject, 4f);
            }
        }
    }
}
