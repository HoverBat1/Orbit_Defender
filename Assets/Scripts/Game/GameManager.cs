using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;








// ----------------------------------------------------------------
public class GameManager:MonoBehaviour 
{
	public bool changingRoom = false;
	public bool blockShop = false;
	public bool hasKeycard;

	public int gameLevel = 0;

    private int[,] moveToPositions = new int[,] {{-20, 0}, {20, 0}, {0, 20}, {0, -20}}; // LEFT, RIGHT, UP, DOWN
	private float[,] doorPositions = new float[,] {{9.75f, 0}, {-9.75f, 0}, {0, -9.75f}, {0, 9.75f}}; // RIGHT, LEFT, DOWN , UP
    private float[,] floorPositions = new float[,] {{20, 0}, {-20, 0}, {0, -20}, {0, 20}};  // RIGHT, LEFT, DOWN , UP
	private float roomYPos;
	private float roomMoveSpeed = 2;
	private Vector3 moveToCoords;

    public Vector2 currentRoomRC = new Vector2(0,0);
    public Vector2 keycardRoomRC = new Vector2(-1,-1);
	private List<GameObject> newRoom;
	private List<GameObject> currentRoom;
	public List<GameObject> doors;

	public Shop shop;
	public Map map;
	public MainChar playerCharacter;
    public EnemyManager enemyManager;

	public Color color;


	// ----------------------------------------------------------------
	void Start() 
	{
		hasKeycard = false;

        shop = GameObject.Find("Shop").GetComponent<Shop>();

		map = GameObject.Find("Map").GetComponent<Map>();
        map.Init();
        
		playerCharacter = GameObject.Find("PlayerCharacter").GetComponent<MainChar>();
		playerCharacter.Init();
        
        currentRoom = new List<GameObject>();
		currentRoom.Add((GameObject)Instantiate(Resources.Load("Room2")));
		roomYPos = currentRoom[0].transform.position.y;
		ChangeRoomColor(currentRoom[0], .8f, .8f, .8f);
		
        newRoom = new List<GameObject>();
		newRoom.Add(currentRoom[0]);
		
		doors = new List<GameObject>();

		enemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
        enemyManager.Init();

        ChangeFloors();
	}
    

    // ----------------------------------------------------------------
    void Update() 
	{
		if(!changingRoom)
		{
            // Check for a user requested room change
			int _clm = (int)currentRoomRC.x;
            int _row = (int)currentRoomRC.y;
            if(!shop.usingShop 
            &&  map.rooms[_row][_clm].completed )
			{
                if (Input.GetKey(KeyCode.RightArrow) 
                &&  _clm+1 < map.floorClms 
                &&  map.rooms[_row][_clm+1].isRoom )
                {
                    MoveRoomsPrep(Other.Constants.direction_RIGHT);
                }
                else if (Input.GetKey(KeyCode.LeftArrow) 
                &&  _clm-1 >= 0 
                &&  map.rooms[_row][_clm-1].isRoom )
                {
                    MoveRoomsPrep(Other.Constants.direction_LEFT);
                }
                else if (Input.GetKey(KeyCode.DownArrow) 
                &&  _row+1 < map.floorRows 
                &&  map.rooms[_row+1][_clm].isRoom )
                {
                    MoveRoomsPrep(Other.Constants.direction_DOWN);
                }
                else if (Input.GetKey(KeyCode.UpArrow) 
                &&  _row-1 >= 0 
                &&  map.rooms[_row-1][_clm].isRoom )
                {
                    MoveRoomsPrep(Other.Constants.direction_UP);
                }
			}
		}
		else
		{
            // Update movement to next room
			currentRoom[0].transform.position = Vector3.MoveTowards(currentRoom[0].transform.position, moveToCoords, roomMoveSpeed*Time.deltaTime);
			    newRoom[0].transform.position = Vector3.MoveTowards(    newRoom[0].transform.position, moveToCoords, roomMoveSpeed*Time.deltaTime);
			//
            roomMoveSpeed = Math.Min(roomMoveSpeed+.2f, 20);
            //roomMoveSpeed = Math.Min(roomMoveSpeed+.2f, 10);
			
			if (currentRoom[0].transform.position == moveToCoords)
			{
				// Finished moving to new room.
                roomMoveSpeed = 2;
                Destroy(currentRoom[0]);
                currentRoom[0] = newRoom[0];
                ArriveRoom();
                changingRoom = false;
			}
		}

        if (changingRoom 
        || !map.rooms[(int)currentRoomRC.y][(int)currentRoomRC.x].completed )
        {
            blockShop = true;
        }
		else
        {
            blockShop = false;
        }
	}


	// ----------------------------------------------------------------
	private void MoveRoomsPrep(uint _DIRECTION)
	{
        switch(_DIRECTION){
        case Other.Constants.direction_RIGHT:{currentRoomRC.x++; break;}
        case Other.Constants.direction_LEFT: {currentRoomRC.x--; break;}
        case Other.Constants.direction_DOWN: {currentRoomRC.y++; break;}
        case Other.Constants.direction_UP:   {currentRoomRC.y--; break;}
        }
        map.rooms[(int)currentRoomRC.y][(int)currentRoomRC.x].visited = true;
		CreateRoom(floorPositions[_DIRECTION,0], floorPositions[_DIRECTION,1]);
		moveToCoords = new Vector3(moveToPositions[_DIRECTION,0], roomYPos, moveToPositions[_DIRECTION,1]);
		RemoveDoors();
		WallCollideToggle(false);
		changingRoom = true;
	}
	
