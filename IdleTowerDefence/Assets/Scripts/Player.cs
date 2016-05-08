using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    private int Currency = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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
