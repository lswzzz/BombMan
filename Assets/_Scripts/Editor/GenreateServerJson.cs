using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using LitJson;
using System.Xml.Linq;
using System.Xml;
using System;

//http://bbs.9ria.com/thread-206230-1-1.html
namespace BombEditor {

    //生成server使用的json文件
    public class GenreateServerJson
    {
        public static string resFolder = "TileSource";        
        
        [MenuItem("BombEditor/GenerateServerJson")]
        static void GeneratedServerJson()
        {
            List<string> list = getAllTileJsonFilePath();
            foreach(string str in list)
            {
                string saveFile = Application.dataPath + "/GenerateJson/Server/" + System.IO.Path.GetFileName(str);
                generateServerJsonFile(str, saveFile);
            }
        }

        public static List<string> getAllTileJsonFilePath()
        {
            string path = Application.dataPath + "/" + resFolder;
            string[] directoryEntries;
            List<string> files = new List<string>();
            
            try
            {
                directoryEntries = System.IO.Directory.GetFileSystemEntries(path);
                for (int i = 0; i < directoryEntries.Length; i++)
                {
                    string p = directoryEntries[i];
                    if (!p.EndsWith(".json")) continue;
                    files.Add(p);
                }

            }
            catch (System.IO.DirectoryNotFoundException)
            {
                Debug.Log("The path encapsulated in the " + path + "Directory object does not exist.");
            }
            return files;
        }

        static void generateServerJsonFile(string path, string saveFile)
        {
            StreamReader sr = new StreamReader(path);
            string json = sr.ReadToEnd();
            JsonData jsonData = JsonMapper.ToObject(json);

            JsonData jsonWriter = new JsonData();
            int height = (int)jsonData["height"];
            int width = (int)jsonData["width"];
            jsonWriter["height"] = jsonData["height"];
            jsonWriter["width"] = jsonData["width"];
            jsonWriter["tileheight"] = jsonData["tileheight"];
            jsonWriter["tilewidth"] = jsonData["tilewidth"];
            List<int> obstacles = new List<int>();
            List<int> statics = new List<int>();
            JsonData jsonPlayer = new JsonData();
            JsonData jsonMonster = new JsonData();
            List<int> colliderNumbers = new List<int>();
            for(int i=0; i<jsonData["tilesets"].Count; i++)
            {
                JsonData tileset = jsonData["tilesets"][i];
                if (tileset.Keys.Contains("tiles"))
                {
                    JsonData tiles = tileset["tiles"];
                    foreach (string numstr in tiles.Keys)
                    {
                        int number = int.Parse(numstr);
                        number += (int)tileset["firstgid"];
                        colliderNumbers.Add(number);
                    }
                }
            }
            int count = jsonData["layers"].Count;
            for(int i=0; i<jsonData["layers"].Count; i++)
            {
                JsonData data = jsonData["layers"][i];
                if (data["name"].Equals(StaticConstant.obstacleLayername))
                {
                    for(int j=0; j< data["data"].Count; j++)
                    {
                        int number = int.Parse(data["data"][j].ToString());
                        if (colliderNumbers.Contains(number))
                            obstacles.Add(j);
                    }
                }
                else if (data["name"].Equals(StaticConstant.staticLayerName))
                {
                    for (int j = 0; j < data["data"].Count; j++)
                    {
                        int number = int.Parse(data["data"][j].ToString());
                        if (colliderNumbers.Contains(number))
                            statics.Add(j);
                    }
                }
                else if (data["name"].Equals(StaticConstant.playerLayer))
                {
                    for(int j = 0; j< data["objects"].Count; j++)
                    {
                        JsonData obj = data["objects"][j];
                        JsonData playerData = new JsonData();
                        playerData["name"] = obj["name"];
                        playerData["x"] = (int)obj["x"];
                        playerData["y"] = -(int)obj["y"];
                        jsonPlayer.Add(playerData);
                    }
                }
                else if (data["name"].Equals(StaticConstant.monsterLayer))
                {
                    for (int j = 0; j < data["objects"].Count; j++)
                    {
                        JsonData obj = data["objects"][j];
                        JsonData playerData = new JsonData();
                        playerData["name"] = obj["name"];
                        playerData["x"] = (int)obj["x"];
                        playerData["y"] = -(int)obj["y"];
                        jsonMonster.Add(playerData);
                    }
                }
            }

            JsonData jsonObstacles = new JsonData();
            JsonData jsonStatics = new JsonData();
            foreach(int number in obstacles)
            {
                jsonObstacles.Add(number);
            }
            foreach (int number in statics)
            {
                jsonStatics.Add(number);
            }
            jsonWriter["obstacle"] = jsonObstacles;
            jsonWriter["static"] = jsonStatics;
            jsonWriter["players"] = jsonPlayer;
            jsonWriter["monsters"] = jsonMonster;

            File.WriteAllText(saveFile, jsonWriter.ToJson());
        }

        
    }
}
