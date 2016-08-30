using System;
using System.Collections;
using Assets.Scripts.Manager;
using Assets.Scripts.Model;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        public Mage MagePrefab;
        public PlayerSpell PlayerSpellPrefab;
        public WaveManager WaveManager;
        public SkillManager SkillManager;
		public UIManager UIManager;
        public LayerMask FloorMask;
        public LayerMask IgnorePlayerSpell;
        public EventSystem MainEventSystem;

		public Texture[] TowerTextures;
		public Texture[] ShrineTextures;
		public Texture[] MageTextures;
        public Spell[] SpellParticles;
        public SkillProjectile[] SkillParticles;
        private MageFactory _mageFactory;
        public PlayerData Data;

        public bool LoadSavedGame;
        public MageAssignableBuilding[] AllAssignableBuildings;

        public Texture2D SkillAimCursor;
        // temp
        private bool _isSkill;
        private Mage _skillMage;

        // Use this for initialization
        private void Start()
        {
            _mageFactory = new MageFactory(MagePrefab);
            ElementController.Instance.TowerTextures = TowerTextures;
			ElementController.Instance.ShrineTextures = ShrineTextures;
			ElementController.Instance.MageTextures = MageTextures;
            ElementController.Instance.SpellParticles = SpellParticles;
            ElementController.Instance.SkillParticles = SkillParticles;

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
                    Data = new PlayerData(1, UpgradeManager.MageDamageInitial, 100, 0, UpgradeManager.MageUpgradePriceInitial, Element.Air, 9);
                }          
                MageListInitializer();
                WaveManager.Data = new WaveData();
                Data.SetWaveData(WaveManager.Data);
            }
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

            // temp 
            _isSkill = false;
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
            UIManager.CreateNotificications("Welcome back!",
                "Your mages killed " + killedMinionCount + " attackers and earned " + currencyGainedWhileIdle +
                " gold while you were gone.");
        }

        // Update is called once per frame
        private void Update()
        {
            //PlayerSpell Targeting

            if (Input.GetMouseButtonDown(0) && !MainEventSystem.IsPointerOverGameObject())
            {
                if (Time.timeScale != 0)
                {
                    if (_isSkill)
                    {
                        if (SkillManager.CastSkill(_skillMage, IgnorePlayerSpell, FloorMask, Input.mousePosition)){
                            _isSkill = false;
                            _skillMage = null;
                            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                        }
                    }
                    else
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
                                    WaveManager.FindClosestMinion(instantPos));
                        }
                    }
                } else
                {
                    foreach (var mage in Data.GetMages())
                    {
                        if (mage.Data.IsDropped())
                        {
                            WaveManager wavemanager = Camera.main.GetComponent<WaveManager>();
                            mage.SetBasePosition(new Vector3(9.5f, 12f, 21f + (wavemanager.Data.CurrentWave / wavemanager.Data.GetMageDropInterval() + 1) * 10f));
                            mage.Data.SetState(MageState.Idle);
                            Time.timeScale = 1;

                            WaveManager.CalculateNextWave();
                        }
                    }
                }           
            }
        }

        public void MageListInitializer()
        {
            for (int i = 0; i < 3; i++)
            {
                var mage = _mageFactory.GetMage(9.5f, 1 + 10 * i);
                mage.transform.position = new Vector3(mage.transform.position.x, 12f, mage.transform.position.z);
                Data.AddMage(mage);
                mage.Data.SetState(MageState.Idle);
            }
        }

        public void SkillCall(Mage mage) {
            _isSkill = true;
            _skillMage = mage;
            Cursor.SetCursor(SkillAimCursor, Vector2.zero, CursorMode.Auto);
        }

        // Minion calls this function, when it is destroyed
        public void MinionDied(Minion minion, BigIntWithUnit currencyGivenOnDeath, float delay)
        {
            if (WaveManager.SafeRemove(minion)){
                Data.IncreaseCurrency(currencyGivenOnDeath);
                if (minion.tag == "Boss"){
                    if (minion.Data.HasMageLoot() && !Data.IsMageListFull())
                    {
                        // Add Mage after the "death" animation of boss finishes.
                        StartCoroutine(AddMage(minion, delay));
                    } else {
                        Destroy(minion.gameObject, delay);
                        Data.IncreaseCurrency(currencyGivenOnDeath);
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

        void OnApplicationPause(bool pauseStatus)
        {
            if (Data == null) return;
            if (pauseStatus)
            {
                PlayerPrefs.SetString("_gameCloseTime", System.DateTime.Now.ToString());
                SaveLoadHelper.SaveGame(Data);
            }
            else
            {
                CalculateIdleIncomeAndShowNotification();
            }
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