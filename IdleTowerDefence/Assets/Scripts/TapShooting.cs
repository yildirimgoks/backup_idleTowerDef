using UnityEngine;
using System.Collections;

public class TapShooting : MonoBehaviour {

	public Rigidbody playerSpellPrefab;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			Vector3 mousePos = Input.mousePosition;
			mousePos.z = 8f;
			Vector3 instantPos = Camera.main.ScreenToWorldPoint (mousePos);
			Rigidbody playerSpellInstance;
			playerSpellInstance = Instantiate (playerSpellPrefab, instantPos, Quaternion.identity) as Rigidbody;
		}
	}
}
