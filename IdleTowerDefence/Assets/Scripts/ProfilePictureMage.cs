using UnityEngine;
using System.Collections;

public class ProfilePictureMage : MonoBehaviour {

    public Animator animator;

    // Use this for initialization
    void Start () {
        SelectRandomAnimation();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void SelectRandomAnimation()
    {
        int rand = Random.Range(1, 3);
        animator.SetTrigger("Animation" + rand.ToString());
    }
}
