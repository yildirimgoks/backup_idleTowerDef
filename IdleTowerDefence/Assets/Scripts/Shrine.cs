using UnityEngine;
using System.Collections;
using System;

namespace Assets.Scripts
{
    public class Shrine : MonoBehaviour
    {

        public bool Occupied;
        public Mage InsideMage;
        DateTime _clickBeginTime;
        DateTime _clickEndTime;

        //Identify Long Press - Variables


        // Use this for initialization
        void Start()
        {
            Occupied = false;
        }

        // Update is called once per frame
        void Update()
        {
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
