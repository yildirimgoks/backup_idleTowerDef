using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class FloatingText : MonoBehaviour
    {
        public Animator Animator;
        public Text DmgText;

        // Use this for initialization
        void Start ()
        {
            AnimatorClipInfo[] clipInfo = Animator.GetCurrentAnimatorClipInfo(0);
            Destroy(gameObject, clipInfo[0].clip.length);
            DmgText = Animator.GetComponent<Text>();
        }

        public void SetText(string text)
        {
            DmgText.text = text;
        }
    }
}
