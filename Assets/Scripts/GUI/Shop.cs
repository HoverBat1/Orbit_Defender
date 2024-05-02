using UnityEngine;
using System.Collections;
using System;




// ============================================================================================================
// ---------------------------------------------------------------------
public class Shop:MonoBehaviour 
{
	public bool usingShop = false;
	private bool notEnoughMatTimer = false;
	public int rawMaterialReserve;

	private int sideMenuButtonsInt;
	private string[] sideMenuButtonsStr;
	private int sideMenuButtonsIntClone = 0;

	private int slotButtonsToolbarInt;
	private string[] slotButtonsToolbarStr;
	private int slotButtonsToolbarIntClone = 0;
	
	private int[] ringSlots = new int[3] {3, 5, 8};

	private int botSizeButtonsInt;
	private string[] botSizeButtonsStr = new string[]{"Small", "Large"};

	private int boxBackgroundHeight;

	private int totalCost;
	private int[] botCostByRing = new int[3] {50, 80, 120};
	
	private bool ring1Complete = false;
	private bool ring2Activated = false;
	public int[] activeSlotCount = new int[] {0, 0};

	private int[] orbitalStartingHP = new int[2] {20, 12};
	private int[] orbitalStartingDmg = new int[2] {1, 3};

	private int statLevel;
	private float statHPMax;
	private float statHP;
	private int statDmg;
	private float statBounce;
	private int statOrbitSpeed;
	private float statStartBaseOrbitSpeed;
	private float statCurrentBaseOrbitSpeed;
	private string statSpecial;
	private string statSize;

	private GameManager gameManager;
	private OrbitalsManager orbitalsManager;
	private MainChar playerCharacter;

	





    // ---------------------------------------------------------------------
	void Start() 
	{
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		playerCharacter = GameObject.Find("PlayerCharacter").GetComponent<MainChar>();
 		orbitalsManager = GameObject.Find("OrbitalsManager").GetComponent<OrbitalsManager>();
		rawMaterialReserve = 0;
	}
	







	// ============================================================================================================
    // ---------------------------------------------------------------------
	void Update() 
	{
        ring1Complete = activeSlotCount[0] == 3;
	}


