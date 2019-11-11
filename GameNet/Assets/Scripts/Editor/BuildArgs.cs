using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;
using System.IO;

public class BuildArgs
{
    public enum BuildArgsEnum
    {
        First = 10,
        ApplicationVer,
        ServerId,
        SDK,
        BuildTarget,
        IsDevMode,
        IsBuildAB,
        BundleId,
        Package_Filename,
    }
    public string _applicationVer = "1.0.0.0";
    public string _shortBundleVersion = "1.0.0";
    public int _serverid = 0;
    public int _sdk = 0;
    public BuildTarget _buildTarget = BuildTarget.Android;
    public bool _isDevMode = true;
    public bool _isBuildAB = true;
    public string _bundleId = "com.sporger.xxx";
    public string _package_filename = "packagefilename";

    public void setArgs(Dictionary<BuildArgs.BuildArgsEnum, string> argsDic)
    {
        string appver = getFromDic(BuildArgsEnum.ApplicationVer,argsDic);
        if (appver != "")
        {
            _applicationVer = appver;
            _shortBundleVersion = getShortBundleVersion();
            Debug.Log("shortBundleVersion " + _shortBundleVersion);
        }
        string serverid = getFromDic(BuildArgsEnum.ServerId, argsDic);
        if (serverid != "")
        {
            Debug.Log("serverid = " + serverid);
            _serverid = int.Parse(serverid);
        }
        string sdk = getFromDic(BuildArgsEnum.SDK, argsDic);
        if (sdk != "")
        {
            _sdk = int.Parse(sdk);
        }
        string buildTarget = getFromDic(BuildArgsEnum.BuildTarget, argsDic);
        if (buildTarget != "")
        {
            if (buildTarget == "ios")
            {
                _buildTarget = BuildTarget.iOS;
            }
            else if (buildTarget == "android")
            {
                _buildTarget = BuildTarget.Android;
            }

        }
        string isDevMode = getFromDic(BuildArgsEnum.IsDevMode, argsDic);
        if (isDevMode != "")
        {
            _isDevMode = isDevMode == "true" ? true : false;
        }
        string isBuildAB = getFromDic(BuildArgsEnum.IsBuildAB, argsDic);
        if (isBuildAB != "")
        {
            _isBuildAB = isBuildAB == "true" ? true : false;
        }
        string bundleId = getFromDic(BuildArgsEnum.BundleId, argsDic);
        if (bundleId != "")
        {
            _bundleId = bundleId;
        }
        string package_filename = getFromDic(BuildArgsEnum.Package_Filename, argsDic);
        if (package_filename != "")
        {
            _package_filename = package_filename;
        }
        saveArgs(argsDic);
    }


    string getFromDic(BuildArgs.BuildArgsEnum e, Dictionary<BuildArgs.BuildArgsEnum, string> argsDic)
    {
        string str = "";
        bool r = argsDic.TryGetValue(e, out str);
        Debug.Log("getBuildArgs " + e.ToString() + " " + str);
        if (r)
        {
            return str;
        }
        return "";
    }

    string getShortBundleVersion()
    {
        string[] s = _applicationVer.Split(new char[]{'.'});
        if (s.Length < 3)
        {
            Debug.LogError("shortBundleID error");
            return _shortBundleVersion;
        }
        string ss = s[0] + "." + s[1] + "." + s[2];
        return ss;
    }

    static public void saveArgs(Dictionary<BuildArgs.BuildArgsEnum, string> argsDic)
    {
        XmlDocument xmldoc = new XmlDocument();
        XmlElement root = xmldoc.CreateElement("sysconf");
        xmldoc.AppendChild(root);
        foreach (KeyValuePair<BuildArgs.BuildArgsEnum, string> data in argsDic)
        {
            XmlElement node = xmldoc.CreateElement("node");
            root.AppendChild(node);
            XmlAttribute name = xmldoc.CreateAttribute("name");
            name.InnerText = data.Key.ToString();
            node.Attributes.Append(name);
            XmlAttribute val = xmldoc.CreateAttribute("val");
            val.InnerText = data.Value;
            node.Attributes.Append(val);

        }
        try
        {
            Directory.CreateDirectory("Assets/Resources/Config");
            xmldoc.Save("Assets/Resources/Config/SysConf.xml");
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }

    }
}