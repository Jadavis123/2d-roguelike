/*
 * Enemy.cs - controls both enemy prefabs
 * 
 * Alek DeMaio, Doug McIntyre, Inaya Alkhatib, JD Davis, June Tejada
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class Enemy : MovingObject
{
    public int playerDamage;
    public int hp = 3;
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;
    public AudioClip chopSound1;
    public AudioClip chopSound2;

    public GameObject[] itemTiles;

    private Animator animator;
    private Transform target;
    private bool skipMove;


    protected override void Start() //sets up animation, selects player target, adds to list of enemies in game manager
    {
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    protected override void AttemptMove<T>(int xDir, int yDir) //in earlier versions, allowed for enemies to move every other turn rather than every turn
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }

        base.AttemptMove<T>(xDir, yDir);

        skipMove = false;
    }


    public void MoveEnemy() //checks if distance to player is greater in x or y direction, then moves in the appropriate direction
    {
        if (hp > 0)
        {
            int xDir = 0;
            int yDir = 0;

        float xDist = Math.Abs(target.position.x - transform.position.x);
        float yDist = Math.Abs(target.position.y - transform.position.y);

            if (xDist > yDist)
                xDir = target.position.x > transform.position.x ? 1 : -1;
            else
                yDir = target.position.y > transform.position.y ? 1 : -1;

            AttemptMove<Player>(xDir, yDir);
        }

    }

    public void DamageEnemy(int loss) //removes given amount of health from enemy, then destroys it and drops an item if health goes below 0
    {
        SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);
        hp -= loss;
        if (hp <= 0)
        {
            float x = target.position.x;
            float y = target.position.y;
            gameObject.SetActive(false);
            //Destroy(this.gameObject);
            Debug.Log("Enemy Destroyed! at (" + x + ", " + y + ")");
            Debug.Log("Putting food at (" + x + ", " + y + ")");
            //put a transform into foodTiles here
            //get tile enemy is on and place item there
            //GameObject[] foodTiles = GameManager.instance.boardScript.foodTiles;
            GameObject tileChoice = itemTiles[Random.Range(0, itemTiles.Length)];
            Instantiate(tileChoice, new Vector3(x, y, 0f), Quaternion.identity);

        }
    }


    protected override void OnCantMove<T>(T component) //overrides MovingObject abstract method - if collision is with player, removes health from player and plays attack animation/sound
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
