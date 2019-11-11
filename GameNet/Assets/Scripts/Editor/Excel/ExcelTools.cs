using UnityEngine;
using UnityEditor;
using System.IO;
using Excel;
using System.Data;
using System;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Collections.Generic;
using System.CodeDom;

public class ExcelTools : Editor
{
    public const string STR_CONFIRM = "确认";
    public const string STR_CANCEL = "取消";
    private const string STR_NOT_FILE = "没有任何EXCEL文件";
    private const string STR_NOT_FIND_PATH = "找不到此Data文件夹";
    private const string STR_WARNING = "警告";
    private static string STR_CSV_FILE_COMPLETE = "生成完毕";

    private const string TYPE_INT = "int";
    private const string TYPE_BOOL = "bool";
    private const string TYPE_UINT = "uint";
    private const string TYPE_FLOAT = "float";
    private const string TYPE_STRING = "string";
    private const string CLASS_NAME_SUFFIX = "";
    
    //public static string EXCEL_FILE_PATH = Application.dataPath + "/ResourcesForAB/Config/Excel/";
    public static string EXCEL_FILE_PATH = Application.dataPath + "/../../Common/Config/";
    public static string XML_FILE_PATH = Application.dataPath + "/ResourcesForAB/Config/ExcelToXml/";
    public static string CONFIG_DATA_PATH = Application.dataPath + "/ResourcesForAB/Config/ConfigData/";

    private static List<string> ConfigCollectList = null;//配置表集合名称list

    private static List<string> AllFilePathList = null;
    private static string NOW_PATH = "";

    private static System.Reflection.BindingFlags flag = System.Reflection.BindingFlags.Public | BindingFlags.Static | System.Reflection.BindingFlags.Instance;
    const int HeadRowcount = 4;
    [MenuItem("GameTools/ConfigTools/ImportAll")]
    static void BuildExcelCSFile()
    {
        if (!Directory.Exists(XML_FILE_PATH))
        {
            Directory.CreateDirectory(XML_FILE_PATH);
        }
        if (!Directory.Exists(CONFIG_DATA_PATH))
        {
            Directory.CreateDirectory(CONFIG_DATA_PATH);
        } 
        ReadExcelFile();
    }

    //遍历文件夹
    static void GetAllFilePathList(DirectoryInfo dir)//搜索文件夹中的文件
    {
        FileInfo[] allFile = dir.GetFiles();
        foreach (FileInfo fi in allFile)
        {
            AllFilePathList.Add(fi.FullName);
        }

        DirectoryInfo[] allDir = dir.GetDirectories();
        foreach (DirectoryInfo d in allDir)
        {
            if (d.Name == "Server") continue;
            GetAllFilePathList(d);
        }
    }

    /// <summary>读取Excel文件</summary>
    private static void ReadExcelFile()
    {
        //string[] directoryEntries = {};
        try
        {
           // directoryEntries = System.IO.Directory.GetFileSystemEntries(EXCEL_FILE_PATH);
            AllFilePathList = new List<string>();
            DirectoryInfo TheFolder = new DirectoryInfo(EXCEL_FILE_PATH);
            GetAllFilePathList(TheFolder);
            if (AllFilePathList.Count == 0)
            {
                EditorUtility.DisplayDialog(STR_WARNING, STR_NOT_FILE, STR_CONFIRM);
                return;
            }
            ConfigCollectList = new List<string>();
            string path = string.Empty;
            int len = AllFilePathList.Count;
            if (len == 0)
                return;

            List<string> urlList = new List<string>();
            for (int i = 0; i < len; i++)
            {
                path = AllFilePathList[i];
                if (path.Contains("~$"))
                {
                    EditorUtility.DisplayDialog(STR_WARNING, "请关闭Excel文件夹下的所有表格，比如：" + path, STR_CONFIRM);
                    return;
                }
                if (!path.Contains(".meta") && path.Contains(".xlsx"))//仅过滤后缀为xlsx的文件
                {
                    urlList.Add(path);
                }
            }

            for (int s = 0; s < urlList.Count; s++)
            {
                NOW_PATH = urlList[s];
                if (EditorUtility.DisplayCancelableProgressBar("导入中", "路径：" + NOW_PATH, (float)s / ((float)urlList.Count - 1)))
                {
                    Debug.Log("Progress bar canceled by the user");
                    EditorUtility.ClearProgressBar();
                    return;
                }
                AnalyzeExcelData(urlList[s]);
            }
            CreateCollectClassFile();
            TryClearProgressBar();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog(string.Empty, STR_CSV_FILE_COMPLETE, STR_CONFIRM);
        }
        catch (System.IO.DirectoryNotFoundException)
        {
            EditorUtility.DisplayDialog(STR_WARNING, STR_NOT_FIND_PATH, STR_CONFIRM);
        }
    }

