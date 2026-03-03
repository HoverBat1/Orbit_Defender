using UnityEngine;
using System.Collections;
using System;








// ============================================================================================================
// ---------------------------------------------------------------------
public class Keycard:MonoBehaviour 
{
	public bool ready = false;
	private float moveSpeed = 0;
	private Vector3 moveToPoint;

	private EnemyManager enemyManager;


    // ---------------------------------------------------------------------
	void Start () 
	{
		enemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
        transform.localScale = new Vector3 (.5f, .5f, .5f);
		moveToPoint = new Vector3 (0, .15f, 0);
	}








	// ============================================================================================================
    // ---------------------------------------------------------------------
	void Update() 
	{
		transform.position = Vector3.MoveTowards(transform.position, moveToPoint, moveSpeed*Time.deltaTime);

		if (enemyManager.allEnemiesDead) StartCoroutine(TimerReady());

		if (ready)
		{
			if (moveSpeed == 0) moveSpeed = .1f;
			moveSpeed += moveSpeed*.2f;
		}
		else
        {
            moveSpeed -= moveSpeed*.2f;
        }
		
        moveSpeed = Math.Min(moveSpeed, 6);
		//if (moveSpeed >= 6) moveSpeed = 6;
	}


    // ---------------------------------------------------------------------
	void OnCollisionEnter(Collision _OTHER)
	{
		if (_OTHER.transform.tag == "PlayerCharacter")
		{
			enemyManager.dropsAquiredCount++;
			enemyManager.CheckIfAllDropsAquired();
			Destroy(gameObject);
		}
	}


    // ---------------------------------------------------------------------
	private IEnumerator TimerReady()
	{
		yield return new WaitForSeconds(.5f);
		ready = true;
	}
}
