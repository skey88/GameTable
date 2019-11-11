using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
public class BuildConfigMgr
{
   const string _buildConfigFilePath = "Assets/ResourcesForBundle/Config/Editor/BuildConfig/BuildABConfig.xml";
    //const string _buildConfigFilePath = "Assets/Resources/Config/BuildABConfig.xml";
    public class BuildABConfig
    {
        public string name;
        /// <summary>
        /// 保存版本目录,为空即为本工程之下的bundles目录
        /// </summary>
        public string _saveBundlePath = "";
        /// <summary>
        /// 上个版本字符串
        /// </summary>
        public string _lastResVer = "";
        /// <summary>
        /// 打完包是否删除在assetlist.xml表里有并且在Resources目录里的资源
        /// </summary>
        public bool _isDelResInResources = false;
        /// <summary>
        /// 是否要显示向导
        /// </summary>
        public bool _isShowWizard = false;
        /// <summary>
        /// 完成后是否要提示用户包打好了
        /// </summary>
        public bool _isTipFinish = true;
        /// <summary>
        /// 是否强制覆盖AssetBundleList
        /// </summary>
        public bool _isForceOverRightAssetBundleList = true;
        /// <summary>
        /// 是否只是生成报告
        /// </summary>
        public bool _isOnlyReport = false;
        /// <summary>
        /// 资源有改变时，是否要升整个版本,读配置表
        /// </summary>
        public bool _isNeedUpgradeVerWhenResChanged = true;
        /// <summary>
        /// 是否将有改变的包改成从网络下载,包括改动和新增，读配置表
        /// </summary>
        public bool _isModifyedResToNetPackage = true;
        /// <summary>
        /// 在开发简单模式下上个版本目录是否使用缺省目录_saveBundlePath + ResourceUpdateManager.getStrVerFromUInt64(curResVer)
        /// </summary>
        public bool _isLastVerFolderUseDefault = false;
        /// <summary>
        /// 从上个版本中拷贝工程内不存在并且未发生改变的AB,只做资源版本升级则不会拷贝
        /// </summary>
        public bool _isCopyNotExitNotChangeAB = true;
    }


    public class BuildType
    {
        public const string DevSimple = "DevSimple";
        public const string PublishOneKey = "PublishOneKey";
        public const string PublishResUpdate = "PublishResUpdate";
    }

    static public BuildABConfig getBuildABConfig(string configName)
    {
        return getBuildABConfig(_buildConfigFilePath, configName);
    }

    static public BuildABConfig getBuildABConfig(string buildConfigFilePath, string configName)
    {
        BuildABConfig bc = null;
        if (File.Exists(buildConfigFilePath) == false) return null;
        XmlDocument xmldoc = new XmlDocument();
        xmldoc.Load(buildConfigFilePath);
        XmlNode rootElement = xmldoc.SelectSingleNode("BuildConfig");
        if (rootElement == null) return null;
        XmlNodeList nodeList = rootElement.ChildNodes;
        foreach (XmlNode xn in nodeList)
        {
            BuildABConfig new_bc = new BuildABConfig();
            foreach (XmlAttribute attr in xn.Attributes)
            {
                if (attr.Name.Equals("name"))
                {
                    new_bc.name = attr.Value;
                }
                else if (attr.Name.Equals("isNeedUpgradeVerWhenResChanged"))
                {
                    new_bc._isNeedUpgradeVerWhenResChanged = !attr.Value.Equals("0");
                }
                else if (attr.Name.Equals("isModifyedResToNetPackage"))
                {
                    new_bc._isModifyedResToNetPackage = !attr.Value.Equals("0");
                }
            }

            if (new_bc.name.Equals(configName))
            {
                bc = new_bc;
                return bc;
            }
        }
        return bc;
    }
}
