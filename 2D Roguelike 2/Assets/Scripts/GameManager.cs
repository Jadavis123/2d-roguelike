/*
 * GameManager.cs - top level script that controls player, board manager, and enemies
 * 
 * Alek DeMaio, Doug McIntyre, Inaya Alkhatib, JD Davis, June Tejada
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;
    public float turnDelay = .05f;
    public static GameManager instance = null;
    public BoardManager boardScript;
    public float playerLight = 5f;
    public int playerHealthPoints = 100;
    public int playerDamage = 1;
    [HideInInspector] public bool playersTurn = true;

    private Text levelText;
    private GameObject levelImage;
    private int level = 0;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup;

    // Start is called before the first frame update
    void Awake() //sets up single instance of GameManager, as well as list of enemies on the board and a board manager
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        //InitGame();
    }

    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    //static public void CallbackInitialization()
    //{
    //    SceneManager.sceneLoaded += OnSceneLoaded;
    //}

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene arg0, LoadSceneMode arg1) //together with OnEnable and OnDisable, increases level and calls InitGame when old level is destroyed and new scene is loaded
    {
        instance.level++;
        instance.InitGame();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //private void OnLevelWasLoaded(int index)
    //{
    //    level++;
    //    Debug.Log("Level " + level + " loaded");

    //    InitGame();
    //}

    void InitGame() //displays the level screen while level is loading, destroys old enemies, calls SetupScene in BoardManager
    {
        doingSetup = true;

        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Level " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();
        boardScript.SetupScene(level);
    }

    private void HideLevelImage() //hides level screen and sets doingSetup flag false so that Update() knows setup is finished
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    public void GameOver() //displays screen showing how far the player got when they die, then disables the GameManager object
    {
        levelText.text = "After " + level + " days, you starved.";
        levelImage.SetActive(true);
        enabled = false;
    }

    // Update is called once per frame
    void Update() //calls MoveEnemies coroutine unless it is the player's turn, the enemies are already moving, or the level is being loaded
    {
        if (playersTurn || enemiesMoving || doingSetup)
            return;

        StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    IEnumerator MoveEnemies() //if there are enemies present, runs for loop for each enemy, sets playersTurn and enemiesMoving flags appropriately
    {
        if (this.gameObject != null)
        {
            enemiesMoving = true;
            yield return new WaitForSeconds(turnDelay);
            if (enemies.Count == 0)
            {
                yield return new WaitForSeconds(turnDelay);
            }

        for (int i = 0; i < enemies.Count; i++) //for each enemy, if they are in the same room as the player, calls that enemy's MoveEnemy()
        {
            if (enemies[i].CheckRoom().Equals(GameObject.Find("Player").GetComponent<Player>().CheckRoom()))
            {
                enemies[i].MoveEnemy();
                UnityEngine.Debug.Log("Enemy " + i + " moving");
                yield return new WaitForSeconds(enemies[i].moveTime);
            }       
        }

            playersTurn = true;
            enemiesMoving = false;
        }
    }
}