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
            get
            {
                if (_wave.Count > 0)
                    return _wave[0].Data.GetSpeed();
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
			TextAsset textAsset = (TextAsset)Resources.Load("GameInput - Wave", typeof(TextAsset));
			var lines = textAsset.text.Split('\n');
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
                if (!info.IsValid())
                {
                    Debug.LogError("Wave " + (waveInfo.Count + 1) +" is not a valid wave, something will break");
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
            yield return new WaitForSeconds(1);
            AudioManager.PlayHornSound();
            foreach (var minion in _wave)
            {
                Destroy(minion.gameObject);
            }
            _wave.Clear();
            _minionSurvived = false;

            var minionData = Data.GetMinionDataForCurrentWave();
            var minionCounts = Data.GetCurrentWaveLengths();

            var lastForward = StartWaypoint.transform.forward*0;
            var minionPrefabIds = GenerateMinionPrefabIdsForCurrentWave();

            for (var i = 0; i < minionCounts.Length; i++)
            {
                for (int j = 0; j < minionCounts[i] ; j++)
                {
                    lastForward += StartWaypoint.transform.forward*Random.Range(4, 9);
                    //Ilk wavepointten sonra look at yuzunden tek sira oluyorlar
                    var rightOffset = StartWaypoint.transform.right*Random.Range(-5f, 5f);
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

        public void CalculateNextWave()
        {
            if (AliveMinionCount == 0)
            {
                Debug.Log("Minions No More");
                if (_minionSurvived)
                {
                    StartCoroutine(SendWave());
                }
                else
                {
                    SendNextLevelIncreaseMax();
                }
            }
        }

        public void SendNextLevelIncreaseMax()
        {
            if (Data.CurrentWave == Data.GetMaxReachedWave())
            {
                Data.IncreaseCurrentWaveAndMaxWave();
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
            if (Data.CurrentWave > 0)
            {
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
            StartCoroutine(SendWave());
            Debug.Log("reset successful?");
        }
    }
}