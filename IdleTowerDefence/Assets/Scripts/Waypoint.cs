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
            if (Other.gameObject.GetComponent<Minion>())
            {
                if (Next != null)
                {
                    Other.gameObject.transform.LookAt(Next.transform);
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
        }
    }
}