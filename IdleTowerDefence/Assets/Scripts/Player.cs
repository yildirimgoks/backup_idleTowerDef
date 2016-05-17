using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        private static float _maxZ;
        private static float _minZ;
        private static float _maxX;
        private static float _minX;

        private int _currency;
        public GameObject Floor;
        public Minion MinionPrefab;

        // If a minion survives from towers, the bool is set to true
        // It is used for reseting the wave.
        private bool _minionSurvived;

        public GameObject PlayerSpellPrefab;

        // Stores minions
        private readonly List<Minion> _wave = new List<Minion>();

        // Minion amount in a wave
        private int _waveLength = 30;

        // Use this for initialization
        private void Start()
        {
            // Calculating area of the Plane
            var plane = GameObject.FindGameObjectsWithTag("Floor")[0];
            var difz = plane.GetComponent<Collider>().bounds.size.z/2;
            var difx = plane.GetComponent<Collider>().bounds.size.x/2;
            _maxZ = plane.transform.position.z + difz;
            _minZ = plane.transform.position.z - difz;
            _maxX = plane.transform.position.x + difx;
            _minX = plane.transform.position.x - difx;

            SendWave(true);
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && PlayerSpellPrefab.GetComponent<PlayerSpell>().FindClosestMinion() != null)
            {
                var mousePos = Input.mousePosition;
                mousePos.z = Camera.main.transform.position.y - 5;
                var instantPos = Camera.main.ScreenToWorldPoint(mousePos);
                Instantiate(PlayerSpellPrefab, instantPos, Quaternion.identity);
            }
        }


        // Minion calls this function, when it is destroyed
        public void MinionDied(Minion minion, int currencyGivenOnDeath)
        {
            if (_wave.Contains(minion))
            {
                IncreaseCurrency(currencyGivenOnDeath);
                _wave.Remove(minion);
                if (_wave.Count == 0)
                {
                    Debug.Log("All Minions are Killed");
                    SendWave(_minionSurvived);
                }
            }
        }

        // Minion calls this function, when it survives from Tower or Player
        public void MinionSurvived(Minion survivor)
        {
            _minionSurvived = true;
            MinionDied(survivor, 0);
            Destroy(survivor.gameObject);
        }

        // Creates a new wave from the beginning point
        // If reset is true, the amount of minions in a wave doesn't change.
        private void SendWave(bool reset)
        {
            if (!reset)
                _waveLength++;
            for (var i = 0; i < _waveLength; i++)
            {
                var instantPos = new Vector3(MinionPrefab.transform.position.x, MinionPrefab.transform.position.y,
                    MinionPrefab.transform.position.z - 2*i);
                var clone = Instantiate(MinionPrefab, instantPos, Quaternion.identity) as Minion;
                if (clone == null) continue;
                clone.tag = "Minion";
                _wave.Add(clone);
            }
        }

        // Returns if the given gameObject is on Map
        public static bool OnMap(GameObject gameObject)
        {
            var pos = gameObject.transform.position;
            return pos.x < _maxX && pos.x > _minX && pos.z > _minZ && pos.z < _maxZ;
        }

        // Returns if there are any Minion on Map
        public static bool AnyMinionOnMap()
        {
            return GameObject.FindGameObjectsWithTag("Minion").Any(OnMap);
        }

        public void IncreaseCurrency(int amount)
        {
            _currency += amount;
        }

        public void DecreaseCurrency(int amount)
        {
            _currency -= amount;
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(10, 0, 100, 20), "Currency: " + _currency);
			GUI.Label(new Rect (110,0, 100, 20), "Wave: " + (_waveLength-29));

		}

        public List<Minion> getList()
        {
            return _wave;
        }
    }
}