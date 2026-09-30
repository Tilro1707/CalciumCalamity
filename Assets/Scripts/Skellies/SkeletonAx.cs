using System;
using UnityEngine;

public class SkeletonAx : SkeletonBase
{
    private Vector2 patrolDirection = Vector2.right;
    
    public override void FindClosestEnemy()
    {
        GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");

        float minDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        foreach (GameObject enemy in enemys)
        {
            float currentDistance = Vector2.Distance(transform.position, enemy.transform.position);
            if(currentDistance < minDistance)
            {
                closestEnemy = enemy;
                minDistance = currentDistance;
            }
        }

        if (closestEnemy != null)
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
        transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + patrolDirection, speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (targetEnemy == null)
        {
            patrolDirection = -patrolDirection; // Richtung umdrehen bei Wand-Aufprall
        }
    }
}