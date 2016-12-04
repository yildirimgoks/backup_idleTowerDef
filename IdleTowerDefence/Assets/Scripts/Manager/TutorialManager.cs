using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Manager
{
    public class TutorialManager : MonoBehaviour
    {
        public Texture2D[] Set1, Set2;
        private bool _show = false;
        private Texture2D[] _currentSet;
        private Texture2D _currentPage;
        private int _index;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                ShowSet(Set1);
            } else if (Input.GetKeyDown(KeyCode.X))
            {
                ShowSet(Set2);
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                PrevPage();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                NextPage();
            }
        }

        public void ShowSet(Texture2D[] currentSet)
        {
            Time.timeScale = 0;
            _currentSet = currentSet;
            _index = 0;
            _show = true;
        }

        public void PrevPage()
        {
            if (_index > 0)
            {
                _index--;
            }
        }

        public void NextPage()
        {
            if (_index <= _currentSet.Length - 1)
            {
                if (_index != _currentSet.Length - 1)
                {
                    _index++;
                } else
                {
                    _show = false;
                    Time.timeScale = 1;
                }
            }
        }

        void OnGUI()
        {
            if (_show)
            {
                _currentPage = _currentSet[_index];
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _currentPage);
            }
        }
    }
}