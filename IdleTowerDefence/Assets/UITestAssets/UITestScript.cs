using UnityEngine;
using System.Collections;

public class UITestScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	 
    void OnGUI() {
        GUI.Label(new Rect(10, 10, 200, 20), "Hello World");
		GUI.Button (new Rect (20, 40, 200, 20), "Ben bir tuşum");
    }
}
