using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSceneManager : MonoBehaviour
{
    protected bool autoRestoreState = true;
    public virtual void Awake()
    {
        if (autoRestoreState)
            RestoreFromData(GameManager.Instance.LoadDataScene(this.GetType().Name));
    }
    public virtual BaseDataEntity GetDataForSave()
    {
        // throw new System.NotImplementedException();
        return null;
    }

    public virtual void RestoreFromData(BaseDataEntity data)
    {
        // throw new System.NotImplementedException();
    }
}
