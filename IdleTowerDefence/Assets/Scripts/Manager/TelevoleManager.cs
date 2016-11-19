using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Manager
{
    public class TelevoleManager : MonoBehaviour
    {
        public ProfilePictureMage[] Mages;

        // Use this for initialization
        void Start () {
            DisableAllMages();
        }

        void SetMageLevel(int i)
        {
            if (Mages.Length <= i || Mages[i].enabled) return;
            DisableAllMages();
            Mages[i].gameObject.SetActive(true);
        }

        private void DisableAllMages()
        {
            foreach (var mage in Mages)
            {
                mage.gameObject.SetActive(false);
            }
        }
    }
}
