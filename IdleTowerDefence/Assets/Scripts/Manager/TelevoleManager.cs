using Assets.Scripts.Model;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Manager
{
    public class TelevoleManager : MonoBehaviour
    {
        public ProfilePictureMage[] Mages;
        private int _activeMage;

        // Use this for initialization
        void Start () {
            DisableAllMages();
        }

        public void SetMage(MageData data)
        {
            SetMageLevel(data.GetPrefabId());
            SetMageElement(data.GetElement());
        }

        private void SetMageElement(Element element)
        {
            if (_activeMage < 0 || _activeMage >= Mages.Length) return;

            foreach (var rend in Mages[_activeMage].gameObject.GetComponentsInChildren<Renderer>())
            {
                if (rend.name.Contains("Body"))
                {
                    rend.material.mainTexture = ElementController.Instance.GetMage(element)[0];
                }
                else
                {
                    rend.material.mainTexture = ElementController.Instance.GetMage(element)[1];
                }
            }
        }

        private void SetMageLevel(int i)
        {
            if (Mages.Length <= i || i == _activeMage) return;
            DisableAllMages();
            Mages[i].gameObject.SetActive(true);
            _activeMage = i;
        }

        private void DisableAllMages()
        {
            foreach (var mage in Mages)
            {
                mage.gameObject.SetActive(false);
            }
            _activeMage = -1;
        }
    }
}
