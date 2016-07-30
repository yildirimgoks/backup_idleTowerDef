using UnityEngine;

namespace Assets.Scripts
{
    public class Waypoint : MonoBehaviour
    {
        public Waypoint Next;

        public bool Last;

        //private bool flag = false;
        //private int dist  = 0;

        private string[] _debugSong = {"No, not I, I will survive",
            "Long as I know how to love",
            "I know I'll stay alive",
            "I've got all my life to live",
            "And all my love to give and I'll survive",
            "I will survive" };

        private int _line;

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
                    Debug.Log(_debugSong[_line % 5]);
                    _line++;
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