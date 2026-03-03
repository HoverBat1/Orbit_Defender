using UnityEngine;








// ============================================================================================================
// ---------------------------------------------------------------------
public class OrbitalsManager:MonoBehaviour 
{
	public int botsOnRingCount = 0;
	private float[] ringRadius = new float[2] {.6f, 1.6f};
	private float[] radiusSpeed = new float[2] {1, 2};
	public float[][] baseOrbitSpeed = new float[2][];

	public float averagedOrbitSpeed = 0;

	private Vector3 botSize;
	private Vector3 spawnPos;

	private float deg = 0;
	private float accumulatedAngle = 0;
	
	public GameObject[][] orbitals = new GameObject[2][];

	
	void Start() 
	{
		orbitals[0] = new GameObject[3]; // creates an array of 'GameObject's in orbitals[0]
		orbitals[1] = new GameObject[5]; // creates an array of 'GameObject's in orbitals[1]
        //Debug.Log(orbitals);
		baseOrbitSpeed[0] = new float[2] {80, 40};
		baseOrbitSpeed[1] = new float[2] {40, 20};
	}








	// ============================================================================================================
    // ---------------------------------------------------------------------
	void Update() 
	{

	}








    // ---------------------------------------------------------------------
	public void CreateOrbital(int _RING_IDX, int _SLOT_IDX, string _SIZE)
	{
		orbitals[_RING_IDX][_SLOT_IDX] = (GameObject)Instantiate(Resources.Load("Orbital"));

		if (_SIZE == "Small")
		{
			botSize = new Vector3(.4f, .4f, .4f);
			SetProperties(_RING_IDX, _SLOT_IDX, baseOrbitSpeed[_RING_IDX][0], 20, 1, 300);
		}
		else if (_SIZE == "Large")
		{
			botSize = new Vector3(.55f, .55f, .55f);
			SetProperties(_RING_IDX, _SLOT_IDX, baseOrbitSpeed[_RING_IDX][1], 12, 3, 300);
		}

		orbitals[_RING_IDX][_SLOT_IDX].transform.localScale = botSize;
		orbitals[_RING_IDX][_SLOT_IDX].gameObject.GetComponent<Orbital>().radiusSpeedBase = radiusSpeed[_RING_IDX];
		orbitals[_RING_IDX][_SLOT_IDX].gameObject.GetComponent<Orbital>().size = _SIZE;
		orbitals[_RING_IDX][_SLOT_IDX].gameObject.GetComponent<Orbital>().special = "None";
		orbitals[_RING_IDX][_SLOT_IDX].gameObject.GetComponent<Orbital>().ring = _RING_IDX;
		orbitals[_RING_IDX][_SLOT_IDX].gameObject.GetComponent<Orbital>().slot = _SLOT_IDX;


		
		for(int i=0; i<orbitals[_RING_IDX].Length; i++)
		{
			if (orbitals[_RING_IDX][i] != null)
			{
				botsOnRingCount++;
				averagedOrbitSpeed += orbitals[_RING_IDX][i].GetComponent<Orbital>().baseOrbitSpeed;
			}
		}

		averagedOrbitSpeed /= botsOnRingCount;
		deg = 360 / botsOnRingCount;

		for(int i=0; i<orbitals[_RING_IDX].Length; i++)
		{
			if (orbitals[_RING_IDX][i] != null)
			{
				orbitals[_RING_IDX][i].GetComponent<Orbital>().orbitSpeed = averagedOrbitSpeed;
				spawnPos = new Vector3(Mathf.Cos(accumulatedAngle*Mathf.Deg2Rad) * ringRadius[_RING_IDX], orbitals[_RING_IDX][i].transform.localScale.y*.5f, Mathf.Sin(accumulatedAngle*Mathf.Deg2Rad) * ringRadius[_RING_IDX]);
				if (orbitals[_RING_IDX][i].gameObject.GetComponent<Orbital>().ready) orbitals[_RING_IDX][i].gameObject.GetComponent<Orbital>().orbitPoint.transform.position = spawnPos;
				else                                                                 orbitals[_RING_IDX][_SLOT_IDX].transform.position = spawnPos;
				accumulatedAngle += deg;
			}
		}

		// Ring bonus
		if (botsOnRingCount == orbitals[_RING_IDX].Length)
		{
			for(int i=0; i<orbitals[_RING_IDX].Length; i++)
			{
				if (orbitals[_RING_IDX][i].GetComponent<Orbital>().size == "Large")
                {
                    i = orbitals[_RING_IDX].Length;
                }
				else if (i == orbitals[_RING_IDX].Length-1)
				{
					for(int j=0; j<orbitals[_RING_IDX].Length; j++)
                    {
                        orbitals[_RING_IDX][j].GetComponent<Orbital>().orbitSpeed = 90; // Extra speed bonus for all being Small
                    }
				}
			}
		}

		orbitals[_RING_IDX][_SLOT_IDX].gameObject.GetComponent<Orbital>().Init();
		orbitals[_RING_IDX][_SLOT_IDX].gameObject.GetComponent<Orbital>().ready = true;

		botsOnRingCount = 0;
		averagedOrbitSpeed = 0;
		accumulatedAngle = 0;
		deg = 0;

		/*
		float distanceBetweenObjects = 50.0; // or however far apart you want them to be
		float distanceFromCenter = (distanceBetweenObjects*0.5)/cos(a*0.5);
		float accumulatedAngle = 0.0;
		foreach( object )
		{
			object.position.x = cos(accumulatedAngle) * distanceFromCenter;
			object.position.y = sin(accumulatedAngle) * distanceFromCenter;
			accumulatedAngle += a;
		}
		*/
	}


    // ---------------------------------------------------------------------
	private void SetProperties(int _RING_IDX, int _SLOT_IDX, float _BaseOrbitSpeed, float _HP_MAX, int _DAMAGE, float _BOUNCE)
	{
		orbitals[_RING_IDX][_SLOT_IDX].gameObject.GetComponent<Orbital>().baseOrbitSpeed = _BaseOrbitSpeed;
		orbitals[_RING_IDX][_SLOT_IDX].gameObject.GetComponent<Orbital>().hpMax = _HP_MAX;
		orbitals[_RING_IDX][_SLOT_IDX].gameObject.GetComponent<Orbital>().damage = _DAMAGE;
		orbitals[_RING_IDX][_SLOT_IDX].gameObject.GetComponent<Orbital>().bounce = _BOUNCE;
	}


    // ---------------------------------------------------------------------
	private void AddRingBonus()
	{

	}


    // ---------------------------------------------------------------------
	public void AverageOrbitSpeed(int _RING_IDX)
	{
		for(int i=0; i<orbitals[_RING_IDX].Length; i++)
		{
			if (orbitals[_RING_IDX][i] != null)
			{
				botsOnRingCount++;
				averagedOrbitSpeed += orbitals[_RING_IDX][i].GetComponent<Orbital>().baseOrbitSpeed;
			}
		}

		averagedOrbitSpeed /= botsOnRingCount;

		for(int i=0; i<orbitals[_RING_IDX].Length; i++)
		{
			if (orbitals[_RING_IDX][i] != null) orbitals[_RING_IDX][i].GetComponent<Orbital>().orbitSpeed = averagedOrbitSpeed;
		}

		averagedOrbitSpeed = 0;
		botsOnRingCount = 0;
	}
}
