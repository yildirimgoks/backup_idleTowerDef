using UnityEngine;
using System.Collections;
using System;

namespace Assets.Scripts
{
    public class Shrine : MageAssignableBuilding
    {
        DateTime _clickBeginTime;
        DateTime _clickEndTime;
        public GameObject[] Banner;

        //Identify Long Press - Variables


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

        public void SetShrineActive() {
            InsideMage.enabled= false;
        }

		public override bool SetMageInside(Mage mage)
		{
			if (!base.SetMageInside(mage)) return false;
           
            for (int i = 0; i < Banner.Length; i++) {
                Banner[i].GetComponent<Renderer>().material.mainTexture = ElementController.Instance.GetShrine(mage.Data.GetElement());
            }
           

            return true;
		}

		public override bool EjectMageInside()
		{
			if (!base.EjectMageInside()) return false;
         
            for (int i = 0; i < Banner.Length; i++) {
                Banner[i].GetComponent<Renderer>().material.mainTexture = ElementController.Instance.ShrineTextures[0];
            }
            
            return true;
		}

        //Useless
        /* public void EjectMage() {

             if (Input.GetMouseButtonDown(0)) {
                 _clickBeginTime = DateTime.Now;

             }
             if (Input.GetMouseButtonUp(0)) {
                 _clickEndTime = DateTime.Now;
                 TimeSpan _clickDuration = _clickEndTime - _clickBeginTime;
                 if (_clickDuration.TotalSeconds > 2) {
                 Debug.Log("This Works");
                 }
             }
         }
         */
    }
}
