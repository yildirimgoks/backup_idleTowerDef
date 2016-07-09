using UnityEngine;
using System.Collections;
using System.Linq;
using Assets.Scripts.Model;

namespace Assets.Scripts
{
    public class SkillProjectile : MonoBehaviour
    {
        private SkillData _data;
        public Player Player;

        // Use this for initialization
        void Start()
        {
            Player = Camera.main.GetComponent<Player>();
            gameObject.GetComponent<Renderer>().material.color = ElementController.Instance.GetColor(_data.GetElement());
        }

        // Update is called once per frame
        void Update()
        {
            if (transform.position.y <= 0)
            {
                DamageMinions();
                Destroy(gameObject);
            }
        }

        public void DamageMinions()
        {
            var minions = Player.WaveManager.GetMinionList();
            foreach (var minion in minions)
            {
                if (InRange(minion))
                {
                    minion.Life -= _data.GetDamage();
                }
            }
        }

        public bool InRange(Minion targetMinion)
        {
            var deltaX = transform.position.x - targetMinion.transform.position.x;
            var deltaZ = transform.position.z - targetMinion.transform.position.z;
            var distanceSq = deltaX * deltaX + deltaZ * deltaZ;
            return Mathf.Sqrt(distanceSq) <= _data.GetRange();
        }

        public static void Clone(SkillProjectile skillPrefab, SkillData data, Vector3 position)
        {
            var skillProjectile = (SkillProjectile)Instantiate(skillPrefab, position, Quaternion.identity);
            skillProjectile.GetComponent<Rigidbody>().velocity = skillProjectile.transform.TransformDirection(Vector3.down) * 100;
            skillProjectile._data = data;
        }
    }
}