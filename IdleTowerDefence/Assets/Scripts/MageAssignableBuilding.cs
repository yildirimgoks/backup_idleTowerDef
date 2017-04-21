using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Assets.Scripts.UI;
using Assets.Scripts.Manager;

namespace Assets.Scripts
{
    public class MageAssignableBuilding : MonoBehaviour
    {
        public bool MenuOpen;
		public BuildingMenu Menu;
        public Mage InsideMage;
        private int _id;
		private Player _player;

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
        protected virtual void Start ()
        {
            MenuOpen = false;
			Menu = null;
            isHighlightOn = false;
            ActionWithEvent action = new ActionWithEvent();
            action.Function = delegate {
				if (InsideMage != null) {
					InsideMage.Eject(false);
				}
			};
            action.TriggerType = EventTriggerType.PointerClick;
            options[0].actions[0] = action;
            if (!InsideMage)
            {
                ParticleEffect.Stop();
            }
        }
	
        // Update is called once per frame
        protected virtual void Update ()
        {
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
                if (Banner[i])
                {
                    Banner[i].SetActive(true);
                    Banner[i].GetComponent<Renderer>().material.mainTexture =
                        ElementController.Instance.GetShrine(mage.Data.GetElement());
                        //Works with old function gor shrines
                }
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
						Menu.CloseMenu();
					}
					InsideMage.Eject(true);
				}
			}
		}

		void OnMouseUp()
		{
			if (Time.time - clickTime < 0.25) {
				if (!MenuOpen) {
					_player.BuildingMenuSpawner.SpawnMenu (this);
					InsideMage.Data.ProfileButton.GetComponent<Toggle> ().isOn=true;
                    if ( !isHighlightOn ){
                        // StartHighlighting(ElementController.Instance.GetColor(InsideMage.Data.GetElement()));
                        StartHighlighting();
                    }
				}
                if (IsOccupied())
                {
                    _player._audioManager.PlayTowerClickSound();
                }
			}
		}

        public void Initialize(int id, Player player)
        {
            _id = id;
            _player = player;
        }

        public int GetId()
        {
            return _id;
        }

        public virtual void DisplayRangeObject()
        {
            if (!InsideMage) return;
            var tempPosition = transform.position;
            tempPosition.y = 0;
            _player.RangeObject.transform.position = tempPosition;
            _player.RangeObject.transform.localScale = new Vector3(2 * InsideMage.GetRange(), 0.01f, 2 * InsideMage.GetRange());
            var tempColor = ElementController.Instance.GetColor(InsideMage.Data.GetElement());
            tempColor.a = 0.5f;
            _player.RangeObject.GetComponent<Renderer>().material.color = tempColor;
            _player.RangeObject.SetActive(true);
        }

        public void HideRangeObject()
        {
            _player.RangeObject.SetActive(false);
        }

        public void StartHighlighting(){
            this.StartHighlighting(new Color(0.8f,0.8f,0.8f,1.0f));
        }

        public void StartHighlighting(Color color){
            if ( gameObject.GetComponent<Renderer>() ){
                var r = gameObject.GetComponent<Renderer>();
                r.material.SetColor("_MainColor", color);
                Tower tower = this as Tower;
                if (tower){
                    r.material.SetFloat("_Dist", 0.25f);
                }else{
                    r.material.SetFloat("_Dist", 0.002f);
                } 
            }
            foreach (var r in gameObject.GetComponentsInChildren<Renderer>())
            {
                if ( r.name != "Slot"){
                    r.material.SetColor("_MainColor", color);
                    Tower tower = this as Tower;
                    if (tower){
                        r.material.SetFloat("_Dist", 0.25f);
                    }else{
                        r.material.SetFloat("_Dist", 0.002f);
                    }
                }
            }
            isHighlightOn = true;
        }
        public void StopHighlighting(){
            if ( gameObject.GetComponent<Renderer>() ){
                var r = gameObject.GetComponent<Renderer>();
                r.material.SetFloat("_Dist", 0.000f);
            }
            foreach (var r in gameObject.GetComponentsInChildren<Renderer>())
            {
                if ( r.name != "Slot"){
                    r.material.SetFloat("_Dist", 0.000f);
                }
            }
            isHighlightOn = false;
        }

    }

    public class ActionWithEvent{
        public UnityAction<BaseEventData> Function;
        public EventTriggerType TriggerType;
    }
}
