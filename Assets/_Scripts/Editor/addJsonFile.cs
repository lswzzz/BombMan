using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

namespace BombEditor {
    //http://blog.csdn.net/cocos2der/article/details/50595585
    //拷贝Json文件进入unity中方便使用
    public class addJsonFile
    {
        [MenuItem("BombEditor/AddJsonFile")]
        static void AddJsonFile()
        {
            string file = EditorUtility.OpenFilePanel("选择一个json文件", "", "json");
            if (file == "") return;
            string saveFile = Application.dataPath + "/TileSource/" + System.IO.Path.GetFileName(file);
            CopyFile(file, saveFile);
        }

        static void CopyFile(string srcFile, string destFile)
        {
            if (IsFileExists(srcFile) && !srcFile.Equals(destFile))
            {
               File.Copy(srcFile, destFile, true);

                AssetDatabase.Refresh();
            }
        }

        public static bool IsFileExists(string file)
        {
            if (file.Equals(string.Empty))
            {
                return false;
            }

            return File.Exists(file);
        }

    }

}

