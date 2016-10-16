using System;
using System.Collections;
using Assets.Scripts.AndroidNotification;
using Assets.Scripts.Manager;
using Assets.Scripts.Model;
using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_IOS
    using NotificationServices = UnityEngine.iOS.NotificationServices;
    using NotificationType = UnityEngine.iOS.NotificationType;
    using LocalNotification = UnityEngine.iOS.LocalNotification;
#endif


namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        public Mage MagePrefab;
        public PlayerSpell PlayerSpellPrefab;
        public WaveManager WaveManager;
        public SkillManager SkillManager;
		public UIManager UIManager;
        public AudioManager AudioManager;
        public LayerMask FloorMask;
        public LayerMask IgnorePlayerSpell;
        public EventSystem MainEventSystem;

		public Texture[] TowerTextures;
		public Texture[] ShrineTextures;
		public Texture[] MageTextures;
        public Spell[] SpellParticles;
		public Sprite[] ElementIcons;

        private MageFactory _mageFactory;
        public PlayerData Data;

        public bool LoadSavedGame;
        public MageAssignableBuilding[] AllAssignableBuildings;

        public Texture2D SkillAimCursor;
        private Mage _skillMage;

		public ActionWithEvent[] upgrade1Actions;
		private bool _startedUpgrading1;
		private float _lastUpgradeTime1;
		private readonly float _autoUpgradeInterval1 = 0.1f;

		public ActionWithEvent[] upgrade2Actions;
		private bool _startedUpgrading2;
		private float _lastUpgradeTime2;
		private readonly float _autoUpgradeInterval2 = 0.1f;

        // Use this for initialization
        private void Start()
        {
            #if UNITY_IOS
                NotificationServices.RegisterForNotifications(NotificationType.Alert | NotificationType.Badge |NotificationType.Sound);
            #endif
            _mageFactory = new MageFactory(MagePrefab);
            ElementController.Instance.TowerTextures = TowerTextures;
			ElementController.Instance.ShrineTextures = ShrineTextures;
			ElementController.Instance.MageTextures = MageTextures;
            ElementController.Instance.SpellParticles = SpellParticles;
			ElementController.Instance.ElementIcons = ElementIcons;

            for (var i = 0; i < AllAssignableBuildings.Length; i++)
            {
                AllAssignableBuildings[i].SetId(i);
            }

            if (LoadSavedGame)
            {
                Data = SaveLoadHelper.LoadGame();
            }
            
            if (Data != null)
            {
                Data.CreateMagesFromDataArray(_mageFactory, AllAssignableBuildings);
                WaveManager.Data = Data.GetWaveData();
            }
            else
            {
                var loadObject = GameObject.FindGameObjectWithTag("LoadObject");
                if (loadObject)
                {
                    Data = loadObject.GetComponent<SceneLoader>().GetPlayerData();
                } else {
                    Data = new PlayerData(Element.Air);
                }          
                MageListInitializer();
                WaveManager.Data = new WaveData();
                Data.SetWaveData(WaveManager.Data);
            }
            WaveManager.Init();
            WaveManager.SendWave();
            if (LoadSavedGame && PlayerPrefs.GetString("_gameCloseTime") != "") {
                //idle income generation
                //Needed to be called after SendWave, so the minions are initialized
                CalculateIdleIncomeAndShowNotification();
            }

            WaveManager.SendWave();

            MageButtons.Instance.AddPlayerButton();
                        
            foreach (var mage in Data.GetMages())
            {
                MageButtons.Instance.AddMageButton(mage);
            }

            UIManager.SkillCancelButton.SetActive(false);

			AssignActions ();
        }

        private void CalculateIdleIncomeAndShowNotification()
        {
            UIManager.CloseNotifIfOpen();
            var idleManager = new IdleManager(this, WaveManager);
            int killedMinionCount;
            int passedWaveCount;
            var currencyGainedWhileIdle = idleManager.CalculateIdleIncome(out killedMinionCount, out passedWaveCount);
            Data.IncreaseCurrency(currencyGainedWhileIdle);
            Debug.Log("currency gained while idle: " + currencyGainedWhileIdle);
            if (killedMinionCount != 0 && passedWaveCount != 0 && currencyGainedWhileIdle != 0)
            {
                UIManager.CreateNotificications("Welcome back!",
                "Your mages killed " + killedMinionCount + " attackers and earned " + currencyGainedWhileIdle +
                " gold while you were gone.");
            }
        }

        // Update is called once per frame
        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
            //PlayerSpell Targeting
            if (Input.GetMouseButtonDown(0) && !MainEventSystem.IsPointerOverGameObject())
            {
                if (Time.timeScale != 0)
                {
                    Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit floorHit;
                    RaycastHit uiHit;

                    if (!Physics.Raycast(camRay, out uiHit, Mathf.Infinity, IgnorePlayerSpell) &&
                    Physics.Raycast(camRay, out floorHit, Mathf.Infinity, FloorMask))
                    {
                        var floor2Cam = Camera.main.transform.position - floorHit.point;
                        var instantPos = floorHit.point + floor2Cam.normalized * 12;
                        Spell.Clone(ElementController.Instance.GetParticle(Data.GetElement()), Data.GetSpellData(), instantPos,
                                WaveManager.FindClosestMinion(instantPos), null);
                        AudioManager.PlaySpellCastingSound(Data.GetElement());
                    }
                } else
                {
                    foreach (var mage in Data.GetMages())
                    {
                        if (mage.Data.IsDropped())
                        {
                            WaveManager wavemanager = Camera.main.GetComponent<WaveManager>();
                            mage.SetBasePosition(new Vector3(10.9f + (wavemanager.Data.CurrentWave / wavemanager.Data.GetMageDropInterval() + 1), 3.5f, 22f + (wavemanager.Data.CurrentWave / wavemanager.Data.GetMageDropInterval() + 1) * 7f));
                            mage.Data.SetState(MageState.Idle);
                            Time.timeScale = 1;

                            WaveManager.CalculateNextWave();
                        }
                    }
                }           
            }

			if (_startedUpgrading1)
			{
				if (_lastUpgradeTime1 > _autoUpgradeInterval1)
				{
					_lastUpgradeTime1 = 0;
					Data.UpgradePlayer();
				}
				else
				{
					_lastUpgradeTime1 += Time.deltaTime;
				}
			}

			if (_startedUpgrading2)
			{
				if (_lastUpgradeTime2 > _autoUpgradeInterval2)
				{
					_lastUpgradeTime2 = 0;
					Data.UpgradeIdleGenerated();
				}
				else
				{
					_lastUpgradeTime2 += Time.deltaTime;
				}
			}
        }

        public void MageListInitializer()
        {
            for (int i = 0; i < 3; i++)
            {
                var mage = _mageFactory.GetMage(8.5f + 1.2f*i, 8 + 7 * i);
                mage.transform.position = new Vector3(mage.transform.position.x, 3.5f, mage.transform.position.z);
                Data.AddMage(mage);
                mage.Data.SetState(MageState.Idle);
            }
        }

        public void SkillCall(Mage mage) {
            _skillMage = mage;
            UIManager.SkillCancelButton.SetActive(true);
            SkillManager.ExitSkillCancel();
            Cursor.SetCursor(SkillAimCursor, Vector2.zero, CursorMode.Auto);
            SkillManager.CallSkill(mage);
        }

        public void CastSkill(){
            if (_skillMage.CanCast())
            {
                SkillManager.CastSkill(_skillMage, IgnorePlayerSpell, FloorMask, Input.mousePosition, false);
                
            }
            CancelSkillCall();
        }

        public void CancelSkillCall(){
            UIManager.SkillCancelButton.SetActive(false);
            SkillManager.StopAnimations();
            _skillMage = null;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        public void IncreaseCurrency(BigIntWithUnit amount)
        {
            Data.IncreaseCurrency(amount);
            var currencyTextPos = new Vector3(0f, 15f, 35f);
            UIManager.CreateFloatingText(amount.ToString(), UIManager.CurrText.transform, UIManager.CurrText.transform.position + currencyTextPos, "c");
        }


        public void DecreaseCurrency(BigIntWithUnit amount)
        {
            Data.DecreaseCurrency(amount);
            //var currencyTextPos = new Vector3(0f, 15f, 35f);
            //UIManager.CreateFloatingText(amount.ToString(), UIManager.CurrText.transform, UIManager.CurrText.transform.position + currencyTextPos, "c");
        }

        // Minion calls this function, when it is destroyed
        public void MinionDied(Minion minion, BigIntWithUnit currencyGivenOnDeath, float delay)
        {
            if (WaveManager.SafeRemove(minion)){
                IncreaseCurrency(currencyGivenOnDeath);
                AudioManager.PlayMinionDeathSound();
                if (minion.tag == "Boss"){
                    if (minion.Data.HasMageLoot() && !Data.IsMageListFull() && WaveManager.AliveMinionCount == 0)
                    {
                        // Add Mage after the "death" animation of boss finishes.
                        StartCoroutine(AddMage(minion, delay));
                    } else {
                        Destroy(minion.gameObject, delay);
                        IncreaseCurrency(currencyGivenOnDeath);
                        StartCoroutine(SendWave(minion, delay));
                    }
                }
            }
            if (!WaveManager.Data.IsBossWave)
            {
                // Destroy the game object and also send a new wave after the "death" animation of boss finishes.
                Destroy(minion.gameObject, delay);
                if (WaveManager.AliveMinionCount == 0)
                    StartCoroutine(SendWave(minion, delay));
            }      
        }

        IEnumerator AddMage(Minion minion, float delay) {
            yield return new WaitForSeconds(delay);
            var newMage = _mageFactory.GetMage(minion.transform.position.x, minion.transform.position.z);
            if (newMage != null){
                Data.AddMage(newMage);
                MageButtons.Instance.AddMageButton(newMage);
                Time.timeScale = 0;
            }
            Destroy(minion.gameObject);
            StopAllCoroutines();
        }

        IEnumerator SendWave(Minion minion, float delay) {
            yield return new WaitForSeconds(delay);
            Debug.Log("Minions No More");
            WaveManager.CalculateNextWave();
            StopAllCoroutines();
        }
        
        //Can be used for any menu
		public void OpenCloseMenu(GameObject menu, bool open)
        {
            var anim = menu.GetComponent<Animator>();
			anim.SetBool("isDisplayed", !anim.GetBool("isDisplayed") && open);
        }

		public void AssignActions(){
			//for player upgrade
			upgrade1Actions = new ActionWithEvent[3];

			ActionWithEvent upgrade1Action1 = new ActionWithEvent();
			upgrade1Action1.function = delegate
			{
				_startedUpgrading1 = true;
				_lastUpgradeTime1 = 0;
			};
			upgrade1Action1.triggerType = EventTriggerType.PointerDown;

			ActionWithEvent upgrade1Action2 = new ActionWithEvent();
			upgrade1Action2.function = delegate {
				_startedUpgrading1 = false;
			};
			upgrade1Action2.triggerType = EventTriggerType.PointerUp;

			upgrade1Actions[0] = upgrade1Action1;
			upgrade1Actions[1] = upgrade1Action2;

			//for idle currency upgrade
			upgrade2Actions = new ActionWithEvent[3];

			ActionWithEvent upgrade2Action1 = new ActionWithEvent();
			upgrade2Action1.function = delegate
			{
				_startedUpgrading2 = true;
				_lastUpgradeTime2 = 0;
			};
			upgrade2Action1.triggerType = EventTriggerType.PointerDown;

			ActionWithEvent upgrade2Action2 = new ActionWithEvent();
			upgrade2Action2.function = delegate {
				_startedUpgrading2 = false;
			};
			upgrade2Action2.triggerType = EventTriggerType.PointerUp;

			upgrade2Actions[0] = upgrade2Action1;
			upgrade2Actions[1] = upgrade2Action2;

		}

        void ScheduleNotification()

        {
            #if UNITY_IOS

                // schedule notification to be delivered in 5 minutes
                LocalNotification notif = new LocalNotification();

                notif.fireDate = DateTime.Now.AddMinutes(5);

                notif.alertBody = "You've generated more coins!Come back and play!";

                NotificationServices.ScheduleLocalNotification(notif);
            #endif
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (Data == null) return;
            if (pauseStatus)
            {
#if UNITY_IOS

                NotificationServices.ClearLocalNotifications();

                NotificationServices.CancelAllLocalNotifications();

                ScheduleNotification ();

#endif
                PlayerPrefs.SetString("_gameCloseTime", System.DateTime.Now.ToString());
                SaveLoadHelper.SaveGame(Data);
            }
            else
            {
#if UNITY_IOS


                Debug.Log("Local notification count = " + NotificationServices.localNotificationCount);


                if (NotificationServices.localNotificationCount > 0) {

 

                Debug.Log(NotificationServices.localNotifications[0].alertBody);

                }

                // cancel all notifications first.

                NotificationServices.ClearLocalNotifications();

                NotificationServices.CancelAllLocalNotifications();

 

#endif

                CalculateIdleIncomeAndShowNotification();
            }
        }

        void OnApplicationQuit()
        {
            NotificationManager.SendWithAppIcon(TimeSpan.FromMinutes(5), "Notification", "Notification with app icon", new Color(0, 0.6f, 1), NotificationIcon.Message);
            PlayerPrefs.SetString("_gameCloseTime", System.DateTime.Now.ToString());
            SaveLoadHelper.SaveGame(Data);
        }

        public void ResetGame()
        {
			foreach (var minion in WaveManager.GetMinionList()) {
				minion.Data.Kill ();
			}
            Data.DecreaseCurrency(Data.GetCurrency());
			foreach (var building in AllAssignableBuildings) {
				building.EjectMageInside ();
				building.MenuOpen = false;
				building.Highlight.enabled = false;
			}
			UIManager.DestroyTowerMenuCloser ();
			BuildingMenuSpawner.INSTANCE.OpenMenu = null;
            Data.DestroyMages();
            MageListInitializer();
            Data.ResetPlayer();
            WaveManager.Reset();
			MageButtons.Instance.ResetMageMenu ();
			MageButtons.Instance.AddPlayerButton();
			foreach (var mage in Data.GetMages())
			{
				MageButtons.Instance.AddMageButton(mage);
			}
        }
    }
}