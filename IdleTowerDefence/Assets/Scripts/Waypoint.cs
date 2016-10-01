using UnityEngine;

namespace Assets.Scripts
{
    public class Waypoint : MonoBehaviour
    {
        public Waypoint Next;
        public Waypoint Previous;

        public bool First;
        public bool Last;

        //private bool flag = false;
        //private int dist  = 0;

        private Player _controller;

        private void Start()
        {
            _controller = Camera.main.gameObject.GetComponent<Player>();
        }

        private void OnTriggerEnter(Collider Other)
        {
            var minion = Other.gameObject.GetComponent<Minion>();
            if (minion)
            {
                if (Next != null)
                {
                    
                    var offset = Other.gameObject.transform.position - gameObject.transform.position;
                    offset.y = 0;
                    
                    //Other.gameObject.transform.LookAt(Next.transform.position + offset);
                    minion.LookAtVector3 = Next.transform.position + offset;
                    minion.shouldUpdateLookAtRot = true;
                    /*
                     * distance calculating part
                     * if (!flag)
                    {
                        dist = (int) Vector3.Distance(Other.gameObject.transform.position,Next.transform.position);
                        WaveManager.totalDistance += dist;
                        Debug.Log(WaveManager.totalDistance);
                        flag = true;
                    }
                    */
                }
                if (Last)
                {
                    Minion survivor = Other.gameObject.GetComponent<Minion>();
                    if (survivor)
                    {
                        _controller.WaveManager.MinionSurvived(survivor);
                        survivor.OnMap = false;
                    }
                }
            }
            else if (Other.gameObject.GetComponent<SkillProjectile>())
            {
                if (Previous != null)
                {
                    Other.gameObject.transform.LookAt(Previous.transform);
                }
                if (First){
                    Destroy(Other.gameObject);
                }
            }
        }
    }
}