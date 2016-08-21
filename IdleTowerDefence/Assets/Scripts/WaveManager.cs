using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assets.Scripts
{
    public class WaveManager : MonoBehaviour
    {
        public Minion MinionPrefab;
        public Minion BossPrefab;
        public Waypoint StartWaypoint;
        public Waypoint EndWaypoint;
        public UIManager UIManager;

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
			
        public void MinionSurvived(Minion survivor)
        {
            _minionSurvived = true;
			survivor.OnMap = false;
			survivor.gameObject.SetActive(false);
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
                    boss.tag = "Boss";
                    _wave.Add(boss);
                }          
            }
            else
            {
                MinionData minionData = Data.GetMinionDataForCurrentWave();
                for (var i = 0; i < Data.GetCurrentWaveLength(); i++)
                {
                    var instantPos = StartWaypoint.transform.position - StartWaypoint.transform.forward * 5 * i;
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
            Debug.Log("reset successful?");
        }
    }
}