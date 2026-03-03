using UnityEngine;
using System.Collections;








// ============================================================================================================
// ---------------------------------------------------------------------
public class PreventDmgTimer:MonoBehaviour 
{
	// ---------------------------------------------------------------------
	void Start() 
	{
		
	}

	public void Init()
	{
		StartCoroutine(TimerDmgInactive());
	}

	private IEnumerator TimerDmgInactive()
	{
		yield return new WaitForSeconds(1);
		Destroy(gameObject);
	}
}
