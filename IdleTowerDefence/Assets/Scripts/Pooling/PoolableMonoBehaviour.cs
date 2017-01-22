using System;
using Assets.Scripts.Manager;
using UnityEngine;
using Object = System.Object;

namespace Assets.Scripts.Pooling
{
    public abstract class PoolableMonoBehaviour : MonoBehaviour
    {
        public static PoolableMonoBehaviour GetPoolable(Type type, PoolableMonoBehaviour original)
        {
            return ObjectPoolingManager.GetInstance().GetPoolableObject(type, original);
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
    }
}
