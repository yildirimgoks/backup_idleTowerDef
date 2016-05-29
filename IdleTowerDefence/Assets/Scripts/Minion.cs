using UnityEngine;

namespace Assets.Scripts
{
    public class Minion : MonoBehaviour
    {

        public static readonly BigIntWithUnit BaseCurrencyGivenOnDeath = 100;
        public static readonly BigIntWithUnit BaseLife = 100;
        Animator minionanim;

        // If the minion enters map, it is changed to true;
        public bool OnMap;

        private Player _controller;
        public TextMesh HealthIndicator;

        public BigIntWithUnit Life = 100;
        public BigIntWithUnit CurrencyGivenOnDeath = 100;
        public float Speed = 0.1f;

        // Use this for initialization
        private void Start()
        {
            _controller = Camera.main.gameObject.GetComponent<Player>();
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
            
            _controller.MinionDied(this, CurrencyGivenOnDeath);
        }

		void Animator(BigIntWithUnit Life) {
			if (Life <= 0) {
				minionanim.SetTrigger("Die");
			}
		}
    }
}