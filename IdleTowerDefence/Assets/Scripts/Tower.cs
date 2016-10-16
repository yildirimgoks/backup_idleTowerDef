using System;
using UnityEngine;

namespace Assets.Scripts
{
    
    public class Tower : MageAssignableBuilding
    {

        GameObject _crystal;
        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            _crystal = gameObject.transform.GetChild(2).gameObject;
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            
            _crystal.transform.RotateAround(gameObject.transform.position, Vector3.up, 20 * Time.deltaTime);
        }

        public override bool SetMageInside(Mage mage)
        {
            if (!base.SetMageInside(mage)) return false;

			gameObject.transform.FindChild ("Crystal").gameObject.SetActive (true);
			gameObject.transform.FindChild ("Banners").gameObject.SetActive (true);
            foreach (var r in gameObject.GetComponentsInChildren<Renderer>())
            {
				if (r.gameObject.name != "Slot") {
					r.material.mainTexture = ElementController.Instance.GetTower (mage.Data.GetElement ());
				}
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

        public Color initialColor;
        public void StartHighlighting(Color color){
            // foreach (var r in gameObject.GetComponentsInChildren<Renderer>())
            // {
            //         initialColor = r.material.color;
			// 		r.material.color = color;
            // }
        }
        public void StopHighlighting(){
            // foreach (var r in gameObject.GetComponentsInChildren<Renderer>())
            // {
            //         Debug.Log(r.material.color);
			// 		r.material.color = initialColor;
            // }
        }
    }
}