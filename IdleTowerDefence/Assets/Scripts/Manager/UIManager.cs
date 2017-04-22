using Assets.Scripts.Model;
using Assets.Scripts.Pooling;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
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

		public Text DamageBonusText;
		public Text CurrencyBonusText;

        private GameObject _openNotif;

		public GameObject AchievementPopup;
	
        // Update is called once per frame
        void Update ()
        {
            Cheats();
            UpdateLabels();
        }

        public void OnSceneChange()
        {
            MainUi.GetComponent<Canvas>().worldCamera = Camera.main;
            TowerUi.GetComponent<Canvas>().worldCamera = Camera.main;
            NonintUi.GetComponent<Canvas>().worldCamera = Camera.main;
        }

        private void Cheats()
        {
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

            if (Input.GetKeyDown(KeyCode.P))
            {
                AchievementPopup.GetComponent<Animator>().SetTrigger("Popup");
                var texts = AchievementPopup.GetComponentsInChildren<Text>();
                texts[0].text = "Deneme";
                texts[1].text = "meraba";
            }

            //Reset wave count
            if (Input.GetKeyDown(KeyCode.R))
            {
                Player.ResetGame();
            }
        }

        private void UpdateLabels()
        {
            if (Player.Data != null)
            {
                CurrText.text = Player.Data.GetCurrency().ToString();
                WaveLifeText.text = Player.WaveManager.WaveLife.ToString();
                if (Player.WaveManager.WaveLife != 0)
                {
                    WaveLifeBar.value = Player.WaveManager.WaveLife/Player.WaveManager.TotalWaveLife;
                }
                else
                {
                    WaveLifeBar.value = 0;
                }
                MageText.text = Player.Data.CumulativeDps().ToString();

                if (Player.WaveManager.Data.CurrentWave == 0)
                {
                    PrevWave.text = "";
                }
                else
                {
                    if (Player.WaveManager.Data.IsPreviousWaveBossWave)
                    {
                        PrevWaveImage.color = new Color(0.78f, 0.78f, 0.78f, 0.5f);
                    }
                    else
                    {
                        PrevWaveImage.color = Color.white;
                    }
                    PrevWave.text = Player.WaveManager.Data.CurrentWave.ToString();
                }

                CurrWave.text = (Player.WaveManager.Data.CurrentWave + 1).ToString();

                if (Player.WaveManager.Data.CurrentWave == Player.WaveManager.Data.GetMaxReachedWave())
                {
                    NextWave.text = "";
                    NextWaveImage.sprite = EmptyWaveButton;
                }
                else
                {
                    NextWave.text = (Player.WaveManager.Data.CurrentWave + 2).ToString();
                    NextWaveImage.sprite = FullWaveButton;
                }
            }
        }

        //Can be used for any menu
        public void OpenCloseMenu(GameObject menu, bool open)
        {
            var anim = menu.GetComponent<Animator>();
            anim.SetBool("isDisplayed", open);
        }

		public void DirectlyOpenCloseMenu(GameObject menu, bool open)
		{
			var anim = menu.GetComponent<Animator> ();
			if (open) {
				anim.CrossFade ("Menu_in", 0f);
				anim.CrossFade ("Menu_idle", 0f);
			} else {
				anim.CrossFade ("Menu_out", 0f);
				anim.CrossFade ("Menu_idle", 0f);

			}
			anim.SetBool ("isDisplayed", open);

		}

        //For handling popup damage text
        public void CreateFloatingText(string text, Transform location, Vector3 pos, bool damage)
        {
            FloatingText instance = (FloatingText) PoolableMonoBehaviour.GetPoolable(PopupDmgText);
            instance.SetType(damage);
            instance.transform.SetParent(NonintUi.transform, false);
            instance.transform.position = pos;
            instance.SetText(text);
            instance.StartAnimating();
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
				Destroy (CurrentMainMenuCloser.gameObject);
				CurrentMainMenuCloser = null;
			}
		}

		public void DestroyTowerMenuCloser(){
			if (CurrentTowerMenuCloser != null) {
				Destroy (CurrentTowerMenuCloser.gameObject);
				CurrentTowerMenuCloser = null;
			}
		}

        public void showAchievementPopup(Achievement ach)
        {
			AchievementPopup.GetComponent<Animator>().SetTrigger("Popup");
			var texts = AchievementPopup.GetComponentsInChildren<Text> ();
			texts [0].text = ach.getTitle ();
			texts [1].text = ach.getSubTitle ();
        }

        public static void SetButtonEvent(Button button, ActionWithEvent actionWithEvent)
        {
            if (actionWithEvent == null) return;
            EventTrigger trigger = button.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = actionWithEvent.TriggerType;
            entry.callback.AddListener(actionWithEvent.Function);
            trigger.triggers.Add(entry);
        }

		public void OpenCurrencyBonus(float modifier){
			CurrencyBonusText.gameObject.SetActive (true);
			CurrencyBonusText.text = "x" + modifier;
		}

		public void OpenDamageBonus(float modifier){
			DamageBonusText.gameObject.SetActive (true);
			DamageBonusText.text = "x" + modifier;
		}

		public void CloseCurrencyBonus(){
			CurrencyBonusText.gameObject.SetActive (false);
		}

		public void CloseDamageBonus(){
			DamageBonusText.gameObject.SetActive (false);
		}

    }
}
