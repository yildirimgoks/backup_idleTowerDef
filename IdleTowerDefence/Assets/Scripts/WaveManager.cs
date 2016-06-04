using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class WaveManager : MonoBehaviour
    {
        public Minion MinionPrefab;
        public Minion BossPrefab;
        public Waypoint StartWaypoint;

        public int CurrentWave { get; private set; }

        private readonly List<Minion> _wave = new List<Minion>();
        public bool _minionSurvived;

		private int _waveLength = 30;
		private int _maxWave = 0;

		private void Start()
		{
			CurrentWave = 0;
		}

		private void Update()
		{
			if (AliveMinionCount==0) {
				SendWave();
			}
		}
			
        public void MinionSurvived(Minion survivor)
        {
            _minionSurvived = true;
            _wave.Remove(survivor);
            Destroy(survivor.gameObject);
        }

        public BigIntWithUnit WaveLife
        {
            get { return _wave.Aggregate(new BigIntWithUnit(), (life, minion) => life + minion.GetComponent<Minion>().Life); }
        }

        public int AliveMinionCount { get { return _wave.Count; } }

        public bool AnyMinionOnMap
        {
            get { return _wave.Any(minion => minion.GetComponent<Minion>().OnMap); }
        }

		public void SendWave()
        {
			foreach(var minion in _wave) {
				Destroy (minion.gameObject);
			}
			_wave.Clear();
			_minionSurvived = false;
            if ((CurrentWave + 1) % 5 == 0)
            {
                var bossPos = StartWaypoint.transform.position;
                var bossRot = StartWaypoint.transform.rotation;
                var boss = Instantiate(BossPrefab, bossPos, bossRot) as Minion;
                if (boss != null)
                {
                    boss.Life = (CurrentWave + 1) * 200;
                    boss.CurrencyGivenOnDeath = boss.Life;
                    boss.tag = "Boss";
                    _wave.Add(boss);
                }          
            }
            else
            {
                var multiplierLife = System.Math.Pow(1.1, CurrentWave);
                var multiplierMoney = System.Math.Pow(1.03, CurrentWave);
                for (var i = 0; i < _waveLength; i++)
                {
                    var instantPos = StartWaypoint.transform.position - StartWaypoint.transform.forward * 5 * i;
                    var instantRot = StartWaypoint.transform.rotation;

                    var clone = Instantiate(MinionPrefab, instantPos, instantRot) as Minion;
                    if (clone == null) continue;
                    clone.Life = BigIntWithUnit.MultiplyPercent(Minion.BaseLife, multiplierLife * 100);
                    clone.CurrencyGivenOnDeath = BigIntWithUnit.MultiplyPercent(Minion.BaseCurrencyGivenOnDeath, multiplierMoney * 100);
                    clone.tag = "Minion";
                    _wave.Add(clone);
                }
            }
        }

		public void SendNextLevelIncreaseMax() {
			if (CurrentWave == _maxWave) {
				_maxWave++;
				SendNextWave();
			}
		}

		public void SendNextWave() {
			if (_maxWave > CurrentWave) {
				CurrentWave++;
				SendWave();
			}
		}

		public void SendPreviousWave() {
			if (CurrentWave > 0) {
				CurrentWave--;
				SendWave();
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
    }
}