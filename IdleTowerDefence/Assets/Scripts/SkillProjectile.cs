using UnityEngine;
using System.Collections;
using System.Linq;
using Assets.Scripts.Model;
using System.Collections.Generic;
using Assets.Scripts.Skills;
using Assets.Scripts.Manager;
using Assets.Scripts.Pooling;

namespace Assets.Scripts
{
    public class SkillProjectile : PoolableMonoBehaviour
    {
        protected Player _player;
        protected SkillData _data;
        protected GameObject _target;
        protected Vector3 _targetPosition;
        // protected bool _isAnimation;
        private bool doneEffects;
        private AudioManager _audioManager;

        // Use this for initialization
        public virtual void Start()
        {  
            // if ( _isAnimation ){
			// 	Color color = ElementController.Instance.GetColor(_data.GetElement());
			// 	GetComponent<ParticleSystem>().startColor = new Color(color.r,color.g,color.b,0.2f) ;
			// }else{
            if (GetComponent<ParticleSystem>() != null){
                GetComponent<ParticleSystem>().startColor = ElementController.Instance.GetColor(_data.GetElement());
            }	
			// }
            _audioManager = Camera.main.GetComponent<Player>().GetAudioManager();
            if (_audioManager)
            {
                _audioManager.PlaySkillCastingSound(_data.GetElement());
            }
        }

        public override void OnDestroy()
        {
            _player.SkillManager.DeleteAnimation(this);
        }

        //AOEs
        protected void DoRangedEffects(){
            // if (_isAnimation)return;
            if (doneEffects == false){
                _data.GetMinionEffects().ForEach(effect => {
                    var minions = _player.WaveManager.GetMinionList();
                    switch (effect){
                        case SkillEffect.Damage:
                            foreach (var minion in minions)
                            {
                                if (InRange(minion.gameObject))
                                {
                                    minion.DecreaseLife(_data.GetDamage());
                                }
                            }
                            break;
                        case SkillEffect.ChangeSpeed:
                            foreach (var minion in minions)
                            {
                                if (InRange(minion.gameObject))
                                {
                                    minion.ChangeSpeed(_data.GetMultiplier(effect)); // Multiplier returns the multiplier according to Increase/Decrease.
                                }
                            }
                            break;
                        default:
                            break;
                    }
                });

                 _data.GetTowerEffects().ForEach(effect => {
                     var towers = new List<Tower>();
                    _player.Data.GetMages().ToList().ForEach((Mage thisMage) => {	
                        if (thisMage.GetBuilding() != null && thisMage.GetBuilding() is Tower){
                            Tower tower = thisMage.GetBuilding() as Tower;
                            towers.Add(tower);
                        }
			        });
                    switch (effect){
                        case SkillEffect.ChangeDamage:
                            foreach (var tower in towers)
                            {
                                if (InRange(tower.gameObject))
                                {
                                    tower.InsideMage.ChangeDamage(_data.GetMultiplier(effect));
                                }
                            }
                            break;
                        case SkillEffect.ChangeRange:
                            foreach (var tower in towers)
                            {
                                if (InRange(tower.gameObject))
                                {
                                    tower.InsideMage.ChangeRange(_data.GetMultiplier(effect));
                                }
                            }
                            break;
                        case SkillEffect.ChangeDelay:
                            foreach (var tower in towers)
                            {
                                if (InRange(tower.gameObject))
                                {
                                    tower.InsideMage.ChangeDelay(_data.GetMultiplier(effect));
                                }
                            }
                            break;
                    }
                });
                doneEffects = true;
            }
        }

        //AllMinions and AllTowers
        protected void DoEffects(GameObject target){
            // if (_isAnimation)return;
            if(target.GetComponent<Minion>()){
                _data.GetMinionEffects().ForEach(effect => {
                    switch (effect){
                        case SkillEffect.Damage:
                            target.GetComponent<Minion>().DecreaseLife(_data.GetDamage());
                            break;
                        case SkillEffect.ChangeSpeed:
                            target.GetComponent<Minion>().ChangeSpeed(_data.GetMultiplier(effect));
                            break;
                    }
                });       
            }
            
            if(target.GetComponent<Tower>()){
                _data.GetTowerEffects().ForEach(effect => {
                switch (effect){
                    case SkillEffect.ChangeDamage:
                        target.GetComponent<Tower>().InsideMage.ChangeDamage(_data.GetMultiplier(effect));
                        break;
                    case SkillEffect.ChangeRange:
                        target.GetComponent<Tower>().InsideMage.ChangeRange(_data.GetMultiplier(effect));
                        break;
                    case SkillEffect.ChangeDelay:
                        target.GetComponent<Tower>().InsideMage.ChangeDelay(_data.GetMultiplier(effect));
                        break;
                }
            });
            }
        }

        private bool InRange(GameObject thisObject)
        {
            var deltaX = transform.position.x - thisObject.transform.position.x;
            var deltaZ = transform.position.z - thisObject.transform.position.z;
            var distanceSq = deltaX * deltaX + deltaZ * deltaZ;
            return Mathf.Sqrt(distanceSq) <= _data.GetRange();
        }

        // public static AllMinionsSkillProjectile Clone(SkillProjectile skillPrefab, Mage mage, Vector3 position, GameObject target, bool isAnimation)
        public static T Clone<T>(SkillProjectile skillPrefab, Mage mage, Vector3 position, GameObject target, Vector3 targetPosition) where T : SkillProjectile
        {
            var skillProjectile = (T)GetPoolable(typeof(T), skillPrefab);
            skillProjectile.transform.position = position;
            skillProjectile.transform.rotation = Quaternion.identity;
            skillProjectile._data = mage.Data.GetSkillData();
            skillProjectile._player = mage.Player;
            skillProjectile._target = target;
            skillProjectile._targetPosition = targetPosition;
            skillProjectile.doneEffects = false;
            // skillProjectile._isAnimation = isAnimation;
            return skillProjectile;
        }
    }
}