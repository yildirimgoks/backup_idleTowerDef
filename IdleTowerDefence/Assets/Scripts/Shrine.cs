using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class Shrine : MonoBehaviour
    {

        public bool Occupied;
        public Mage InsideMage;
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
    }
}
