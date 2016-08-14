using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Assets.Scripts
{
    public class MageAssignableBuilding : MonoBehaviour
    {
        public bool MenuOpen;
        public Mage InsideMage;
        private int _id;
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
			if (IsOccupied()) {
				if (!MenuOpen) {
					BuildingMenuSpawner.INSTANCE.SpawnMenu (this);
					InsideMage.ProfileButton.GetComponent<Toggle> ().isOn=true;
					if (!Highlight.enabled) {
						Highlight.enabled = true;
					}
				} else {
					/*if (Towerın önünde sadece Tower button varsa içeri almıycak bir koşul(transformlu falan belik))*/{
						InsideMage.ProfileButton.GetComponent<Toggle> ().isOn = false;
						MenuOpen = false;
						Highlight.enabled = false;
					}
				}
			}
    	}

        public void SetId(int id)
        {
            _id = id;
        }

        public int GetId()
        {
            return _id;
        }
    }
}
