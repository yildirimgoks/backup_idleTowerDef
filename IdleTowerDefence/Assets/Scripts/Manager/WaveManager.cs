using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Model;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Manager
{
    public class WaveManager : MonoBehaviour
    {
        public Minion MinionPrefab;
        public Minion BossPrefab;
        public Waypoint StartWaypoint;
        public Waypoint EndWaypoint;
        public UIManager UIManager;
        public AudioManager AudioManager;

        private static readonly List<Minion> _wave = new List<Minion>();
        private bool _minionSurvived;

        public WaveData Data;

        //Computed Properties

        public BigIntWithUnit WaveLife
        {
			get { return _wave.Aggregate(new BigIntWithUnit(), (life, minion) => life + minion.GetComponent<Minion>().Data.GetCurrentLife()); }
        }

        public BigIntWithUnit TotalWaveLife;

		public float WaveSpeed
		{
			get {
				if (_wave.Count > 0)
					return _wave [0].Data.GetSpeed();
				return 0;
			}
		}

		public int AliveMinionCount { get { return _wave.Count((minion) => minion.OnMap); } }

        public bool AnyMinionOnMap
        {
            get { return _wave.Any(minion => minion.OnMap); }
        }

        private void Start()
		{

		}
        public void Init()
        {
            var file = File.OpenRead("Assets\\Scripts\\Manager\\GameInput - Wave.csv");
            var reader = new StreamReader(file);

            var waveInfo = new List<SingleWaveInfo>();
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                SingleWaveInfo info;
                info.Type = values[1];
                info.BossWave = values[2].Equals("TRUE");
                info.MageDropWave = values[3].Equals("TRUE");
                info.Count = int.Parse(values[4]);
                info.Speed = float.Parse(values[5]);
                info.CurrencyOnDeath = new BigIntWithUnit(values[6]);
                info.Life = new BigIntWithUnit(values[7]);
                waveInfo.Add(info);
            }
            Data.ReadWaveInfo(waveInfo);
        }
			
        public void MinionSurvived(Minion survivor)
        {
            _minionSurvived = true;
			survivor.OnMap = false;
			survivor.gameObject.SetActive(false);
            AudioManager.PlayMinionSurviveSound();
			if (AliveMinionCount == 0) {
				CalculateNextWave ();
			}
        }

		public void SendWave()
        {
			foreach(var minion in _wave) {
				Destroy (minion.gameObject);
			}
			_wave.Clear();
			_minionSurvived = false;
            if (Data.IsBossWave)
            {
                var bossPos = StartWaypoint.transform.position;
                var bossRot = StartWaypoint.transform.rotation;
                var boss = Instantiate(BossPrefab, bossPos, bossRot) as Minion;
                if (boss != null)
                {
                    boss.SetUiManager(UIManager);
                    boss.Data = Data.GetMinionDataForCurrentWave();
                    if (Data.IsDropWave)
                    {
                        boss.Data.SetMageLoot(true);
                    }
                    boss.tag = "Boss";
                    _wave.Add(boss);
                }          
            }
            else
            {
                MinionData minionData = Data.GetMinionDataForCurrentWave();

                var lastForward = StartWaypoint.transform.forward * 0;
                for (var i = 0; i < Data.GetCurrentWaveLength(); i++)
                {
                    lastForward += StartWaypoint.transform.forward * Random.Range(4,9);
                    //Ilk wavepointten sonra look at yuzunden tek sira oluyorlar
                    var rightOffset = StartWaypoint.transform.right * Random.Range(-5f, 5f);
                    var instantPos = StartWaypoint.transform.position - lastForward + rightOffset;
                    var instantRot = StartWaypoint.transform.rotation;

                    var clone = Instantiate(MinionPrefab, instantPos, instantRot) as Minion;
                    if (clone == null) continue;
                    clone.SetUiManager(UIManager);
                    clone.Data = (MinionData) minionData.Clone();
                    clone.tag = "Minion";
                    _wave.Add(clone);
                }
            }
			TotalWaveLife = WaveLife;
        }

        public void CalculateNextWave() {
            if (AliveMinionCount == 0)
            {
                Debug.Log("Minions No More");
                if (_minionSurvived) {
                    SendWave();
                } else {
                    SendNextLevelIncreaseMax();
                }
            }
        }

		public void SendNextLevelIncreaseMax() {
			if (Data.CurrentWave == Data.GetMaxReachedWave()) {
                Data.IncreaseCurrentWaveAndMaxWave();
				SendWave();
			} else if (Data.CurrentWave < Data.GetMaxReachedWave()) {
		        SendWave();
		    }
		}

		public void SendNextWave() {
			if (Data.IncreaseCurrentWaveIfLessThanMax()) {
				SendWave();
			}
		}

        //ToDo: needs clean up
		public void SendPreviousWave() {
			if (Data.CurrentWave > 0) {
				Data.DecreaseCurrentWave();
                if (Data.IsBossWave)
			    {
			        Data.IncreaseCurrentWaveIfLessThanMax();
			    }
			    else
			    {
			        SendWave();
			    }
			}
		}
        
        public Minion FindClosestMinion(Vector3 position)
        {
            Minion closestMinion = null;
            var distance = Mathf.Infinity;
            foreach (var minion in _wave)
            {
                var curDistance = Vector3.Distance(minion.transform.position, position);
                if (curDistance < distance)
                {
                    closestMinion = minion;
                    distance = curDistance;
                }
            }
            return closestMinion;
        }

        public List<Minion> GetMinionList()
        {
            return _wave;
        }

        public bool Contains(Minion minion)
        {
            return _wave.Contains(minion);
        }
        
        /// <summary>
        /// Safely removes element if the list contains it,
        /// returns true if the element is removed false otherwise
        /// </summary>
        /// <param name="minion"></param>
        /// <returns></returns>
        public bool SafeRemove(Minion minion)
        {
            return Contains(minion) && _wave.Remove(minion);
        }

        public void Reset()
        {
            Data.ResetWave();
            SendWave();
            Debug.Log("reset successful?");
        }
    }
}