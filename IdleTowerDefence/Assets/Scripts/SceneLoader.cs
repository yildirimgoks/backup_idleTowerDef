using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Scripts.Model;
using Assets.Scripts.Manager;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class SceneLoader : MonoBehaviour
    {
        public static string DefaultStartScene = "ForestScene1";
        public bool LoadSavedGame;

        public Text LoadingText;
        public Player Player;

        public string SceneName;

        private PlayerData _data;

        private bool _firstScene;
        private bool _saveLoaded;
        private bool _load;

        void Start()
        {
            SceneManager.sceneLoaded += SceneChanged;
            _firstScene = true;
            if (LoadSavedGame)
            {
                _data = SaveLoadHelper.LoadGame();
            }

            if (_data != null)
            {
                DeactivateUsernameUi();
                _load = true;
                _saveLoaded = true;
                SceneName = _data.GetLoadedScene();
                if (string.IsNullOrEmpty(SceneName))
                {
                    SceneName = SceneLoader.DefaultStartScene;
                }

                StartCoroutine(LoadNewScene());
            } else {
                _data = new PlayerData(Element.Air);
                SceneName = _data.GetLoadedScene();
            }
        }

        private static void DeactivateUsernameUi()
        {
            var namePanel = GameObject.FindGameObjectWithTag("NamePanel");
            var elementPanel = GameObject.FindGameObjectWithTag("ElementPanel");
            namePanel.SetActive(false);
            elementPanel.SetActive(false);
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

        void OnDisable()
        {
            SceneManager.sceneLoaded -= SceneChanged;
        }

        private void SceneChanged(Scene scene, LoadSceneMode mode)
        {
            if (_firstScene)
            {
                Player.OnFirstSceneLoaded();
                _firstScene = false;
            }
            else
            {
                Player.OnSceneChange(scene.name);
            }
        }

        IEnumerator LoadNewScene()
        {
            Player.OnBeforeSceneChange();
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

        public void StartWithFire()
        {
            SetElement(Element.Fire);
        }

        public void StartWithWater()
        {
            SetElement(Element.Water);
        }

        public void StartWithAir()
        {
            SetElement(Element.Air);
        }

        public void StartWithEarth()
        {
            SetElement(Element.Earth);
        }

        private void SetElement(Element element)
        {
            _data.SetPlayerElement(element);
            AnalyticsManager.SendStartMageChosen(element);
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