    static void TryClearProgressBar()
    {
        try
        {
            EditorUtility.ClearProgressBar();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    [MenuItem("GameTools/ConfigTools/ImportSigle(SelectFirst)")]
    static void BuildPitchExcelCSFile()
    {
        var select = Selection.activeObject; 
        
        DirectoryInfo TheFolder = new DirectoryInfo(EXCEL_FILE_PATH);
        AllFilePathList = new List<string>();
        GetAllFilePathList(TheFolder);
        string path = "";
        for (int i = 0; i < AllFilePathList.Count; i++)
        {
            path = AllFilePathList[i];
            if (path.Contains("~$"))
            {
                EditorUtility.DisplayDialog(STR_WARNING, "请关闭Excel文件夹下的所有表格，比如：" + path, STR_CONFIRM);
                return;
            }
        }

        path = AssetDatabase.GetAssetPath(select);
        if (!path.Contains(".meta") && path.Contains(".xlsx"))//仅过滤后缀为xlsx的文件
        {
            ConfigCollectList = new List<string>();
            AnalyzeExcelData(path);
            if (AllFilePathList.Count == 0)
            {
                EditorUtility.DisplayDialog(STR_WARNING, STR_NOT_FILE, STR_CONFIRM);
                return;
            }
            ConfigCollectList.Clear();
            int len = AllFilePathList.Count;
            if (len == 0)
                return;

            for (int i = 0; i < len; i++)
            {
                path = AllFilePathList[i];
                if (!path.Contains(".meta") && path.Contains(".xlsx"))//仅过滤后缀为xlsx的文件
                {
                    ConfigCollectList.Add(System.IO.Path.GetFileNameWithoutExtension(path));
                }
            }
            CreateCollectClassFile();
            //EditorUtility.DisplayCancelableProgressBar("导入中", "路径：" + path, 1);
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog(string.Empty, STR_CSV_FILE_COMPLETE, STR_CONFIRM);
        }
        else
        {
            EditorUtility.DisplayDialog(STR_WARNING, "此文件不是xlsx格式(" + path + ")", STR_CONFIRM);
        }
    }

    /// <summary>解析excel数据</summary>
    private static void AnalyzeExcelData(string path)
    {
        string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
        using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read))
        {
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet result = excelReader.AsDataSet();
            DataTable dataTable = result.Tables[0];//默认第一张表
            
            try
            {
                CreateCsFile(fileName, dataTable);
            }
            catch(Exception e)
            {
                TryClearProgressBar();
                EditorUtility.DisplayDialog(STR_WARNING, "创建cs文件失败,请检查excel表格式," + "路径：" + NOW_PATH, STR_CONFIRM);
                throw e;
            }
            
            stream.Close();
            excelReader.Close();
        }
    }

