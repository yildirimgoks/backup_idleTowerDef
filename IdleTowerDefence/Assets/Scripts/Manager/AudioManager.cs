using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Manager
{
    public class AudioManager : MonoBehaviour
    {
        private AudioSource _audio;

        void Start()
        {
            _audio = GetComponents<AudioSource>()[1];
        }

        public void PlaySpellCastingSound(Element element)
        {
            _audio.clip = ElementController.Instance.GetSoundEffects(element);
            _audio.Play();
        }

        public void PlaySkillCastingSound()
        {

        }

        public void PlaySpellCollisionSound(Element element)
        {
            _audio.clip = ElementController.Instance.GetSpellCollisionEffects(element);
            _audio.Play();
        }

        public void PlaySkillCollisionSound()
        {

        }

        public void PlayMinionDeathSound()
        {

        }

        public void PlayMinionSurviveSound()
        {

        }
    }
}
