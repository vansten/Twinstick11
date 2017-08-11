using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GrowthStrategy
{
    ResizeByOne,
    DoubleSize,
}

public class ObjectPool<T> where T : MonoBehaviour
{
    #region Variables

    protected T _prefab;
    protected Transform _spawnedObjectsParent;
    protected List<T> _spawnedObjects;
    protected GrowthStrategy _growthStrategy;

    #endregion

    #region Methods

    public void Init(T prefab, Transform parent, GrowthStrategy growthStrategy = GrowthStrategy.DoubleSize, int initialCount = 32)
    {
        _prefab = prefab;
        _spawnedObjectsParent = parent;
        _spawnedObjects = new List<T>();
        _growthStrategy = growthStrategy;

        SpawnObjects(initialCount);
    }

    public void CollectInactiveObjects()
    {
        int objectsCount = _spawnedObjects.Count;
        for(int i = 0; i < objectsCount; ++i)
        {
            if(!_spawnedObjects[i].gameObject.activeInHierarchy && _spawnedObjects[i].transform.parent == null)
            {
                _spawnedObjects[i].transform.parent = _spawnedObjectsParent;
            }
        }
    }

    public T GetObject(Transform transform)
    {
        if(_spawnedObjects == null)
        {
            Debug.LogError("GetObject called on unitialized pool!");
            return null;
        }

        int objectsCount = _spawnedObjects.Count;
        for(int i = 0; i < objectsCount; ++i)
        {
            if(!_spawnedObjects[i].gameObject.activeInHierarchy)
            {
                _spawnedObjects[i].gameObject.SetActive(true);
                _spawnedObjects[i].transform.parent = transform;
                if(transform != null)
                {
                    _spawnedObjects[i].transform.position = transform.position;
                    _spawnedObjects[i].transform.rotation = transform.rotation;
                }

                return _spawnedObjects[i];
            }
        }

        Grow();

        if(_spawnedObjects.Count > objectsCount)
        {
            _spawnedObjects[objectsCount].gameObject.SetActive(true);
            _spawnedObjects[objectsCount].transform.parent = transform;
            if (transform != null)
            {
                _spawnedObjects[objectsCount].transform.position = transform.position;
                _spawnedObjects[objectsCount].transform.rotation = transform.rotation;
            }

            return _spawnedObjects[objectsCount];
        }

        return null;
    }

    protected void Grow()
    {
        switch(_growthStrategy)
        {
            case GrowthStrategy.DoubleSize:
                SpawnObjects(_spawnedObjects.Count);
                break;
            case GrowthStrategy.ResizeByOne:
                SpawnObjects(1);
                break;
        }
    }

    protected void SpawnObjects(int count)
    {
        for(int i = 0; i < count; ++i)
        {
            T newObject = UnityEngine.Object.Instantiate(_prefab, _spawnedObjectsParent.position, _spawnedObjectsParent.rotation, _spawnedObjectsParent);
            newObject.gameObject.SetActive(false);
            _spawnedObjects.Add(newObject);
        }
    }

    #endregion
}
