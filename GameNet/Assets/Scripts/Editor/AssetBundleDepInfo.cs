
using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
public class AssetBundleDepInfo
{
    public class ABDepInfo
    {
        public string _url;
        public List<string> _depAssetGuid = new List<string>();
    }

    Dictionary<string, ABDepInfo> _ABDepDic = new Dictionary<string, ABDepInfo>();

    public bool read(string filename)
    {
        if (File.Exists(filename) == false) return false;
        _ABDepDic.Clear();
        XmlDocument xmldoc = new XmlDocument();
        xmldoc.Load(filename);
        XmlNode rootElement = xmldoc.SelectSingleNode("ABDepInfo");
        if (rootElement == null) return false;
        XmlNodeList nodeList = rootElement.ChildNodes;
        foreach (XmlNode xn in nodeList)
        {
            ABDepInfo depInfoUnit = new ABDepInfo();
            foreach (XmlAttribute attr in xn.Attributes)
            {
                if (attr.Name.Equals("url"))
                {
                    depInfoUnit._url = attr.Value;
                }
            }
            XmlNodeList xnChildNodeList = xn.ChildNodes;
            foreach (XmlNode xnChild in xnChildNodeList)
            {
                foreach (XmlAttribute attr in xnChild.Attributes)
                {
                    if (attr.Name.Equals("guid"))
                    {
                        depInfoUnit._depAssetGuid.Add( attr.Value);
                    }
                }
            }

            _ABDepDic.Add(depInfoUnit._url, depInfoUnit);
        }
        return true;
    }

    public void write(string filename)
    {
        XmlDocument xmldoc = new XmlDocument();
        XmlElement root = xmldoc.CreateElement("ABDepInfo");
        xmldoc.AppendChild(root);
        foreach (ABDepInfo data in _ABDepDic.Values)
        {
            XmlElement node = xmldoc.CreateElement("ab");
            root.AppendChild(node);
            XmlAttribute url = xmldoc.CreateAttribute("url");
            url.InnerText = data._url;
            node.Attributes.Append(url);
            foreach (string guid in data._depAssetGuid)
            {
                XmlElement asset = xmldoc.CreateElement("asset");
                node.AppendChild(asset);
                XmlAttribute guidAttr = xmldoc.CreateAttribute("guid");
                guidAttr.InnerText = guid;
                asset.Attributes.Append(guidAttr);
            }
        }
        xmldoc.Save(filename);
    }

    public void add_by_path(string url, string[] paths)
    {
        List<string> depAssetGuid = new List<string>();
        for (int i = 0; i < paths.Length; i++)
        {
            depAssetGuid.Add(AssetDatabase.AssetPathToGUID(paths[i]));
        }
        add(url, depAssetGuid);
    }

    public void add(string url, List<string> depAssetGuid)
    {
        ABDepInfo info = new ABDepInfo();
        info._url = url;
        info._depAssetGuid = depAssetGuid;
        _ABDepDic[url] = info;
    }

    public void add(ABDepInfo info)
    {
        _ABDepDic[info._url] = info;
    }

    public bool tryGetValue(string url, out ABDepInfo abDepInfo)
    {
        return _ABDepDic.TryGetValue(url, out abDepInfo);
    }
}
