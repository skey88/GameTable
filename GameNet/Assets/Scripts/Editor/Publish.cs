using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using Stars;
using System.IO;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using UnityEditor.Callbacks;
#endif

public class Publish : UnityEditor.Build.BuildFailedException
{

    

    static string[] scenes = new string[]
    {
        "Assets/Scene/start.unity",
        "Assets/Scene/empty.unity"
    };

    static bool _isCanBuildAfterChangeBuildTarget = false;
    static BuildArgs _args;
    public int callbackOrder { get { return 0; } }

    public static string CODE_SIGN_DEVELOPER { get; private set; }
    public static string CODE_SIGN_DISTRIBUTION { get; private set; }
    public static string PROVISIONING_DEVELOPER { get; private set; }
    public static string PROVISIONING_DISTRIBUTION { get; private set; }
    //public static string MYTEAM { get { return "Sporger Technoiogy Co.,Ltd"; } }
    public static string MYTEAM { get { return "Y345RLKHVA"; } }

    static BuildOptions buildOption = 0;

    public Publish(string message) : base(message)
    {
    }

    static public void BuildAPK(bool tip = true,bool isRecomress = false,bool isEncrypt = false,string fileName = "")
    {
        if (tip)
        {
            if (!EditorUtility.DisplayDialog("提示", "要开始打APK包啰？", "确定", "取消"))
            {
                return;
            }
        }
        string result = null;
        DateTime dt = DateTime.Now.ToLocalTime();
        string apkFilename = dt.Year.ToString("d2") + dt.Month.ToString("d2") + dt.Day.ToString("d2") +
            dt.Hour.ToString("d2") + dt.Minute.ToString("d2");
        string apkPath = apkFilename + ".apk";
        if (fileName != "")
        {
            apkPath = fileName;
        }
        Debug.Log("apk filename = " + apkPath);
        //PlayerSettings.Android.keystoreName = "Assets/AndroidKeystore/ty-prod.keystore";
        PlayerSettings.Android.keystorePass = "tiyou2016";
        //PlayerSettings.Android.keyaliasName = "ty-prod";
        PlayerSettings.Android.keyaliasPass = "tiyou2016";
        try
        {
            //result =
            //    BuildPipeline.BuildPlayer(
            //        (from scene in EditorBuildSettings.scenes where scene.enabled select scene.path).ToArray(),
            //        apkPath, BuildTarget.Android, BuildOptions.None);
            result =
                BuildPipeline.BuildPlayer(
                    scenes,
                    apkPath, BuildTarget.Android, buildOption).ToString();

        }
        catch (Exception exception)
        {
            Debug.LogError(exception);
            return;
        }
        if (!String.IsNullOrEmpty(result))
        {
            Debug.LogError(result);
            return;
        }
        if (tip)
        {
            EditorUtility.DisplayDialog("提示", "恭喜！您的APK包" + System.IO.Directory.GetCurrentDirectory() + "/" + apkPath + "生成完毕!", "确定");
        }
    }


    //在这里找出你当前工程所有的场景文件，假设你只想把部分的scene文件打包 那么这里可以写你的条件判断 总之返回一个字符串数组。
    static string[] GetBuildScenes()
    {
        List<string> names = new List<string>();

        foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
        {
            if (e == null)
                continue;
            if (e.enabled)
                names.Add(e.path);
        }
        return names.ToArray();
    }


    //shell脚本直接调用这个静态方法
    static public void BuildForIPhone()
    {
        BuildPipeline.BuildPlayer(scenes, "xcodeprj", BuildTarget.iOS, buildOption);
    }


    static void commandLineBuild()
    {
        BuildArgs ba  = parseCommandLine();
        buildFromArgs(ba);
        EditorApplication.Exit(0);
    }

