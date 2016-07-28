using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Scripts.Model;

namespace Assets.Scripts
{
    public class SceneLoader : MonoBehaviour
    {
        private bool _load = false;
        private bool _elementSet = false;
        private bool _sceneChanged = false;

        [SerializeField]
        private int sceneNum;
        [SerializeField]
        private Text loadingText;

        public PlayerData Data;

        void Start()
        {
            Data = new PlayerData(20, 100, 0, 100, 1, Element.Air);
        }
        
        void Update()
        {
            if (!_sceneChanged)
            {
                if (Input.GetKeyUp(KeyCode.Space) && !_load)
                {
                    _load = true;
                    loadingText.text = "Loading...";
                    StartCoroutine(LoadNewScene());
                }
                if (_load == true)
                {
                    loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));
                }
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
            AsyncOperation async = Application.LoadLevelAsync(sceneNum);

            while (!async.isDone)
            {
                yield return null;
            }
            _sceneChanged = true;
        }

        // initial element setting functions
        public void SetPlayerElementFire()
        {
            Data.SetPlayerElement(Element.Fire);
            _elementSet = true;
        }

        public void SetPlayerElementWater()
        {
            Data.SetPlayerElement(Element.Water);
            _elementSet = true;
        }

        public void SetPlayerElementEarth()
        {
            Data.SetPlayerElement(Element.Earth);
            _elementSet = true;
        }

        public void SetPlayerElementAir()
        {
            Data.SetPlayerElement(Element.Air);
            _elementSet = true;
        }
        // initial element setting functions end here
    }
}


