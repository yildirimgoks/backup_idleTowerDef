using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Assets.Script
{
    public class SceneLoader : MonoBehaviour
    {
        private bool load = false;

        [SerializeField]
        private int sceneNum;
        [SerializeField]
        private Text loadingText;
        
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Space) && load == false)
            {
                load = true;
                loadingText.text = "Loading...";            
                StartCoroutine(LoadNewScene());
            }
            if (load == true)
            {
                loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));
            }
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
        }
    }
}