    // ---------------------------------------------------------------------
	void OnGUI()
	{
		totalCost = 0;

		// Main Shop Background
		if (usingShop 
        && !gameManager.blockShop )
        {
            GUI.Box(new Rect(4, 4, 818, 760), "");
        }

		// Shop Toggle
		if(!gameManager.blockShop)
		{
			if (GUI.Button(new Rect(27, 22, 70, 30), "Shop")) ToggleShop();
		}
		else
		{
			GUI.Box(new Rect(27, 22, 70, 30), "");
			GUI.Label(new Rect(47, 27, 30, 24), "Shop");
		}

		// ---------------------------------------------------------------------
		if (usingShop 
        && !gameManager.blockShop )
		{
			// Side Menu
			GUI.Box(new Rect(10, 72, 104, 680), "");
			if (ring1Complete 
            ||  ring2Activated )
            {
                sideMenuButtonsStr = new string[]{"Main Core", "Ring 1", "Ring 2"};
            }
			else
            {
                sideMenuButtonsStr = new string[]{"Main Core", "Ring 1"};
            }
			
            sideMenuButtonsInt = GUI.SelectionGrid(new Rect(10, 72, 104, 130), sideMenuButtonsInt, sideMenuButtonsStr, 1);

			// Box for Main menu item selected text
			GUI.Box(new Rect(128, 10, 420, 54), "");

			GUI.Label(new Rect(600, 25, 200, 50), "Raw Material Reserve: " + rawMaterialReserve.ToString());

			// ---------------------------------------------------------------------
			if (sideMenuButtonsInt == 0) // Main Core
			{
				// Main menu item selected text
				GUI.Label(new Rect(300, 25, 200, 50), "MAIN CORE");

				// Main background box for item selected
				GUI.Box(new Rect(128, 72, 688, 512), "");

				// Heal 5 HP for 5 Material
				playerCharacter.hp += BuyHP(playerCharacter.hpMax, playerCharacter.hp);
                playerCharacter.hp = Math.Min(playerCharacter.hp, playerCharacter.hpMax);

				// Level Up
				//playerCharacter.level += levelUp(playerCharacter.nextLevelCost);

				DisplayStats();
			}
			else // Rings/Orbitals
			{
				int _RING_IDX = sideMenuButtonsInt-1;
                slotButtonsToolbarInt = Math.Clamp(slotButtonsToolbarInt,0,orbitalsManager.orbitals[_RING_IDX].Length-1);
                totalCost = botCostByRing[_RING_IDX];
				if (orbitalsManager.orbitals[_RING_IDX][slotButtonsToolbarInt]==null) boxBackgroundHeight = 468;
				else                                                                  boxBackgroundHeight = 512;

				// Main menu item selected text
				GUI.Label(new Rect(280, 25, 200, 50), "RING " + sideMenuButtonsInt.ToString() + " DEFENSE");

				// Main background box for item selected
				GUI.Box(new Rect(128, 116, 688, boxBackgroundHeight), "");

				// Ring Slot Buttons
				GUI.Box(new Rect(128, 72, 688, 38), "");
				slotButtonsToolbarStr = new string[ringSlots[_RING_IDX]];
				for(int i=1; i<ringSlots[_RING_IDX]+1; i++) slotButtonsToolbarStr[i-1] = "BOT "+i.ToString();
				slotButtonsToolbarInt = GUI.Toolbar(new Rect(136, 76, 672, 30), slotButtonsToolbarInt, slotButtonsToolbarStr);

                int _SLOT_IDX = slotButtonsToolbarInt;
				// Heal 5 HP for 5 Material
				if (orbitalsManager.orbitals[_RING_IDX][_SLOT_IDX] != null)
				{
					    orbitalsManager.orbitals[_RING_IDX][_SLOT_IDX].GetComponent<Orbital>().hp += BuyHP(orbitalsManager.orbitals[_RING_IDX][_SLOT_IDX].GetComponent<Orbital>().hpMax, orbitalsManager.orbitals[_RING_IDX][_SLOT_IDX].GetComponent<Orbital>().hp);
					if (orbitalsManager.orbitals[_RING_IDX][_SLOT_IDX].GetComponent<Orbital>().hp >        orbitalsManager.orbitals[_RING_IDX][_SLOT_IDX].GetComponent<Orbital>().hpMax)
                    {
                        orbitalsManager.orbitals[_RING_IDX][_SLOT_IDX].GetComponent<Orbital>().hp =        orbitalsManager.orbitals[_RING_IDX][_SLOT_IDX].GetComponent<Orbital>().hpMax;
                    }
				}

				// Level Up
				if (orbitalsManager.orbitals[_RING_IDX][_SLOT_IDX] != null 
                &&  LevelUpButton(orbitalsManager.orbitals[_RING_IDX][_SLOT_IDX].GetComponent<Orbital>().nextLevelCost) == 1 )
				{
					orbitalsManager.orbitals[_RING_IDX][_SLOT_IDX].GetComponent<Orbital>().LevelUp();
                    
                    
                    int _level = orbitalsManager.orbitals[_RING_IDX][_SLOT_IDX].GetComponent<Orbital>().level;
                    for(int i=orbitalsManager.orbitals[_RING_IDX].Length-1; i>=0; i--)
                    {
                        if (orbitalsManager.orbitals[_RING_IDX][i].GetComponent<Orbital>().level < _level)
                        {
                            _level = orbitalsManager.orbitals[_RING_IDX][i].GetComponent<Orbital>().level;
                        }
                    }

                    float _SPEED = orbitalsManager.baseOrbitSpeed[_RING_IDX][0] + (_level*8);
                    for(int i=orbitalsManager.orbitals[_RING_IDX].Length-1; i>=0; i--)
                    {
                        orbitalsManager.orbitals[_RING_IDX][i].GetComponent<Orbital>().baseOrbitSpeed = _SPEED;
                    }
				}

				DisplayStats();
				// ---------------------------------------------------------------------------------------

				if (orbitalsManager.orbitals[_RING_IDX][_SLOT_IDX] == null)
				{
					// Bot creation menu at bottom of menu
					GUI.Box(new Rect(128, 590, 688, 120), "");

					if (notEnoughMatTimer) DisplayNotEnoughMatLabel(590, 590);

					GUI.Box(new Rect(132, 614, 88, 92), "Bot Size");
					botSizeButtonsInt = GUI.SelectionGrid(new Rect(136, 640, 80, 60), botSizeButtonsInt, botSizeButtonsStr, 1);
					
					GUI.Box(new Rect(528, 614, 284, 92), "TOTAL COST: " + totalCost.ToString());
					if (GUI.Button(new Rect(532, 636, 276, 66), "CREATE BOT"))
					{
						if (rawMaterialReserve >= totalCost)
						{
							rawMaterialReserve -= totalCost;
							orbitalsManager.CreateOrbital(_RING_IDX, slotButtonsToolbarInt, botSizeButtonsStr[botSizeButtonsInt]);
							activeSlotCount[_RING_IDX]++;
							if (sideMenuButtonsInt == 2 
                            &&  ring1Complete )
                            {
                                ring2Activated = true;
                            }
						}
						else
						{
							notEnoughMatTimer = true;
							StartCoroutine(TimerNotEnoughMat());
						}
					}
				}
			}

			if (GUI.changed)
			{
				if (slotButtonsToolbarInt != slotButtonsToolbarIntClone)
				{
					botSizeButtonsInt = 0;
					slotButtonsToolbarIntClone = slotButtonsToolbarInt;
				}

				if (sideMenuButtonsInt != sideMenuButtonsIntClone)
				{
					botSizeButtonsInt = 0;
					slotButtonsToolbarInt = 0;
					sideMenuButtonsIntClone = sideMenuButtonsInt;
				}
			}
		}
	}








