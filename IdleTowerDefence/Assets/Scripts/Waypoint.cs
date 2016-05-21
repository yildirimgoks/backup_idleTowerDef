using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class Waypoint : MonoBehaviour {

    public Waypoint Next;

    public bool last;

    private string[] debugSong = {"No, not I, I will survive",
                                "Long as I know how to love",
                                 "I know I'll stay alive",
                                "I've got all my life to live",
                                "And all my love to give and I'll survive",
                                "I will survive" };

    private int line = 0;

    void OnTriggerEnter(Collider Other)
    {
        if (Other.gameObject.GetComponent<Minion>())
        {
            if (Next != null)
            {
                Other.gameObject.transform.LookAt(Next.transform);
            }
            if (last)
            {   
                if(line == 5){
                    line = 0;
                }
                Debug.Log(debugSong[line]);
                line++;
            }
        }
            
    }

}
