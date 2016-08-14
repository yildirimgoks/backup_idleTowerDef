using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Scripts.Model;
using System;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class SceneLoader : MonoBehaviour
    {
        private bool _load;
        
        public int SceneNum;
        public Text LoadingText;

        private PlayerData _data;

        void Start()
        {
            if(System.IO.File.Exists("saveGame.xml"))
            {
                var namePanel = GameObject.FindGameObjectWithTag("NamePanel");
                var elementPanel = GameObject.FindGameObjectWithTag("ElementPanel");
                namePanel.SetActive(false);
                elementPanel.SetActive(false);
                _load = true;
                StartCoroutine(LoadNewScene());
            } else {
                _data = new PlayerData(1, 20, 100, 0, 100, 1, Element.Air);
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

        IEnumerator LoadNewScene()
        {
            // Şimdilik yanıp sönmeyi görebilmek için kullanılıyor. (Scene hızlı yüklendiği için "Loading..." yanıp sönmüyor.)
            yield return new WaitForSeconds(3);
            _load = false;
            var async = SceneManager.LoadSceneAsync(SceneNum);
            while (!async.isDone)
            {
                yield return null;
            }
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
    }
}