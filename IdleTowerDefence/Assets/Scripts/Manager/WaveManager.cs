using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Manager
{
    public class WaveManager : MonoBehaviour
    {
        public string[] MinionPrefabNames;
        public Minion[] MinionPrefabs;
        public Waypoint StartWaypoint;
        public Waypoint EndWaypoint;
        public UIManager UIManager;
        public AudioManager AudioManager;
        public AchievementManager AchievementManager;
        public TutorialManager TutorialManager;

        private static readonly List<Minion> _wave = new List<Minion>();
        private bool _minionSurvived;
        private bool _firstBoss = true;

        public WaveData Data;

        //Computed Properties

        public BigIntWithUnit WaveLife
        {
            get { return _wave.Aggregate(new BigIntWithUnit(), (life, minion) => life + minion.GetComponent<Minion>().Data.GetCurrentLife()); }
        }

        public BigIntWithUnit TotalWaveLife;

        public float WaveSpeed
        {
            get
            {
                if (_wave.Count > 0)
                    return _wave.Aggregate(0.0f, (speed, minion) => speed + minion.Data.GetSpeed())/_wave.Count;
                return 0;
            }
        }

        public int AliveMinionCount { get { return _wave.Count((minion) => minion.OnMap); } }

        public bool AnyMinionOnMap
        {
            get { return _wave.Any(minion => minion.OnMap); }
        }

        public BigIntWithUnit WaveReward
        {
            get { return _wave.Aggregate(new BigIntWithUnit(), (reward, minion) => reward + minion.GetComponent<Minion>().Data.GetDeathLoot()); }
        }

        public void Init()
        {
			TextAsset textAsset = (TextAsset)Resources.Load("GameInput - Wave", typeof(TextAsset));
			var lines = textAsset.text.Replace("\r", "").Split('\n');
            var waveInfo = new List<SingleWaveInfo>();
			for(var i = 1; i<lines.Length; i++)
            {
				var values = lines[i].Split(',');
                SingleWaveInfo info;
                info.Type = values[1].Split(';');
                info.BossWave = values[2].Equals("TRUE");
                info.MageDropWave = values[3].Equals("TRUE");
                
                info.Count = values[4].Split(';').Select(elem => int.Parse(elem)).ToArray();
                info.Speed = values[5].Split(';').Select(elem => float.Parse(elem)).ToArray();
                info.CurrencyOnDeath = values[6].Split(';').Select(elem => new BigIntWithUnit(elem)).ToArray();
                info.Life = values[7].Split(';').Select(elem => new BigIntWithUnit(elem)).ToArray();
                info.ResetBonus = float.Parse(values[8]);
                if (!info.IsValid())
                {
                    Debug.LogError("Wave " + (waveInfo.Count + 1) + " is not a valid wave, something will break");
                }
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
            if (AliveMinionCount == 0)
            {
                CalculateNextWave();
            }
        }

        private int[] GenerateMinionPrefabIdsForCurrentWave()
        {
            var types = Data.CurrentWaveMinionType;
            var result = new int[types.Length];
            for (int i = 0; i < result.Length; i++)
            {
                for (var j = 0; j < MinionPrefabNames.Length; j++)
                {
                    if (Data.CurrentWaveMinionType[i].Equals(MinionPrefabNames[j]))
                    {
                        result[i] = j;
                    }
                }
            }
            return result;
        }

        public IEnumerator SendWave()
        {
            if (_firstBoss)
            {
                if(Data.CurrentWave == 0)
                {
                    TutorialManager.ShowSet(TutorialManager.Set1);
                } else if (Data.IsBossWave)
                {
                    _firstBoss = false;
                    TutorialManager.ShowSet(TutorialManager.Set2);
                }
            }
            
            ClearCurrentWave();
            
            AudioManager.PlayHornSound();
            yield return new WaitForSeconds(1.0f);

            CreateCurrentWave();
            yield return null;
            foreach (var minion in _wave)
            {
                minion.StartWalking();
            }
            yield return null;
        }

        public void CreateCurrentWave()
        {
            var minionData = Data.GetMinionDataForCurrentWave();
            var minionCounts = Data.GetCurrentWaveLengths();

            var lastForward = StartWaypoint.transform.forward*0;
            var minionPrefabIds = GenerateMinionPrefabIdsForCurrentWave();

            for (var i = 0; i < minionCounts.Length; i++)
            {
                for (int j = 0; j < minionCounts[i]; j++)
                {
                    lastForward += StartWaypoint.transform.forward*Random.Range(4, 9);
                    var rightOffset = StartWaypoint.transform.right*Random.Range(-5f, 5f);
                    if (Data.IsBossWave)
                    {
                        rightOffset = StartWaypoint.transform.right;
                    }
                    var instantPos = StartWaypoint.transform.position - lastForward + rightOffset;
                    var instantRot = StartWaypoint.transform.rotation;

                    var clone = Instantiate(MinionPrefabs[minionPrefabIds[i]], instantPos, instantRot) as Minion;
                    if (clone == null) continue;
                    clone.SetUiManager(UIManager);
                    clone.Data = (MinionData) minionData[i].Clone();
                    clone.Data.SetMageLoot(Data.IsDropWave);
                    clone.tag = "Minion";

                    if (Data.IsBossWave)
                    {
                        clone.tag = "Boss";
                    }
                    _wave.Add(clone);
                }
            }
            TotalWaveLife = WaveLife;
        }

        public void ClearCurrentWave()
        {
            foreach (var minion in _wave)
            {
                Destroy(minion.gameObject);
            }
            _wave.Clear();
            _minionSurvived = false;
        }

        public void CalculateNextWave()
        {
            if (AliveMinionCount != 0) return;
            if (_minionSurvived)
            {
                StartCoroutine(SendWave());
            }
            else
            {
                SendNextLevelIncreaseMax();
            }
        }

        public void SendNextLevelIncreaseMax()
        {
            if (Data.CurrentWave == Data.GetMaxReachedWave())
            {
                Data.IncreaseCurrentWaveAndMaxWave();
                AchievementManager.RegisterEvent(AchievementType.Wave, Data.GetMaxReachedWave());
                StartCoroutine(SendWave());
            }
            else if (Data.CurrentWave < Data.GetMaxReachedWave())
            {
                StartCoroutine(SendWave());
            }
        }

        public void SendNextWave()
        {
            if (Data.IncreaseCurrentWaveIfLessThanMax())
            {
                StartCoroutine(SendWave());
            }
        }

        //ToDo: needs clean up
        public void SendPreviousWave()
        {
            if (Data.CurrentWave <= 0) return;
            Data.DecreaseCurrentWave();
            if (Data.IsBossWave)
            {
                Data.IncreaseCurrentWaveIfLessThanMax();
            }
            else
            {
                StartCoroutine(SendWave());
            }
        }

        public Minion FindClosestMinion(Vector3 position)
        {
            Minion closestMinion = null;
            var distance = Mathf.Infinity;
            foreach (var minion in _wave)
            {
                if (!minion.OnMap) continue;

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
            StartCoroutine(SendWave());
        }
    }
}