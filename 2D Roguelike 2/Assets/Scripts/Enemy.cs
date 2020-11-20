using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MovingObject
{
    public int playerDamage;
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

    private Animator animator;
    private Transform target;
    private bool skipMove;

    
    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }

        base.AttemptMove<T>(xDir, yDir);

        skipMove = false;
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        float xDist = Math.Abs(target.position.x - transform.position.x);
        float yDist = Math.Abs(target.position.y - transform.position.y);

        //if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
        //    yDir = target.position.y > transform.position.y ? 1 : -1;
        //else
        //    xDir = target.position.x > transform.position.x ? 1 : -1;

        if (xDist > yDist)
            xDir = target.position.x > transform.position.x ? 1 : -1;
        else
            yDir = target.position.y > transform.position.y ? 1 : -1;

        AttemptMove<Player>(xDir, yDir);
    }

    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;

        animator.SetTrigger("enemyAttack");

        hitPlayer.LoseFood(playerDamage);

        SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
    }

    //public bool CheckRoom<T>(T component)
    //{
    //    Vector2 start = transform.position;
    //    Vector2 end = target.position;
    //    boxCollider.enabled = false;
    //    RaycastHit2D hit = Physics2D.Linecast(start, end, blockingLayer);
    //    boxCollider.enabled = true;

    //    OuterWall wall = hit.transform.GetComponent<OuterWall>();
    //}
}
