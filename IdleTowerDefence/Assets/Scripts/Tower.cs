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
            foreach (var r in gameObject.GetComponentsInChildren<Renderer>())
            {
                r.material.mainTexture = ElementController.Instance.GetTower(mage.Data.GetElement());
            }
            return true;
        }

        public override bool EjectMageInside()
        {
            if (!base.EjectMageInside()) return false;
            
            foreach (var r in gameObject.GetComponentsInChildren<Renderer>())
            {
                r.material.mainTexture = ElementController.Instance.TowerTextures[0];
            }
            return true;
        }
    }
}