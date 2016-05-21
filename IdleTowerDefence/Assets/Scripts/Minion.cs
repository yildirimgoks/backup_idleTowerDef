using UnityEngine;

namespace Assets.Scripts
{
    public class Minion : MonoBehaviour
    {
        private readonly BigIntWithUnit _currencyGivenOnDeath = 100;

        // If the minion enters map, it is changed to true;
        private bool _enteredMap;

        private readonly Player _controller = Camera.main.gameObject.GetComponent<Player>();
        public TextMesh HealthIndicator;

        public int Life = 100;
        public float Speed = 0.1f;

        // Use this for initialization
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            HealthIndicator.text = "" + Life;
            if (Life <= 0)
            {
                Destroy(gameObject);
            }
            Walk();
            if (Player.OnMap(gameObject))
            {
                // If minion enters map
                _enteredMap = true;
            }
            if (_enteredMap && !Player.OnMap(gameObject))
            {
                // If minion entered map before, and not on the map right now -> Minion leaves the map
                _controller.MinionSurvived(this);
            }
        }

        // To UYGAR from HAYDAR: Denerken kullandim bu şekilde, istediğin şekilde değiştirirsin
        private void Walk()
        {
            transform.Translate(Vector3.forward*Speed*Time.deltaTime);
        }

        private void OnDestroy()
        {
            if (_controller == null)
                return;
            _controller.MinionDied(this, _currencyGivenOnDeath);
        }
    }
}