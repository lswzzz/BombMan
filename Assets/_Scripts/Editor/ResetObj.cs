using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

namespace BombEditor
{
    //刷新Tiled2Unity中的obj文件的z让他保持正确的z轴
    //http://www.unity.5helpyou.com/2803.html
    public class ResetObj
    {
        public static string path = Application.dataPath + "/Tiled2Unity/Meshes/";

        [MenuItem("BombEditor/ResetAllObjs")]
        static void resetObjs()
        {
            List<string> list = getAllMeshsFilePath();
            foreach(string file in list)
            {
                resetObjFile(file);
            }
        }

        public static List<string> getAllMeshsFilePath()
        {
            string[] directoryEntries;
            List<string> files = new List<string>();
            try
            {
                directoryEntries = System.IO.Directory.GetFileSystemEntries(path);
                for (int i = 0; i < directoryEntries.Length; i++)
                {
                    string p = directoryEntries[i];
                    if (p.EndsWith(".meta")) continue;
                    if (!p.EndsWith(".obj")) continue;
                    files.Add(p);
                }

            }
            catch (System.IO.DirectoryNotFoundException)
            {
                Debug.Log("The path encapsulated in the " + path + "Directory object does not exist.");
            }
            return files;
        }

        public static void resetObjFile(string file)
        {
            StreamReader sr = new StreamReader(file);
            string objData = "";
            string data = "";
            int index = 0;
            while(!sr.EndOfStream)
            {
                
                data = sr.ReadLine();
                if (!data.StartsWith("v "))
                {
                    objData += data;
                    objData += "\r\n";
                }
                else
                {
                    string[] splits = data.Split(' ');
                    splits[3] = "0";
                    string newStr = "";
                    for(int i=0; i<splits.Length; i++)
                    {
                        string str = splits[i];
                        newStr += str;
                        if(i < splits.Length - 1)
                        {
                            newStr += " ";
                        }
                    }
                    objData += newStr;
                    objData += "\r\n";
                }
            }
            sr.Close();
            File.WriteAllText(file, objData);
            AssetDatabase.Refresh();
        }
    }
}

