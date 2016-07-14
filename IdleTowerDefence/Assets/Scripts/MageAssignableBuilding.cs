using System;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts
{
    public class MageAssignableBuilding : MonoBehaviour, IComparable
    {
        public bool MenuOpen;
        public Mage InsideMage;
        public int Id;

		public Player Player;

        [System.Serializable]
        public class Action
        {
            public Sprite sprite;
			public UnityAction function;
        }

        public Action[] options;    //For different options on Tower Menu

        public Behaviour Highlight;

        // Use this for initialization
        protected virtual void Start () {
            MenuOpen = false;
            Highlight = (Behaviour)GetComponent("Halo");
			options [0].function = delegate {
				if (InsideMage != null) {
					InsideMage.Eject ();
				}
			};
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
			if (!Player.MainEventSystem.IsPointerOverGameObject ()) {
				if (!MenuOpen) {
					BuildingMenuSpawner.INSTANCE.SpawnMenu (this);
					Highlight.enabled = true;
				} else {
					MenuOpen = false;
					Highlight.enabled = false;
				}
			}
    	}
    }
}
