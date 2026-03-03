using UnityEngine;








// ============================================================================================================
// ---------------------------------------------------------------------
public class MainChar:MonoBehaviour 
{
	private bool canTakeDmg = true;
	public int level;
	public int nextLevelCost = 50;
	public float hpMax;
	public float hp;
	public int damage;
	public float bounce;
	public string special;
	private Vector3 mainCharScale;
	private Vector3 mainCharPos;

	private GameObject hpIndicator;
	private GameObject hpIndicatorOutline;

	private Enemy enemy;
	private Shop shop;
	private GameManager gameMgr;
	

	// ---------------------------------------------------------------------
	void Start() 
	{
        //Debug.Log("MainChar.Start");
	}

	public void Init()
	{
		level = 1;
		hpMax = 32;
		hp = hpMax;
		damage = 1;
		bounce = 300;
		special = "None";

		shop = GameObject.Find("Shop").GetComponent<Shop>();
		gameMgr = GameObject.Find("GameManager").GetComponent<GameManager>();
		
		mainCharScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
		mainCharPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		
		hpIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		hpIndicator.transform.localScale = new Vector3(mainCharScale.x*.5f, mainCharScale.y*.5f, mainCharScale.z*.5f);
		hpIndicator.transform.position = new Vector3(0, mainCharPos.y + (mainCharScale.y*.35f), 0);
		hpIndicator.GetComponent<Renderer>().material.shader = Shader.Find("Self-Illumin/Diffuse");
		
		hpIndicatorOutline = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		hpIndicatorOutline.transform.localScale = new Vector3(mainCharScale.x*.8f, mainCharScale.y*.8f, mainCharScale.z*.8f);
		hpIndicatorOutline.transform.position = new Vector3(0, mainCharPos.y + (mainCharScale.y*.15f), 0);
		hpIndicatorOutline.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 1);
	}








	// ============================================================================================================
    // ---------------------------------------------------------------------
	void Update() 
	{
		transform.position = mainCharPos;
		
		if (hp/hpMax >= .5f) hpIndicator.GetComponent<Renderer>().material.color = Color.Lerp(Color.yellow, Color.green, (hp-(hpMax*.5f)) / (hpMax*.5f));
		else                 hpIndicator.GetComponent<Renderer>().material.color = Color.Lerp(Color.red,    Color.yellow, hp              / (hpMax*.5f));
	}


    // ---------------------------------------------------------------------
	void OnCollisionEnter(Collision _OTHER)
	{
		if (_OTHER.transform.tag == "Enemy")
		{
			enemy = _OTHER.gameObject.GetComponent<Enemy>();
		}

		if (_OTHER.transform.tag == "Raw Material") shop.rawMaterialReserve += 5;
		if (_OTHER.transform.tag == "Keycard") gameMgr.hasKeycard = true;

		if (hp <= 0) Destroy(gameObject);
	}
}
