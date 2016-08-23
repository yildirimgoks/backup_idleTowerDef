﻿using UnityEngine;
using System.Collections;

public class ProfilePictureMage : MonoBehaviour {

    public Animator animator;

	private Quaternion _baseRotation;

    // Use this for initialization
    void Start () {
        SelectRandomAnimation();
		_baseRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation = _baseRotation;
	}

    private void SelectRandomAnimation()
    {
        int rand = Random.Range(1, 3);
        animator.SetTrigger("Animation" + rand.ToString());
    }
}