using UnityEngine;

namespace Assets.Scripts
{
    public class Minion : MonoBehaviour
    {
        public static readonly BigIntWithUnit BaseCurrencyGivenOnDeath = 100;
        public static readonly BigIntWithUnit BaseLife = 100;

        // If the minion enters map, it is changed to true;
        public bool OnMap;

        public float Speed = 0.1f;

        private Player _controller;
        private Animator _minionAnimator;

        public BigIntWithUnit Life = 100;
        public BigIntWithUnit CurrencyGivenOnDeath = 100;

        // Use this for initialization
        private void Start()
        {
            _controller = Camera.main.gameObject.GetComponent<Player>();
            OnMap = true;
            _minionAnimator = gameObject.GetComponent<Animator>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (Life <= 0)
            {
                if (gameObject.tag != "Boss")
                {
                    gameObject.tag = "Untagged";
                }           
                if (_minionAnimator)
                {
                    _minionAnimator.SetTrigger("Die");
                }
				MinionKilled(2.0f);
            }
            else
            {
                Walk();
            }
        }

        private void Walk()
        {
            transform.Translate(Vector3.forward * Speed * Time.deltaTime);
        }

		private void MinionKilled(float delay){
			_controller.MinionDied(this, CurrencyGivenOnDeath);
			Destroy (gameObject, delay);
		}

        private void OnDestroy()
        {
            OnMap = false;
            if (_controller == null)
                return;
        }
    }
}