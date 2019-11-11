
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Core;

using System.IO;
using System;
using Stars;
public class APKZip : MonoBehaviour
{
    /// <summary>
    /// 压缩多层目录
    /// </summary>
    /// <param name="strDirectory">The directory.</param>
    /// <param name="zipedFile">The ziped file.</param>
    public static void ZipFileDirectory(string strDirectory, string zipedFile,List<string> storeLvFiles)
    {
        using (System.IO.FileStream ZipFile = System.IO.File.Create(zipedFile))
        {
            using (ZipOutputStream s = new ZipOutputStream(ZipFile))
            {
                string strDirectoryCopy = string.Copy(strDirectory);
                strDirectoryCopy = strDirectoryCopy.Replace("\\", "/");
                ZipSetp(strDirectoryCopy, s, "", storeLvFiles);
            }
        }
    }

    /// <summary>
    /// 递归遍历目录
    /// </summary>
    /// <param name="strDirectory">The directory.必须提前将所有"\\"替换成"/"</param>
    /// <param name="s">The ZipOutputStream Object.</param>
    /// <param name="parentPath">The parent path.</param>
    private static void ZipSetp(string strDirectory, ZipOutputStream s, string parentPath, List<string> storeLvFiles)
    {
        if (strDirectory[strDirectory.Length - 1] != Path.DirectorySeparatorChar)
        {
            strDirectory += Path.DirectorySeparatorChar;
        }
        Crc32 crc = new Crc32();

        string[] filenames = Directory.GetFileSystemEntries(strDirectory);

        foreach (string onefile in filenames)// 遍历所有的文件和目录
        {
            string file = onefile.Replace("\\", "/");
            if (Directory.Exists(file))// 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
            {
                string pPath = parentPath;
                pPath += file.Substring(file.LastIndexOf("/") + 1);
                pPath += "/";
                ZipSetp(file, s, pPath, storeLvFiles);
            }

            else // 否则直接压缩文件
            {
                //打开压缩文件
                using (FileStream fs = File.OpenRead(file))
                {

                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);

                    string fileName = parentPath + file.Substring(file.LastIndexOf("/") + 1);
                    ZipEntry entry = new ZipEntry(fileName);

                    entry.DateTime = DateTime.Now;
                    entry.Size = fs.Length;
                    if (storeLvFiles.Contains(fileName))
                    {
                        entry.CompressionMethod = CompressionMethod.Stored;
                    }
                    fs.Close();

                    crc.Reset();
                    crc.Update(buffer);

                    entry.Crc = crc.Value;
                    s.PutNextEntry(entry);

                    s.Write(buffer, 0, buffer.Length);
                }
            }
        }
    }
    static public void reCompress(string apkFilename)
    {
        string FileNameWithoutExtension = Path.GetFileNameWithoutExtension(apkFilename);
        ZipHelper.UnZip(apkFilename, FileNameWithoutExtension, "", true);
        List<string> storeLvFiles = new List<string>();
        storeLvFiles.Add("assets/GameStory.mp4");
        storeLvFiles.Add("assets/FSN_Show1.mp4");
        storeLvFiles.Add("assets/FSN_GilgameshShow.mp4");
        APKZip.ZipFileDirectory(FileNameWithoutExtension + "/", FileNameWithoutExtension + "_Compress.apk", storeLvFiles);
        Directory.Delete(FileNameWithoutExtension,true);
        File.Delete(apkFilename);
        File.Move(FileNameWithoutExtension + "_Compress.apk", apkFilename);
    }
}
