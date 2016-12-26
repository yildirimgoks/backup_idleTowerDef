using UnityEngine;

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
        public AudioClip CoinEffect;
        
        public AudioSource SFXAudio;
        private AudioSource _musicAudio;
        private AudioClip _audioClip;

        private void Start()
        {
            AssignAudioSource();
        }

        public void AssignAudioSource()
        {
            _musicAudio = GameObject.Find("MusicObject").GetComponent<AudioSource>();
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
            SFXAudio.PlayOneShot(_audioClip);
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
            SFXAudio.PlayOneShot(_audioClip);
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
            SFXAudio.PlayOneShot(_audioClip);
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
            SFXAudio.PlayOneShot(_audioClip);
        }

        public void PlayMinionDeathSound()
        {
            _audioClip = MinionEffects[0];
            SFXAudio.PlayOneShot(_audioClip);
        }

        public void PlayMinionSurviveSound()
        {
            _audioClip = MinionEffects[1];
            SFXAudio.PlayOneShot(_audioClip);
        }

        public void PlayTowerClickSound()
        {
            _audioClip = ClickEffects[0];
            SFXAudio.PlayOneShot(_audioClip);
        }

        public void PlayButtonClickSound()
        {
            _audioClip = ClickEffects[1];
            SFXAudio.PlayOneShot(_audioClip);
        }

        public void PlayHornSound()
        {
            _audioClip = WaveEffects[0];
            SFXAudio.PlayOneShot(_audioClip);
        }

        public void PlayCoinSound()
        {
            _audioClip = CoinEffect;
            SFXAudio.PlayOneShot(_audioClip);
        }

        public void ToggleSound()
        {
            SFXAudio.mute = !SFXAudio.mute;
            if (SFXAudio.mute)
            {
                PlayerPrefs.SetInt("sfxMute", 1);
            }
            else
            {
                PlayerPrefs.SetInt("sfxMute", 0);
            }
        }

        public void ToggleMusic()
        {
            _musicAudio.mute = !_musicAudio.mute;
            if (_musicAudio.mute)
            {
                PlayerPrefs.SetInt("musicMute", 1);
            }
            else
            {
                PlayerPrefs.SetInt("musicMute", 0);
            }
        }
    }
}