    // ============================================================================================================
    // ---------------------------------------------------------------------
	private void ToggleShop()
	{
		if (usingShop)
        {
            usingShop = false;
        }
		else
		{
			sideMenuButtonsInt = 0;
			slotButtonsToolbarInt = 0;
			usingShop = true;
		}
	}


    // ---------------------------------------------------------------------
	// Get and display stats for the Main Core or a Bot
	private void DisplayStats()
	{
		if (sideMenuButtonsInt == 0)
		{
			statLevel   = playerCharacter.level;
			statHPMax   = playerCharacter.hpMax;
			statHP      = playerCharacter.hp;
			statDmg     = playerCharacter.damage;
			statBounce  = playerCharacter.bounce;
			statSpecial = playerCharacter.special;
		}
		else
		{
			if (orbitalsManager.orbitals[sideMenuButtonsInt-1][slotButtonsToolbarInt] != null)
			{
				statLevel                 = orbitalsManager.orbitals[sideMenuButtonsInt-1][slotButtonsToolbarInt].gameObject.GetComponent<Orbital>().level;
				statHPMax                 = orbitalsManager.orbitals[sideMenuButtonsInt-1][slotButtonsToolbarInt].gameObject.GetComponent<Orbital>().hpMax;
				statHP                    = orbitalsManager.orbitals[sideMenuButtonsInt-1][slotButtonsToolbarInt].gameObject.GetComponent<Orbital>().hp;
				statDmg                   = orbitalsManager.orbitals[sideMenuButtonsInt-1][slotButtonsToolbarInt].gameObject.GetComponent<Orbital>().damage;
				statBounce                = orbitalsManager.orbitals[sideMenuButtonsInt-1][slotButtonsToolbarInt].gameObject.GetComponent<Orbital>().bounce;
				statOrbitSpeed       = (int)orbitalsManager.orbitals[sideMenuButtonsInt-1][slotButtonsToolbarInt].gameObject.GetComponent<Orbital>().orbitSpeed;
				statCurrentBaseOrbitSpeed = orbitalsManager.orbitals[sideMenuButtonsInt-1][slotButtonsToolbarInt].gameObject.GetComponent<Orbital>().baseOrbitSpeed;
				statSpecial               = orbitalsManager.orbitals[sideMenuButtonsInt-1][slotButtonsToolbarInt].gameObject.GetComponent<Orbital>().special;
				statSize                  = orbitalsManager.orbitals[sideMenuButtonsInt-1][slotButtonsToolbarInt].gameObject.GetComponent<Orbital>().size;
			}
			else
			{
				statLevel = 1;
				statHPMax = orbitalStartingHP[botSizeButtonsInt];
				statHP    = orbitalStartingHP[botSizeButtonsInt];
				statDmg = orbitalStartingDmg[botSizeButtonsInt];
				statBounce = 300;
				statOrbitSpeed = (int)orbitalsManager.baseOrbitSpeed[sideMenuButtonsInt-1][botSizeButtonsInt];
				statSpecial = "None";
				statSize = botSizeButtonsStr[botSizeButtonsInt];
			}
		}

		GUI.Box(new Rect(210, 210, 180, 184), "");
		GUI.Box(new Rect(210, 210, 180, 24), "LEVEL " + statLevel.ToString());
		
		GUI.Label(new Rect(220, 240, 100, 22), "Health:");
		GUI.Label(new Rect(340, 240, 100, 22), statHP.ToString() + "/" + statHPMax.ToString());
		
		GUI.Label(new Rect(220, 260, 100, 22), "Damage:");
		GUI.Label(new Rect(340, 260, 100, 22), statDmg.ToString());
		
		GUI.Label(new Rect(220, 280, 100, 22), "Knockback:");
		GUI.Label(new Rect(340, 280, 100, 22), statBounce.ToString());

		if (sideMenuButtonsInt > 0)
		{
			if (orbitalsManager.orbitals[sideMenuButtonsInt-1][slotButtonsToolbarInt] == null)
			{
				GUI.Label(new Rect(220, 300, 100, 22), "Orbit Speed:");
				GUI.Label(new Rect(340, 300, 100, 22), statOrbitSpeed.ToString());

				GUI.Label(new Rect(220, 320, 120, 22), "Base Orbit Speed:");
				GUI.Label(new Rect(340, 320, 100, 22), statOrbitSpeed.ToString());
			}
			else
			{
				GUI.Label(new Rect(220, 300, 100, 22), "Orbit Speed:");
				GUI.Label(new Rect(340, 300, 100, 22), statOrbitSpeed.ToString());

				GUI.Label(new Rect(220, 320, 120, 22), "Base Orbit Speed:");
				GUI.Label(new Rect(340, 320, 100, 22), statCurrentBaseOrbitSpeed.ToString());
			}
		}

		GUI.Label(new Rect(220, 340, 100, 22), "Special:");
		GUI.Label(new Rect(340, 340, 100, 22), statSpecial);

		if (sideMenuButtonsInt > 0)
		{
			GUI.Label(new Rect(220, 360, 100, 22), "Size:");
			GUI.Label(new Rect(340, 360, 100, 22), statSize);
		}
	}


