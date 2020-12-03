using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{
    [SerializeField] private FieldOfView fieldOfView;

    public int enemyDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public int maxHealth = 100;
    public int playerHealth = 50;
    public float restartLevelDelay = 1f;
    public Text foodText;
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    private Animator animator;
    private int food;
    private float viewDistance;
    //private int playerDamage;

    // Start is called before the first frame update
    protected override void Start()
    {
        animator = GetComponent<Animator>();

        fieldOfView = GameObject.Find("FieldOfView").GetComponent<FieldOfView>();

        playerHealth = GameManager.instance.playerHealthPoints;

        viewDistance = GameManager.instance.playerLight;

        fieldOfView.viewDistance = viewDistance;

        foodText.text = healthString();

        base.Start();
    }

    private void OnDisable()
    {
        GameManager.instance.playerHealthPoints = playerHealth;
        viewDistance = fieldOfView.viewDistance;
        GameManager.instance.playerLight = viewDistance;
    }

    // Update is called once per frame
    void Update()
    {
        fieldOfView.SetOrigin(transform.position);
        if (!GameManager.instance.playersTurn) return;

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
            vertical = 0;

        if (horizontal != 0 || vertical != 0)
            AttemptMove<Enemy>(horizontal, vertical);
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        //food--;
        foodText.text = healthString();

        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit1, hit2;

        if (Move(xDir, yDir, out hit1, out hit2))
        {
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }

        CheckIfGameOver();

        GameManager.instance.playersTurn = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit")
        {
            Destroy(other);
            enabled = false;
            Invoke("Restart", restartLevelDelay);
        }
        else if (other.tag == "Food")
        {
            playerHealth += pointsPerFood;
            if (playerHealth > maxHealth)
            {
                playerHealth = maxHealth;
            }
            foodText.text = "+" + pointsPerFood + " " + healthString();
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Soda")
        {
            playerHealth += pointsPerSoda;
            if (playerHealth > maxHealth)
            {
                playerHealth = maxHealth;
            }
            foodText.text = "+" + pointsPerSoda + " " + healthString();
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Light")
        {
            UnityEngine.Debug.Log("light item pickup");
            fieldOfView.viewDistance += 1f;
            viewDistance += 1f;
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Damage")
        {
            UnityEngine.Debug.Log("Damage Item pickedup");
            enemyDamage += 1;
            other.gameObject.SetActive(false);
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        //Wall hitWall = component as Wall;
        //hitWall.DamageWall(wallDamage);
        //animator.SetTrigger("playerChop");

        Enemy hitEnemy = component as Enemy;
        hitEnemy.DamageEnemy(enemyDamage);
        animator.SetTrigger("playerChop");
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        playerHealth -= loss;
        foodText.text = "-" + loss + " " + healthString();
        CheckIfGameOver();
    }

    public string healthString() {
      return "Health: " + playerHealth + "/" + maxHealth;
    }

    private void CheckIfGameOver()
    {
        if (playerHealth <= 0)
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();

        }
    }
}