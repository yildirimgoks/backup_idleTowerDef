using UnityEngine;

namespace Assets.Scripts
{
    public class Waypoint : MonoBehaviour
    {
        public Waypoint Next;
        public Waypoint Previous;

        public bool First;
        public bool Last;
        
        private void OnTriggerEnter(Collider Other)
        {
            var minion = Other.gameObject.GetComponent<Minion>();
            if (minion)
            {
                if (Next != null)
                {
                    
                    var offset = Other.gameObject.transform.position - gameObject.transform.position;
                    offset.y = 0;
                    minion.LookAtVector3 = Next.transform.position + offset;
                    minion.ShouldUpdateLookAtRot = true;
                }
                if (Last)
                {
                    Minion survivor = Other.gameObject.GetComponent<Minion>();
                    if (survivor)
                    {
                        survivor.OnSurvived();
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
                    Other.gameObject.GetComponent<SkillProjectile>().Destroy();
                }
            }
        }
    }
}