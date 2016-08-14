using UnityEngine;
using System.Collections;
using System.Linq;
using Assets.Scripts.Model;

namespace Assets.Scripts
{
    public class SkillProjectile : MonoBehaviour
    {
        
        private Player _player;
        private SkillData _data;
        private bool _hasPositionTarget;
        private Vector3 _targetPosition;
        private bool _hasObjectTarget;
        private GameObject _target;
        private bool _followsPath;

        private bool doneEffects = false;

        // Use this for initialization
        void Start()
        {
            gameObject.GetComponent<Renderer>().material.color = ElementController.Instance.GetColor(_data.GetElement());
        }

        // Update is called once per frame
        void Update()
        {
            
            switch (_data.GetSkillType()) {
                case SkillType.AreaTop:
                    transform.position = Vector3.MoveTowards (transform.position, _targetPosition, _data.GetSpeed() * Time.deltaTime);
                    if ( transform.position.y <= _targetPosition.y ){
                        DoEffects();
                        Destroy(gameObject, 2);
                    }
                    break;
                case SkillType.AreaBottom:
                    transform.position = Vector3.MoveTowards (transform.position, _targetPosition, _data.GetSpeed() * Time.deltaTime);
                    if ( transform.position.y >= _targetPosition.y ){
                        DoEffects();
                        Destroy(gameObject, 5);
                    }
                    break;
                case SkillType.PathFollower:
                    transform.Translate(Vector3.forward * _data.GetSpeed() * Time.deltaTime);
                    break;
                case SkillType.AllMinions:case SkillType.AllTowers:
                    var targetPosition = _target.transform.position;
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, _data.GetSpeed() * Time.deltaTime);
                    if ( transform.position.y <= targetPosition.y ){
                        // Debug.Log("Projectile Position: "+ transform.position.y + "\t Target Position: "+ targetPosition.y);
                        DoEffects();
                        Destroy(gameObject);
                    }
                    break;
            }
        }

        private void DoEffects(){
            if (doneEffects == false){
                Debug.Log(_data.GetMinionEffects());
                _data.GetMinionEffects().ForEach((SkillEffect effect) => {
                    switch (effect){
                        case SkillEffect.Damage:
                            DamageMinions();
                            break;
                        case SkillEffect.IncreaseDamage:
                            break;
                        case SkillEffect.DecreaseDamage:
                            break;
                        case SkillEffect.IncreaseRange:
                            break;
                        case SkillEffect.DecreaseRange:
                            break;
                        case SkillEffect.IncreaseSpeed:
                            break;
                        case SkillEffect.DecreaseSpeed:
                            break;
                    }
                });
                doneEffects = true;
            }
        }

        public void OnTriggerEnter(Collider Other){
            if (_data.GetSkillType() == SkillType.PathFollower){
                if ( Other.gameObject.GetComponent<Minion>()){
                    _data.GetMinionEffects().ForEach((SkillEffect effect) => {
                        switch (effect){
                            case SkillEffect.Damage:
                                Other.gameObject.GetComponent<Minion>().Data.DecreaseLife(_data.GetDamage());
                                break;
                            case SkillEffect.IncreaseDamage:
                                break;
                            case SkillEffect.DecreaseDamage:
                                break;
                            case SkillEffect.IncreaseRange:
                                break;
                            case SkillEffect.DecreaseRange:
                                break;
                            case SkillEffect.IncreaseSpeed:
                                break;
                            case SkillEffect.DecreaseSpeed:
                                break;
                        }
                    });
                }
                
            }
        }

        private void DamageMinions()
        {
            var minions = _player.WaveManager.GetMinionList();
            foreach (var minion in minions)
            {
                if (InRange(minion))
                {
                    minion.Data.DecreaseLife(_data.GetDamage());
                }
            }
        }

        private bool InRange(Minion targetMinion)
        {
            var deltaX = transform.position.x - targetMinion.transform.position.x;
            var deltaZ = transform.position.z - targetMinion.transform.position.z;
            var distanceSq = deltaX * deltaX + deltaZ * deltaZ;
            return Mathf.Sqrt(distanceSq) <= _data.GetRange();
        }

        
        // For Type All-Minions and All-Towers
        public static void Clone(SkillProjectile skillPrefab, Mage mage, Vector3 position, GameObject target)
        {
            var skillProjectile = (SkillProjectile)Instantiate(skillPrefab, position, Quaternion.identity);
            skillProjectile._data = mage.Data.GetSkillData();
            skillProjectile._player = mage.Player;
            skillProjectile._hasObjectTarget = true;
            skillProjectile._target = target;
            skillProjectile._hasPositionTarget = false;
            skillProjectile._followsPath = false;  
        }

        // For Type AOE
        public static void Clone(SkillProjectile skillPrefab, Mage mage, Vector3 position, Vector3 targetPosition)
        {
            var skillProjectile = (SkillProjectile)Instantiate(skillPrefab, position, Quaternion.identity);
            skillProjectile._data = mage.Data.GetSkillData();
            skillProjectile._player = mage.Player;
            skillProjectile._hasPositionTarget = true;
            skillProjectile._targetPosition = targetPosition;
            skillProjectile._hasObjectTarget = false;
            skillProjectile._followsPath = false;  
        }

        // For Type Path Follower
        public static void Clone(SkillProjectile skillPrefab, Mage mage, Waypoint EndWaypoint)
        {
            Vector3 pos = new Vector3(EndWaypoint.transform.position.x, EndWaypoint.transform.position.y ,EndWaypoint.transform.position.z);
            var skillProjectile = (SkillProjectile)Instantiate(skillPrefab, pos, Quaternion.identity);
            skillProjectile.transform.LookAt(EndWaypoint.Previous.transform);
            skillProjectile._data = mage.Data.GetSkillData();
            skillProjectile._player = mage.Player;
            skillProjectile._followsPath = true; 
            skillProjectile._hasPositionTarget = false;
            skillProjectile._hasObjectTarget = false;
        }
    }
}