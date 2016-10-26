using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.Manager
{
    public class UIManager : MonoBehaviour
    {
        public Player Player;

        public Text CurrText;
        public Text WaveLifeText;
        public Text MageText;
        public Text PrevWave;
        public Text CurrWave;
        public Text NextWave;
		public Image PrevWaveImage;
        public Image NextWaveImage;
        public Sprite EmptyWaveButton;
        public Sprite FullWaveButton;
        public Slider WaveLifeBar;

        public FloatingText PopupDmgText;
        public FloatingText PopupCurText;
        public GameObject NonintUi;

		public GameObject Notification;
		public GameObject MainUi;

		public GameObject MenuCloser;
		public GameObject CurrentMainMenuCloser;
		public GameObject CurrentTowerMenuCloser;
		public GameObject NextTowerMenuCloser;
		public GameObject TowerUi;
        public GameObject SkillCancelButton;

        private GameObject _openNotif;

        // Use this for initialization
        void Start () {
			CurrentMainMenuCloser = null;
			CurrentTowerMenuCloser = null;
        }
	
        // Update is called once per frame
        void Update () {
            //1M Currency Cheat
            if (Input.GetKeyDown(KeyCode.M))
            {
                Player.Data.IncreaseCurrency(1000000);
            }

            // Kill wave cheat
            if (Input.GetKeyDown(KeyCode.K))
            {
                foreach (var minion in Player.WaveManager.GetMinionList())
                {
                    minion.Data.Kill();
                }
            }

            //Reset wave count
            if (Input.GetKeyDown(KeyCode.R))
            {
                Player.ResetGame();
            }
            UpdateLabels();
        }

        private void UpdateLabels()
        {
            CurrText.text = Player.Data.GetCurrency().ToString();
            WaveLifeText.text = Player.WaveManager.WaveLife.ToString();
            if (Player.WaveManager.WaveLife != 0)
            {
                WaveLifeBar.value = Player.WaveManager.WaveLife/Player.WaveManager.TotalWaveLife;
            } else {
                WaveLifeBar.value = 0;
            }
            MageText.text = Player.Data.CumulativeDps().ToString();

            if (Player.WaveManager.Data.CurrentWave == 0) {
                PrevWave.text = "";
            } else {
				if (Player.WaveManager.Data.IsPreviousWaveBossWave) {
					PrevWaveImage.color = new Color(0.78f, 0.78f, 0.78f, 0.5f);
				} else {
					PrevWaveImage.color = Color.white;
				}
                PrevWave.text = Player.WaveManager.Data.CurrentWave.ToString ();
            }

            CurrWave.text = (Player.WaveManager.Data.CurrentWave+1).ToString();

            if (Player.WaveManager.Data.CurrentWave == Player.WaveManager.Data.GetMaxReachedWave ()) {
                NextWave.text = "";
                NextWaveImage.sprite = EmptyWaveButton;
            } else {
                NextWave.text = (Player.WaveManager.Data.CurrentWave + 2).ToString ();
                NextWaveImage.sprite = FullWaveButton;
            }

        }

        //Can be used for any menu
        public void OpenCloseMenu(GameObject menu, bool open)
        {
            var anim = menu.GetComponent<Animator>();
            anim.SetBool("isDisplayed", !anim.GetBool("isDisplayed") && open);
        }

        //For handling popup damage text
        public void CreateFloatingText(string text, Transform location, Vector3 pos, string setting)
        {
            FloatingText instance = null;
            if (setting == "d")
            {
                instance = Instantiate(PopupDmgText);
            }
            else if (setting == "c")
            {
                instance = Instantiate(PopupCurText);
            }
            instance.transform.SetParent(NonintUi.transform, false);
            instance.transform.position = pos;
            instance.SetText(text);
        }

		//For Notifications
		public void CreateNotificications(string header, string text){
            var notif = Instantiate(Notification);
			var texts = notif.GetComponentsInChildren<Text>();
			notif.transform.SetParent(MainUi.transform, false);
			notif.GetComponent<Button>().onClick.AddListener(delegate {
                _openNotif = null;
				Destroy(notif);
			});
			texts [0].text = header;
			texts [1].text = text;
		    _openNotif = notif;
			OpenCloseMenu(notif, true);
		}

        public void CloseNotifIfOpen()
        {
            if (!_openNotif) return;
            Destroy(_openNotif);
            _openNotif = null;
        }

		public void CreateMainMenuCloser(UnityAction closingAction){//true if Main UI,false if Tower UI
			if (CurrentMainMenuCloser == null) {
				CurrentMainMenuCloser = Instantiate (MenuCloser);
				var ui=MainUi;
                CurrentMainMenuCloser.transform.SetParent (ui.transform, false);
				CurrentMainMenuCloser.transform.SetAsFirstSibling ();
                CurrentMainMenuCloser.GetComponentInChildren<Button>().onClick.AddListener (closingAction);
			}
		}

		public void CreateTowerMenuCloser(UnityAction closingAction){
			if (CurrentTowerMenuCloser == null) {
				CurrentTowerMenuCloser = Instantiate (MenuCloser);
				var ui = TowerUi;
				CurrentTowerMenuCloser.transform.SetParent (ui.transform, false);
				CurrentTowerMenuCloser.transform.SetAsFirstSibling ();
				CurrentTowerMenuCloser.GetComponentInChildren<Button> ().onClick.AddListener (closingAction);
			}
		}

		public void DestroyMainMenuCloser(){
			if (CurrentMainMenuCloser != null) {
				Destroy (CurrentMainMenuCloser);
				CurrentMainMenuCloser = null;
			}
		}

		public void DestroyTowerMenuCloser(){
			if (CurrentTowerMenuCloser != null) {
				Destroy (CurrentTowerMenuCloser);
				CurrentTowerMenuCloser = null;
			}
		}
    }
}