	private void CreateRoom(float _X_POS, float _Z_POS)
	{
		newRoom[0] = (GameObject)Instantiate(Resources.Load("Room2"));
        newRoom[0].transform.position = new Vector3(_X_POS, roomYPos, _Z_POS);
		if (map.rooms[(int)currentRoomRC.y][(int)currentRoomRC.x].isElevator) ChangeRoomColor(newRoom[0],   .8f,   .8f,  .8f);
		else                                                                  ChangeRoomColor(newRoom[0], .773f, .757f, .59f);
	}

	private void ChangeRoomColor(GameObject _ROOM, float _R, float _G, float _B)
	{
		_ROOM.GetComponent<Renderer>().material.color = new Color(_R*.5f, _G*.5f, _B*.5f, 1);
		for(int i=0; i<4; i++) _ROOM.transform.GetChild(i).GetComponent<Renderer>().material.color = new Color(_R, _G, _B, 1);
	}

	public void AddDoors()
	{
        bool _isInGrid = false;
        for(int i=0; i<4; i++)
		{
            _isInGrid = false;
            Vector2 _roomRC = currentRoomRC;
            switch(i){
            case 0:{_isInGrid=currentRoomRC.x+1 < map.floorClms;  _roomRC.x++; break;} // RIGHT
            case 1:{_isInGrid=currentRoomRC.x-1 >= 0;             _roomRC.x--; break;} // LEFT
            case 2:{_isInGrid=currentRoomRC.y+1 < map.floorRows;  _roomRC.y++; break;} // DOWN
            case 3:{_isInGrid=currentRoomRC.y-1 >= 0;             _roomRC.y--; break;} // UP
            }

            if (_isInGrid 
            &&  map.rooms[(int)_roomRC.y][(int)_roomRC.x].isRoom )
			{
				doors.Add(GameObject.CreatePrimitive(PrimitiveType.Cube));
				doors[doors.Count-1].transform.localScale = new Vector3(1.8f, 2.8f, 1.8f);
				doors[doors.Count-1].transform.position   = new Vector3(0+doorPositions[i,0], 1.4f, 0+doorPositions[i,1]);
				if (map.rooms[(int)currentRoomRC.y][(int)currentRoomRC.x].completed) doors[doors.Count-1].GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
				else                                                                 doors[doors.Count-1].GetComponent<Renderer>().material.color = new Color(1, 0, 0, 1);
			}
		}
	}

	public void RemoveDoors()
	{
		for(int i=0; i<doors.Count; i++) Destroy(doors[i]);
		doors.Clear();
	}

	private void WallCollideToggle(bool _TOGGLE)
	{
		for(int i=0; i<4; i++)
		{
			    newRoom[0].transform.GetChild(i).GetComponent<Collider>().enabled = _TOGGLE;
			currentRoom[0].transform.GetChild(i).GetComponent<Collider>().enabled = _TOGGLE;
		}
	}


    public void ChangeFloors()
    {
        int _idx, _count;
        List<Vector2> _keycardRoomPossibilities = new List<Vector2>();

        RemoveDoors();
        gameLevel++;
        map.CreateNewFloor(gameLevel);
        currentRoomRC.x = map.elevatorRoomRC.x;
        currentRoomRC.y = map.elevatorRoomRC.y;
        hasKeycard = false;
        //hasKeycard = true; // testing

        // Set enemy and keycardRoom data
        int _ENEMY_COUNT_MAX = (gameLevel>>1) + 2;
        //int _ENEMY_COUNT_MAX = gameLevel + 2;
        keycardRoomRC.x = -1;
        keycardRoomRC.y = -1;
        for(int i=0; i<map.floorRows; i++)
        {
            for(int j=0; j<map.floorClms; j++)
            {
                //Debug.Log("i="+i+" j="+j+", map.rooms[i][j].isRoom="+map.rooms[i][j].isRoom+", map.rooms[i][j].isElevator="+map.rooms[i][j].isElevator);
                if (map.rooms[i][j].isRoom 
                && !map.rooms[i][j].isElevator )
                {
                    _count = UnityEngine.Random.Range(0,_ENEMY_COUNT_MAX);
                    map.rooms[i][j].enemyCount = _count;
                    //Debug.Log("i="+i+" j="+j+", map.rooms[i][j].enemyCount="+map.rooms[i][j].enemyCount);
                    if (_count == 0 
                    &&  UnityEngine.Random.Range(0,2) == 0 )
                    {
                        _count = 1;
                    }

                    if (_count > 0)
                    {
                        map.rooms[i][j].enemyCount = _count;
                        if (map.rooms[i][j].isEndRoom)
                        {
                            _keycardRoomPossibilities.Add(new Vector2(j,i));
                        }
                    }
                    else
                    {
                        map.rooms[i][j].completed = true;
                    }
                }
            }
        }

        if (_keycardRoomPossibilities.Count > 0)
        {
            _idx = UnityEngine.Random.Range(0,_keycardRoomPossibilities.Count);
            keycardRoomRC = _keycardRoomPossibilities[_idx];
            Debug.Log("keycardRoomRC="+keycardRoomRC);
        }


        ArriveRoom();
    }

    private void ArriveRoom()
    {
        int _CLM = (int)currentRoomRC.x;
        int _ROW = (int)currentRoomRC.y;
        WallCollideToggle(true);
        AddDoors();
        map.rooms[_ROW][_CLM].visited = true;
        map.UpdateRoomVisitedProps();

        //Debug.Log("ArriveRoom"+", map.rooms[_ROW][_CLM].completed="+map.rooms[_ROW][_CLM].completed+", map.rooms[_ROW][_CLM].enemyCount="+map.rooms[_ROW][_CLM].enemyCount);
        if (map.rooms[_ROW][_CLM].enemyCount > 0 
        && !map.rooms[_ROW][_CLM].completed )
        {
        	enemyManager.SpawnEnemies(map.rooms[_ROW][_CLM].enemyCount);
        }
    }
}
