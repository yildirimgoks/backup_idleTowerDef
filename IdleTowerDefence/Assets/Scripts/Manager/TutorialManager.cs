using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.Scripts.UI;

namespace Assets.Scripts.Manager
{
    public class TutorialManager : MonoBehaviour
    {
		public Sprite[] Set1, Set2, Set3, Set4;
        private bool _show = false;
		private Sprite[] _currentSet;
        private Sprite _currentPage;
        private int _index;
		public GameObject TutorialPanel;

        public Player Player;

        void Update()
        {
			if (_index == 0) {
				TutorialPanel.transform.Find ("Prew").gameObject.SetActive (false);
			} else {
				TutorialPanel.transform.Find ("Prew").gameObject.SetActive (true);
			}
			if (_show && _currentSet.Length > _index)
			{
				_currentPage = _currentSet[_index];
				TutorialPanel.GetComponentInChildren<Image> ().sprite = _currentPage;
			}

        }

        public void ShowSet(Sprite[] currentSet)
        {
			if (currentSet.Length > 0) {
				Player.MageButtons.DirectlyCloseMageButtonsMenu ();
				Time.timeScale = 0;
				_currentSet = currentSet;
				_index = 0;
				_show = true;
				TutorialPanel.SetActive (true);
			}
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
					TutorialPanel.SetActive (false);
                    Time.timeScale = 1;
                }
            }
        }
    }
}