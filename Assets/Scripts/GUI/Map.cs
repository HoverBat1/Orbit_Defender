using System;
using System.Collections.Generic;
using UnityEngine;








// ============================================================================================================
// ---------------------------------------------------------------------
public class Map:MonoBehaviour
{
    public const int ROOM_COUNT_BASE = 5;
    public int roomCount = ROOM_COUNT_BASE;

    public static Vector2 mapBackgroundSize = new Vector2(256, 256);
    public static Vector2 mapBackgroundMargin = new Vector2(16,16); // Distance from screen edge
    public static float mapBackgroundPadding = 16; // Pad between background edge and floor grid
    public Vector2 mapBackground_xy = new Vector2(Screen.width-mapBackgroundSize.x-mapBackgroundMargin.x, Screen.height-mapBackgroundSize.y-mapBackgroundMargin.y);

    private static float mapRoomMargin = 2; // Distance between each room
    private static float mapRoomSize = mapRoomMargin * 4;
    private float mapFloorScale = 1; // If floor is too big to fit in 'mapBackgroundSize', this will scale it down

    private Vector2 mapFloorSize = new Vector2(mapBackgroundSize.x-(mapBackgroundPadding*2), mapBackgroundSize.y-(mapBackgroundPadding*2));

    public int floorClms = 0;
    public int floorRows = 0;
    public int clmVisitedRight = 0; //  right-most room visited on current floor
    public int clmVisitedLeft  = 0; //   left-most room visited on current floor
    public int rowVisitedDown  = 0; // bottom-most room visited on current floor
    public int rowVisitedUp    = 0; //    top-most room visited on current floor
    public List<List<RoomData>> rooms;
    
    public Vector2 elevatorRoomRC; // RC: Row Clm
    private GameManager gameManager;

    public Texture2D rectangleTexture;








    // ---------------------------------------------------------------------
    void Start()
    {
        rooms = new List<List<RoomData>>();
        elevatorRoomRC = new Vector2(0,0);
        rectangleTexture = new Texture2D((int)mapRoomSize,(int)mapRoomSize);
    }


    public void Init()
    {
        // This is here because GameManager spawns Map
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }








    // ============================================================================================================
    // ---------------------------------------------------------------------
    void Update()
    {
        
    }



    // ============================================================================================================
    // ---------------------------------------------------------------------
	void OnGUI()
	{
        if(!gameManager.shop.usingShop)
        {
            GUI.color = Color.black;
            GUI.Box(new Rect(mapBackground_xy.x,mapBackground_xy.y, mapBackgroundSize.x,mapBackgroundSize.y), "");
            
            if (rooms.Count != 0)
            {
                float _x,_y;
                float _distance = 0;
                Color _color = GUI.color;
                int _clms = Math.Clamp((clmVisitedRight-clmVisitedLeft)+1, 1, floorClms);
                int _rows = Math.Clamp((rowVisitedDown-rowVisitedUp)+1,    1, floorRows);
                mapFloorScale = ((mapRoomSize+mapRoomMargin)*Math.Max(floorClms,floorRows)) - mapRoomMargin;
                mapFloorScale = mapFloorSize.x / mapFloorScale; // This assumes the floor-grid is square
                //mapFloorScale = 1; // testing

                float _roomSize = Math.Clamp(mapRoomSize*mapFloorScale, 6f, 24f);
                float _roomMargin = mapRoomMargin*(_roomSize/mapRoomSize);
                float _roomDistance = _roomSize+_roomMargin;

                _distance = (_roomDistance*_clms)-_roomMargin;
                float _mapFloor_x = mapBackground_xy.x + ((mapBackgroundSize.x-_distance)/2);
                _mapFloor_x = Math.Clamp(_mapFloor_x, mapBackground_xy.x, (mapBackground_xy.x+mapBackgroundSize.x)-_distance);

                _distance = (_roomDistance*_rows)-_roomMargin;
                float _mapFloor_y = mapBackground_xy.y + ((mapBackgroundSize.y-_distance)/2);
                _mapFloor_y = Math.Clamp(_mapFloor_y, mapBackground_xy.y, (mapBackground_xy.y+mapBackgroundSize.y)-_distance);

                
                _distance = (mapBackgroundSize.x - ((floorClms*_roomDistance)-_roomMargin)) / 2;
                _mapFloor_x = mapBackground_xy.x + _distance;
                _distance = (mapBackgroundSize.y - ((floorRows*_roomDistance)-_roomMargin)) / 2;
                _mapFloor_y = mapBackground_xy.y + _distance;
                for(int i=rooms.Count-1; i>=0; i--)
                {
                    for(int j=rooms[i].Count-1; j>=0; j--)
                    {
                        _x = _mapFloor_x + (_roomDistance*j);
                        _y = _mapFloor_y + (_roomDistance*i);
                        GUI.color = Color.black;
                        // For testing, to see the full floor grid
                        //GUI.DrawTexture(new Rect(_x,_y, _roomSize,_roomSize), rectangleTexture);

                        if (rooms[i][j].isRoom 
                        &&  rooms[i][j].visited )
                        {
                            if (rooms[i][j].isElevator) GUI.color = Color.yellow;
                            else                        GUI.color = Color.grey;
                            _x = _mapFloor_x + (_roomDistance*j);
                            _y = _mapFloor_y + (_roomDistance*i);
                            // Draw a room on the map
                            GUI.DrawTexture(new Rect(_x,_y, _roomSize,_roomSize), rectangleTexture);

                            if (i == gameManager.currentRoomRC.y 
                            &&  j == gameManager.currentRoomRC.x )
                            {
                                float _SIZE = _roomSize*.5f;
                                _distance = (_roomSize-_SIZE) / 2;
                                _x += _distance;
                                _y += _distance;
                                GUI.color = Color.white;
                                // Indicate which room the player is in
                                GUI.DrawTexture(new Rect(_x,_y, _SIZE,_SIZE), rectangleTexture);
                            }
                        }
                    }
                }

                GUI.color = _color;
            }
        }
    }








