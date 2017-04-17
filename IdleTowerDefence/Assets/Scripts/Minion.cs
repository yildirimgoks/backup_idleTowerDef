﻿using Assets.Scripts.Manager;
using UnityEngine;

namespace Assets.Scripts
{
    public class Minion : MonoBehaviour
    {
        private const double DEFAULTSPEEDMULTIPLIER = 1.0;
        private static readonly string DeathAnimationName = "Death";

        public MinionData Data;
        // If the minion enters map, it is changed to true;
        public bool OnMap;
        public Color InitialColor;

        public Vector3 LookAtVector3;
        public bool ShouldUpdateLookAtRot;
        
        private Player _player;
        private Animation _minionAnimation;

        private double _speedMultiplier = DEFAULTSPEEDMULTIPLIER;
        private float _speedChangeTime = 0f;

        private Quaternion _lookAtQuaternion;
        private float _deathDelay;
        private bool _isWalking;


        // Use this for initialization
        private void Start()
        {
            enabled = false;
        }

        public void Initialize(Player player)
        {
            _player = player;
            OnMap = true;
            _minionAnimation = gameObject.GetComponent<Animation>();
            if (Data == null)
            {
                Data = new MinionData();
            }
            ShouldUpdateLookAtRot = false;
            _lookAtQuaternion = transform.rotation;
            _minionAnimation["Walk"].speed = Data.GetSpeed() / 5.0f;
            _minionAnimation[DeathAnimationName].speed = 2;
            _deathDelay = _minionAnimation[DeathAnimationName].length / 2;
            enabled = true;
        }

        // Update is called once per frame
        private void Update()
        {
            UpdateTime();
            if (!Data.IsAlive())
            {
                if (gameObject.tag != "Boss")
                {
                    gameObject.tag = "Untagged";
                }           
                if (_minionAnimation)
                {
                    _minionAnimation.Play(DeathAnimationName);
                    MinionKilled(_deathDelay);
                } else {
				    MinionKilled(0);
                }
            }
            else if (_isWalking)
            {
                if (ShouldUpdateLookAtRot)
                {
                    _lookAtQuaternion = Quaternion.LookRotation(LookAtVector3 - transform.position);
                    ShouldUpdateLookAtRot = false;
                }
                transform.rotation = Quaternion.Slerp(transform.rotation, _lookAtQuaternion, 10*Time.deltaTime);
                Walk();
            }
        }

        public void StartWalking()
        {
            _isWalking = true;
        }

        public void StopWalking()
        {
            _isWalking = false;
        }

        private void Walk()
        {
            transform.Translate(Vector3.forward * Data.GetSpeed() * Time.deltaTime * (float)_speedMultiplier);
        }

        private void UpdateTime(){
            if (_speedChangeTime <= 0) {
                _speedMultiplier = DEFAULTSPEEDMULTIPLIER;
            } else {
                _speedChangeTime -= Time.deltaTime;
            }
        }

		private void MinionKilled(float delay)
        {
			_player.MinionDied(this, Data.GetDeathLoot(), delay);
		}

        private void OnDestroy()
        {
            OnMap = false;
        }

        public BigIntWithUnit DecreaseLife(BigIntWithUnit damage)
        {
            var updatedDamage = damage*_player.GetModifier(Player.AdSelector.Damage);
            var dmg = Data.DecreaseLife(updatedDamage);
            var height = new Vector3(-5f, 12f, 0f);
            _player.UIManager.CreateFloatingText(updatedDamage.ToString(), transform, transform.position + height, true);
            return dmg;
        }

        public bool ChangeSpeed(double multiplier){
            _speedMultiplier = DEFAULTSPEEDMULTIPLIER * multiplier;
            _speedChangeTime = 5f;
            return true;
        }

        public void StartHighlighting(Color color){
            if ( gameObject.tag == "Boss" ){
                foreach (var r in gameObject.GetComponentsInChildren<Renderer>())
                {
                        r.material.SetColor("_MainColor", color);
                        r.material.SetFloat("_Dist", 0.09f);   
                }
            }else{
                foreach (var r in gameObject.GetComponentsInChildren<Renderer>())
                {
                        r.material.SetColor("_MainColor", color);
                        r.material.SetFloat("_Dist", 0.06f);
                }
            }
        }
        public void StopHighlighting(){
            foreach (var r in gameObject.GetComponentsInChildren<Renderer>())
            {
                    r.material.SetFloat("_Dist", 0.000f);   
            }
        }
    }
}