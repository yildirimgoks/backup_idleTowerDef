using System;
using UnityEngine;

namespace Assets.Scripts
{
    
    public class Tower : MageAssignableBuilding
    {
        public GameObject Banners;
        public GameObject Crystal;
        public ParticleSystem ParticleEffect;

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            ParticleEffect.Stop();
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();

            Crystal.transform.RotateAround(gameObject.transform.position, Vector3.up, 20 * Time.deltaTime);
        }

        public override bool SetMageInside(Mage mage)
        {
            if (!base.SetMageInside(mage)) return false;

            ParticleEffect.startColor = ElementController.Instance.GetColor(mage.Data.GetElement());
            ParticleEffect.Play();
            Crystal.gameObject.SetActive(true);
            Banners.gameObject.SetActive(true);

            var texture = ElementController.Instance.GetTower(mage.Data.GetElement());
            Banners.GetComponent<Renderer>().material.mainTexture = texture;
            Crystal.GetComponent<Renderer>().material.mainTexture = texture;

            return true;
        }

        public override bool EjectMageInside()
        {
            if (!base.EjectMageInside()) return false;
            ParticleEffect.Stop();
            Crystal.gameObject.SetActive(false);
			Banners.gameObject.SetActive(false);

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