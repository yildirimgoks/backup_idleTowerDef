﻿using UnityEngine;
using System.Collections;

public class PlayerSpell : MonoBehaviour {

	public float spellSpeed = 0.05f;
	public int damage = 20;



	// Use this for initialization
	void Start () {
	
	}


	//Update is called once per frame
	void Update () {
		
			GameObject targetMinion = FindClosestMinion ();
			Vector3 spellTarget = targetMinion.transform.position - transform.position;
			transform.Translate (spellTarget * spellSpeed * Time.deltaTime);
		} 

	// Find closest minion's name
	GameObject FindClosestMinion()	{
		GameObject[] minions;
		minions = GameObject.FindGameObjectsWithTag("Minion");
		GameObject closestMinion = null;
		float distance = Mathf.Infinity;
		Vector3 position = transform.position;
		foreach (GameObject minion in minions) {
			if (!Player.onMap (minion.gameObject))
				continue;
			float curDistance = Vector3.Distance(minion.transform.position, position);
			if (curDistance < distance) {
				closestMinion = minion;
				distance = curDistance;
			}
		}

		return closestMinion;
	}



	void OnCollisionEnter(Collision coll){
		if (coll.gameObject.tag=="Minion") {
			Destroy (gameObject);
			coll.gameObject.GetComponent<Minion> ().Life = coll.gameObject.GetComponent<Minion> ().Life - damage;
		}
	}

}
