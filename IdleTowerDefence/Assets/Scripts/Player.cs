using System;
using System.Collections;
using Assets.Scripts.Model;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        public Mage MagePrefab;
        public PlayerSpell PlayerSpellPrefab;
        public SkillProjectile SkillPrefab;
        public WaveManager WaveManager;
        
        public LayerMask FloorMask;
        public LayerMask IgnorePlayerSpell;
        public EventSystem MainEventSystem;

		public Texture[] TowerTextures;
        private MageFactory _mageFactory;
        public PlayerData Data;

        public bool LoadSavedGame;
        public MageAssignableBuilding[] AllAssignableBuildings;

		public bool _elementSet;
        
        public Texture2D SkillAimCursor;
        // temp
        private bool _isSkill;

        // Use this for initialization
        private void Start()
        {
			_elementSet = false;
            _mageFactory = new MageFactory(MagePrefab);
            ElementController.Instance.textures = TowerTextures;

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
                SceneLoader loadObject = GameObject.FindGameObjectWithTag("LoadObject").GetComponent<SceneLoader>();
                if (loadObject != null)
                {
                    Data = loadObject.Data;
                } else {
                    Data = new PlayerData(20, 100, 0, 100, 1, Element.Air);
                }          
                for (int i = 0; i < 3; i++)
                {
                    var mage = _mageFactory.GetMage(6.1f, 13 + 8 * i);
                    mage.transform.position = new Vector3(mage.transform.position.x, 12f, mage.transform.position.z);
                    Data.AddMage(mage);
                    mage.Data.SetState(MageState.Idle);
                }
                WaveManager.Data = new WaveData();
                Data.SetWaveData(WaveManager.Data);
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
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit floorHit;
                        RaycastHit uiHit;
                        if (!Physics.Raycast(ray, out uiHit, Mathf.Infinity, IgnorePlayerSpell) &&
                        Physics.Raycast(ray, out floorHit, Mathf.Infinity, FloorMask))
                        {
                            SkillProjectile.Clone(SkillPrefab, new SkillData(50, 20, Element.Fire), new Vector3(floorHit.point.x, 50, floorHit.point.z));
                            _isSkill = false;
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
                            Spell.Clone(PlayerSpellPrefab, Data.GetSpellData(), instantPos,
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
                            mage.transform.position = new Vector3(6.1f, 12f, 21f + (wavemanager.Data.CurrentWave / 5 + 2) * 8f);
                            mage.Data.SetState(MageState.Idle);
                            Time.timeScale = 1;

                            WaveManager.CalculateNextWave();
                        }
                    }
                }           
            }
        }

        public void TemporarySkillCall() {
            _isSkill = true;
            Cursor.SetCursor(SkillAimCursor, Vector2.zero, CursorMode.Auto);
        }

        // Minion calls this function, when it is destroyed
        public void MinionDied(Minion minion, BigIntWithUnit currencyGivenOnDeath, float delay)
        {
            if (WaveManager.SafeRemove(minion)){
                Data.IncreaseCurrency(currencyGivenOnDeath);
                if (minion.tag == "Boss"){
                    // Add Mage after the "death" animation of boss finishes.
                    StartCoroutine(AddMage(minion, delay));
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

        void OnApplicationQuit()
        {
            SaveLoadHelper.SaveGame(Data);
        }
    }
}