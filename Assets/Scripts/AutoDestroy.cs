using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
	public float destroyTime = 1.0f;
	// Use this for initialization
	void Start()
	{
    	StartCoroutine(DestroySelf());
	}

	IEnumerator DestroySelf()
	{
    	// yield return new WaitForEndOfFrame();
    	yield return new WaitForSeconds(destroyTime);
    	Destroy(this.gameObject);
	}
}
