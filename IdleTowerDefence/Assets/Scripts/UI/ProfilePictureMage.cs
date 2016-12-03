using UnityEngine;

namespace Assets.Scripts.UI
{
    public class ProfilePictureMage : MonoBehaviour {
        private Animator _animator;

        private Vector3 _basePosition;
        private Quaternion _baseRotation;

        // Use this for initialization
        void Start ()
        {
            _animator = GetComponent<Animator>();
            _basePosition = transform.position;
            _baseRotation = transform.rotation;
            StartAnimation();
        }
	
        // Update is called once per frame
        void Update ()
        {
            transform.rotation = _baseRotation;
            transform.position = _basePosition;
        }

        private void StartAnimation(){
            _animator.SetTrigger("Initial");
        }
    }
}
