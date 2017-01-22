using Assets.Scripts.Pooling;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class FloatingText : PoolableMonoBehaviour
    {
        private static Color _damagColor = new Color(246.0f / 255, 49.0f / 255, 49.0f / 255);
        private static Color _incomeColor = new Color(255.0f / 255, 248.0f / 255, 49.0f / 255);

        public Animator Animator;
        public Text DmgText;

        public void StartAnimating()
        {
            AnimatorClipInfo[] clipInfo = Animator.GetCurrentAnimatorClipInfo(0);
            Animator.Play("popup_dmg");
            Destroy(clipInfo[0].clip.length);
        }

        public void SetType(bool damage)
        {
            DmgText.color = damage ? _damagColor : _incomeColor;
        }

        public void SetText(string text)
        {
            DmgText.text = text;
        }
    }
}
