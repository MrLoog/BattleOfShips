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
    protected BaseStoreData() { }

    public static Encoding EncodingUse = Encoding.UTF8;
    public virtual T LoadData<T>(string key)
    {
        string path = GetDataPath(key);
        Debug.Log("Store Load data from " + GetDataPath(key));
        if (!File.Exists(path)) return default(T);
        byte[] data = File.ReadAllBytes(path);
        if (data.Length > 0)
        {
            string strData = EncodingUse.GetString(data);

            T objData = (T)(typeof(MyJsonUtil).GetMethod("DeserializeObject").MakeGenericMethod(typeof(T)).Invoke(null, new object[] { strData }));
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

    public virtual bool SaveData(string key, BaseDataEntity data)
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
