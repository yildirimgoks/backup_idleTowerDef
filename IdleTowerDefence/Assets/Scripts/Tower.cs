using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class Tower : MageAssignableBuilding
    {
        // Use this for initialization
        protected override void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }

        public override bool SetMageInside(Mage mage)
        {
            if (!base.SetMageInside(mage)) return false;

			gameObject.transform.FindChild ("Crystal").gameObject.SetActive (true);
			gameObject.transform.FindChild ("Banners").gameObject.SetActive (true);
            foreach (var r in gameObject.GetComponentsInChildren<Renderer>())
            {
                r.material.mainTexture = ElementController.Instance.GetTower(mage.Data.GetElement());
            }

            return true;
        }

        public override bool EjectMageInside()
        {
            if (!base.EjectMageInside()) return false;

			gameObject.transform.FindChild ("Crystal").gameObject.SetActive (false);
			gameObject.transform.FindChild ("Banners").gameObject.SetActive (false);

            return true;
        }
    }
}