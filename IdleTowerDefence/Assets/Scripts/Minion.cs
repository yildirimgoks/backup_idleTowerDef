using Assets.Scripts.Manager;
using UnityEngine;

namespace Assets.Scripts
{
    public class Minion : MonoBehaviour
    {
        private const double DEFAULTSPEEDMULTIPLIER = 1.0;

        public MinionData Data;
        // If the minion enters map, it is changed to true;
        public bool OnMap;

        private UIManager _uiman;
        
        private Player _controller;
        // private Animator _minionAnimator;
        private Animation _minionAnimation;

        private double speedMultiplier = DEFAULTSPEEDMULTIPLIER;
        private float speedChangeTime = 0f;

        public Vector3 LookAtVector3;
        private Quaternion lookAtQuaternion;
        public bool shouldUpdateLookAtRot;

        // Use this for initialization
        private void Start()
        {
            _controller = Camera.main.gameObject.GetComponent<Player>();
            OnMap = true;
            // _minionAnimator = gameObject.GetComponent<Animator>();
            _minionAnimation = gameObject.GetComponent<Animation>();
            if (Data == null)
            {
                Data = new MinionData();
            }
            shouldUpdateLookAtRot = false;
            lookAtQuaternion = transform.rotation;
            _minionAnimation["Walk"].speed = Data.GetSpeed()/10.0f;
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
                    _minionAnimation.Play("Death");
                    var delay = _minionAnimation.GetClip("Death").length;
                    MinionKilled(delay);
                    // _minionAnimator.SetTrigger("Die");
                } else {
				    MinionKilled(0);
                }
            }
            else
            {
                if (shouldUpdateLookAtRot)
                {
                    lookAtQuaternion = Quaternion.LookRotation(LookAtVector3 - transform.position);
                    shouldUpdateLookAtRot = false;
                }
                transform.rotation = Quaternion.Slerp(transform.rotation, lookAtQuaternion, 10*Time.deltaTime);
                Walk();
            }
        }

        private void Walk()
        {
            transform.Translate(Vector3.forward * Data.GetSpeed() * Time.deltaTime * (float)speedMultiplier);
        }

        private void UpdateTime(){
            if ( speedChangeTime <= 0){
                speedMultiplier = DEFAULTSPEEDMULTIPLIER;
            }else{
                speedChangeTime -= Time.deltaTime;
            }
        }

		private void MinionKilled(float delay)
        {
			_controller.MinionDied(this, Data.GetDeathLoot(), delay);
		}

        private void OnDestroy()
        {
            OnMap = false;
        }

        public void SetUiManager(UIManager uiManager)
        {
            _uiman = uiManager;
        }

        public BigIntWithUnit DecreaseLife(BigIntWithUnit damage)
        {
            var dmg = Data.DecreaseLife(damage);
            var height = new Vector3(-5f, 12f, 0f);
            _uiman.CreateFloatingText(damage.ToString(), transform, transform.position + height, "d");
            return dmg;
        }

        public bool ChangeSpeed(double multiplier){
            speedMultiplier = DEFAULTSPEEDMULTIPLIER * multiplier;
            speedChangeTime = 5f;
            return true;
        }

        public Color initialColor;
        public void StartHighlighting(Color color){
            foreach (var r in gameObject.GetComponentsInChildren<Renderer>())
            {
                    r.material.SetColor("_MainColor", color);
                    r.material.SetFloat("_Dist", 0.05f);   
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