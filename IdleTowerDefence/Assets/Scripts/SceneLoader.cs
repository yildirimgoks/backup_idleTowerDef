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

        [SerializeField]
        private int _sceneNum;
        [SerializeField]
        private Text _loadingText;

        public PlayerData Data;

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
                Data = new PlayerData(1, 20, 100, 0, 100, 1, Element.Air);
            }         
        }
        
        void Update()
        {
            if (_load)
            {
                _loadingText.color = new Color(_loadingText.color.r, _loadingText.color.g, _loadingText.color.b, Mathf.PingPong(Time.time, 1));
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
            var async = SceneManager.LoadSceneAsync(_sceneNum);
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
                    Data.SetPlayerElement(Element.Fire); break;
                case 2:
                    Data.SetPlayerElement(Element.Water); break;
                case 3:
                    Data.SetPlayerElement(Element.Earth); break;
                case 4:
                    Data.SetPlayerElement(Element.Air); break;
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
                Data.SetPlayerName(inputField.text);
            }
        }
    }
}