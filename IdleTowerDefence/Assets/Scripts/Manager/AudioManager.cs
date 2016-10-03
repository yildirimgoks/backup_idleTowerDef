using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Manager
{
    public class AudioManager : MonoBehaviour
    {
        public AudioClip[] MinionEffects;

        private AudioSource _audio;
        private AudioClip _audioClip;

        public Player Player;

        void Start()
        {
            _audio = GetComponents<AudioSource>()[1];
        }

        public void PlaySpellCastingSound(Element element)
        {
            _audioClip = ElementController.Instance.GetSpellCastEffects(element);
            _audio.PlayOneShot(_audioClip);
        }

        public void PlaySkillCastingSound(Element element)
        {
            _audioClip = ElementController.Instance.GetSkillCastEffects(element);
            _audio.PlayOneShot(_audioClip);
        }

        public void PlaySpellCollisionSound(Element element)
        {
            _audioClip = ElementController.Instance.GetSpellCollisionEffects(element);
            _audio.PlayOneShot(_audioClip);
        }

        public void PlaySkillCollisionSound(Element element)
        {
            _audioClip = ElementController.Instance.GetSkillCollisionEffects(element);
            _audio.PlayOneShot(_audioClip);
        }

        public void PlayMinionDeathSound()
        {
            _audioClip = MinionEffects[0];
            _audio.PlayOneShot(_audioClip);
        }

        public void PlayMinionSurviveSound()
        {
            _audioClip = MinionEffects[1];
            _audio.PlayOneShot(_audioClip);
        }
    }
}
