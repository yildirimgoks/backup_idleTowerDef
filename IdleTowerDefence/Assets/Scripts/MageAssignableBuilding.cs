using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class MageAssignableBuilding : MonoBehaviour, IComparable
    {
        public bool MenuOpen;
        public Mage InsideMage;
        public int Id;

        [System.Serializable]
        public class Action
        {
            public Sprite sprite;
        }

        public Action[] options;    //For different options on Tower Menu

        public Behaviour highlight;

        // Use this for initialization
        protected virtual void Start () {
            MenuOpen = false;
            highlight = (Behaviour)GetComponent("Halo");
        }
	
        // Update is called once per frame
        protected virtual void Update () {
	
        }

        public int CompareTo(object obj)
        {
            var otherTower = obj as MageAssignableBuilding;
            if (otherTower == null) return -1;
            return Id.CompareTo(otherTower.Id);
        }

        public virtual bool SetMageInside(Mage mage)
        {
            if (InsideMage) return false;
            InsideMage = mage;
            return true;
        }

        public virtual bool EjectMageInside()
        {
            if (!InsideMage) return false;
            InsideMage = null;
            return true;
        }

        public bool IsOccupied()
        {
            return InsideMage != null;
        }

        void OnMouseDown()
        {
            if (!MenuOpen)
            {
                BuildingMenuSpawner.INSTANCE.SpawnMenu(this);
                highlight.enabled = true;
            }
            else
            {
                MenuOpen = false;
                highlight.enabled = false;
            }
        }
    }
}
