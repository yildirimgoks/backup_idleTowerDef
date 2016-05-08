using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public Rigidbody playerSpellPrefab;
    private int Currency = 0;

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

    public void IncreaseCurrency(int amount)
    {
        Currency += amount;
    }

    public void DecreaseCurrency(int amount)
    {
        Currency -= amount;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 20), "Currency: " + Currency);
    }
}
