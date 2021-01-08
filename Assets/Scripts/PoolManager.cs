using System;
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


    IPoolAble GetPrev();
    void SetPrev(IPoolAble newObj);
    void ClearPrev();
    void Destroy();
}

[Serializable]
public class WrapPool : ScriptableObject, IPoolAble
{
    private IPoolAble prev;
    private IPoolAble nextAvaiable;
    public GameObject cannonBall;

    public void Destroy()
    {
        Destroy(cannonBall);
    }

    public WrapPool(GameObject cannonBall)
    {
        this.cannonBall = cannonBall;
    }

    public WrapPool()
    {
    }
    public void ClearPrev()
    {
        if (prev != null)
        {
            prev.ClearNextAvaiable();
        }
        prev = null;
    }

    public IPoolAble GetPrev()
    {
        return prev;
    }
    public void SetPrev(IPoolAble obj)
    {
        prev = obj;
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
        newObj.SetPrev(this);
    }
}
public class Factory<T> : ScriptableObject where T : ScriptableObject
{
    public T seed;

    public void SetSeed(T seed)
    {
        this.seed = seed;
    }
    public Factory(T seed)
    {
        this.seed = seed;
    }
    public T CreateNew(T newSeed = null)
    {
        if (newSeed != null) seed = newSeed;
        return Instantiate<T>(seed);
    }

    public void Reclaim(T item)
    {
        Debug.Log("reduce size destroy" + item.GetType());
        Destroy(item);
    }
}


[Serializable]
public class PoolManager<T> where T : ScriptableObject, IPoolAble
{
    public List<T> pooledObjects;
    private T currentAvaiable;
    private IPoolAble lastAvaiable;
    private Factory<T> factory;

    public int limit = -1;
    public int buffer = 10;
    public int numActive = 0;
    public int numInActive = 0;
    public int total = 0;

    public T seedItem;

    public T newInstance;

    public UnityAction OnCreateNew;

    public PoolManager(T seed)
    {
        seedItem = seed;
        factory = new Factory<T>(seed);
        // pooledObjects = new List<T>();
        // pooledObjects.Add(currentAvaiable);


        // currentAvaiable = seed;
        // lastAvaiable = seed;
        // total++;
        // numInActive++;
    }

    private T InstanceNew()
    {
        // T obj = Instantiate(objectToPool).GetComponent<MonsterAttack>();
        T obj = factory.CreateNew();
        // pooledObjects.Add(obj);
        newInstance = obj;
        if (OnCreateNew != null)
        {
            OnCreateNew.Invoke();
        }
        obj.SetActive(false);
        total++;
        numInActive++;
        return obj;
    }

    public T GetPooledObject()
    {
        T obj = currentAvaiable;
        if (obj == null)
        {
            if (limit > -1 && total == limit)
            {
            }
            else
            {
                obj = InstanceNew();
                obj.SetNextAvaiable(InstanceNew());
                // obj.NextAvaiable().SetPrev(obj);
                currentAvaiable = obj;
            }
        }
        currentAvaiable = (T)currentAvaiable?.NextAvaiable();
        if (currentAvaiable == null)
        {
            if (limit > -1 && total == limit)
            {
                lastAvaiable = null;
            }
            else
            {
                currentAvaiable = InstanceNew();
                // currentAvaiable.SetPrev(lastAvaiable);
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
        numActive++;
        numInActive--;
        return obj;
    }

    public void RePooledObject(T obj)
    {
        obj.SetActive(false);
        obj.ClearNextAvaiable();
        if (lastAvaiable != null)
        {
            // obj.SetPrev(lastAvaiable);
            lastAvaiable.SetNextAvaiable(obj);
        }
        else
        {
            obj.ClearPrev();
            currentAvaiable = obj;
        }
        lastAvaiable = obj;
        numInActive++;
        numActive--;
        if (numInActive > buffer)
        {
            ReduceSize();
        }
    }

    private void ReduceSize()
    {
        while (numInActive > buffer && lastAvaiable.GetPrev() != null)
        {
            IPoolAble remove = lastAvaiable;
            lastAvaiable = lastAvaiable.GetPrev();
            lastAvaiable.ClearNextAvaiable();
            remove.ClearPrev();
            remove.Destroy();
            factory.Reclaim((T)remove);
            total--;
            numInActive--;
            // Debug.Log("reduce size " + total + " " + numInActive + " " + (numInActive > buffer) + " " + (lastAvaiable.GetPrev() != null));
        }
    }
}
