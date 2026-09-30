using UnityEngine;

public class SkeletonBase : MonoBehaviour
{
    protected Animator anim;
    protected SpriteRenderer rend;

    // Standard-Werte für jedes Skelett
    public float speed = 2f;
    public int damage = 1;
    public float attackRange = 1;
    public float attackRate = 1f;
    public float maxHP = 5;
    public float currentHP;

    private float retargetCooldown = 0.5f;
    private float retargetTimer;
    private float nextAttackTime;

    protected Transform targetEnemy;
    protected Church churchScript;

    [Header("Elementar-Status")]
    public string currentElement = ""; // "Fire", "Ice" oder "" (normal)

    protected virtual void Start()
    {
        currentHP = maxHP;
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>(); // Wichtig, um das Skelett zu spiegeln und zu färben!
    }

    private void Update()
    {
        // 1. Gegner suchen
        retargetTimer += Time.deltaTime;
        if (retargetTimer >= retargetCooldown)
        {
            FindClosestEnemy();
            retargetTimer = 0f;
        }

        // 2. Bewegen oder Angreifen
        if (targetEnemy != null)
        {
            float currentDistance = Vector2.Distance(transform.position, targetEnemy.position);

            if (currentDistance > attackRange)
            {
                // BEWEGUNG NACH LINKS ODER RECHTS MESSEN:
                // Wenn das Ziel links von uns ist, ist der X-Unterschied negativ.
                bool nachLinksLaufen = targetEnemy.position.x < transform.position.x;

                // Hier spiegeln wir das Skelett einfach direkt!
                rend.flipX = nachLinksLaufen;

                // Eigentliche Bewegung
                transform.position = Vector2.MoveTowards(transform.position, targetEnemy.position, speed * Time.deltaTime);

                // Dem Animator sagen: Ich laufe!
                if (anim != null) anim.SetBool("isWalking", true);
            }
            else
            {
                // Wenn wir nah genug dran sind: Stehenbleiben und Schlagen
                if (anim != null) anim.SetBool("isWalking", false);

                if (Time.time >= nextAttackTime)
                {
                    Attack();
                    nextAttackTime = Time.time + 1f / attackRate;
                }
            }
        }
        else
        {
            // Kein Gegner da? Zur Kirche laufen
            NoTargetBehavior();
        }
    }

    public virtual void FindClosestEnemy() { }

    public virtual void NoTargetBehavior()
    {
        if (churchScript == null) churchScript = FindFirstObjectByType<Church>();

        if (churchScript != null)
        {
            // Auch auf dem Weg zur Kirche richtig herum umdrehen
            rend.flipX = churchScript.transform.position.x < transform.position.x;

            transform.position = Vector2.MoveTowards(transform.position, churchScript.transform.position, speed * Time.deltaTime);
            if (anim != null) anim.SetBool("isWalking", true);
        }
        else
        {
            if (anim != null) anim.SetBool("isWalking", false);
        }
    }

    public virtual void Attack()
    {
        if (targetEnemy == null) return;

        // Spielt die Angriffs-Animation ab
        if (anim != null) anim.SetTrigger("Attack");

        EnemyBase enemyScript = targetEnemy.GetComponent<EnemyBase>();
        if (enemyScript != null)
        {
            enemyScript.TakeDamage(damage);
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayRandomAttackSound();
            }
            // NEU: Mutation-Effekt übertragen
            if (currentElement == "Ice") enemyScript.ApplyEffect("Ice", 2f); // 2 Sek Eis
            if (currentElement == "Lightning") enemyScript.ApplyEffect("Lightning", 0.5f); // 0.5 Sek Stun
            if (currentElement == "Fire") enemyScript.StartBurning();        
        }
    }

    // DIE SIMPELSTE MUTATION DER WELT:
    // Wir ändern einfach nur die Werte und färben das Skelett in Unity ein!
    public virtual void ApplyElementMutation(string elementType)
    {
        currentElement = elementType;

        switch (currentElement)
        {
            case "Fire":
                damage += 3;
                rend.color = Color.red;
                break;

            case "Ice":
                speed *= 0.9f;
                rend.color = Color.cyan;
                break;

            case "Earth":
                // Earth: HP +50%, Speed -30%
                maxHP *= 1.5f;
                currentHP = maxHP; // Skelett sofort heilen bei Mutation
                speed *= 0.7f;
                rend.color = new Color(0.5f, 0.3f, 0.1f); // Braun
                break;

            case "Lightning":
                // Lightning: Kurz stunnen (Logik in EnemyBase nötig)
                damage += 1;
                rend.color = Color.yellow;
                break;
        }
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayRandomMutationsSound();
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHP -= damageAmount;
        if (currentHP <= 0)
        {
            gameObject.tag = "Untagged";
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

    void OnDestroy()
        {
            PlayerController.currentSkeletonCount--;
    }
}