using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    public Animator animator;

    private Text dmgText;

	// Use this for initialization
	void Start ()
	{
	    AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        Destroy(gameObject, clipInfo[0].clip.length);
	    dmgText = animator.GetComponent<Text>();
	}

    public void SetText(string text)
    {
        dmgText.text = text;
    }
}
