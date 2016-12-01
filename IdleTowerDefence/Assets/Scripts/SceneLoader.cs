﻿using UnityEngine;
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
        private bool _load;
        
        public int SceneNum;
        public Text LoadingText;

        private PlayerData _data;

        void Start()
        {
            if(System.IO.File.Exists(SaveLoadHelper.GetSaveGameFilePath()))
            {
                var namePanel = GameObject.FindGameObjectWithTag("NamePanel");
                var elementPanel = GameObject.FindGameObjectWithTag("ElementPanel");
                namePanel.SetActive(false);
                elementPanel.SetActive(false);
                _load = true;
                StartCoroutine(LoadNewScene());
            } else {
                _data = new PlayerData(Element.Air);
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
            var async = SceneManager.LoadSceneAsync(SceneNum);
            async.allowSceneActivation = false;
            // Şimdilik yanıp sönmeyi görebilmek için kullanılıyor. (Scene hızlı yüklendiği için "Loading..." yanıp sönmüyor.)
            yield return new WaitForSeconds(3);
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
    }
}