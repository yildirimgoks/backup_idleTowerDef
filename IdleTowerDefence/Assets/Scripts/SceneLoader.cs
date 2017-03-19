using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Scripts.Model;
using System;
using Assets.Scripts.Manager;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class SceneLoader : MonoBehaviour
    {
        public static string DefaultStartScene = "ForestScene1";
        public bool LoadSavedGame;
        private bool _saveLoaded;
        private bool _load;
        
        public string SceneName;
        public Text LoadingText;

        public AudioManager AudioManager;

        private PlayerData _data;

        private Player _player;

        void Start()
        {
            if (LoadSavedGame)
            {
                _data = SaveLoadHelper.LoadGame();
            }

            if (_data != null)
            {
                var namePanel = GameObject.FindGameObjectWithTag("NamePanel");
                var elementPanel = GameObject.FindGameObjectWithTag("ElementPanel");
                namePanel.SetActive(false);
                elementPanel.SetActive(false);
                _load = true;
                _saveLoaded = true;
                SceneName = _data.GetLoadedString();
                if (string.IsNullOrEmpty(SceneName))
                {
                    SceneName = SceneLoader.DefaultStartScene;
                }

                StartCoroutine(LoadNewScene());
            } else {
                _data = new PlayerData(Element.Air);
                SceneName = _data.GetLoadedString();
            }         
            if (PlayerPrefs.GetInt("sfxMute") == 1)
            {
                AudioManager.ToggleSound();
            }
            if (PlayerPrefs.GetInt("musicMute") == 1)
            {
                AudioManager.ToggleMusic();
            }
        }
        
        void Update()
        {
            if (_load)
            {
                LoadingText.color = new Color(LoadingText.color.r, LoadingText.color.g, LoadingText.color.b, Mathf.PingPong(Time.time, 1));
            }
        }

        void Awake()
        {
            DontDestroyOnLoad(transform.gameObject);
        }

        void OnEnable()
        {
            SceneManager.sceneLoaded += SceneChanged;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= SceneChanged;
        }

        private void SceneChanged(Scene scene, LoadSceneMode mode)
        {
            _player = Camera.main.GetComponent<Player>();
            if (_player != null)
            {
                _player.SetAudioManager(AudioManager);
            }
        }

        IEnumerator LoadNewScene()
        {
            var async = SceneManager.LoadSceneAsync(SceneName);
            async.allowSceneActivation = false;
            // Şimdilik yanıp sönmeyi görebilmek için kullanılıyor. (Scene hızlı yüklendiği için "Loading..." yanıp sönmüyor.)
            yield return new WaitForSeconds(1);
            while (!(async.progress < 0.9))
            {
                yield return null;
            }
            _load = false;
            async.allowSceneActivation = true;
        }

        public void SetElement(int elementNum)
        {
             switch(elementNum)
            {
                case 1:
                    _data.SetPlayerElement(Element.Fire); break;
                case 2:
                    _data.SetPlayerElement(Element.Water); break;
                case 3:
                    _data.SetPlayerElement(Element.Earth); break;
                case 4:
                    _data.SetPlayerElement(Element.Air); break;
                default:
                    throw new ArgumentException("Illegal argument passed.");
            }        
            _load = true;
            StartCoroutine(LoadNewScene());
        }

        public void SetName()
        {
            var inputField = GameObject.FindGameObjectWithTag("NamePanel").GetComponentInChildren<InputField>();
            if (inputField != null)
            {
                _data.SetPlayerName(inputField.text);
            }
        }

        public PlayerData GetPlayerData()
        {
            return _data;
        }

        public bool IsLoadSuccesfull()
        {
            return _saveLoaded;
        }
    }
}