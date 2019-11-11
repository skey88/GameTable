using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Text; 

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Stars
{
    //项目工具类
    public class RG_Utils
    {
        static string ten_thousand_String = "";
        static string hundred_million_String = "";
#if UNITY_EDITOR
        public static string _editordownloadpath = Directory.GetCurrentDirectory() + "/download";
#endif
        public static int currentSecond = 0;

        static ulong guid_ = 0;
        public static Vector3 strToVector3(string str)
        {
            string[] splits = str.Split(',');
            List<float> tmp = new List<float>();
            for (int i = 0; i < splits.Length; i++)
            {
                tmp.Add(float.Parse(splits[i]));
            }
            return new Vector3(tmp[0], tmp[1], tmp[2]);
        }

         

        public static ulong getGuid()
        {
            return guid_++;
        }


        
         
        //得到当前平台存放bundle文件的路径
        public static string getPathURL()
        {
            string PathURL =
#if UNITY_EDITOR
 "file://" + Application.dataPath + "/StreamingAssets/";
#elif UNITY_ANDROID
		"jar:file://" + Application.dataPath + "!/assets/";	
#elif UNITY_IPHONE
		"file://" + Application.dataPath + "/Raw/";
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR || UNITY_WEBPLAYER || UNITY_WP8
	"file://" + Application.dataPath + "/StreamingAssets/";
#else
        string.Empty;
#endif
            return PathURL;
        }
        //得到当前平台下载下来的存放bundle文件的路径,用于WWW加载
        public static string getDownloadPathURL()
        {
            string PathURL =
#if UNITY_EDITOR
#if UNITY_EDITOR_WIN
 "file://" + _editordownloadpath + "/";
#else
        "file://" + RG_Utils.getDownloadPath() + "/";
#endif

#elif UNITY_ANDROID
		"file://" + RG_Utils.getDownloadPath() + "/";
#elif UNITY_IPHONE
		"file://" + RG_Utils.getDownloadPath() + "/";
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR || UNITY_WEBPLAYER || UNITY_WP8
	"file://" + RG_Utils.getDownloadPath() + "/";
#else
        string.Empty;
#endif
            return PathURL;
        }


        // 注意：不同平台下StreamingAssets文件夹下的数据存放路径不一样。
        //C#路径                windows                Application.streamingAssetsPath+”/Myfile.txt” 
        //                      ios                    Application.streamingAssetsPath+”/Myfile.txt”
        //WWW路径               windows                "file://"+Application.streamingAssetsPath+”/Myfile.txt”
        //                      ios                    "file://"+Application.streamingAssetsPath+”/Myfile.txt”
        //                      android                Application.streamingAssetsPath+”/Myfile.txt”

        //得到当前平台存放本平台专用bundle文件的路径
        public static string getLocalBundlePathForPlatform()
        {
            return getPathURL() + getPlatformString();
        }

        //得到下载下来用于WWW加载时针对平台所用的路径
        public static string getDownLoadBundlePathForPlatform()
        {
            return getDownloadPathURL() + getPlatformString();
        }
        //得到当前平台存放本平台专用bundle文件的路径,CS版本
        public static string getLocalBundlePathForPlatform_CS()
        {
            return Application.streamingAssetsPath + "/" + getPlatformString();
        }
        //得到下载下来用于WWW加载时针对平台所用的路径,CS版本
        public static string getDownLoadPathForPlatform_CS()
        {
#if UNITY_EDITOR_WIN
            return Path.Combine( _editordownloadpath , getPlatformString());
#else
        return RG_Utils.getDownloadPath() + "/" + getPlatformString();
#endif
        }


        //因为在Android下会提前拷贝除场景之外的Bundle包到下载路径，目前是一样的做法
        public static string getAndroidCopyAssetsBundlePath_CS()
        {
            return getDownLoadPathForPlatform_CS();
        }
        //Android下 WWW版本
        public static string getAndroidCopyAssetsBundlePath()
        {
            return getDownLoadBundlePathForPlatform();
        }
        //得到当前平台所代表的字符串
        public static string getPlatformString()
        {
            string platform =
#if UNITY_ANDROID
		"android";
#elif UNITY_IPHONE
		"iphone";
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR || UNITY_WEBPLAYER || UNITY_WP8
    "stlwin";
#else
        string.Empty;
#endif
         return platform;
        }

        internal static float mkmtoms(object simSpeed)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 得到下载目录或者保存文件目录
        /// </summary>
        /// <returns></returns>
        public static string getDownloadPath()
        {
#if UNITY_EDITOR
            if(!Directory.Exists(_editordownloadpath))
            {
                Directory.CreateDirectory(_editordownloadpath);
            }
            return _editordownloadpath;
#else
            return Application.persistentDataPath;
#endif
        }

        //从当前的时间DateTime格式转化为从1970.1.1到现在经过的秒数
        public static uint dateTimeToSecond(DateTime dt)
        {
            DateTime begin = new DateTime(1970, 1, 1);
            long time = dt.Ticks - begin.Ticks;
            long t = (time / (long)(10000000));
            return (uint)t;
        }
        /// <summary>
        /// 从1970.1.1到当前经过的秒数转化为本地DateTime,为各地通用起见，尽量避免使用 clime
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>

        public static DateTime secondToDateTime(uint t)
        {
            DateTime begin = new DateTime(1970, 1, 1);
            DateTime dt = begin.Add(new TimeSpan((long)t * TimeSpan.TicksPerSecond));

            return dt.ToLocalTime();
        }

        /// <summary>
        /// 从1970.1.1到当前经过的秒数转化为世界时间（格林尼治（平均）时）
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>

        public static DateTime secondToUniversalDateTime(uint t)
        {
            DateTime begin = new DateTime(1970, 1, 1);
            DateTime dt = begin.Add(new TimeSpan((long)t * TimeSpan.TicksPerSecond));
            return dt;
        }
        /// <summary>
        /// 得到当前的世界时间
        /// </summary>
        /// <returns></returns>

        public static DateTime getNowTime()
        {
            return secondToUniversalDateTime(getNowSecond());
        }
        /// <summary>
        /// 得到当前的本地时间
        /// </summary>
        /// <returns></returns>

        public static DateTime getNowLocalTime()
        {
            return secondToDateTime(getNowSecond());
        }
        //得到服务器当前离1970.1.1经过的秒数
        //要通过cli_get_time消息返回才能得到准确的计数
        public static uint getNowSecond()
        {
            DateTime begin = new DateTime(1970, 1, 1);
            DateTime now = DateTime.UtcNow;
            TimeSpan sp = now - begin;
            return (uint)sp.TotalSeconds;

            {
                //return svrTime + (uint)Time.realtimeSinceStartup - CalendarDataManger.localGetSvrTime;
            }
        }

        /// <summary>
        /// 调用静态Android Java函数
        /// </summary>
        /// <param name="className">类名</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="args">参数</param>
        static public void callAndroidJavaStaticFun(string className, string methodName, params object[] args)
        {
            //TyLogger.Log("callAndroidJavaStaticFun " + className + " " + methodName);
#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass (className);
        jc.CallStatic(methodName, args);
#endif
        }

        /// <summary>
        /// 调用静态Android Java函数
        /// </summary>
        /// <typeparam name="T">返回值</typeparam>
        /// <param name="className">类名</param>
        /// <param name="methodName">参数</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        static public T callAndroidJavaStaticFun<T>(string className, string methodName, params object[] args)
        {
            //TyLogger.Log("callAndroidJavaStaticFun " + className + " " + methodName);
#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass (className);
        return jc.CallStatic<T>(methodName, args);
#endif
            return default(T);
        }

        //调用Android插件中的函数
        static public void callAndroidJava(string methodName, params object[] args)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject> ("currentActivity");
		jo.Call (methodName, args);
		Debug.Log (methodName + " Called");
#endif
        }
        /// <summary>
        /// 调用Androi java代码中的函数，并返回值，参数要java支持的
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="methodName">方法</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        static public T callAndroidJava<T>(string methodName, params object[] args)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
		Debug.Log (methodName + " Called");
		AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject> ("currentActivity");
		T t = jo.Call<T> (methodName, args);
        return t;
#endif
            return default(T);
        }


        public static string getMD5HashFromString(string str,Encoding encoding)
        {
            byte[] bt = encoding.GetBytes(str);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(bt);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 计算文件的MD5校验
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string getMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }

   

         
        /// <summary>
        /// 对ETC（opaque） + ETC（alpha） 格式的材质设置纹理
        /// </summary>
        /// <param name="mat">目标材质</param>
        /// <param name="mainTex">主纹理</param>
        /// <param name="alphaTex">透明纹理</param>
        static public void setMatTexture(Material mat, Texture mainTex, Texture alphaTex)
        {
            mat.SetTexture("_MainTex", mainTex);
            mat.SetTexture("_AlphaTex", alphaTex);
        }
        /// <summary>
        /// 对ETC（opaque） + ETC（alpha）+ ETC(mask)格式的材质设置纹理
        /// </summary>
        /// <param name="mat">目标材质</param>
        /// <param name="mainTex">主纹理</param>
        /// <param name="alphaTex">透明纹理</param>
        /// <param name="maskTex">遮罩纹理</param>
        static public void setMatTexture(Material mat, Texture mainTex, Texture alphaTex, Texture maskTex)
        {
            mat.SetTexture("_MainTex", mainTex);
            mat.SetTexture("_AlphaTex", alphaTex);
            if (maskTex != null)
                mat.SetTexture("_Mask", maskTex);
        }

        public static void RemoveAllChildObject(GameObject parent, bool bImmediate)
        {
            for (int n = parent.transform.childCount - 1; 0 <= n; n--)
            {
                if (n < parent.transform.childCount)
                {
                    Transform obj = parent.transform.GetChild(n);
                    if (bImmediate)
                        GameObject.DestroyImmediate(obj.gameObject);
                    else GameObject.Destroy(obj.gameObject);
                }
            }
        }

      
        public static int[] getRandomNum(int len, int min, int max)
        {
            int[] arr = new int[len];
            int num = 0, count = 0;
            while (count < len)
            {
                num = UnityEngine.Random.Range(min, max) + 1;
                for (int j = 0; j < count; j++)
                    if (arr[j] == num) { count--; break; }
                arr[count] = num;
                count++;
            }
            return arr;
        }

        //设置每3位数加一个逗号隔开
        public static string SetNumPoint(string numStr)
        {
            if (numStr.Length <= 3)
            {
                return numStr;
            }

            numStr = DeleNumPoint(numStr);

            int count = (numStr.Length - 1) / 3;
            for (int j = 0; j < count; j++)
            {
                numStr = numStr.Insert((numStr.Length - 3 * (j + 1) - j), ",");
            }
            return numStr;

        }

        //将数字中的逗号删除
        public static string DeleNumPoint(string numStr)
        {
            if (numStr.IndexOf(',') != -1)
            {
                string[] arrNum = numStr.Split(',');
                numStr = "";
                for (int i = 0; i < arrNum.Length; i++)
                {
                    numStr += arrNum[i];
                }
            }
            return numStr;
        }


  
        /// <summary>
        /// 检查字符是否是非简体字，如果是则返回true，如果不是繁体或不是中文则返回false
        /// </summary>
        /// <param name="str">某个unicode字符</param>
        /// <returns></returns>
        public static bool IsCHTorCHS(char str)
        {
            if ((str >= 0x4e00 && str <= 0x9fa5))
            {
                var gb = Encoding.GetEncoding("gb2312");
                var gb2312Bytes = gb.GetBytes(str.ToString());
                //GB2312的字体范围 72 * 94 = 6768 个字
                if (gb2312Bytes[0] >= 0xB0 && gb2312Bytes[0] <= 0xF7
                    && gb2312Bytes[1] >= 0xA1 && gb2312Bytes[1] <= 0xFE)
                {
                    return false;
                }
                //包含繁体和其它异体字
                return true;
            }
            return false;
        }
        /// <summary>
        /// 检查一串字符中是否有非简体字的汉字
        /// </summary>
        /// <param name="str">某个unicode字符串</param>
        /// <returns></returns>
        public static bool IsCHTorCHS(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if (IsCHTorCHS(c))
                {
                    return true;
                }
            }
            return false;
        }

        //清除子物体
        public static void ClearChildrenObj(Transform parent)
        {
            if (parent != null)
            {
                List<GameObject> listobj = new List<GameObject>();
                for (int i = 0; i < parent.childCount; i++)
                {
                    listobj.Add(parent.GetChild(i).gameObject);
                }
                for (int i = 0; i < listobj.Count; i++)
                {
                    GameObject.DestroyImmediate(listobj[i]);
                }
            }

        }


 

        /// <summary>
        /// 从Resources目录的路径配置状态解析成资源名
        /// 用法：        
        /// Debug.Log(RG_Utils.getResNameFromPath("sdfsf/sser"));
        /// Debug.Log(RG_Utils.getResNameFromPath("sser"));
        /// Debug.Log(RG_Utils.getResNameFromPath("fer/sdfsf/sser"));
        /// Debug.Log(RG_Utils.getResNameFromPath("/sdfsf/sser"));
        /// 都可以得到"sser"
        /// </summary>
        /// <param name="str">Resources目录</param>
        /// <returns>资源名</returns>
        public static string getResNameFromPath(string str)
        {
            return Path.GetFileName(str);
        }


        //颜色转化
        public static string color2string(Color c)
        {
            return "[" + ((int)c.r).ToString("X2") + ((int)c.g).ToString("X2") + ((int)c.b).ToString("X2") + "]";
        }
        public static string color2string(int r, int g, int b)
        {
            return "[" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2") + "]";
        }


        /// <summary>
        /// 由当前节点向上循环取相对父节点的坐标
        /// </summary>
        /// <param name="child">当前节点</param>
        /// <param name="parentCount">向上寻几级父节点</param>
        /// <returns></returns>
        public static Vector3 childT2parentT(Transform child, int parentCount, ref Transform outParent)
        {
            outParent = child;
            Vector3 _result = child.localPosition;
            for (int i = 0; i < parentCount; i++)
            {
                outParent = outParent.parent;
                _result += outParent.localPosition;
            }

            return _result;
        }

        /// <summary>
        /// 计算ngui屏幕的width值
        /// </summary>
        /// <returns></returns>
        public static int CalculationScreenToNguiWidth()
        {
            int _nguiWidth = 0;

            if (Screen.height == 768 && Screen.width == 1024)
            {
                _nguiWidth = Mathf.CeilToInt((float)Screen.width / (float)Screen.height * 768);
            }
            else
            {
                _nguiWidth = Mathf.CeilToInt((float)Screen.width / (float)Screen.height * 960);
            }

            return _nguiWidth;
        }

        /// <summary>
        /// 存储二进制文件到Application.dataPath + "/download/";
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="bytes">二进制内容</param>
        /// <returns></returns>
        static public bool Save(string fileName, byte[] bytes)
        {
#if UNITY_WEBPLAYER || UNITY_FLASH || UNITY_METRO || UNITY_WP8 || UNITY_WP_8_1
		return false;
#else

#if UNITY_EDITOR_WIN
            string path = Path.Combine(_editordownloadpath , fileName);
#else
        string path = RG_Utils.getDownloadPath() + "/" + fileName;
#endif



            if (bytes == null)
            {
                if (File.Exists(path)) File.Delete(path);
                return true;
            }

            FileStream file = null;

            try
            {
                file = File.Create(path);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
                return false;
            }

            file.Write(bytes, 0, bytes.Length);
            file.Close();
            return true;
#endif
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="path">保存文件路径</param>
        /// <param name="name">保存文件名称</param>
        /// <param name="info">文件内容</param>
        public static void CreateFile(string path, string name, string info)
        {
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
                //文件流信息
            StreamWriter sw = new StreamWriter(Path.Combine( path , name), false, System.Text.Encoding.UTF8);
            //以行的形式写入信息
            sw.WriteLine(info);
            sw.Flush();
            //关闭流
            sw.Close();
            //销毁流
            sw.Dispose();
        }
        /// <summary>
        /// 每列的字符串中提取共有字符串
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string[] checkSameArray(List<string[]> list)
        {
            List<string> result = new List<string>();
            List<string> checkList = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                var array = list[i];
                for (int j = 0; j < array.Length; j++)
                {
                    if (checkList.Contains(array[j]))
                    {
                        if (!result.Contains(array[j]))
                            result.Add(array[j]);
                    }
                    else
                    {
                        checkList.Add(array[j]);
                    }
                }
            }
            return result.ToArray();
        }

        public static string getScriptSerializeFields(string path)
        {
            string serializeFields = "";
            string extension = Path.GetExtension(path);
            if (extension.Equals(".cs") || extension.Equals(".CS"))
            {
                string classname = Path.GetFileNameWithoutExtension(path);
                try
                {
                    Type monoClassType = Type.GetType(classname);
                    FieldInfo[] fields = monoClassType.GetFields();
                    foreach (FieldInfo info in fields)
                    {
                        bool isSerialize = info.IsPublic;
                        Debug.Log(info.Name + "\n");
                        Debug.Log("IsPublic " + info.IsPublic + "\n");
                        object[] attrs = info.GetCustomAttributes(true);
                        for (int i = 0; i < attrs.Length; i++)
                        {
                            string attrName = attrs[i].GetType().Name;
                            Debug.Log("attrs " + attrName + "\n");
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
                    Debug.LogError("can't get class " + path);
                }
            }
            return serializeFields;
        }

        //加载预置件
        public static GameObject loadPrefeb(string name, string PrefabPath)
        {
            GameObject mygo = null;
            try
            {
                GameObject prototype1 = (GameObject)Resources.Load(PrefabPath);

                mygo = (GameObject)GameObject.Instantiate(prototype1);

                mygo.transform.position = new Vector3(0, 0, 0);
                mygo.transform.localPosition = new Vector3(0, 0, 0);
                mygo.transform.localScale = new Vector3(1, 1, 1);
                if (name != "")
                    mygo.name = name;
                prototype1 = null;
            }
            catch
            {
                Debug.Log(name + " " + PrefabPath + " load error!!!");
            }

            return mygo;
        }
#if UNITY_EDITOR

        public static GameObject loadPrefebInEditor(string name, string PrefabPath)
        {
            GameObject mygo = null;
            try
            {
                //GameObject prototype1 = (GameObject)Resources.Load(PrefebPath);
                GameObject prototype1 = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
                mygo = (GameObject)GameObject.Instantiate(prototype1);

                mygo.transform.position = new Vector3(0, 0, 0);
                mygo.transform.localPosition = new Vector3(0, 0, 0);
                mygo.transform.localScale = new Vector3(1, 1, 1);
                if (name != "")
                    mygo.name = name;
                prototype1 = null;
            }
            catch
            {
                Debug.Log(name + " " + PrefabPath + " load error!!!");
            }

            return mygo;
        }

#endif
        /// <summary>
        /// 计算配速 分钟/公里
        /// </summary>
        /// <param name="time">时间，秒为单位</param>
        /// <param name="distance">距离，米为单位</param>
        /// <returns></returns>
        public static float calSpeedPerKm(float time,float distance)
        {
            return distance == 0 ? 0 : time / 60 / (distance / 1000);
        }



        /// <summary>
        /// 米/秒 转 分钟/公里
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        public static float mstomkm(float speed)
        {
            float speedtmp = 1 / speed / 60 * 1000;
            return speedtmp;
        }

        /// <summary>
        /// 米/秒 转 秒/公里
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        public static int mstoskm(float speed)
        {
            return (int)(mstomkm(speed) * 60);
        }

        /// <summary>
        /// 分钟/公里 转米/秒
        /// </summary>
        /// <param name="speed">速度 单位为分钟/公里</param>
        /// <returns></returns>
        public static float mkmtoms(float speed)
        {
            float speedtmp = 1000 / (speed * 60);
            return speedtmp;
        }
        /// <summary>
        /// 秒 / 公里 转 米/秒
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        public static float skmtoms(uint speed)
        {
            return  mkmtoms(speed / 60.0f);
        }


        private const float EARTH_RADIUS = 6378.137f;//地球半径
        private static float rad(float d)
        {
            return d * Mathf.PI / 180.0f;
        }
        /// <summary>
        /// 得到地球上两点间距离
        /// </summary>
        /// <param name="lng1"></param>
        /// <param name="lat1"></param>
        /// <param name="lng2"></param>
        /// <param name="lat2"></param>
        /// <returns></returns>
        public static float GetDistanceOnEarth(float lng1, float lat1, float lng2, float lat2)
        {
            if (lng1 == lng2 && lat1 == lat2) return 0;
            float radLat1 = rad(lat1);
            float radLat2 = rad(lat2);
            float a = radLat1 - radLat2;
            float b = rad(lng1) - rad(lng2);

            float s = 2 * Mathf.Asin(Mathf.Sqrt(Mathf.Pow(Mathf.Sin(a / 2), 2) +
             Mathf.Cos(radLat1) * Mathf.Cos(radLat2) * Mathf.Pow(Mathf.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Mathf.Round(s * 10000) / 10000;
            return s * 1000;
        }
        
        /// <summary>
        /// 获取当前时间，编辑器下为了方便调试使用Time.time,
        /// 手机上使用真实时间
        /// </summary>
        /// <returns></returns>
        public static float time
        {
            get
            {
#if UNITY_EDITOR
                return Time.time;
#else
            return Time.realtimeSinceStartup;
#endif
            }
        }
        /// <summary>
        /// 获取帧之间的时间，编辑器下为了方便调试使用Time.deltaTime,
        /// 手机上使用真实时间
        /// </summary>
        /// <returns></returns>
        public static float deltaTime
        {
            get{ 
#if UNITY_EDITOR
                return Time.deltaTime;
#else
            return Time.unscaledDeltaTime;
#endif
            }
        }
          
        public static string getAppVersion()
        {
            return Application.version;
        }

        /// <summary>
        /// 使用节点名称查找节点下某个子节点
        /// </summary>
        /// <param name="parent">父节点</param>
        /// <param name="name">节点名</param>
        /// <returns></returns>
        public static GameObject DFSFindObject(Transform parent, string name)
        {
            for (int i = 0; i < parent.childCount; ++i)
            {
                Transform node = parent.GetChild(i);
                if (node.name == name)
                    return node.gameObject;

                GameObject target = DFSFindObject(node, name);
                if (target != null)
                    return target;
            }
            return null;
        } 
    }

}