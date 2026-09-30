using UnityEngine;

public class EnemyTank : EnemyBase
{
    public override void ExecuteBehavior()
    {
        // Ignoriert Skelette komplett, läuft einfach stur zur Kirche
        MoveTowardsTarget(churchTransform);
        if (anim != null) anim.SetBool("isWalking", true);
    }
}
