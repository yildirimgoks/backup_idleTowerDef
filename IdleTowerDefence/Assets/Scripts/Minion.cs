﻿using UnityEngine;

namespace Assets.Scripts
{
    public class Minion : MonoBehaviour
    {
        private BigIntWithUnit _currencyGivenOnDeath = 100;
		Animator minionanim;

        // If the minion enters map, it is changed to true;
        public bool OnMap;

        private readonly Player _controller = Camera.main.gameObject.GetComponent<Player>();
        public TextMesh HealthIndicator;

        public BigIntWithUnit Life = 100;
        public float Speed = 0.1f;

        // Player gets BaseCurrency times Life points on minion death
        private BigIntWithUnit BaseCurrency;

        // Use temp until BigIntWithUnit multiplication is implemented
        private BigIntWithUnit temp;

        // Use this for initialization
        private void Start()
        {
            temp = Life;
            OnMap = true;
        }

        // Update is called once per frame
        private void Update()
        {
            HealthIndicator.text = "" + Life;
            if (Life <= 0)
            {
				Animator (Life);
				Destroy(gameObject, 2f);
            }
            Walk();

        }

        private void Walk()
        {
            transform.Translate(Vector3.forward*Speed*Time.deltaTime);
        }

        private void OnDestroy()
        {
            if (_controller == null)
                return;
            OnMap = false;
            _currencyGivenOnDeath = temp;
            _controller.MinionDied(this, _currencyGivenOnDeath);
        }

		void Animator(BigIntWithUnit Life) {
			if (Life <= 0) {
				minionanim.SetTrigger("Die");
			}
		}
    }
}