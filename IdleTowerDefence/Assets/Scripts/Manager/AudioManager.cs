using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Manager
{
    public class AudioManager : MonoBehaviour
    {
        private AudioSource _audio1, _audio2, _audio3;

        public Player Player;

        void Start()
        {
            _audio1 = GetComponents<AudioSource>()[1];
            _audio2 = GetComponents<AudioSource>()[2];
            _audio3 = GetComponents<AudioSource>()[3];
        }

        public void PlaySpellCastingSound(Element element)
        {
            _audio1.clip = ElementController.Instance.GetSpellCastEffects(element);
            _audio1.Play();
        }

        public void PlaySkillCastingSound(Element element)
        {
            _audio1.clip = ElementController.Instance.GetSkillCastEffects(element);
            _audio1.Play();
        }

        public void PlaySpellCollisionSound(Element element)
        {
            _audio2.clip = ElementController.Instance.GetSpellCollisionEffects(element);
            _audio2.Play();
        }

        public void PlaySkillCollisionSound(Element element)
        {
            _audio2.clip = ElementController.Instance.GetSkillCollisionEffects(element);
            _audio2.Play();
        }

        public void PlayMinionDeathSound()
        {
            _audio3.clip = Player.MinionEffects[0];
            _audio3.Play();
        }

        public void PlayMinionSurviveSound()
        {
            _audio3.clip = Player.MinionEffects[1];
            _audio3.Play();
        }
    }
}
