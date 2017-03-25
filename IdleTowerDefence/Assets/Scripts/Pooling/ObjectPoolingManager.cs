﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

namespace Assets.Scripts.Pooling
{
    public class ObjectPoolingManager
    {
        private static ObjectPoolingManager _instance;

        private readonly Dictionary<int, Queue<PoolableMonoBehaviour>> _pooledObjectDictionary;

        private ObjectPoolingManager()
        {
            _pooledObjectDictionary = new Dictionary<int, Queue<PoolableMonoBehaviour>>();
        }

        public static ObjectPoolingManager GetInstance()
        {
            return _instance ?? (_instance = new ObjectPoolingManager());
        }

        public void OnDestroyObject(PoolableMonoBehaviour poolableMonoBehaviour)
        {
            var poolableClass = poolableMonoBehaviour.GetOriginalInstanceId();
            if (!_pooledObjectDictionary.ContainsKey(poolableClass))
            {
                Queue<PoolableMonoBehaviour> poolableQue = new Queue<PoolableMonoBehaviour>();
                poolableQue.Enqueue(poolableMonoBehaviour);
                _pooledObjectDictionary.Add(poolableClass, poolableQue);
            }
            else
            {
                Queue<PoolableMonoBehaviour> poolableQueue = _pooledObjectDictionary[poolableClass];
                poolableQueue.Enqueue(poolableMonoBehaviour);
            }
        }

        public PoolableMonoBehaviour GetPoolableObject(PoolableMonoBehaviour original)
        {
            int instanceID = original.GetInstanceID();
            PoolableMonoBehaviour objectToReturn = null;
            if (_pooledObjectDictionary.ContainsKey(instanceID))
            {
                Queue<PoolableMonoBehaviour> poolableQueue = _pooledObjectDictionary[instanceID];
                if (poolableQueue.Count > 0)
                {
                    objectToReturn = poolableQueue.Dequeue();
                }
            }
            if (objectToReturn == null)
            {
                objectToReturn = MonoBehaviour.Instantiate(original);
            }
            objectToReturn.SetActive(true);
            objectToReturn.SetOriginalInstanceId(original);
            return objectToReturn;
        }

    }
}