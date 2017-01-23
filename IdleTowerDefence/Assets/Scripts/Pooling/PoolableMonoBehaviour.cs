using System;
using Assets.Scripts.Manager;
using Assets.Scripts.UI;
using UnityEngine;
using Object = System.Object;

namespace Assets.Scripts.Pooling
{
    public abstract class PoolableMonoBehaviour : MonoBehaviour
    {
        private int OriginalInstanceId;

        public static PoolableMonoBehaviour GetPoolable(PoolableMonoBehaviour original)
        {
            return ObjectPoolingManager.GetInstance().GetPoolableObject(original);
        }
        
        public void SetActive(bool active)
        {
            this.gameObject.SetActive(active);
            this.enabled = active;
        }

        public virtual void OnDestroy()
        {
            
        }

        public void Destroy()
        {
            this.OnDestroy();           
            SetActive(false);
            ObjectPoolingManager.GetInstance().OnDestroyObject(this);
        }

        public void Destroy(float t)
        {
            Invoke("Destroy", t);
        }

        public void SetOriginalInstanceId(PoolableMonoBehaviour original)
        {
            OriginalInstanceId = original.GetInstanceID();
        }

        public int GetOriginalInstanceId()
        {
            return OriginalInstanceId;
        }
    }
}