    /// <summary>创建文件</summary>
    private static void CreateFile(string path , StringBuilder valueStr)
    {
        FileInfo fileInfo = new FileInfo(path);
        if (fileInfo.Directory.Exists)
        {
           File.Delete(path);
        }
        fileInfo.Directory.Create();
        using (FileStream fs = new FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write))
        {
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            sw.WriteLine(valueStr);
            sw.Flush();
            sw.Close();
            fs.Close();
        }
        //AssetDatabase.Refresh();
    }

    /// <summary>创建cs文件</summary>
    private static void CreateCsFile(string fileName, DataTable dataTable)
    {
        StringBuilder headStr = new StringBuilder();
        headStr.Append("using System;" + "\r\n");
 //       headStr.Append("using Stars;" + "\r\n");

        StringBuilder nterfaceStr = new StringBuilder();
        nterfaceStr.Append("using System;" + "\r\n");
 //       nterfaceStr.Append("using Stars;" + "\r\n");
        nterfaceStr.Append("public interface IConfig{" + "\r\n");
        nterfaceStr.Append("}");

       StringBuilder classStrValue = new StringBuilder();
       classStrValue.Append("[Serializable]" + "\r\n");
       classStrValue.Append("public class " + fileName + " :IConfig{" + "\r\n");

        string invokeStr = "";//函数参数 字符
        string variableStr = "";//get 变量 字符
        string privateVaribaleStr = "";//私有变量 字符
        string assignmentValueStr = "";//函数赋值 字符

        for(int i = 0 ; i <  dataTable.Columns.Count ; i++)
        {
            string variableValue = dataTable.Rows[2][i].ToString();
            string notesValue = dataTable.Rows[1][i].ToString();//注释文本
            notesValue = notesValue.Replace("\n", " ");
            notesValue = notesValue.Replace("\r", " ");
            if (dataTable.Rows[0][i].ToString() == "2" || dataTable.Rows[0][i].ToString() == "3") //== ""通用 == 1仅前端用  == 2仅后端用 == 3都不用
                continue;
            string ID = "";
            if (dataTable.Rows[3][i] == null)
            {
                Debug.LogError("行3" + "列" + i + "为空！");
            }else
            {
                //Debug.Log(dataTable.Rows[3][i]);
                try
                {
                    ID = (string)dataTable.Rows[3][i];
                }
                catch (Exception e)
                {
                    Debug.LogError("行3" + "列" + i + "为空！");
                    TryClearProgressBar();
                    EditorUtility.DisplayDialog(STR_WARNING, "可能存在多余的列数，总列数为" + dataTable.Columns.Count, STR_CONFIRM);
                    throw e;
                }

            }
            if (i == 0 && ID != "id")
            {
                string tempID = ID;
                ID = "id";
                invokeStr += variableValue + " " + "type_" + tempID + (i == dataTable.Columns.Count - 1 ? "" : ",");
                variableStr += "  /// <summary>" + notesValue + "</summary>" + "\r\n";
                variableStr += "  public " + "uint" + " " + ID + "{ get { return _" + ID + "; }}" + "\r\n";// + notesValue + "\r\n";
                assignmentValueStr += "     _" + ID + " =  (uint)type_" + tempID + ";\r\n";
                privateVaribaleStr += "   private " + "uint" + " _" + ID + ";\r\n";
                ID = tempID;

                //invokeStr += variableValue + " " + "type_" + ID + (i == dataTable.Columns.Count - 1 ? "" : ",");
                variableStr += "  /// <summary>" + notesValue + "</summary>" + "\r\n";
                variableStr += "  public " + variableValue + " " + ID + "{ get { return _" + ID + "; }}" + "\r\n";// + notesValue + "\r\n";
                assignmentValueStr += "     _" + ID + " =  type_" + ID + ";\r\n";
                privateVaribaleStr += "   private " + variableValue + " _" + ID + ";\r\n";
            }
            else if (i == 0 && ID == "id")
            {
                invokeStr += variableValue + " " + "type_" + ID + (i == dataTable.Columns.Count - 1 ? "" : ",");
                variableStr += "  /// <summary>" + notesValue + "</summary>" + "\r\n";
                variableStr += "  public " + "uint" + " " + ID + "{ get { return _" + ID + "; }}" + "\r\n";// + notesValue + "\r\n";
                assignmentValueStr += "     _" + ID + " =  (uint)type_" + ID + ";\r\n";
                privateVaribaleStr += "   private " + "uint" + " _" + ID + ";\r\n";
            }
            else
            {
                invokeStr += variableValue + " " + "type_" + ID + (i == dataTable.Columns.Count - 1 ? "" : ",");
                variableStr += "  /// <summary>" + notesValue + "</summary>" + "\r\n";
                variableStr += "  public " + variableValue + " " + ID + "{ get { return _" + ID + "; }}" + "\r\n";// + notesValue + "\r\n";
                assignmentValueStr += "     _" + ID + " =  type_" + ID + ";\r\n";
                privateVaribaleStr += "   private " + variableValue + " _" + ID + ";\r\n";
            }

            
            
        }

        classStrValue.Append(privateVaribaleStr);
        classStrValue.Append("   public " + fileName + " (" + invokeStr + ")" + "{" + "\r\n");
        classStrValue.Append(assignmentValueStr);
        classStrValue.Append("   }\r\n");
        classStrValue.Append(variableStr);
        classStrValue.Append('}');

        string filePath = Application.dataPath + "/" + "Scripts/GameData/ConfigData/" + fileName + ".cs";
        headStr.Append(classStrValue.ToString());
        CreateFile(filePath,headStr);
        
        nterfaceStr.Append(classStrValue);
        //Debug.Log(nterfaceStr);
        Assembly assembly = CreateDynamiClass(fileName, nterfaceStr.ToString());

        try
        {
            SerializeHandler(assembly, dataTable, fileName);
        }
        catch(Exception e)
        {
            TryClearProgressBar();
            EditorUtility.DisplayDialog(STR_WARNING, "创建bytes文件失败,请检查excel字段类型," + "路径：" + NOW_PATH, STR_CONFIRM);
            throw e; 
        }

        CreateXmlFile(dataTable, fileName);
        ConfigCollectList.Add(fileName);
    }

    //创建配置表集合
    private static void CreateCollectClassFile()
    {
        if (ConfigCollectList.Count == 0)
            return;

        StringBuilder classStrValue = new StringBuilder();
        classStrValue.Append("using System;" + "\r\n");
 //       classStrValue.Append("using Stars;" + "\r\n");
        classStrValue.Append("[Serializable]" + "\r\n");
        classStrValue.Append(" /// <summary>配置表集合</summary>" + "\r\n");
        classStrValue.Append("public class ConfigCollect " + " {" + "\r\n");
        classStrValue.Append(" public static Type[] CONFIG_ARRAY = {" + "\r\n");
        for(int i = 0 ; i < ConfigCollectList.Count ; i++)
        {
            classStrValue.Append("     typeof(" + ConfigCollectList[i] + ")" + (i == ConfigCollectList.Count - 1 ? "" : ",") + "\r\n");
        }
        classStrValue.Append("    };" + "\r\n");
        classStrValue.Append("}");
        string filePath = Application.dataPath + "/" + "Scripts/GameData/ConfigData/ConfigCollect.cs";
        CreateFile(filePath, classStrValue);
    }

    //创建表数据接口
    private static void CreateInterface()
    {
        StringBuilder classStrValue = new StringBuilder();
        classStrValue.Append("using System;" + Environment.NewLine);
 //       classStrValue.Append("using Stars;" + Environment.NewLine);
        classStrValue.Append("public interface IConfig{" + "\r\n");
        classStrValue.Append("  int id { get; }" + "\r\n");
        classStrValue.Append("}");
        string filePath = Application.dataPath + "/" + "Scripts/GameData/ConfigData/interface/IConfig.cs";
       // CreateFile(filePath, classStrValue);

        Assembly assembly;
        CSharpCodeProvider provider = new CSharpCodeProvider();
        CompilerParameters paras = new CompilerParameters();
        paras.ReferencedAssemblies.Add("System.dll");
        paras.GenerateExecutable = false;
        paras.GenerateInMemory = false;
        //paras.OutputAssembly = "IConfig.dll";
        CompilerResults result = provider.CompileAssemblyFromSource(paras, classStrValue.ToString());
        assembly = result.CompiledAssembly;
        AssetDatabase.Refresh();
    }

    //创建xml
    private static void CreateXmlFile(DataTable dataTable, string fileName)
    {    
        string xmlFilePath = XML_FILE_PATH + fileName + ".xml";
        if (!File.Exists(xmlFilePath))
        {
            File.Delete(xmlFilePath);
        }
        XmlDocument xmlDoc = new XmlDocument();
        XmlElement root = xmlDoc.CreateElement(fileName);
        if(fileName == "RaceOptionTable")
        {
            Debug.Log("RaceOptionTable");
        }
        for (int w = 0; w < dataTable.Rows.Count; w++)
        {
            if (w < HeadRowcount)
                continue;

            XmlElement iData = xmlDoc.CreateElement("IData");
            for (int s = 0; s < dataTable.Columns.Count; s++)
            {
                string needValue = (dataTable.Rows[0][s]).ToString();
                if (needValue == "1" || needValue == "" || needValue == "0")
                    iData.SetAttribute((dataTable.Rows[3][s]).ToString(), (dataTable.Rows[w][s]).ToString());
            }
             root.AppendChild(iData);
        }
        xmlDoc.AppendChild(root);
        xmlDoc.Save(xmlFilePath);
    }

    //编译类文件
    private static Assembly CreateDynamiClass(string className, string classSource)
    {
        Assembly assembly;
        CSharpCodeProvider provider = new CSharpCodeProvider();
        CompilerParameters paras = new CompilerParameters();
        //paras.ReferencedAssemblies.Add("Assets/Plugins/Editor/Excel/IConfig.dll");
        paras.GenerateExecutable = false;
        paras.GenerateInMemory = false;
        CompilerResults result = provider.CompileAssemblyFromSource(paras, classSource);
        assembly = result.CompiledAssembly;
        provider.Dispose();
        if (result.Errors.Count > 0)
        {
            foreach (CompilerError vCompilerError in result.Errors)
            {
                Debug.Log("错误：" + vCompilerError.ErrorText);
            }
            return null;
        }
        return assembly;
    }
    
    //序列化数据
    private static void SerializeHandler(Assembly assembly, DataTable dataTable, string fileName)
    {
        int columns = dataTable.Columns.Count;
        int rows = dataTable.Rows.Count;
        string tableValue = string.Empty;
        string firstValue = string.Empty;
        object[] dataArr = new object[rows - HeadRowcount];
        object obj = null;
        List<object> variableList = null;
        List<uint> idArr = new List<uint>();
        for (int i = 0; i < rows; i++)
        {
            variableList = new List<object>();
            for (int j = 0; j < columns; j++)
            {
                firstValue = dataTable.Rows[0][j].ToString();
                if (firstValue == "2" || firstValue == "3") //== ""通用 == 1仅前端用  == 2仅后端用  == 3都不用
                    continue;

                if (i < HeadRowcount)// || dataTable.Rows[i][0].ToString() == string.Empty
                {
                    obj = null;
                    continue;
                }

                if(j == 0)
                {
                    DataRow ction = dataTable.Rows[i];
                    string idStr = dataTable.Rows[i][0].ToString();
                    if(idStr == string.Empty)
                    {
                        TryClearProgressBar();
                        EditorUtility.DisplayDialog(STR_WARNING, "excel中的id不能为空,行：" + (i+1) + " 路径：" + NOW_PATH + "总行数为:" + rows + ",可能需要删除多余空白行!!", STR_CONFIRM);
                        throw new Exception(NOW_PATH);
                    }
                    uint id = uint.Parse(idStr);
                    if (idArr.Count == 0)
                    {
                        idArr.Add(id);
                    } 
                    else
                    {
                        //检查id是否重复
                        for (int s = 0; s < idArr.Count; s++)
                        {

                            if (idArr[s] == id)
                            {
                                TryClearProgressBar();
                                EditorUtility.DisplayDialog(STR_WARNING, "excel中的id有重复,:" + id + " 路径：" + NOW_PATH, STR_CONFIRM);
                                throw new Exception(NOW_PATH);
                            }
                        }
                        idArr.Add(id);
                    }
                }

                tableValue = dataTable.Rows[i][j].ToString();
                string variableValueType = dataTable.Rows[2][j].ToString();//字段类型行
                string variableValue = dataTable.Rows[3][j].ToString();//字段行

                if (tableValue == string.Empty)
                {
                    switch (variableValueType)
                    {
                        case TYPE_INT:
                            tableValue = Convert.ToString(0);
                            break;
                        case TYPE_BOOL:
                            tableValue = Convert.ToString(false);
                            break;
                        case TYPE_UINT:
                            tableValue = Convert.ToString(0);
                            break;
                        case TYPE_FLOAT:
                            tableValue = Convert.ToString(0);
                            break;
                        default:
                            tableValue = string.Empty;
                            break;
                    }
                }

                object str = getTypeValue(variableValueType, tableValue,j == 0);
                variableList.Add(str);
            }

            if (variableList.Count == 0)
                continue;

            obj = assembly.CreateInstance(fileName, false, flag, null, variableList.ToArray(), null, null);
            dataArr[i - HeadRowcount] = obj;
        }

        using (FileStream fs = new FileStream(CONFIG_DATA_PATH + fileName + ".bytes", FileMode.Create))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, dataArr);
            fs.Close();
        }
    }

    /// <summary>将value转换成type格式</summary>
    private static object getTypeValue(string type,string value,bool isID)
    {
        //if (isID)
          //  type = TYPE_UINT;

        switch(type)
        {
            case TYPE_INT:
                return Convert.ToInt32(value);
            case TYPE_BOOL:
                return Convert.ToBoolean(value);
            case TYPE_UINT:
                return Convert.ToUInt32(value);
            case TYPE_FLOAT:
                return float.Parse(value);//Convert.ToDouble(value);
            default:
                return value == "null" ?"":value;
        }
    }
}