    [MenuItem("GameTools/一键设置keystore密码")]
    static void OneKeyKeystorePassword()
    {
        //PlayerSettings.Android.keystoreName = "Assets/AndroidKeystore/ty-prod.keystore";
        PlayerSettings.Android.keystorePass = "tiyou2016";
        //PlayerSettings.Android.keyaliasName = "ty-prod";
        PlayerSettings.Android.keyaliasPass = "tiyou2016";
        //BuildPipeline.BuildPlayer(
        //            (from scene in EditorBuildSettings.scenes where scene.enabled select scene.path).ToArray(),
        //            "1.apk", BuildTarget.Android, BuildOptions.None);
    }

    static void checkBuildTarget()
    {
        BuildArgs ba = parseCommandLine();
        if (ba._buildTarget != EditorUserBuildSettings.activeBuildTarget)
        {
            if(ba._buildTarget == BuildTarget.Android)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, ba._buildTarget);
            }else if(ba._buildTarget == BuildTarget.iOS)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, ba._buildTarget);
            }

        }
        EditorApplication.Exit(0);
    }

    static BuildArgs parseCommandLine()
    {
        string[] args = System.Environment.GetCommandLineArgs();
        Dictionary<BuildArgs.BuildArgsEnum, string> argsDic = new Dictionary<BuildArgs.BuildArgsEnum, string>();
        for (int i = 0; i < args.Length; i++)
        {
            if (i > (int)BuildArgs.BuildArgsEnum.First)
            {
                argsDic.Add((BuildArgs.BuildArgsEnum)i, args[i]);
                Debug.Log(i + " " + args[i]);
            }
            Debug.Log(args[i]);
        }
        BuildArgs ba = new BuildArgs();
        ba.setArgs(argsDic);
        return ba;
    }

    static public void buildAfterTagetSwitch(BuildArgs args)
    {
        Debug.Log("buildAfterTagetSwitch");
        buildOption = BuildOptions.None;
        //PlayerSettings.bundleIdentifier = args._bundleId;
        //PlayerSettings.bundleVersion = args._applicationVer;
        //PlayerSettings.shortBundleVersion = args._shortBundleVersion;

        EditorUserBuildSettings.development = args._isDevMode;
        if(args._isDevMode)
        {
            buildOption |= BuildOptions.Development;
        }
        Debug.Log("BuildAssetBundles");
        //AssetBundleBuildPanel.BuildAssetBundles();

        if (args._buildTarget == UnityEditor.BuildTarget.Android)
        {
            Publish.BuildAPK(false, false, false, args._package_filename + ".apk");
        }
        else if (args._buildTarget == UnityEditor.BuildTarget.iOS)
        {
            BuildForIPhone();
        }
    }

    static public void buildFromArgs(BuildArgs args)
    {
        Debug.Log("buildFromArgs");

        if (args._buildTarget == EditorUserBuildSettings.activeBuildTarget)
        {
            buildAfterTagetSwitch(args);
        }
        else
        {


#if UNITY_2017_1_OR_NEWER
            _isCanBuildAfterChangeBuildTarget = true;
            _args = args;
#else
            EditorUserBuildSettings.activeBuildTargetChanged = delegate ()
            {
                buildAfterTagetSwitch(args);
            };
#endif
            if (args._buildTarget == BuildTarget.Android)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, args._buildTarget);
            }
            else if (args._buildTarget == BuildTarget.iOS)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, args._buildTarget);
            }
        }
    }



    public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
    {
        if (_isCanBuildAfterChangeBuildTarget)
        {
            Debug.Log("Publish.OnActiveBuildTargetChanged " + previousTarget + " " + newTarget);
            try
            {
                buildAfterTagetSwitch(_args);
            }
            catch
            {

            }
            _isCanBuildAfterChangeBuildTarget = false;
        }
    }
