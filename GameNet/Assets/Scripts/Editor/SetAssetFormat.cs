using UnityEngine;
using System.Collections;
using UnityEditor;

public class SetAssetFormat
{
    //[MenuItem("Assets/设置选中资源格式/纹理格式/通用纹理(UITexture,Atlas,不带MipMap特效)")]
    static void SetCommonTextureFormat()
    {
        Object[] textures = GetSelectedTextures();
        int i = 1;
        foreach (Texture2D texture in textures)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            if (EditorUtility.DisplayCancelableProgressBar("设置图片格式", path, (float)i / (float)textures.Length))
            {
                break;
            }
            bool changed = false;
            if (textureImporter.textureType != TextureImporterType.Default)
            {
                textureImporter.textureType = TextureImporterType.Default;
                changed = true;
            }
            if (textureImporter.generateCubemap != TextureImporterGenerateCubemap.None)
            {
                textureImporter.generateCubemap = TextureImporterGenerateCubemap.None;
                changed = true;
            }
            if (textureImporter.isReadable != false)
            {
                textureImporter.isReadable = false;
                changed = true;
            }
            if (textureImporter.spriteImportMode != SpriteImportMode.None)
            {
                textureImporter.spriteImportMode = SpriteImportMode.None;
                changed = true;
            }
            if (textureImporter.mipmapEnabled != false)
            {
                textureImporter.mipmapEnabled = false;
                changed = true;
            }
            if (textureImporter.wrapMode != TextureWrapMode.Clamp)
            {
                textureImporter.wrapMode = TextureWrapMode.Clamp;
                changed = true;
            }
            if (textureImporter.filterMode != FilterMode.Bilinear)
            {
                textureImporter.filterMode = FilterMode.Bilinear;
                changed = true;
            }
            if (textureImporter.anisoLevel != 0)
            {
                textureImporter.anisoLevel = 0;
                changed = true;
            }
            if (textureImporter.compressionQuality != 100)
            {
                textureImporter.compressionQuality = 100;
                changed = true;
            }
            int maxSize = textureImporter.maxTextureSize;
            if (setPlatformTexture("iPhone", maxSize, textureImporter))
            {
                changed = true;
            }
            if(setPlatformTexture("Android", maxSize, textureImporter)){
                changed = true;
            }
            if (changed)
            {
                AssetDatabase.ImportAsset(path);
                Debug.Log(path + "设置成功");
            }
            else
            {
                Debug.Log(path + "以前已成功设置");
            }
            i++;
        }
        EditorUtility.ClearProgressBar();
    }

    //[MenuItem("Assets/设置选中资源格式/纹理格式/带MipMap的人物，场景纹理,特效")]
    static void SetMipMapTextureFormat()
    {
        Object[] textures = GetSelectedTextures();
        int i = 1;
        foreach (Texture2D texture in textures)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            if (EditorUtility.DisplayCancelableProgressBar("设置图片格式", path, (float)i / (float)textures.Length))
            {
                break;
            }
            bool changed = false;
            if (textureImporter.textureType != TextureImporterType.Default)
            {
                textureImporter.textureType = TextureImporterType.Default;
                changed = true;
            }
            if (textureImporter.generateCubemap != TextureImporterGenerateCubemap.None)
            {
                textureImporter.generateCubemap = TextureImporterGenerateCubemap.None;
                changed = true;
            }
            if (textureImporter.isReadable != false)
            {
                textureImporter.isReadable = false;
                changed = true;
            }
            if (textureImporter.spriteImportMode != SpriteImportMode.None)
            {
                textureImporter.spriteImportMode = SpriteImportMode.None;
                changed = true;
            }
            if (textureImporter.mipmapEnabled != true)
            {
                textureImporter.mipmapEnabled = true;
                changed = true;
            }
            if (textureImporter.wrapMode != TextureWrapMode.Repeat)
            {
                textureImporter.wrapMode = TextureWrapMode.Repeat;
                changed = true;
            }
            if (textureImporter.filterMode != FilterMode.Trilinear)
            {
                textureImporter.filterMode = FilterMode.Trilinear;
                changed = true;
            }
            if (textureImporter.anisoLevel != 4)
            {
                textureImporter.anisoLevel = 4;
                changed = true;
            }
            if (textureImporter.compressionQuality != 100)
            {
                textureImporter.compressionQuality = 100;
                changed = true;
            }
            int maxSize = textureImporter.maxTextureSize;
            if (setPlatformTexture("iPhone", maxSize, textureImporter))
            {
                changed = true;
            }
            if (setPlatformTexture("Android", maxSize, textureImporter))
            {
                changed = true;
            }
            if (changed)
            {
                AssetDatabase.ImportAsset(path);
                Debug.Log(path + "设置成功");
            }
            else
            {
                Debug.Log(path + "以前已成功设置");
            }
            i++;
        }
        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// 使用原最大纹理大小来设置平台相关纹理
    /// </summary>
    /// <param name="platform">平台</param>
    /// <param name="oriMaxSize">各平台通用大小</param>
    /// <param name="textureImporter">TextureImporter</param>
    /// 返回值：设置成功返回true

    
    static bool setPlatformTexture(string platform, int oriMaxSize, TextureImporter textureImporter)
    {
        int maxTextureSize = 0;//平台最大尺寸
        TextureImporterFormat textureFormat;
        int compressionQuality = 0;
        TextureImporterFormat platformtextureFormat = platform == "iPhone" ? TextureImporterFormat.PVRTC_RGB4 : TextureImporterFormat.ETC_RGB4;

        if (textureImporter.GetPlatformTextureSettings(platform, out maxTextureSize, out textureFormat, out compressionQuality))
        {
            if ((platformtextureFormat != textureFormat) || (compressionQuality != 100))
            {
                textureImporter.SetPlatformTextureSettings(platform, maxTextureSize, platformtextureFormat, 100,true);
                return true;
            }
            else
            {
                Debug.Log(platformtextureFormat.ToString() + " " + textureFormat.ToString());
                Debug.Log(compressionQuality);
            }
        }
        else
        {
            textureImporter.SetPlatformTextureSettings(platform, oriMaxSize, platformtextureFormat, 100, true);
            return true;
        }
        return false;
    }

    //[MenuItem("SetAssetFormat/纹理格式/设置下的纹理")]
    static void SetChrTextureFormat()
    {
        string[] path = new string[1];
        path[0] = @"Assets/ResourcesForAB/Model/Monster";
        string[] dirRes = AssetDatabase.FindAssets("t:texture2D", path);
        int i = 1;
        foreach (string s in dirRes)
        {
            string assetPathName = AssetDatabase.GUIDToAssetPath(s);
            if (EditorUtility.DisplayCancelableProgressBar("设置图片格式", assetPathName, (float)i / (float)dirRes.Length))
            {
                break;
            }
            TextureImporter textureImporter = AssetImporter.GetAtPath(assetPathName) as TextureImporter;
            //int maxSize = 512;
            //if (assetPathName.IndexOf("boss") == -1) maxSize = 128;
            //textureImporter.SetPlatformTextureSettings("iPhone", maxSize, TextureImporterFormat.PVRTC_RGB4, 100);
            //textureImporter.SetPlatformTextureSettings("Android", maxSize, TextureImporterFormat.ETC_RGB4, 100);

            bool changed = false;
            if (textureImporter.textureType != TextureImporterType.Default)
            {
                textureImporter.textureType = TextureImporterType.Default;
                changed = true;
            }
            if (textureImporter.generateCubemap != TextureImporterGenerateCubemap.None)
            {
                textureImporter.generateCubemap = TextureImporterGenerateCubemap.None;
                changed = true;
            }
            if (textureImporter.isReadable != false)
            {
                textureImporter.isReadable = false;
                changed = true;
            }
            if (textureImporter.spriteImportMode != SpriteImportMode.None)
            {
                textureImporter.spriteImportMode = SpriteImportMode.None;
                changed = true;
            }
            if (textureImporter.mipmapEnabled != true)
            {
                textureImporter.mipmapEnabled = true;
                changed = true;
            }
            if (textureImporter.wrapMode != TextureWrapMode.Repeat)
            {
                textureImporter.wrapMode = TextureWrapMode.Repeat;
                changed = true;
            }
            if (textureImporter.filterMode != FilterMode.Trilinear)
            {
                textureImporter.filterMode = FilterMode.Trilinear;
                changed = true;
            }
            if (textureImporter.anisoLevel != 4)
            {
                textureImporter.anisoLevel = 4;
                changed = true;
            }
            if (textureImporter.compressionQuality != 100)
            {
                textureImporter.compressionQuality = 100;
                changed = true;
            }
            int maxSize = textureImporter.maxTextureSize;
            if (setPlatformTexture("iPhone", maxSize, textureImporter))
            {
                changed = true;
            }
            if (setPlatformTexture("Android", maxSize, textureImporter))
            {
                changed = true;
            }
            if (changed)
            {
                AssetDatabase.ImportAsset(assetPathName);
                Debug.Log(assetPathName + "设置成功");
            }
            else
            {
                Debug.Log(assetPathName + "以前已成功设置");
            }
            i++;
        }
        EditorUtility.ClearProgressBar();
    }
    //[MenuItem("SetAssetFormat/纹理格式/设置XX下的纹理")]
    static void SetAtlasTextureFormat()
    {
        string[] path = new string[2];
        path[0] = @"Assets/ResourcesForAB/Atlas";
        path[1] = @"Assets/ResourcesForAB/UITexture";
        string[] dirRes = AssetDatabase.FindAssets("t:texture2D", path);
        int i = 1;
        foreach (string s in dirRes)
        {
            string assetPathName = AssetDatabase.GUIDToAssetPath(s);
            if (EditorUtility.DisplayCancelableProgressBar("设置图片格式", assetPathName, (float)i / (float)dirRes.Length))
            {
                break;
            }
            TextureImporter textureImporter = AssetImporter.GetAtPath(assetPathName) as TextureImporter;
            //textureImporter.textureType = TextureImporterType.Advanced;
            //textureImporter.generateCubemap = TextureImporterGenerateCubemap.None;
            //textureImporter.isReadable = false;
            ////textureImporter.grayscaleToAlpha = false;
            //textureImporter.spriteImportMode = SpriteImportMode.None;
            //textureImporter.mipmapEnabled = false;
            //textureImporter.wrapMode = TextureWrapMode.Clamp;
            //textureImporter.filterMode = FilterMode.Bilinear;
            //textureImporter.anisoLevel = 0;
            //textureImporter.compressionQuality = 100;
            //int maxSize = textureImporter.maxTextureSize;
            //setPlatformTexture("iPhone", maxSize, textureImporter);
            //setPlatformTexture("Android", maxSize, textureImporter);
            //AssetDatabase.ImportAsset(assetPathName);
            //Debug.Log(assetPathName);
            bool changed = false;
            if (textureImporter.textureType != TextureImporterType.Default)
            {
                textureImporter.textureType = TextureImporterType.Default;
                changed = true;
            }
            if (textureImporter.generateCubemap != TextureImporterGenerateCubemap.None)
            {
                textureImporter.generateCubemap = TextureImporterGenerateCubemap.None;
                changed = true;
            }
            if (textureImporter.isReadable != false)
            {
                textureImporter.isReadable = false;
                changed = true;
            }
            if (textureImporter.spriteImportMode != SpriteImportMode.None)
            {
                textureImporter.spriteImportMode = SpriteImportMode.None;
                changed = true;
            }
            if (textureImporter.mipmapEnabled != false)
            {
                textureImporter.mipmapEnabled = false;
                changed = true;
            }
            if (textureImporter.wrapMode != TextureWrapMode.Clamp)
            {
                textureImporter.wrapMode = TextureWrapMode.Clamp;
                changed = true;
            }
            if (textureImporter.filterMode != FilterMode.Bilinear)
            {
                textureImporter.filterMode = FilterMode.Bilinear;
                changed = true;
            }
            if (textureImporter.anisoLevel != 0)
            {
                textureImporter.anisoLevel = 0;
                changed = true;
            }
            if (textureImporter.compressionQuality != 100)
            {
                textureImporter.compressionQuality = 100;
                changed = true;
            }
            int maxSize = textureImporter.maxTextureSize;
            if (setPlatformTexture("iPhone", maxSize, textureImporter))
            {
                changed = true;
            }
            if (setPlatformTexture("Android", maxSize, textureImporter))
            {
                changed = true;
            }
            if (changed)
            {
                AssetDatabase.ImportAsset(assetPathName);
                Debug.Log(assetPathName + "设置成功");
            }
            else
            {
                Debug.Log(assetPathName + "以前已成功设置");
            }
            i++;
        }
        EditorUtility.ClearProgressBar();
    }
    static Object[] GetSelectedTextures()
    {

        return Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);

    }

}
