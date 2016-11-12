using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Assets.Scripts.UI;

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

        public GameObject Crystal;
        public ParticleSystem ParticleEffect;
        public GameObject[] Banner;

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
        public bool isHighlightOn;
		public GameObject Slot;

        // Use this for initialization
        protected virtual void Start () {
            MenuOpen = false;
			Menu = null;
            isHighlightOn = false;
            ActionWithEvent action = new ActionWithEvent();
            action.function = delegate {
				if (InsideMage != null) {
					InsideMage.Eject(false);
				}
			};
            action.triggerType = EventTriggerType.PointerClick;
            options[0].actions[0] = action;


            if (!InsideMage)
            {
                ParticleEffect.Stop();
            }
        }
	
        // Update is called once per frame
        protected virtual void Update () {

            Crystal.transform.RotateAround(gameObject.transform.position, Vector3.up, 20 * Time.deltaTime);
        }

        public virtual bool SetMageInside(Mage mage)
        {
            if (InsideMage) return false;
            InsideMage = mage;

            ParticleEffect.startColor = ElementController.Instance.GetColor(mage.Data.GetElement());
            ParticleEffect.Play();
            Crystal.gameObject.SetActive(true);

            var texture = ElementController.Instance.GetTower(mage.Data.GetElement());
            Crystal.GetComponent<Renderer>().material.mainTexture = texture;

            for (var i = 0; i < Banner.Length; i++)
            {
                Banner[i].SetActive(true);
                Banner[i].GetComponent<Renderer>().material.mainTexture = texture;
            }
            return true;
        }

        public virtual bool EjectMageInside()
        {
            if (!InsideMage) return false;
            InsideMage = null;

            ParticleEffect.Stop();
            Crystal.gameObject.SetActive(false);

            foreach (GameObject banner in Banner)
            {
                banner.SetActive(false);
            }

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
					InsideMage.Data.ProfileButton.GetComponent<Toggle> ().isOn=true;
                    if ( !isHighlightOn ){
                        StartHighlighting(ElementController.Instance.GetColor(InsideMage.Data.GetElement()));
                    }
				}
                if (IsOccupied())
                {
                    Player.AudioManager.PlayTowerClickSound();
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

        public virtual void DisplayRangeObject()
        {
            RangeObject.transform.position = this.transform.position;
            RangeObject.transform.localScale = new Vector3(2 * InsideMage.GetRange(), 0.01f, 2 * InsideMage.GetRange());
            Color tmp = ElementController.Instance.GetColor(InsideMage.Data.GetElement());
            tmp.a = 0.5f;
            RangeObject.GetComponent<Renderer>().material.color = tmp;
            RangeObject.SetActive(true);
        }

        public void HideRangeObject()
        {
            RangeObject.SetActive(false);
        }

        public void StartHighlighting(Color color){
            if ( gameObject.GetComponent<Renderer>() ){
                var r = gameObject.GetComponent<Renderer>();
                r.material.SetColor("_MainColor", color);
                Tower tower = this as Tower;
                if (tower){
                    r.material.SetFloat("_Dist", 0.002f);
                }else{
                    r.material.SetFloat("_Dist", 0.05f);
                }
                
            }else{
                foreach (var r in gameObject.GetComponentsInChildren<Renderer>())
                {
                    if ( r.name != "Slot"){
                        r.material.SetColor("_MainColor", color);
                        Tower tower = this as Tower;
                        if (tower){
                            r.material.SetFloat("_Dist", 0.002f);
                        }else{
                            r.material.SetFloat("_Dist", 0.05f);
                        }
                    }
                }
            }
            isHighlightOn = true;
        }
        public void StopHighlighting(){
            if ( gameObject.GetComponent<Renderer>() ){
                var r = gameObject.GetComponent<Renderer>();
                r.material.SetFloat("_Dist", 0.000f);
            }else{
                foreach (var r in gameObject.GetComponentsInChildren<Renderer>())
                {
                    if ( r.name != "Slot"){
                        r.material.SetFloat("_Dist", 0.000f);
                    }
                }
            }
            isHighlightOn = false;
        }

    }

    public class ActionWithEvent{
        public UnityAction<BaseEventData> function;
        public EventTriggerType triggerType;
    }
}