    // ---------------------------------------------------------------------
	private void DisplayNotEnoughMatLabel(float _LEFT, float _TOP)
	{
		GUI.Label(new Rect(_LEFT, _TOP, 200, 22), "NOT ENOUGH MATERIAL");
	}

	private IEnumerator TimerNotEnoughMat()
	{
		yield return new WaitForSeconds(2);
		notEnoughMatTimer = false;
	}


    // ---------------------------------------------------------------------
	private float BuyHP(float _HP_MAX, float _HP)
	{
		// Heal 5 HP
		GUI.Box(new Rect(210, 404, 180, 52), "");
		if (notEnoughMatTimer) DisplayNotEnoughMatLabel(220, 412);
		else GUI.Label(new Rect(280, 434, 100, 22), "Cost: 5");
		if (GUI.Button(new Rect(210, 404, 180, 30), "HEAL 5HP"))
		{
			if (rawMaterialReserve < 5)
			{
				notEnoughMatTimer = true;
				StartCoroutine(TimerNotEnoughMat());
				return 0;
			}
			else if (_HP < _HP_MAX)
			{
				rawMaterialReserve -= 5;
				return 5;
			}
			else return 0;
		}
		else return 0;
	}


    // ---------------------------------------------------------------------
	private int LevelUpButton(int _COST)
	{
		// Heal 5 HP
		GUI.Box(new Rect(398, 210, 180, 52), "");
		if (notEnoughMatTimer) DisplayNotEnoughMatLabel(408, 218);
		else GUI.Label(new Rect(488, 240, 100, 22), "Cost: " + _COST.ToString());
		
        if (GUI.Button(new Rect(398, 210, 180, 30), "LEVEL UP"))
		{
			if (rawMaterialReserve < _COST)
			{
				notEnoughMatTimer = true;
				StartCoroutine(TimerNotEnoughMat());
				return 0;
			}
			else
			{
				rawMaterialReserve -= _COST;
				return 1;
			}
		}
		else return 0;
	}
}
