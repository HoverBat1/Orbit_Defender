using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager:MonoBehaviour
{
    //public bool isKeycardRoom = false;
	public bool allEnemiesDead = false;
	public int deadEnemyCount = 0;
    private bool enemiesSpawned = false;

    private GameManager gameManager;

    public List<GameObject> enemies;
    public List<GameObject> enemyDrops;
    public int dropsAquiredCount = 0;

    private MainChar playerCharacter;


    void Start()
    {
        enemies = new List<GameObject>();
		enemyDrops = new List<GameObject>();
    }


    public void Init()
    {
        // This is here because GameManager spawns EnemyManager
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerCharacter = GameObject.Find("PlayerCharacter").GetComponent<MainChar>();
    }








    // ============================================================================================================
    // ---------------------------------------------------------------------
    void Update()
    {
        if (enemiesSpawned 
        && !gameManager.map.rooms[(int)gameManager.currentRoomRC.y][(int)gameManager.currentRoomRC.x].completed)
        {
            if (GameObject.FindObjectOfType<Enemy>() == null 
            &&  GameObject.FindObjectOfType<RawMaterialUnit>() == null )
            {
                gameManager.map.rooms[(int)gameManager.currentRoomRC.y][(int)gameManager.currentRoomRC.x].completed = true;
                for(int i=0; i<gameManager.doors.Count; i++) gameManager.doors[i].GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
                enemies.Clear();
                enemyDrops.Clear();
                enemiesSpawned = false;
            }
        }
    }








    // ============================================================================================================
    // ---------------------------------------------------------------------
    public void SpawnDrop(Vector3 _POSITION)
	{
		if (gameManager.currentRoomRC == gameManager.keycardRoomRC 
        && !gameManager.hasKeycard 
        &&  GameObject.FindObjectOfType<Keycard>() == null )
		{
			enemyDrops.Add((GameObject)Instantiate(Resources.Load("Keycard")));
			enemyDrops[enemyDrops.Count-1].transform.position = _POSITION;
		}
		else
		{
			enemyDrops.Add((GameObject)Instantiate(Resources.Load("Raw Material Unit")));
			enemyDrops[enemyDrops.Count-1].transform.position = _POSITION;
		}

		if (deadEnemyCount == enemies.Count)
		{
			deadEnemyCount = 0;
			enemies.Clear();
			allEnemiesDead = true;
		}
	}

	public void CheckIfAllDropsAquired()
	{
		if (dropsAquiredCount == enemyDrops.Count)
		{
			dropsAquiredCount = 0;
			enemyDrops.Clear();
			allEnemiesDead = false;
			gameManager.map.rooms[(int)gameManager.currentRoomRC.y][(int)gameManager.currentRoomRC.x].completed = true;
			for(int i=0; i<gameManager.doors.Count; i++) gameManager.doors[i].GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
		}
	}

    public void SpawnEnemies(int _ENEMY_COUNT)
	{
		Vector3 _spawnPoint;
        enemiesSpawned = _ENEMY_COUNT>0;
        enemies.Clear();
        enemyDrops.Clear();

        for(int i=0; i<_ENEMY_COUNT; i++)
		{
            switch(Random.Range(0,4)){
            default:{_spawnPoint = new Vector3( 9, .15f, (Random.Range(-9, 9))); break;} // RIGHT
            case  1:{_spawnPoint = new Vector3(-9, .15f, (Random.Range(-9, 9))); break;} // LEFT
            case  2:{_spawnPoint = new Vector3((Random.Range(-9, 9)), .15f, -9); break;} // DOWN
            case  3:{_spawnPoint = new Vector3((Random.Range(-9, 9)), .15f,  9); break;} // UP
            }

            enemies.Add((GameObject)Instantiate(Resources.Load("Enemy")));
            int _HP = Random.Range(0,3) + 1;
			enemies[i].gameObject.GetComponent<Enemy>().HP = _HP;
            enemies[i].gameObject.GetComponent<Enemy>().hp = _HP;
			enemies[i].gameObject.GetComponent<Enemy>().dmg = 1;
			enemies[i].gameObject.GetComponent<Enemy>().moveSpeed = .5f;
			enemies[i].gameObject.GetComponent<Enemy>().moveSpeedMax = 3;
			enemies[i].gameObject.GetComponent<Enemy>().bounce = gameManager.playerCharacter.bounce;
			enemies[i].gameObject.GetComponent<Enemy>().transform.localScale = new Vector3(.3f, .3f, .3f);
			enemies[i].gameObject.GetComponent<Enemy>().transform.position = _spawnPoint;
			enemies[i].gameObject.GetComponent<Enemy>().Init();
		}
	}
}