    // ============================================================================================================
    // ---------------------------------------------------------------------
    public void CreateNewFloor(int _FLOOR_NUMBER)
    {
        int _idx, _count;
        int _clm,_row, _room0Idx,_room1Idx;
        bool _roomAdded = false;
        bool _mustAddRoom = false;
        List<Vector2> _endRoomsRC = new List<Vector2>();
        List<int> _directions = new List<int>();
        List<int> _roomsToProcess = new List<int>();
        List<List<int>> _roomLayout = new List<List<int>>();
        for(int i=rooms.Count-1; i>=0; i--)
        {
            for(int j=rooms[i].Count-1; j>=0; j--)
            {
                Destroy(rooms[i][j]);
            }
        }
        rooms.Clear();


        _count = Math.Min(8, 3+(_FLOOR_NUMBER>>3));
        _count = UnityEngine.Random.Range(0, _count); // extra rooms
        roomCount = ROOM_COUNT_BASE+(_FLOOR_NUMBER>>1)+_count;
        roomCount = Math.Min(32, roomCount);
        roomCount++; // For elevator room (floor start room)
        int _roomsLeftToAdd = roomCount;

        // Make floor grid big enough to handle any room layout
        _count = roomCount-1; // remove elevator room
        _count = _count + 1 + _count; // +1 is elevator room (floor start room)
        // Floor grid is always square
        floorClms = _count;
        floorRows = _count;

        // '_roomLayout' will be used later in this function to build 'rooms'
        // It's values are; 0: No room, 1: Normal room, 2: Elevator room
        for(int i=0; i<floorRows; i++)
        {
            _roomLayout.Add(new List<int>());
            for(int j=0; j<floorClms; j++)
            {
                _roomLayout[i].Add(0);
            }
        }

        // Set elevator room
        _clm = floorClms>>1;
        _row = floorRows>>1;
        _roomLayout[_row][_clm] = 2;
        _idx = (_row*floorClms) + _clm;
        _roomsLeftToAdd--;
        _roomsToProcess.Add(_idx);

        while (_roomsToProcess.Count > 0)
        {
            _roomAdded = false;
            _idx = UnityEngine.Random.Range(0, _roomsToProcess.Count);
            _room0Idx = _roomsToProcess[_idx];
            _roomsToProcess.RemoveAt(_idx);


            // Determine available directions to add more rooms
            _directions.Clear();
            _clm = _room0Idx%floorClms;
            _row = _room0Idx/floorClms;

            // RIGHT
            if (_clm+1<floorClms && _roomLayout[_row+0][_clm+1]==0   // right
            &&  _row-1>=0        && _roomLayout[_row-1][_clm+1]==0   // upper right
            &&  _row+1<floorRows && _roomLayout[_row+1][_clm+1]==0 ) // bottom right
            {
                // Also make sure 1 more room-slot further doesn't already have a room
                if (_clm+2 >= floorClms 
                ||  _roomLayout[_row][_clm+2] == 0 )
                {
                    _directions.Add(Other.Constants.direction_RIGHT);
                }
            }

            // LEFT
            if (_clm-1>=0        && _roomLayout[_row+0][_clm-1]==0   // left
            &&  _row-1>=0        && _roomLayout[_row-1][_clm-1]==0   // upper left
            &&  _row+1<floorRows && _roomLayout[_row+1][_clm-1]==0 ) // bottom left
            {
                // Also make sure 1 more room-slot further doesn't already have a room
                if (_clm-2 < 0 
                ||  _roomLayout[_row][_clm-2] == 0 )
                {
                    _directions.Add(Other.Constants.direction_LEFT);
                }
            }
            
            // DOWN
            if (_row+1<floorRows && _roomLayout[_row+1][_clm+0]==0   // below
            &&  _clm-1>=0        && _roomLayout[_row+1][_clm-1]==0   // bottom left
            &&  _clm+1<floorClms && _roomLayout[_row+1][_clm+1]==0 ) // bottom right
            {
                // Also make sure 1 more room-slot further doesn't already have a room
                if (_row+2 >= floorRows 
                ||  _roomLayout[_row+2][_clm] == 0 )
                {
                    _directions.Add(Other.Constants.direction_DOWN);
                }
            }

            // UP
            if (_row-1>=0        && _roomLayout[_row-1][_clm+0]==0   // above
            &&  _clm-1>=0        && _roomLayout[_row-1][_clm-1]==0   // upper left
            &&  _clm+1<floorClms && _roomLayout[_row-1][_clm+1]==0 ) // upper right
            {
                // Also make sure 1 more room-slot further doesn't already have a room
                if (_row-2 < 0 
                ||  _roomLayout[_row-2][_clm] == 0 )
                {
                    _directions.Add(Other.Constants.direction_UP);
                }
            }

            //Debug.Log("_directions.Count="+_directions.Count);
            if (_directions.Count > 0) // if any directions haven't already been used
            {
                // In case rng doesn't add a room but more still need to be added
                _mustAddRoom = _roomsToProcess.Count==0 && _roomsLeftToAdd>0;

                // Shuffle the available directions for better variation
                Other.Various.Shuffle(_directions);

                _count = _directions.Count;
                for(int i=0; i<_count; i++)
                {
                    if (_roomsLeftToAdd <= 0)
                    {
                        break;//i
                    }

                    if (UnityEngine.Random.Range(0,5) < 3  // 'Random.Range(0,5)<3': 60% chance to add a room
                    ||  _mustAddRoom )
                    {
                        _room1Idx = -1;
                        switch(_directions[i]){
                        case Other.Constants.direction_RIGHT:{_room1Idx=_room0Idx+1;         break;}
                        case Other.Constants.direction_LEFT: {_room1Idx=_room0Idx-1;         break;}
                        case Other.Constants.direction_DOWN: {_room1Idx=_room0Idx+floorClms; break;}
                        case Other.Constants.direction_UP:   {_room1Idx=_room0Idx-floorClms; break;}
                        }//switch(_directions[i])

                        if (_room1Idx != -1)
                        {
                            _roomAdded = true;
                            _mustAddRoom = false;
                            _roomLayout[_room1Idx/floorClms][_room1Idx%floorClms] = 1;
                            _roomsToProcess.Add(_room1Idx);
                            _roomsLeftToAdd--;
                        }
                    }
                }
            }

            if(!_roomAdded 
            &&  _roomLayout[_row][_clm] != 2 )
            {
                _endRoomsRC.Add(new Vector2(_clm,_row));
            }
        }

        


        // ------------------------------------------------------------------------------
        // The following lines are for truncating any excess clms and rows
        int _clmLeft   = floorClms-1;
        int _clmRight  = 0;
        int _rowTop    = floorRows-1;
        int _rowBottom = 0;

        // Set _clmLeft,_clmRight, _rowTop,_rowBottom
        for(int i=0; i<floorRows; i++)
        {
            for(int j=0; j<floorClms; j++)
            {
                if (_roomLayout[i][j] > 0)
                {
                    if (j < _clmLeft)  _clmLeft  = j;
                    if (j > _clmRight) _clmRight = j;

                    if (i < _rowTop)    _rowTop    = i;
                    if (i > _rowBottom) _rowBottom = i;
                }
            }
        }

        int _clms = (_clmRight-_clmLeft)+1;
        int _rows = (_rowBottom-_rowTop)+1;
        
        // The new floor-grid will be square so _LENGTH is its clm AND row count
        int _LENGTH = Math.Max(_clms,_rows);

        // Since the new floor-grid is square and _clms and _rows may not be equal, this will center the rooms w/in the new floor-grid
        int _excessClms = 0;
        int _excessRows = 0;
        _count = _LENGTH - Math.Min(_clms,_rows);
        if (_count > 1)
        {
            if (_clms < _rows) _excessClms = _count>>1;
            else               _excessRows = _count>>1;
        }

        rooms.Clear();
        for(int i=0; i<_LENGTH; i++)
        {
            rooms.Add(new List<RoomData>());
            for(int j=0; j<_LENGTH; j++)
            {
                rooms[i].Add(gameObject.AddComponent<RoomData>());
                rooms[i][j].roomRC.x = j;
                rooms[i][j].roomRC.y = i;
                rooms[i][j].visited = false;
                rooms[i][j].completed = false;
                //rooms[i][j].completed = true; // testing
            }
        }

        Debug.Log("floorClms="+floorClms+", _LENGTH="+_LENGTH+", _clmLeft="+_clmLeft+", _clmRight="+_clmRight+", _rowTop="+_rowTop+", _rowBottom="+_rowBottom+", _clms="+_clms+", _rows="+_rows);
        for(int i=0; i<floorRows; i++)
        {
            for(int j=0; j<floorClms; j++)
            {
                if (_roomLayout[i][j] > 0)
                {
                    _row = (i-_rowTop)  + (_excessRows>>1);
                    _clm = (j-_clmLeft) + (_excessClms>>1);

                    rooms[_row][_clm].isRoom = true;
                    if (_roomLayout[i][j] == 2)
                    {
                        elevatorRoomRC.x = _clm;
                        elevatorRoomRC.y = _row;
                        rooms[_row][_clm].isElevator = true;
                        rooms[_row][_clm].completed = true;
                    }

                    for(int k=_endRoomsRC.Count-1; k>=0; k--)
                    {
                        if (_endRoomsRC[k].x == j 
                        &&  _endRoomsRC[k].y == i )
                        {
                            rooms[_row][_clm].isEndRoom = true;
                        }
                    }
                }
            }
        }
        Debug.Log("elevatorRoomRC.x="+elevatorRoomRC.x+", elevatorRoomRC.y="+elevatorRoomRC.y);

        floorClms = _LENGTH;
        floorRows = _LENGTH;
    }


    public void UpdateRoomVisitedProps()
    {
        clmVisitedRight = 0;
        clmVisitedLeft  = floorClms-1;
        rowVisitedDown  = 0;
        rowVisitedUp    = floorRows-1;

        for(int i=rooms.Count-1; i>=0; i--)
        {
            for(int j=rooms[i].Count-1; j>=0; j--)
            {
                if (rooms[i][j].isRoom 
                &&  rooms[i][j].visited )
                {
                    if (j > clmVisitedRight) clmVisitedRight = j;
                    if (j < clmVisitedLeft)  clmVisitedLeft  = j;
                    if (i > rowVisitedDown)  rowVisitedDown  = i;
                    if (i < rowVisitedUp)    rowVisitedUp    = i;
                }
            }
        }
    }
}
