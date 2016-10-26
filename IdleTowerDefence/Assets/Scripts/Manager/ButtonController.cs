using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Assets.Scripts.Manager
{
    public class ButtonController : MonoBehaviour
    {
        private AudioManager _audioManager;
        private Button _thisButton;

        void Awake()
        {
            _audioManager = Camera.main.GetComponent<AudioManager>();
            _thisButton = GetComponent<Button>();
            _thisButton.onClick.AddListener(() => { _audioManager.PlayButtonClickSound();});
        }
    }
}
