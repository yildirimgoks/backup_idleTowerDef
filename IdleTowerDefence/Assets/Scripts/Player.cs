using System;
using System.Collections;
using System.Linq;
using Assets.Scripts.AndroidNotification;
using Assets.Scripts.Manager;
using Assets.Scripts.Model;
using Assets.Scripts.UI;
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
        public WaveManager WaveManager;
        public MageUpgradeManager MageUpgradeManager;
        public SkillManager SkillManager;
		public UIManager UIManager;
        public LayerMask FloorMask;
        public LayerMask IgnorePlayerSpell;
        public EventSystem MainEventSystem;
        public AchievementManager AchievementManager;
        public DailyBonusManager DailyBonusManager;
        public AudioManager _audioManager;
        public SceneLoader SceneLoader;
        public MageButtons MageButtons;
        public TutorialManager TutorialManager;
        public TelevoleManager TelevoleManager;
        public BuildingMenuSpawner BuildingMenuSpawner;

        public Texture[] TowerTextures;
		public Texture[] ShrineTextures;
		public Texture[] MageTextures;
        public Spell[] SpellParticles;
        //public Spell[] SpellCollisionParticles;
		public Sprite[] ElementIcons;

        private MageFactory _mageFactory;
        public PlayerData Data;
        
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

        public GameObject RangeObject;

        public ToggleSlider SFXSlider;
        public ToggleSlider MusicSlider;
        
		public AdManager AdManager;

        private float _currencyModifier = 1f;
        private DateTime _currencyModifierEndTime;

        private float _damageModifier = 1f;
        private DateTime _damageModifierEndTime;

        private bool _shouldHandleDroppedMage;
        private SceneReferenceManager _sceneReferenceManager;

        public enum AdSelector
        {
            Damage = 0, //can add more
            Currency = 1
        }

        public AdSelector CurrentBonus = AdSelector.Damage;

        // Use this for initialization
        private void Start()
        {
            #if UNITY_IOS
				NotificationServices.RegisterForNotifications(NotificationType.Alert | NotificationType.Badge |NotificationType.Sound, false);
            #endif

            _mageFactory = new MageFactory(MageUpgradeManager.MagePrefabs, this);
            ElementController.Instance.TowerTextures = TowerTextures;
			ElementController.Instance.ShrineTextures = ShrineTextures;
			ElementController.Instance.MageTextures = MageTextures;
            ElementController.Instance.SpellParticles = SpellParticles;
			ElementController.Instance.ElementIcons = ElementIcons;

            MageUpgradeManager.Init();

            if (PlayerPrefs.GetInt("sfxMute") == 1)
            {
                SFXSlider.AssignSlider();  
                if (_audioManager && _audioManager.SFXAudio.mute)
                {
                    _audioManager.ToggleSound();
                }             
                SFXSlider.ChangeValue();
            }

            if (PlayerPrefs.GetInt("musicMute") == 1)
            {
                MusicSlider.AssignSlider();
                if (_audioManager && _audioManager.MusicAudio.mute)
                {
                    _audioManager.ToggleMusic();
                }
                MusicSlider.ChangeValue();
            }
            
			//Currency Bonus
			if (!string.IsNullOrEmpty(PlayerPrefs.GetString ("_currencyBonusEndTime"))) {
				_currencyModifierEndTime = DateTime.Parse (PlayerPrefs.GetString ("_currencyBonusEndTime"));
				if (_currencyModifierEndTime < DateTime.Now) {
					var pastTime = AdManager.BonusTime - (DateTime.Now - _currencyModifierEndTime).TotalSeconds;
					_currencyModifier = PlayerPrefs.GetFloat ("_currencyModifier");
					UIManager.OpenCurrencyBonus (_currencyModifier);
					AdManager.Timer.Cooldown (AdManager.BonusTime, (float)pastTime);
				}
			}
			//Damage Bonus
			if (!string.IsNullOrEmpty(PlayerPrefs.GetString ("_damageBonusEndTime"))) {
				_damageModifierEndTime = DateTime.Parse (PlayerPrefs.GetString ("_damageBonusEndTime"));
				if (_damageModifierEndTime < DateTime.Now) {
					var pastTime = AdManager.BonusTime - (DateTime.Now - _damageModifierEndTime).TotalSeconds;
					_damageModifier = PlayerPrefs.GetFloat ("_damageModifier");
					UIManager.OpenDamageBonus (_damageModifier);
					AdManager.Timer.Cooldown (AdManager.BonusTime, (float)pastTime);
				}
			}
        }

        public void OnFirstSceneLoaded()
        {
            UIManager.gameObject.SetActive(true);
            Data = SceneLoader.GetPlayerData();
            if (SceneLoader.IsLoadSuccesfull())
            {
                InitGameForLoadedData();
            }
            else
            {
                if (Data == null)
                {
                    Data = new PlayerData(Element.Air);
                }

                InitializeGameForFirstPlay();
            }
            
            MageButtons.OnFirstSceneLoaded();

            MageButtons.AddPlayerButton();

            UIManager.SkillCancelButton.SetActive(false);

            AssignActions();

            // Give Daily Bonus
            DailyBonusManager.InitiateRewardPage();

            AchievementManager.SetAchievementKeeper(Data.GetAchievementData());
            OnSceneChange(Data.GetLoadedScene());
            if (PlayerPrefs.GetString("_gameCloseTime") != "")
            {
                //idle income generation
                CalculateIdleIncomeAndShowNotification();
            }
        }

        public void OnSceneChange(string sceneName)
        {
            UIManager.OnSceneChange();
            Data.SetCurrentScene(sceneName);
            _sceneReferenceManager = GameObject.FindObjectOfType<SceneReferenceManager>();
            _mageFactory.SetStationObjects(_sceneReferenceManager.StationObjects);
            WaveManager.SetWaypoints(_sceneReferenceManager.StartWaypoint, _sceneReferenceManager.EndWaypoint);
            for (var i = 0; i < _sceneReferenceManager.AllAssignableBuildings.Length; i++)
            {
                _sceneReferenceManager.AllAssignableBuildings[i].Initialize(i, this);
            }
            InitializeMages();
            StartCoroutine(WaveManager.SendWave());
        }

        private void InitializeMages()
        {
            Data.CreateMagesFromDataArray(_mageFactory);
            foreach (var mage in Data.GetMages())
            {
                mage.Initialize(this);
            }
            Data.PutMagesToBuildings(_sceneReferenceManager.AllAssignableBuildings);
            MageButtons.RemoveMageButtons();
            foreach (var mage in Data.GetMages())
            {
                MageButtons.AddMageButton(mage);
            }
        }

        private void InitGameForLoadedData()
        {
            WaveManager.Data = Data.GetWaveData();
            WaveManager.Init();
        }

        private void InitializeGameForFirstPlay()
        {
            // Reset player prefs to avoid possible bugs
            ResetPlayerPrefs();

            Data.InitializeMageDataArrayForStartup(_mageFactory);
            WaveManager.Data = new WaveData();
            WaveManager.Init();
            Data.SetWaveData(WaveManager.Data);
        }

        private void CalculateIdleIncomeAndShowNotification()
        {
            UIManager.CloseNotifIfOpen();
            var idleManager = new IdleManager(this, WaveManager);
            int killedMinionCount;
            int passedWaveCount;
            var currencyGainedWhileIdle = idleManager.CalculateIdleIncome(out killedMinionCount, out passedWaveCount);
            Data.IncreaseCurrency(currencyGainedWhileIdle);
            //Debug.Log("currency gained while idle: " + currencyGainedWhileIdle);
            if (currencyGainedWhileIdle != 0)
            {
                if (killedMinionCount == 0)
                {
                    UIManager.CreateNotificications("Welcome back!",
                    "Your mages earned " + currencyGainedWhileIdle +
                    " gold while you were gone.");
                }
                else
                {
                    UIManager.CreateNotificications("Welcome back!",
                    "Your mages killed " + killedMinionCount + " attackers and earned " + currencyGainedWhileIdle +
                    " gold while you were gone.");
                }
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
            Vector3? inputPosition = null;
            if (Input.touches.Length == 0 && Input.GetMouseButtonDown(0) && !MainEventSystem.IsPointerOverGameObject())
            {
                inputPosition = Input.mousePosition;
            }
            foreach (var touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began && !MainEventSystem.IsPointerOverGameObject(touch.fingerId))
                {
                    inputPosition = new Vector3(touch.position.x, touch.position.y, 0);
                    break;
                }
            }

            if (inputPosition != null)
            {
                    Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit floorHit;
                    RaycastHit uiHit;

                    if (!Physics.Raycast(camRay, out uiHit, Mathf.Infinity, IgnorePlayerSpell) &&
                    Physics.Raycast(camRay, out floorHit, Mathf.Infinity, FloorMask))
                    {
                        var floor2Cam = Camera.main.transform.position - floorHit.point;
                        var instantPos = floorHit.point + floor2Cam.normalized * 15;
                        Spell.Clone(this, ElementController.Instance.GetParticle(Data.GetElement()), Data.GetSpellData(), instantPos,
                                WaveManager.FindClosestMinion(instantPos), null);
                        if (_audioManager)
                        {
                            _audioManager.PlaySpellCastingSound(Data.GetElement());
                        }
                    }
                if (_shouldHandleDroppedMage)
                {
                    HandleDroppedMage();
                }           
            }

			if (_startedUpgrading1)
			{
				if (_lastUpgradeTime1 > _autoUpgradeInterval1)
				{
					_lastUpgradeTime1 = 0;
					UpgradePlayer();
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
					UpgradeIdleGenerated();
				}
				else
				{
					_lastUpgradeTime2 += Time.deltaTime;
				}
			}

            if (_currencyModifier != 1 && _currencyModifierEndTime < DateTime.Now)
            {
                _currencyModifier = 1;
				UIManager.CloseCurrencyBonus ();
            }

            if (_damageModifier != 1 && _damageModifierEndTime < DateTime.Now)
            {
                _damageModifier = 1;
				UIManager.CloseDamageBonus ();
            }
        }

        private void HandleDroppedMage()
        {
            foreach (var mage in Data.GetMages())
            {
                if (mage.Data.IsDropped())
                {
                    mage.SetBasePosition(_sceneReferenceManager.StationObjects[Data.GetMages().Count() - 1].transform.position);
                    mage.Data.SetState(MageState.Idle);
                    //Time.timeScale = 1;
                    _shouldHandleDroppedMage = false;
                    WaveManager.CalculateNextWave();
                }
            }
        }

        public void SkillCall(Mage mage) {
			_skillMage = mage;
			UIManager.SkillCancelButton.SetActive (true);
			SkillManager.ExitSkillCancel ();
			Cursor.SetCursor (SkillAimCursor, Vector2.zero, CursorMode.Auto);
			SkillManager.CallSkill (mage);
        }

        public void CastSkill(){
            SkillManager.CastSkill(_skillMage, FloorMask, Input.mousePosition, false);
            CancelSkillCall();
        }

        public void CancelSkillCall(){
            UIManager.SkillCancelButton.SetActive(false);
            SkillManager.StopAnimations();
            _skillMage = null;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        public void IncreaseCurrency(BigIntWithUnit amount, Vector3 objpos)
        {
            if (amount == 0) return;

            amount *= _currencyModifier;
            Data.IncreaseCurrency(amount);

            AchievementManager.RegisterEvent(AchievementType.Earn, amount);
            var currencyTextPos = new Vector3(0f, 12f, 0f);
            UIManager.CreateFloatingText(amount.ToString(), UIManager.CurrText.transform, objpos + currencyTextPos, false);
        }

        public float GetModifier(AdSelector type)
        {
            switch (type)
            {
                case AdSelector.Currency:
                    return _currencyModifier;
                case AdSelector.Damage:
                    return _damageModifier;
            }
            return 1.0f;
        }

        public DateTime GetModifierTime(AdSelector type)
        {
            switch (type)
            {
                case AdSelector.Currency:
                    return _currencyModifierEndTime;
                case AdSelector.Damage:
                    return _damageModifierEndTime;
            }
            return DateTime.Now;
        }

        public void SetModifier(AdSelector type, float modifier, int time)
        {
            //sets the modifier as the new modifier 
            //DO NOT FORGET TO PASS MODIFIER >=1 TO MAKE IT ACTUALLY A BONUS
            switch (type)
            {
			case AdSelector.Currency:
				_currencyModifierEndTime = DateTime.Now.AddSeconds (time);
				PlayerPrefs.SetString ("_currencyBonusEndTime", _currencyModifierEndTime.ToString ());
				_currencyModifier *= modifier;
				PlayerPrefs.SetFloat ("_currencyModifier", modifier);
				UIManager.OpenCurrencyBonus (modifier);
                    return;
			case AdSelector.Damage:
				_damageModifierEndTime = DateTime.Now.AddSeconds (time);
				PlayerPrefs.SetString ("_damageBonusEndTime", _damageModifierEndTime.ToString ());
				_damageModifier *= modifier;
				PlayerPrefs.SetFloat ("_damageModifier", modifier);
				UIManager.OpenDamageBonus (modifier);
                    return;
            }
        }

		public void UpgradePlayer()
		{
			if (Data.GetCurrency() < Data.GetUpgradePrice()) return;
			DecreaseCurrency(Data.GetUpgradePrice());
			Data.UpgradePlayer();
		}

		public void UpgradeIdleGenerated()
		{
			if (Data.GetCurrency() < Data.GetIdleUpgradePrice()) return;
			DecreaseCurrency(Data.GetIdleUpgradePrice());
			Data.UpgradeIdleGenerated();
		}

        public void DecreaseCurrency(BigIntWithUnit amount)
        {
            Data.DecreaseCurrency(amount);

            AchievementManager.RegisterEvent(AchievementType.Spend, amount);

            var currencyTextPos = new Vector3(-9f, 0f, 19f);
			UIManager.CreateFloatingText("-" + amount.ToString(), UIManager.CurrText.transform, UIManager.CurrText.transform.position + currencyTextPos, true);
        }

        // Minion calls this function, when it is destroyed
        public void MinionDied(Minion minion, BigIntWithUnit currencyGivenOnDeath, float delay)
        {
            if (WaveManager.SafeRemove(minion)){
                IncreaseCurrency(currencyGivenOnDeath, minion.transform.position);
                if (_audioManager)
                {
                    _audioManager.PlayMinionDeathSound();
                }
                if (minion.tag == "Boss"){
                    if (minion.Data.HasMageLoot() && !Data.IsMageListFull() && WaveManager.AliveMinionCount == 0)
                    {
                        // Add Mage after the "death" animation of boss finishes.
                        StartCoroutine(AddMage(minion, delay));
                    } else {
                        Destroy(minion.gameObject, delay);
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
            var newMage = _mageFactory.CreateMage(minion.transform.position);
            newMage.Initialize(this);
            if (newMage != null){
                Data.AddMage(newMage);
                MageButtons.AddMageButton(newMage);
                //Time.timeScale = 0;
            }
            Destroy(minion.gameObject);
            StopAllCoroutines();
            _shouldHandleDroppedMage = true;
        }

        IEnumerator SendWave(Minion minion, float delay) {
            yield return new WaitForSeconds(delay);
            WaveManager.CalculateNextWave();
            StopAllCoroutines();
        }

		public void AssignActions(){
			//for player upgrade
			upgrade1Actions = new ActionWithEvent[3];

			ActionWithEvent upgrade1Action1 = new ActionWithEvent();
			upgrade1Action1.Function = delegate
			{
				_startedUpgrading1 = true;
				_lastUpgradeTime1 = 0;
			};
			upgrade1Action1.TriggerType = EventTriggerType.PointerDown;

			ActionWithEvent upgrade1Action2 = new ActionWithEvent();
			upgrade1Action2.Function = delegate {
				_startedUpgrading1 = false;
			};
			upgrade1Action2.TriggerType = EventTriggerType.PointerUp;

			ActionWithEvent upgrade1Action3 = new ActionWithEvent();
			upgrade1Action3.Function = delegate {
				UpgradePlayer();
			};
			upgrade1Action3.TriggerType = EventTriggerType.PointerClick;

			upgrade1Actions[0] = upgrade1Action1;
			upgrade1Actions[1] = upgrade1Action2;
			upgrade1Actions[2] = upgrade1Action3;

			//for idle currency upgrade
			upgrade2Actions = new ActionWithEvent[3];

			ActionWithEvent upgrade2Action1 = new ActionWithEvent();
			upgrade2Action1.Function = delegate
			{
				_startedUpgrading2 = true;
				_lastUpgradeTime2 = 0;
			};
			upgrade2Action1.TriggerType = EventTriggerType.PointerDown;

			ActionWithEvent upgrade2Action2 = new ActionWithEvent();
			upgrade2Action2.Function = delegate {
				_startedUpgrading2 = false;
			};
			upgrade2Action2.TriggerType = EventTriggerType.PointerUp;

			ActionWithEvent upgrade2Action3 = new ActionWithEvent();
			upgrade2Action3.Function = delegate {
				UpgradeIdleGenerated();
			};
			upgrade2Action3.TriggerType = EventTriggerType.PointerClick;

			upgrade2Actions[0] = upgrade2Action1;
			upgrade2Actions[1] = upgrade2Action2;
			upgrade2Actions[2] = upgrade2Action3;

		}

        void ScheduleNotification()
        {
#if UNITY_IOS

            var damageBonusTime = _damageModifierEndTime - DateTime.Now;
            var moneyBonusTime = _currencyModifierEndTime - DateTime.Now;
            //schedule notification
            if (damageBonusTime.TotalSeconds > 0)
            {
                LocalNotification damageNotif = new LocalNotification();
                damageNotif.fireDate = DateTime.Now.AddMinutes(damageBonusTime.Minutes);
                damageNotif.alertBody = "Your damage bonus has expired! Come and get another bonus!";
                NotificationServices.ScheduleLocalNotification(damageNotif);
            }
            if (moneyBonusTime.TotalSeconds > 0)
            {
                LocalNotification moneyNotif = new LocalNotification();
                moneyNotif.fireDate = DateTime.Now.AddMinutes(moneyBonusTime.Minutes);
				moneyNotif.alertBody = "Your income bonus has expired! Come and get another bonus!";
                NotificationServices.ScheduleLocalNotification(moneyNotif);
            }
            // schedule notification to be delivered in 120 minutes
            LocalNotification notif = new LocalNotification();
            notif.fireDate = DateTime.Now.AddMinutes(120);
            notif.alertBody = "The village is under attack! Defend it and gain loot!";
            NotificationServices.ScheduleLocalNotification(notif);

            // schedule notification to be delivered in 24 hours
            LocalNotification dayNotif = new LocalNotification();
            dayNotif.fireDate = DateTime.Now.AddHours(24);
            dayNotif.alertBody = "Your mages earned a lot of gold! Come and upgrade them!";
            NotificationServices.ScheduleLocalNotification(dayNotif);
            

#endif
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (Data == null) return;
            if (pauseStatus)
            {
                Time.timeScale = 0;
                #if UNITY_IOS
                NotificationServices.ClearLocalNotifications();
                NotificationServices.CancelAllLocalNotifications();
                ScheduleNotification();
                #endif

                PlayerPrefs.SetString("_gameCloseTime", System.DateTime.Now.ToString());
                Data.SetAchievementData(AchievementManager.GetAchievementKeeper());
                SaveLoadHelper.SaveGame(Data);
            }
            else
            {
                HandleDroppedMage();
                Time.timeScale = 1;
                #if UNITY_IOS
                //Debug.Log("Local notification count = " + NotificationServices.localNotificationCount);
                
                //if (NotificationServices.localNotificationCount > 0) {
                //    Debug.Log(NotificationServices.localNotifications[0].alertBody);
                //}

                // cancel all notifications first.
                NotificationServices.ClearLocalNotifications();
                NotificationServices.CancelAllLocalNotifications();
                #endif

                CalculateIdleIncomeAndShowNotification();
                StartCoroutine(WaveManager.SendWave());
            }
        }

        void OnApplicationQuit()
        {

            #if UNITY_ANDROID && !UNITY_EDITOR

            var damageBonusTime = _damageModifierEndTime - DateTime.Now;
            var moneyBonusTime = _currencyModifierEndTime - DateTime.Now;

            NotificationManager.CancelAll();
            if (damageBonusTime.TotalSeconds > 0)
            {
			NotificationManager.SendWithAppIcon(TimeSpan.FromMinutes(damageBonusTime.Minutes), "Hey, Grandmage!", "Your damage bonus has expired! Come and get another bonus!", new Color(0, 0.6f, 1), NotificationIcon.Message);
            }
            if(moneyBonusTime.TotalSeconds > 0)
            {
			NotificationManager.SendWithAppIcon(TimeSpan.FromMinutes(damageBonusTime.Minutes), "Hey, Grandmage!", "Your income bonus has expired! Come and get another bonus!", new Color(0, 0.6f, 1), NotificationIcon.Message);
            }
            
            NotificationManager.SendWithAppIcon(TimeSpan.FromHours(2), "Help!", "The village is under attack! Defend it and gain loot!", new Color(0, 0.6f, 1), NotificationIcon.Message);
            NotificationManager.SendWithAppIcon(TimeSpan.FromHours(24), "We need you!", "Your mages earned a lot of gold! Come and upgrade them!", new Color(0, 0.6f, 1), NotificationIcon.Message);
            #endif

            if (Data != null)
            {
                PlayerPrefs.SetString("_gameCloseTime", System.DateTime.Now.ToString());
                Data.SetAchievementData(AchievementManager.GetAchievementKeeper());
                SaveLoadHelper.SaveGame(Data);
            }
        }

        public void ResetGameWithFire()
        {
            ResetGame(Element.Fire);
        }

        public void ResetGameWithWater()
        {
            ResetGame(Element.Water);
        }

        public void ResetGameWithAir()
        {
            ResetGame(Element.Air);
        }

        public void ResetGameWithEarth()
        {
            ResetGame(Element.Earth);
        }

        private void ResetGame(Element bonusElement)
        {
            AnalyticsManager.GameReset(this, bonusElement);
            RangeObject.SetActive(false);
            Data.DecreaseCurrency(Data.GetCurrency());
			foreach (var building in _sceneReferenceManager.AllAssignableBuildings) {
				building.EjectMageInside();
				building.MenuOpen = false;
                building.StopHighlighting();
			}
			UIManager.DestroyTowerMenuCloser();
			BuildingMenuSpawner.OpenMenu = null;
            Data.DestroyMages();
            
            MageButtons.ResetMageMenu();
            WaveManager.Reset();
            Data.ResetPlayer(bonusElement);
            
            AchievementManager.RegisterEvent(AchievementType.Reset, Data.GetResetAmount());

            Data.InitializeMageDataArrayForStartup(_mageFactory);
            MageButtons.AddPlayerButton();
			foreach (var mage in Data.GetMages())
			{
				MageButtons.AddMageButton(mage);
			}
            InitializeMages();
        }

        public void UpdateMagePrefab(Mage mage)
        {
            var id = Data.GetMages().ToList().FindIndex(m => m == mage);
            if (id != -1)
            {
                var newMage = Data.RecreateMage(id, _mageFactory, _sceneReferenceManager.AllAssignableBuildings);
                newMage.Initialize(this);
                MageButtons.OnMagePrefabUpdated(newMage);
            }
        }

        private void ResetPlayerPrefs()
        {
            DailyBonusManager.InitializePrefs();
            PlayerPrefs.SetString("_gameCloseTime", "");
            PlayerPrefs.SetInt("TutorialShown1", 0);
            PlayerPrefs.SetInt("TutorialShown2", 0);
            PlayerPrefs.SetInt("TutorialShown3", 0);
            PlayerPrefs.SetInt("sfxMute", 0);
            PlayerPrefs.SetInt("musicMute", 0);
			PlayerPrefs.SetString ("_currencyBonusEndTime", "");
			PlayerPrefs.SetString ("_damageBonusEndTime", "");
			PlayerPrefs.SetFloat ("_currencyModifier", 1);
			PlayerPrefs.SetFloat ("_damageModifier", 1);
        }

        public SceneReferenceManager GetSceneReferenceManager()
        {
            return _sceneReferenceManager;
        }

        public float GetElementBonus(Element randomElement)
        {
            return Data.GetElementBonus(randomElement);
        }
    }
}