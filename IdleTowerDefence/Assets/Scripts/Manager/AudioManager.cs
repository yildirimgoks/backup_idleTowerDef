using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Manager
{
    public class AudioManager : MonoBehaviour
    {
        public AudioClip[] SpellCastEffects;
        public AudioClip[] SpellCollisionEffects;
        public AudioClip[] SkillCastEffects;
        public AudioClip[] SkillCollisionEffects;
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
            switch (element)
            {
                case Element.Fire:
                    _audioClip = SpellCastEffects[1]; break;
                case Element.Water:
                    _audioClip = SpellCastEffects[2]; break;
                case Element.Earth:
                    _audioClip = SpellCastEffects[3]; break;
                case Element.Air:
                    _audioClip = SpellCastEffects[4]; break;
                default:
                    _audioClip = SpellCastEffects[0]; break;
            }
            _audio.PlayOneShot(_audioClip);
        }

        public void PlaySkillCastingSound(Element element)
        {
            switch (element)
            {
                case Element.Fire:
                    _audioClip = SkillCastEffects[1]; break;
                case Element.Water:
                    _audioClip = SkillCastEffects[2]; break;
                case Element.Earth:
                    _audioClip = SkillCastEffects[3]; break;
                case Element.Air:
                    _audioClip = SkillCastEffects[4]; break;
                default:
                    _audioClip = SkillCastEffects[0]; break;
            }
            _audio.PlayOneShot(_audioClip);
        }

        public void PlaySpellCollisionSound(Element element)
        {
            switch (element)
            {
                case Element.Fire:
                    _audioClip = SpellCollisionEffects[1]; break;
                case Element.Water:
                    _audioClip = SpellCollisionEffects[2]; break;
                case Element.Earth:
                    _audioClip = SpellCollisionEffects[3]; break;
                case Element.Air:
                    _audioClip = SpellCollisionEffects[4]; break;
                default:
                    _audioClip = SpellCollisionEffects[0]; break;
            }
            _audio.PlayOneShot(_audioClip);
        }

        public void PlaySkillCollisionSound(Element element)
        {
            switch (element)
            {
                case Element.Fire:
                    _audioClip = SkillCollisionEffects[1]; break;
                case Element.Water:
                    _audioClip = SkillCollisionEffects[2]; break;
                case Element.Earth:
                    _audioClip = SkillCollisionEffects[3]; break;
                case Element.Air:
                    _audioClip = SkillCollisionEffects[4]; break;
                default:
                    _audioClip = SkillCollisionEffects[0]; break;
            }
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