#if UNITY_IOS
    [PostProcessBuild(999)]
    public static void OnPostprocessBuild(BuildTarget BuildTarget, string path)
    {
        if (BuildTarget == BuildTarget.iOS)
        {

            string projPath = PBXProject.GetPBXProjectPath(path);
            PBXProject proj = new PBXProject();

            proj.ReadFromString(File.ReadAllText(projPath));
            string target = proj.TargetGuidByName(PBXProject.GetUnityTargetName());

            //Handle xcodeproj  
            //File.Copy(Application.dataPath + "/Editor/xcodeapi/Res/KeychainAccessGroups.plist", path + "/KeychainAccessGroups.plist", true);

            //proj.AddFile("KeychainAccessGroups.plist", "KeychainAccessGroups.plist");

            //string codesign = Debug.isDebugBuild ? CODE_SIGN_DEVELOPER : CODE_SIGN_DISTRIBUTION;
            //string provision = Debug.isDebugBuild ? PROVISIONING_DEVELOPER : PROVISIONING_DISTRIBUTION;

            //proj.SetBuildProperty(target, "CODE_SIGN_IDENTITY", codesign);
            //proj.SetBuildProperty(target, "PROVISIONING_PROFILE_SPECIFIER", provision);
            //proj.SetBuildProperty(target, "CODE_SIGN_ENTITLEMENTS", "KeychainAccessGroups.plist");
            proj.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
            proj.SetBuildProperty(target, "DEVELOPMENT_TEAM", MYTEAM);
            proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC");
            proj.SetBuildProperty(target, "GCC_ENABLE_OBJC_EXCEPTIONS", "YES");

            proj.AddFrameworkToProject(target, "CoreTelephony.framework", true);
            proj.AddFrameworkToProject(target, "SystemConfiguration.framework", true);
            proj.AddFrameworkToProject(target, "Security.framework", true);
            proj.AddFrameworkToProject(target, "JavaScriptCore.framework", true);

            //proj.AddFileToBuild(target, proj.AddFile("usr/lib/libz.dylib", "Frameworks/libz.dylib", PBXSourceTree.Sdk));
            //proj.AddFileToBuild(target, proj.AddFile("usr/lib/libc++.dylib", "Frameworks/libc++.dylib", PBXSourceTree.Sdk));






            //proj.SetSystemCapabilities (target, "com.apple.Push","1");  
            //移除指定目录  
            //proj.RemoveFilesByProjectPathRecursive("Libraries/Plugins/Android");没此方法


            //proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC");
            //proj.AddBuildProperty(target, "FRAMEWORK_SEARCH_PATHS", "xxxxxxx");
            //proj.AddBuildProperty(target, "HEADER_SEARCH_PATHS", "xxxxxxxx");

            //默认提供的添加framework方法，默认会去system / library / framework查找。
            //如果想用自定义目录的framework
            //var fileGUID = proj.AddFile("srcpath", "targetpath");
            //proj.AddFileToBuild(target, fileGUID);

            File.WriteAllText(projPath, proj.WriteToString());

            //Handle plist  
            string plistPath = path + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));
            PlistElementDict rootDict = plist.root;

            //rootDict.SetString("CFBundleVersion", "1.0");

            rootDict.SetString("NSLocationAlwaysUsageDescription", "LocationAlwaysUsageDescription");
            rootDict.SetString("NSLocationWhenInUseUsageDescription", "LocationWhenInUseUsageDescription");
            rootDict.SetBoolean("UIFileSharingEnabled",true);
            rootDict.SetBoolean("NSAllowsArbitraryLoads",true);
            rootDict.SetString("NSMicrophoneUsageDescription","NSMicrophoneUsageDescription");

            PlistElementArray array = rootDict.CreateArray("UIBackgroundModes");
            array.AddString("location");


            //rootDict.SetString("NSPhotoLibraryUsageDescription", "Use Photo");
            //rootDict.SetString("NSCameraUsageDescription", "Use Camera");

            File.WriteAllText(plistPath, plist.WriteToString());



        }
    }
#endif
}
