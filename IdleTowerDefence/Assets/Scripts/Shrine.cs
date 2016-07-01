using UnityEngine;
using System.Collections;
using System;

namespace Assets.Scripts
{
    public class Shrine : MageAssignableBuilding
    {
        DateTime _clickBeginTime;
        DateTime _clickEndTime;

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
