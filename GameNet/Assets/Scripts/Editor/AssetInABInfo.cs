
using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using System.Reflection;
using Stars;
public class AssetInABInfo 
{
    Assembly _assembly_CSharp = Assembly.Load("Assembly-CSharp");
    Assembly _asembly_CSharp_firstpass = Assembly.Load("Assembly-CSharp-firstpass");
    public class AssetInfoUnit
    {
        public string _guid;
        public string _md5_self;
        public string _md5_meta;
        public string _serializeFields = "";
        public bool equals(AssetInfoUnit unit)
        {
            if (_guid.Equals(unit._guid) && _md5_self.Equals(unit._md5_self)
                && _md5_meta.Equals(unit._md5_meta) && _serializeFields.Equals(unit._serializeFields))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    Dictionary<string, AssetInfoUnit> _ABDepInfoDic = new Dictionary<string, AssetInfoUnit>();
    /// <summary>
    /// 读取AssetBundleDepInfo.xml文件
    /// </summary>
    /// <param name="filename">文件路径</param>
    /// <returns>读取成功返回真</returns>
    public bool read(string filename)
    {
        if (File.Exists(filename) == false) return false;
        _ABDepInfoDic.Clear();
        XmlDocument xmldoc = new XmlDocument();
        xmldoc.Load(filename);
        XmlNode rootElement = xmldoc.SelectSingleNode("AssetInfo");
        if (rootElement == null) return false;
        XmlNodeList nodeList = rootElement.ChildNodes;
        foreach (XmlNode xn in nodeList)
        {
            AssetInfoUnit depInfoUnit = new AssetInfoUnit();
            foreach (XmlAttribute attr in xn.Attributes)
            {
                if (attr.Name.Equals("guid"))
                {
                    depInfoUnit._guid = attr.Value;
                }
                else if (attr.Name.Equals("md5_self"))
                {
                    depInfoUnit._md5_self = attr.Value;
                }
                else if (attr.Name.Equals("md5_meta"))
                {
                    depInfoUnit._md5_meta = attr.Value;
                }
                else if (attr.Name.Equals("serializeFields"))
                {
                    depInfoUnit._serializeFields = attr.Value;
                }
            }
            _ABDepInfoDic.Add(depInfoUnit._guid, depInfoUnit);
        }
        return true;
    }
    /// <summary>
    /// 生成AssetBundleDepInfo.xml文件
    /// </summary>
    /// <param name="filename">文件路径</param>
    public void write(string filename)
    {
        XmlDocument xmldoc = new XmlDocument();
        XmlElement root = xmldoc.CreateElement("AssetInfo");
        xmldoc.AppendChild(root);
        foreach (AssetInfoUnit data in _ABDepInfoDic.Values)
        {
            XmlElement node = xmldoc.CreateElement("file");
            root.AppendChild(node);
            XmlAttribute guid = xmldoc.CreateAttribute("guid");
            guid.InnerText = data._guid;
            node.Attributes.Append(guid);
            XmlAttribute md5_self = xmldoc.CreateAttribute("md5_self");
            md5_self.InnerText = data._md5_self;
            node.Attributes.Append(md5_self);
            XmlAttribute md5_meta = xmldoc.CreateAttribute("md5_meta");
            md5_meta.InnerText = data._md5_meta;
            node.Attributes.Append(md5_meta);
            XmlAttribute serializeFields = xmldoc.CreateAttribute("serializeFields");
            serializeFields.InnerText = data._serializeFields;
            node.Attributes.Append(serializeFields);
        }
        xmldoc.Save(filename);
    }
    /// <summary>
    /// 获取某文件信息
    /// </summary>
    /// <param name="guid">guid</param>
    /// <param name="infoUnit">信息内容</param>
    /// <returns>获取成功返回真</returns>
    public bool tryGetValue(string guid, out AssetInfoUnit infoUnit)
    {
        return _ABDepInfoDic.TryGetValue(guid, out infoUnit);
    }

    /// <summary>
    /// 获取某文件信息
    /// </summary>
    /// <param name="guid">guid</param>
    /// <param name="infoUnit">信息内容</param>
    /// <returns>获取成功返回真</returns>
    public bool tryGetValueByPath(string path, out AssetInfoUnit infoUnit)
    {
        string guid = AssetDatabase.AssetPathToGUID(path);
        return tryGetValue(guid, out infoUnit);
    }
    /// <summary>
    /// 增加文件信息
    /// </summary>
    /// <param name="guid">guid</param>
    /// <param name="infoUnit">信息内容</param>
    public void add(string guid, AssetInfoUnit infoUnit)
    {
        _ABDepInfoDic[guid] = infoUnit;
    }
    /// <summary>
    /// 通过guid添加文件信息
    /// </summary>
    /// <param name="guid">guid</param>
    public void add(string guid)
    {
        string path = AssetDatabase.GUIDToAssetPath(guid);
        add(guid, path);
    }
    /// <summary>
    /// 通过文件路径添加文件信息
    /// </summary>
    /// <param name="path">路径</param>
    public void add_by_path(string path)
    {
        string guid = AssetDatabase.AssetPathToGUID(path);
        add(guid, path);
    }
    /// <summary>
    /// 通过文件guid和路径添加文件信息
    /// </summary>
    /// <param name="guid"></param>
    /// <param name="path"></param>
    public void add(string guid, string path)
    {
        if (_ABDepInfoDic.ContainsKey(guid)) return;
        try
        {
            AssetInfoUnit infoUnit = new AssetInfoUnit();
            infoUnit._guid = guid;
            infoUnit._md5_self = RG_Utils.getMD5HashFromFile(path);
            infoUnit._md5_meta = RG_Utils.getMD5HashFromFile(path + ".meta");
            infoUnit._serializeFields = getScriptSerializeFields(path);
            _ABDepInfoDic[guid] = infoUnit;
        }
        catch
        {
            Debug.LogError("add error");
        }
    }

    string getScriptSerializeFields(string path)
    {
        string serializeFields = "";
        string extension = Path.GetExtension(path);
        Assembly assemblyDll = _assembly_CSharp;
        if(extension.Equals(".cs") || extension.Equals(".CS")){
            string classname = Path.GetFileNameWithoutExtension(path);
            try
            {
                Type monoClassType = assemblyDll.GetType(classname);
                string nameSpace = "";
                if (monoClassType == null)
                {
                    nameSpace = getNameSpace(path);
                    if (nameSpace != "")
                    {
                        classname = nameSpace + classname;
                        monoClassType = assemblyDll.GetType(classname);
                    }
                    if (monoClassType == null)
                    {
                        assemblyDll = _asembly_CSharp_firstpass;
                        monoClassType = assemblyDll.GetType(classname);
                    }
                }
                FieldInfo[] fields = monoClassType.GetFields();
                foreach (FieldInfo info in fields)
                {
                    bool isSerialize = info.IsPublic;
                    //Debug.Log(info.Name + "\n");
                    //Debug.Log("IsPublic " + info.IsPublic + "\n");
                    object[] attrs = info.GetCustomAttributes(true);
                    for (int i = 0; i < attrs.Length; i++)
                    {
                        string attrName = attrs[i].GetType().Name;
                        //Debug.Log("attrs " + attrName + "\n");
                        if (attrName.Equals("SerializeField"))
                        {
                            isSerialize = true;
                            break;
                        }
                        if (attrName.Equals("HideInInspector"))
                        {
                            isSerialize = false;
                        }
                    }
                    if (isSerialize)
                    {
                        serializeFields += info.Name + ";";
                    }
                }
            }
            catch
            {
                //Debug.LogError("can't get class " + path);
            }
        }
        return serializeFields;
    }

    string getNameSpace(string path)
    {
        string nameSpace = "";
        FileStream f = new FileStream(path,FileMode.Open);
        StreamReader sr = new StreamReader(f);
        while (!sr.EndOfStream)
        {
            string line = sr.ReadLine();
            int index = line.IndexOf("namespace ");
            if (index != -1)
            {
                string temp_namespace = line.Substring(index + 10);
                temp_namespace.Trim(' ','{','}');
                nameSpace += temp_namespace + ".";
            }
        }
        return nameSpace;
    }
}
