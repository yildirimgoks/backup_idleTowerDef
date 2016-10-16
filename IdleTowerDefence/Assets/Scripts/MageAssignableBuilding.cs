using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Assets.Scripts.Model;

namespace Assets.Scripts
{
    public class MageAssignableBuilding : MonoBehaviour
    {
        public bool MenuOpen;
		public BuildingMenu Menu;
        public Mage InsideMage;
        private int _id;
		public Player Player;
        public GameObject RangeObject;

		private float clickTime;

        [System.Serializable]
        public class Action
        {
            public Sprite sprite;
			public bool condition;
			// public UnityAction function;
            public ActionWithEvent[] actions = new ActionWithEvent[3];
        }

        public Action[] options;    //For different options on Tower Menu

        public Behaviour Highlight;
		public GameObject Slot;

        // Use this for initialization
        protected virtual void Start () {
            MenuOpen = false;
			Menu = null;
            Highlight = (Behaviour)GetComponent("Halo");
			Slot = transform.FindChild ("Slot").gameObject;
            ActionWithEvent action = new ActionWithEvent();
            action.function = delegate {
				if (InsideMage != null) {
					InsideMage.Eject(false);
				}
			};
            action.triggerType = EventTriggerType.PointerClick;
            options[0].actions[0] = action;
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
				clickTime = Time.time;
			}
    	}

		void OnMouseDrag()
		{
			if (IsOccupied ()) {
				if (Time.time - clickTime > 0.25) {
					if (MenuOpen) {
						Menu.CloseMenu (Menu);
					}
					InsideMage.Eject(true);
				}
			}
		}

		void OnMouseUp()
		{
			if (Time.time - clickTime < 0.25) {
				if (!MenuOpen) {
					BuildingMenuSpawner.INSTANCE.SpawnMenu (this);
					InsideMage.ProfileButton.GetComponent<Toggle> ().isOn=true;
					if (!Highlight.enabled) {
						Highlight.enabled = true;
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

        public void DisplayRangeObject()
        {
            if (RangeObject != null)
            {
                RangeObject.transform.position = this.transform.position;
                RangeObject.transform.localScale = new Vector3(InsideMage.GetRange(), 0.01f, InsideMage.GetRange());
                RangeObject.SetActive(true);
            }         
        }

        public void HideRangeObject()
        {
            if (RangeObject != null)
            {
                RangeObject.SetActive(false);
            }
        }
    }

    public class ActionWithEvent{
        public UnityAction<BaseEventData> function;
        public EventTriggerType triggerType;
    }
}
