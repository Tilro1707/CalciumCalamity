using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Basis Einstellungen")]
    public int maxHP = 2;
    public int currentHP;
    public float speed = 1.5f;
    public int damage = 5;
    public int soulReward = 1;

    [Header("Angriffs Einstellungen")]
    public float attackRate = 1f;
    protected float nextAttackTime;

    [Header("Status-Effekte")]
    protected bool isStunned = false;
    protected float currentSpeed;
    [Header("DoT Variablen")]
    public bool isBurning = false;
    private float fireDamagePerSecond = 1f;
    private float burnTimer = 0f;
    private float burnAccumulator = 0f;

    protected SpriteRenderer rend;
    protected Animator anim;
    protected Transform churchTransform;
    protected Church churchScript;
    protected Transform currentTarget;

    private Coroutine effectCoroutine;

    protected virtual void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        churchScript = FindFirstObjectByType<Church>();
        if (churchScript != null) churchTransform = churchScript.transform;
        currentHP = maxHP;
        currentSpeed = speed;
    }

    void Update()
    {
        if (churchTransform == null) return;

        // Burning / DoT (sauberer Accumulator statt int-cast pro Frame)
        if (isBurning)
        {
            burnTimer += Time.deltaTime;
            burnAccumulator += fireDamagePerSecond * Time.deltaTime;

            int damageThisFrame = Mathf.FloorToInt(burnAccumulator);
            if (damageThisFrame > 0)
            {
                currentHP -= damageThisFrame;
                burnAccumulator -= damageThisFrame;

                if (currentHP <= 0)
                {
                    if (churchScript != null) churchScript.souls += soulReward;
                    Destroy(gameObject);
                    return;
                }
            }

            if (burnTimer >= 3f)
            {
                isBurning = false;
                burnTimer = 0f;
                burnAccumulator = 0f;
            }
        }

        // Nur einmaliges Verhalten pro Frame und nur wenn nicht gestunned
        if (!isStunned)
        {
            ExecuteBehavior();
        }
    }

    public void StartBurning() { isBurning = true; burnTimer = 0f; burnAccumulator = 0f; }

    // Wird von den Kindern überschrieben
    public virtual void ExecuteBehavior()
    {
        MoveTowardsTarget(churchTransform);
    }

    protected void MoveTowardsTarget(Transform target)
    {
        bool nachLinksLaufen = target.position.x < transform.position.x;
        if (target != null)
        {
            rend.flipX = nachLinksLaufen;
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            if (anim != null) anim.SetBool("isWalking", true);
        }
    }

    // Berechnet, wer näher ist: Ein Skelett (im Radius) oder die Kirche
    protected Transform GetClosestTarget(float detectionRadius)
    {
        if (churchTransform == null) return null;

        float distanceToChurch = Vector2.Distance(transform.position, churchTransform.position);
        GameObject[] skeletons = GameObject.FindGameObjectsWithTag("Skeleton");

        Transform closestSkeleton = null;
        float minSkeletonDistance = Mathf.Infinity;

        foreach (GameObject skelly in skeletons)
        {
            float distanceToSkelly = Vector2.Distance(transform.position, skelly.transform.position);
            if (distanceToSkelly < minSkeletonDistance)
            {
                minSkeletonDistance = distanceToSkelly;
                closestSkeleton = skelly.transform;
            }
        }

        // Wenn ein Skelett da ist, im Radius liegt UND näher als die Kirche ist -> Ziel wechseln!
        if (closestSkeleton != null && minSkeletonDistance <= detectionRadius && minSkeletonDistance < distanceToChurch)
        {
            return closestSkeleton;
        }

        return churchTransform;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHP -= damageAmount;
        if (currentHP <= 0)
        {
            speed = 0f;
            gameObject.tag = "Untagged";
            if (churchScript != null) churchScript.souls += soulReward;
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayRandomMonsterSound();
            }
            if (anim != null) anim.SetTrigger("Death");
            StartCoroutine(DeathRoutine());
        }
    }

    private System.Collections.IEnumerator DeathRoutine()
    {
        currentHP = 100;
        yield return new WaitForSecondsRealtime(1);
        Destroy(gameObject);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Church"))
        {
            if (anim != null) anim.SetTrigger("Attack");
            if (anim != null) anim.SetBool("isWalking", false);
            if (churchScript != null) churchScript.TakeDamage(damage);
        }
    }

    public void ApplyEffect(string element, float duration)
    {
        // Nur die aktive Effect-Coroutine stoppen, nicht alles
        if (effectCoroutine != null) StopCoroutine(effectCoroutine);
        effectCoroutine = StartCoroutine(EffectRoutine(element, duration));
    }

    private System.Collections.IEnumerator EffectRoutine(string element, float duration)
    {
        if (element == "Ice")
        {
            speed = currentSpeed * 0.4f; // Verlangsamen
            yield return new WaitForSeconds(duration);
            speed = currentSpeed; // Reset
        }
        else if (element == "Lightning")
        {
            isStunned = true;
            speed = 0f;
            yield return new WaitForSeconds(duration);
            isStunned = false;
            speed = currentSpeed; // Reset auf Referenzwert
        }
    }
}