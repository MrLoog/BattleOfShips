using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public class BaseStoreData : IStoreData
{
    public const string TEMPLATE_PATH = "{0}/{1}.save";

    public const string version = "1.0.0";
    public const string gameSystemFileName = "game_system";
    public Dictionary<string, object> systemSettings;
    public List<string> AllSaveFiles
    {
        get
        {
            if (!systemSettings.ContainsKey(SYSTEM_SETTINGS_KEY_ALL_FILES))
            {
                systemSettings.Add(SYSTEM_SETTINGS_KEY_ALL_FILES, new List<string>());
            }
            return (List<string>)systemSettings[SYSTEM_SETTINGS_KEY_ALL_FILES];
        }
    }
    public const string SYSTEM_SETTINGS_KEY_VERSION = "version";
    public const string SYSTEM_SETTINGS_KEY_ALL_FILES = "all_files";
    protected bool isInit = false;
    protected BaseStoreData() { }

    public static Encoding EncodingUse = Encoding.UTF8;

    protected virtual void LoadSystemData(bool force = false)
    {
        systemSettings = LoadData<Dictionary<string, object>>(gameSystemFileName, true);
        if (systemSettings == null)
        {
            systemSettings = new Dictionary<string, object>();
            AddSettings(SYSTEM_SETTINGS_KEY_VERSION, version, true);
            AddSaveKey(gameSystemFileName);
        }
        if (systemSettings.ContainsKey(SYSTEM_SETTINGS_KEY_VERSION) && !systemSettings[SYSTEM_SETTINGS_KEY_VERSION].ToString().Equals(version))
        {
            OnChangeVersion();
        }
        isInit = true;
    }

    protected void AddSaveKey(string key)
    {
        if (!AllSaveFiles.Contains(key))
        {
            AllSaveFiles.Add(key);
            AddSettings(SYSTEM_SETTINGS_KEY_ALL_FILES, AllSaveFiles);
        }
    }

    public void AddSettings(string key, object value, bool save = true)
    {
        if (systemSettings.ContainsKey(key))
        {
            systemSettings[key] = value;
        }
        else
        {
            systemSettings.Add(key, value);
        }
        if (save)
        {
            SaveSystemSettings();
        }
    }

    public void RemoveSettings(string key, bool save = true)
    {
        if (systemSettings.ContainsKey(key))
        {
            systemSettings.Remove(key);
            if (save)
            {
                SaveSystemSettings();
            }
        }
    }

    public void SaveSystemSettings()
    {
        SaveData(gameSystemFileName, systemSettings);
    }
    protected virtual void OnChangeVersion()
    {
        //do whatever to update
        AddSettings(SYSTEM_SETTINGS_KEY_VERSION, version); //done update save new version
    }

    public virtual T LoadData<T>(string key, bool ignoreCheckInit = false)
    {
        if (!isInit && !ignoreCheckInit)
        {
            LoadSystemData();
        }
        string path = GetDataPath(key);
        Debug.Log("Store Load data from " + GetDataPath(key));
        if (!File.Exists(path)) return default(T);
        byte[] data = File.ReadAllBytes(path);
        if (data.Length > 0)
        {
            string strData = EncodingUse.GetString(data);
            Debug.Log("Store Load data from " + GetDataPath(key) + " : " + strData);


            T objData = (T)(typeof(MyJsonUtil).GetMethod("DeserializeScriptableObject").MakeGenericMethod(typeof(T)).Invoke(null, new object[] { strData }));
            return objData;
        }
        return default(T);

        // BinaryFormatter bf = new BinaryFormatter();
        // string path = GetDataPath(key);
        // Debug.Log("Store Load data from " + GetDataPath(key));
        // if (!File.Exists(path)) return null;
        // using (FileStream file = File.Open(GetDataPath(key), FileMode.Open))
        // {
        //     string fileContents;
        //     using (StreamReader reader = new StreamReader(file))
        //     {
        //         fileContents = reader.ReadToEnd();
        //     }
        //     // String save = bf.Deserialize<String>(file);
        //     Debug.Log("Store Load data from " + GetDataPath(key) + " : " + fileContents);
        //     BaseDataEntity data = MyJsonUtil.DeserializeObject<BaseDataEntity>(fileContents);
        //     file.Close();
        //     return data;
        // }
    }

    public virtual bool SaveData(string key, object data)
    {
        try
        {
            // BinaryFormatter bf = new BinaryFormatter();
            // FileStream file = File.Create(GetDataPath(key));
            // string dataStr = MyJsonUtil.SerializeScriptableObject(data);
            // Debug.Log("Store Save data to " + GetDataPath(key) + " : " + dataStr);
            // bf.Serialize(file, dataStr);
            // ByteStream
            // file.Close();
            // return true;
            string dataStr = MyJsonUtil.SerializeScriptableObject(data);
            byte[] save = EncodingUse.GetBytes(dataStr);
            Debug.Log("Store Save data to " + GetDataPath(key) + " : " + dataStr);
            File.WriteAllBytes(GetDataPath(key), save);
            AddSaveKey(key);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public string GetDataPath(string key)
    {
        return string.Format(TEMPLATE_PATH, Application.persistentDataPath, key);
    }
}
