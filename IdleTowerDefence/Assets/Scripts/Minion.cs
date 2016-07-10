using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class Minion : MonoBehaviour
    {
        public MinionData Data;
        // If the minion enters map, it is changed to true;
        public bool OnMap;
        
        private Player _controller;
        // private Animator _minionAnimator;
        private Animation _minionAnimation;
        
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
        }

        // Update is called once per frame
        private void Update()
        {
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
                Walk();
            }
        }

        private void Walk()
        {
            transform.Translate(Vector3.forward * Data.GetSpeed() * Time.deltaTime);
        }

		private void MinionKilled(float delay)
        {
			_controller.MinionDied(this, Data.GetDeathLoot(), delay);
		}

        private void OnDestroy()
        {
            OnMap = false;
        }
    }
}