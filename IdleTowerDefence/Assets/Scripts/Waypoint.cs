using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class Waypoint : MonoBehaviour {

    public Waypoint Next;

    public bool Last;

    private string[] _debugSong = {"No, not I, I will survive",
                                "Long as I know how to love",
                                 "I know I'll stay alive",
                                "I've got all my life to live",
                                "And all my love to give and I'll survive",
                                "I will survive" };

    private int _line = 0;

    private Player _controller;
    void Start()
    {
        _controller = Camera.main.gameObject.GetComponent<Player>();
    }

    void OnTriggerEnter(Collider Other)
    {
        if (Other.gameObject.GetComponent<Minion>())
        {
            if (Next != null)
            {
                Other.gameObject.transform.LookAt(Next.transform);
            }
            if (Last)
            {
                
                if(_line == 5){
                    _line = 0;
                }
                Debug.Log(_debugSong[_line]);
                _line++;
                Minion survivor = Other.gameObject.GetComponent<Minion>();
                if (survivor)
                {
                    _controller.MinionSurvived(survivor);
                    survivor.OnMap = false;
                }
                
            }
        }
            
    }

}
