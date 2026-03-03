using UnityEngine;








// ============================================================================================================
// ---------------------------------------------------------------------
public class Orbital:MonoBehaviour 
{
	public bool ready = false;
	private bool canTakeDmg = true;
	public int level;
	public int nextLevelCost = 50;
	public float hp;
	public float hpMax;
	public int damage;
	public string special;
	public string size;
	public int ring;
	public int slot;

	private float radiusSpeed;
	public float radiusSpeedBase;
	public float orbitSpeed;
	public float baseOrbitSpeed;
	public float bounce;
	private float test1;
	private Transform hpIndicator;
	public Vector3 desiredPos;
	public Vector3 newPos;

	private Vector3 targetDir;
	private Vector3 forward;
	private float angle;

	public GameObject orbitPoint;

	private OrbitalsManager orbitalsMgr;
	private GameObject playerCharacter;
	private Enemy enemy;
	private Shop shop;


	// ---------------------------------------------------------------------
	void Start() 
	{
		
	}

	public void Init()
	{
		level = 1;
		hp = hpMax;

		orbitalsMgr = GameObject.Find("OrbitalsManager").GetComponent<OrbitalsManager>();
		
		orbitPoint = new GameObject();
        //orbitPoint = (GameObject)Instantiate(Resources.Load("Empty GameObject"));
		orbitPoint.transform.localScale = transform.localScale;
		orbitPoint.transform.position = transform.position;
		
		playerCharacter = GameObject.Find("PlayerCharacter");
		hpIndicator = gameObject.transform.GetChild(0);
		hpIndicator.GetComponent<Renderer>().material.shader = Shader.Find("Self-Illumin/Diffuse");
		
		shop = GameObject.Find("Shop").GetComponent<Shop>();
		
		radiusSpeed = radiusSpeedBase;
	}








	// ============================================================================================================
    // ---------------------------------------------------------------------
	void Update() 
	{
		if (ready)
		{
			if (hp/hpMax >= .5f) hpIndicator.GetComponent<Renderer>().material.color = Color.Lerp(Color.yellow, Color.green, (hp - (hpMax*.5f)) / (hpMax*.5f));
			else                 hpIndicator.GetComponent<Renderer>().material.color = Color.Lerp(Color.red,    Color.yellow, hp / (hpMax*.5f));

			// transform.RotateAround (playerCharacter.transform.position, new Vector3 (0, 1, 0), rotateSpeed * Time.deltaTime);
			// desiredPos = (transform.position - playerCharacter.transform.position).normalized * radius + playerCharacter.transform.position;
			// transform.position = Vector3.MoveTowards (transform.position, desiredPos, Time.deltaTime * radiusSpeed);
			
			orbitPoint.transform.RotateAround(playerCharacter.transform.position, new Vector3(0,1,0), orbitSpeed*Time.deltaTime);
			// transform.position = orbitPoint.transform.position;
			transform.position = Vector3.MoveTowards(transform.position, orbitPoint.transform.position, Time.deltaTime*radiusSpeed);
		}
	}


    // ---------------------------------------------------------------------
	void OnCollisionEnter(Collision _OTHER)
	{
		if (_OTHER.transform.tag == "Enemy")
		{
			enemy = _OTHER.gameObject.GetComponent<Enemy>();
		}

		if (hp <= 0)
		{
			shop.activeSlotCount[ring]--;
			Destroy(gameObject);
		}
	}


    // ---------------------------------------------------------------------
	public void LevelUp()
	{
		level++;
		if (size == "Small")
		{
			hpMax += 4+level;
			//baseOrbitSpeed += 8;
		}
		else
		{
			hpMax += (4+level) * .6f;
			Mathf.Round(hpMax);
			//baseOrbitSpeed += 4;
		}

        hp = hpMax;
		if (level%2 == 1) damage++;

		//orbitalsMgr.AverageOrbitSpeed(ring);
	}
    /*
    public void LevelUp()
	{
		level++;
		if (size == "Small")
		{
			hpMax += 4+level;
			baseOrbitSpeed += 8;
		}
		else
		{
			hpMax += (4+level) * .6f;
			Mathf.Round(hpMax);
			baseOrbitSpeed += 4;
		}

        hp = hpMax;
		if (level%2 == 1) damage++;

		orbitalsMgr.AverageOrbitSpeed(ring);
	}
    */
}
