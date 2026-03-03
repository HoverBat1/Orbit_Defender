using UnityEngine;
using System.Collections;
using System;

public class Enemy:MonoBehaviour 
{
	public bool readyMove = false;
	private bool isPendingDeath = false;
	private bool damageFlash = false;
	public int HP;
    public int hp;
	public int dmg;
	public float moveSpeed;
	public float moveSpeedMax;
	private Vector3 moveToPoint;
	public Vector3 currentPos = new Vector3(0, 0, 0);
	public float bounce;
	public float force;
    private int frameCounter = 0;
    private int framesColliding = 0;
    private Color enemyColor;

	// Each orbital gets a timer where it cannot take damage from this enemy after taking damage from it
	private GameObject[][] orbitalsDelayDmgTimers;
	private GameObject mainCharDelayDmgTimer;

	private EnemyManager enemyManager;
	private MainChar playerCharacter;
	private Orbital orbital;
	private OrbitalsManager orbitalsMgr;

	
	void Start() 
	{
		enemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
		orbitalsMgr = GameObject.Find("OrbitalsManager").GetComponent<OrbitalsManager>();
        playerCharacter = GameObject.Find("PlayerCharacter").GetComponent<MainChar>();

		orbitalsDelayDmgTimers = new GameObject[orbitalsMgr.orbitals.Length][];
		for(int i=0; i<orbitalsMgr.orbitals.Length; i++) orbitalsDelayDmgTimers[i] = new GameObject[orbitalsMgr.orbitals[i].Length];

        enemyColor = GetComponent<Renderer>().material.color;

		moveToPoint = new Vector3(0, .15f, 0);
	}
	
	
	void Update()
    {
        frameCounter++;
        if (damageFlash)
		{
			int _FRAME = frameCounter&1;
            if (_FRAME != 0) GetComponent<Renderer>().material.color = Color.white;
            else             GetComponent<Renderer>().material.color = enemyColor;
		}
    }

    void FixedUpdate() 
	{
		if (readyMove)
		{
			transform.position = Vector3.MoveTowards(transform.position, moveToPoint, moveSpeed*Time.deltaTime);
			moveSpeed += moveSpeed*.02f;
		}
		moveSpeed = Math.Min(moveSpeed, moveSpeedMax);
		currentPos = transform.position;
	}

	public void Init()
	{
		StartCoroutine(TimerReadyMove());
	}

	private IEnumerator TimerReadyMove()
	{
		yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 1f));
		readyMove = true;
	}

	private IEnumerator TimerDmgInactive()
	{
		yield return new WaitForSeconds(.35f);
		GetComponent<Renderer>().material.shader = Shader.Find("Diffuse");
        GetComponent<Renderer>().material.color = enemyColor;
        damageFlash = false;
		readyMove = true;
	}
	

	private IEnumerator TimerDestroy()
	{
		yield return new WaitForSeconds(.5f);
		enemyManager.deadEnemyCount++;
		enemyManager.SpawnDrop(transform.position);
		Destroy(gameObject);
	}


	void OnCollisionEnter(Collision _OTHER)
	{
		Vector3 _direction = transform.position - _OTHER.transform.position;
		_direction.y = 0;

		if (_OTHER.transform.tag == "PlayerCharacter" 
        ||  _OTHER.transform.tag == "Orbital" )
		{
			if (_OTHER.transform.tag == "PlayerCharacter")
			{
				GetComponent<Rigidbody>().AddForce(_direction.normalized * playerCharacter.bounce);

				if (mainCharDelayDmgTimer == null)
				{
					playerCharacter.hp -= dmg;
					mainCharDelayDmgTimer = (GameObject)Instantiate(Resources.Load("Prevent Damage Timer"));
					mainCharDelayDmgTimer.GetComponent<PreventDmgTimer>().Init();
				}
			}
			else if (_OTHER.transform.tag == "Orbital")
			{
				orbital = _OTHER.gameObject.GetComponent<Orbital>();
				GetComponent<Rigidbody>().AddForce(_direction.normalized * orbital.bounce);

				for(int i=0; i<orbitalsMgr.orbitals.Length; i++)
				{
					for(int j=0; j<orbitalsMgr.orbitals[i].Length; j++)
					{
						if (orbitalsMgr.orbitals[i][j] == _OTHER.gameObject 
                        &&  orbitalsDelayDmgTimers[i][j] == null )
						{
							orbital.hp -= dmg;
							orbitalsDelayDmgTimers[i][j] = (GameObject)Instantiate(Resources.Load("Prevent Damage Timer"));
							orbitalsDelayDmgTimers[i][j].GetComponent<PreventDmgTimer>().Init();
						}
					}
				}
			}

			hp--;
			damageFlash = true;
			CheckIfDead();
			StartCoroutine(TimerDmgInactive());
		}
	}

    void OnCollisionStay(Collision _OTHER)
    {
        if (_OTHER.transform.tag == "PlayerCharacter" 
        ||  _OTHER.transform.tag == "Orbital" )
		{
            framesColliding++;
            if (framesColliding >= 120)
            {
                GetComponent<Rigidbody>().AddForce((transform.position-playerCharacter.transform.position).normalized * playerCharacter.bounce);
            }
        }
    }

    void OnCollisionExit(Collision _OTHER)
    {
        framesColliding = 0;
    }

	public void CheckIfDead()
	{
		if (hp <= 0 
        && !isPendingDeath )
		{
			isPendingDeath = true; // To prevent the enemy from spawning more than 1 drop
			dmg = 0;
			moveSpeed = 0;
			StartCoroutine(TimerDestroy());
		}
	}
}
