using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IPoolAble
{
    void SetActive(bool active);
    IPoolAble NextAvaiable();
    void SetNextAvaiable(IPoolAble newObj);
    void ClearNextAvaiable();
}

public class WrapPool : ScriptableObject, IPoolAble
{
    private IPoolAble nextAvaiable;
    public GameObject cannonBall;

    public WrapPool(GameObject cannonBall)
    {
        this.cannonBall = cannonBall;
    }

    public WrapPool()
    {
    }

    public void ClearNextAvaiable()
    {
        nextAvaiable = null;
    }

    public IPoolAble NextAvaiable()
    {
        return nextAvaiable;
    }

    public void SetActive(bool active)
    {
        cannonBall.SetActive(active);
    }

    public void SetNextAvaiable(IPoolAble newObj)
    {
        nextAvaiable = newObj;
    }
}
public class Factory<T> : ScriptableObject where T : ScriptableObject
{
    public T seed;
    public Factory(T seed)
    {
        this.seed = seed;
    }
    public T CreateNew()
    {
        return Instantiate<T>(seed);
    }
}


public class PoolManager<T> where T : ScriptableObject, IPoolAble
{
    public List<T> pooledObjects;
    private T currentAvaiable;
    private IPoolAble lastAvaiable;
    private Factory<T> factory;

    public int limit = -1;

    public UnityAction OnCreateNew;

    public PoolManager(T seed)
    {
        factory = new Factory<T>(seed);
        pooledObjects = new List<T>();
        currentAvaiable = seed;
        pooledObjects.Add(currentAvaiable);
        lastAvaiable = seed;
    }

    private T InstanceNew()
    {
        // T obj = Instantiate(objectToPool).GetComponent<MonsterAttack>();
        T obj = factory.CreateNew();
        pooledObjects.Add(obj);
        if (OnCreateNew != null)
        {
            OnCreateNew.Invoke();
        }
        obj.SetActive(false);
        return obj;
    }

    public T GetPooledObject()
    {
        T obj = currentAvaiable;
        if (obj == null)
        {
            if (limit > -1 && pooledObjects.Count == limit)
            {
            }
            else
            {
                obj = InstanceNew();
                obj.SetNextAvaiable(InstanceNew());
                currentAvaiable = obj;
            }
        }
        currentAvaiable = (T)currentAvaiable?.NextAvaiable();
        if (currentAvaiable == null)
        {
            if (limit > -1 && pooledObjects.Count == limit)
            {
                lastAvaiable = null;
            }
            else
            {
                currentAvaiable = InstanceNew();
                lastAvaiable = currentAvaiable;
            }
        }
        // if (currentAvaiable.NextAvaiable() == null)
        // {
        //     Debug.Log("pool 3");
        //     T newObj = InstanceNew();
        //     currentAvaiable.SetNextAvaiable(newObj);
        //     lastAvaiable = newObj;
        // }
        return obj;
    }

    public void RePooledObject(T obj)
    {
        obj.SetActive(false);
        obj.ClearNextAvaiable();
        if (lastAvaiable != null)
        {
            lastAvaiable.SetNextAvaiable(obj);
        }
        else
        {
            currentAvaiable = obj;
        }
        lastAvaiable = obj;
    }
}
