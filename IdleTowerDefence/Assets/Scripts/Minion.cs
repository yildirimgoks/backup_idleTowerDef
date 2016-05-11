﻿using UnityEngine;
using System.Collections;

public class Minion : MonoBehaviour {

    public TextMesh healthIndicator;
	public Player controller;

    private int CurrencyGivenOnDeath = 5;
    public int Life = 100;

	public float speed = 0.1f;

	// If the minion enters map, it is changed to true;
	private bool enteredMap = false;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        healthIndicator.text = "" + Life;
        if(Life <= 0)
        {
            Destroy(gameObject);
        }
		Walk ();
		if (Player.onMap (gameObject)) {
			// If minion enters map
			enteredMap = true;
		}
		if (enteredMap && !Player.onMap (gameObject)) {
			// If minion entered map before, and not on the map right now -> Minion leaves the map
			controller.MinionSurvived (this);
		}
	}

	// To UYGAR from HAYDAR: Denerken kullandim bu şekilde, istediğin şekilde değiştirirsin
	void Walk (){
		Vector3 desiredPosition = new Vector3 (this.transform.position.x, this.transform.position.y, this.transform.position.z + 100);
		transform.Translate (desiredPosition * speed * Time.deltaTime);
	}

    void OnDestroy() {
		if (controller == null || this == null)
			return;
		controller.MinionDied (this, CurrencyGivenOnDeath);
    }
}
