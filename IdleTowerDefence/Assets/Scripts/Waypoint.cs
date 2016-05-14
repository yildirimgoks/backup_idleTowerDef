using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class Waypoint : MonoBehaviour {

    public Waypoint Next;

    void OnTriggerEnter(Collider Other)
    {
        if (Other.gameObject.GetComponent<Minion>() && Next != null)
        {
            Other.gameObject.transform.LookAt(Next.transform);
        }
    }

}
