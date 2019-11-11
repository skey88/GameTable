using UnityEngine;
using System.Collections.Generic;
using System;

public interface IConfig
{
    uint id { get; }
}
namespace Stars
{
    public class ConfigDataManager
    {
        //数据字典
        private static readonly Dictionary<Type, IConfig[]> dataDictionary = new Dictionary<Type, IConfig[]>();


        private static readonly Dictionary<Type, Dictionary<uint, IConfig>> dataDictionaryDic = new Dictionary<Type, Dictionary<uint, IConfig>>();

        /// <summary>存储序列化数据</summary>
        public static void addConfigData(Type key, IConfig[] dataArr)
        {
            if (dataDictionary.ContainsKey(key))
            {
                TyLogger.Log("table is contain same the key:"+ key);
                return;
            }
            dataDictionary.Add(key, dataArr);
            int len = dataArr.Length;
            Dictionary<uint, IConfig> dic = new Dictionary<uint, IConfig>();
            for (int i = 0;i < len;i++)
            {
                dic.Add(dataArr[i].id,dataArr[i]);
            }
            dataDictionaryDic[key] = dic;
        }

        /// <summary>通过表名获取表引用</summary>
        public static IConfig[] GetDataArrByName(Type tableName)
        {
            IConfig[] config;
            if(dataDictionary.TryGetValue(tableName,out config))
            {
                return config;
            }
            else
            {
                Debug.LogError("can not found this table:" + tableName);
                return null;
            }
        }
        public static IConfig[] GetDataArrByName(string tableName)
        {
            Type t = Type.GetType(tableName);

            return GetDataArrByName(t);
        }
        public static IConfig[] GetDataArrByName<T>()
        {
            Type t = typeof(T);
            IConfig[] iconfigs = GetDataArrByName(t);
            return iconfigs;
        }

        public static Dictionary<uint, IConfig> GetDataDictionary<T>() where T : IConfig
        {
            Dictionary<uint, IConfig> result;
            dataDictionaryDic.TryGetValue(typeof(T), out result);
            return result;
        }
        /// <summary>根据id获取相关数据</summary>
        public static IConfig GetDataById(Type tableName, uint id)
        {
            Dictionary<uint, IConfig> result;
            if (dataDictionaryDic.TryGetValue(tableName, out result))
            {
                IConfig config;
                if (result.TryGetValue(id, out config))
                {
                    return config;
                }
            }
            TyLogger.LogError("can not found this id:" + id.ToString());
            return null;
        }

        public static IConfig GetDataById<T>(uint id)
        {
            Dictionary<uint, IConfig> result;
            if (dataDictionaryDic.TryGetValue(typeof(T), out result))
            {
                IConfig config;
                if (result.TryGetValue(id, out config))
                {
                    return config;
                }
            }
            TyLogger.LogError(typeof(T) + " can not found this id:" + id.ToString());
            return null;
        }
    }

}