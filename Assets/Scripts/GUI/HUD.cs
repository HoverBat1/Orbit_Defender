using UnityEngine;
using System.Collections;
using System.Collections.Generic;







// ============================================================================================================
// ---------------------------------------------------------------------
public class HUD:MonoBehaviour 
{
	private int margin1 = 0;

	private GameManager gameManager;
	private Map map;
	private MainChar playerCharacter;
	private Shop shop;
	private OrbitalsManager orbitalsManager;
	private EnemyManager enemyManager;

    public Texture2D rectangleTexture;
    private Vector2 keycardDimensions = new Vector2(24,32);


	// ---------------------------------------------------------------------
	void Start() 
	{
		gameManager     = GameObject.Find("GameManager").GetComponent<GameManager>();
		map             = GameObject.Find("Map").GetComponent<Map>();
		playerCharacter = GameObject.Find("PlayerCharacter").GetComponent<MainChar>();
		shop            = GameObject.Find("Shop").GetComponent<Shop>();
		orbitalsManager = GameObject.Find("OrbitalsManager").GetComponent<OrbitalsManager>();
		enemyManager    = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
        //Debug.Log("orbitalsManager==null: "+orbitalsManager==null);

        rectangleTexture = new Texture2D(8,8);
	}
	







    // ============================================================================================================
    // ---------------------------------------------------------------------
	void Update() 
	{
        //
	}








    // ============================================================================================================
    // ---------------------------------------------------------------------
	void OnGUI()
	{
		string _str;
        float _x,_y, _width,_height;
        Color _color;

		// In case an enemy gets stuck
        _str = "DESTROY ALL ENEMIES";
        _width  = 9*_str.Length;
        _height = 30;
        _x = 16;
        _y = Screen.height-(_height+12);
		if (GUI.Button(new Rect(_x,_y, _width,_height), _str))
		{
			for(int i=0; i<enemyManager.enemies.Count; i++)
			{
				if (enemyManager.enemies[i] != null)
				{
					enemyManager.enemies[i].GetComponent<Enemy>().hp = 0;
					enemyManager.enemies[i].GetComponent<Enemy>().CheckIfDead();
				}
			}
		}


		// ---------------------------------------------------------------------
		if(!shop.usingShop)
		{
			// Raw Material Reserve
            _str = shop.rawMaterialReserve.ToString();
            _width = 120;
            _height = 30;
            _x = 42;
            _y = (int)(Screen.height*.4f);
			GUI.Label(new Rect(_x,_y, _width,_height), _str);

            // Raw Material Icon
            _width = _height*.6f;
            _height = _width;
            _x -= _width+(_width*.4f);
            _y += 1;
            _color = GUI.color;
            GUI.color = Color.cyan;
            GUI.DrawTexture(new Rect(_x,_y, _width,_height), rectangleTexture);
            GUI.color = _color;
			

            // Main Core HP
            _str = "CORE HP: " + playerCharacter.hp.ToString() + "/" + playerCharacter.hpMax.ToString();
            _width = 120;
            _height = 30;
            _x = 16;
            _y += 30;
			GUI.Label(new Rect(_x,_y, _width,_height), _str);
			/*
            Orbital _orbital = null;
            // Orbitals HP
			for(int i=0; i<orbitalsManager.orbitals.Length; i++)
			{
                for(int j=0; j<orbitalsManager.orbitals[i].Length; j++)
				{
					if (i == 0) margin1 = 0;
					else        margin1 = 74;

                    if (orbitalsManager.orbitals[i][j].GetType() == typeof(Orbital))
                    {
                        _orbital = orbitalsManager.orbitals[i][j].GetComponent<Orbital>();
                        if (_orbital != null)
                        {
                            _str  = "BOT "  +(_orbital.slot+1).ToString();
                            _str += " HP: " + _orbital.hp.ToString();
                            _str += "/"     + _orbital.hpMax.ToString();
                            GUI.Label(new Rect(10, 276 + ((20*_orbital.slot) + margin1), 120, 30), _str);
                        }
                    }
				}
			}
            */
		}


		// ---------------------------------------------------------------------
		// Floor number
        _str = "FLOOR: " + gameManager.gameLevel.ToString();
        _width = 120;
        _height = 30;
        _x = map.mapBackground_xy.x + 4;
        _y = map.mapBackground_xy.y - _height + 7;
		GUI.Label(new Rect(_x,_y, _width,_height), _str);


		// ---------------------------------------------------------------------
		// Keycard Icon. Visible if player has the keycard
        if (gameManager.hasKeycard)
        {
            _color = GUI.color;
            _x = (int)gameManager.map.mapBackground_xy.x + 96;
            _y = (int)gameManager.map.mapBackground_xy.y - ((keycardDimensions.y+6));
            GUI.color = Color.white;
            GUI.DrawTexture(new Rect(_x,_y, keycardDimensions.x,keycardDimensions.y), rectangleTexture);

            _x += (int)(keycardDimensions.x*.15f);
            GUI.color = Color.black;
            GUI.DrawTexture(new Rect(_x,_y, (int)(keycardDimensions.x*.2f),keycardDimensions.y), rectangleTexture);
            GUI.color = _color;
        }


		// ---------------------------------------------------------------------
		// Elevator (for moving to next floor)
		if(!shop.usingShop 
        &&  map.rooms[(int)gameManager.currentRoomRC.y][(int)gameManager.currentRoomRC.x].isElevator 
        && !gameManager.changingRoom )
		{
			GUI.Label(new Rect((Screen.width*.5f) - 60, 300, 120, 30), "ELEVATOR ROOM");
			GUI.Label(new Rect((Screen.width*.5f) - 120, 318, 240, 30), "(Keycard required to move to next floor)");
			
			//if (true) // testing
            if (gameManager.hasKeycard)
			{
				GUI.Box(new Rect(700, 600, 100, 86), "");
				GUI.Label(new Rect(706, 630, 100, 50), "Call elevator to move to next floor.");
				if (GUI.Button(new Rect(700, 600, 100, 30), "NEXT FLOOR"))
				{
					gameManager.ChangeFloors();
				}
			}
		}
	}
}
