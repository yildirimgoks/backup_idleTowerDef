﻿using UnityEngine;

namespace Assets.Scripts.Manager
{
    public class AudioManager : MonoBehaviour
    {
        public AudioClip[] SpellCastEffects;
        public AudioClip[] SpellCollisionEffects;
        public AudioClip[] SkillCastEffects;
        public AudioClip[] SkillCollisionEffects;
        public AudioClip[] MinionEffects;
        public AudioClip[] ClickEffects;
        public AudioClip[] WaveEffects;

        public AudioSource _musicAudio;
        public AudioSource _sfxAudio;
        private AudioClip _audioClip;

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
            _sfxAudio.PlayOneShot(_audioClip);
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
            _sfxAudio.PlayOneShot(_audioClip);
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
            _sfxAudio.PlayOneShot(_audioClip);
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
            _sfxAudio.PlayOneShot(_audioClip);
        }

        public void PlayMinionDeathSound()
        {
            _audioClip = MinionEffects[0];
            _sfxAudio.PlayOneShot(_audioClip);
        }

        public void PlayMinionSurviveSound()
        {
            _audioClip = MinionEffects[1];
            _sfxAudio.PlayOneShot(_audioClip);
        }

        public void PlayTowerClickSound()
        {
            _audioClip = ClickEffects[0];
            _sfxAudio.PlayOneShot(_audioClip);
        }

        public void PlayButtonClickSound()
        {
            _audioClip = ClickEffects[1];
            _sfxAudio.PlayOneShot(_audioClip);
        }

        public void PlayHornSound()
        {
            _audioClip = WaveEffects[0];
            _sfxAudio.PlayOneShot(_audioClip);
        }

        public void ToggleMusic()
        {
            _musicAudio.mute = !_musicAudio.mute;
        }

        public void ToggleSound()
        {
            _sfxAudio.mute = !_sfxAudio.mute;
        }
    }
}
