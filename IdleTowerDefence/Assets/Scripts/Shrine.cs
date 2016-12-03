using UnityEngine;

namespace Assets.Scripts
{
    public class Shrine : MageAssignableBuilding
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

        public void SetShrineActive() {
            InsideMage.enabled = false;
        }

        public override void DisplayRangeObject()
        {
            base.HideRangeObject();
            base.StopHighlighting();
        }
    }
}
