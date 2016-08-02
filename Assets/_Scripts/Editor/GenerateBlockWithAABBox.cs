using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using LitJson;
using System;

namespace BombEditor {
    //将Tiled2Unity生成的prefab文件进行修改添加obstacle进去
    public class GenerateBlockWithAABBox
    {
        public struct BlockData {
            public int gid;
            public int index;
        }

        public static List<BlockData> blockList;
        public static int width;
        public static int height;
        public static int tileWidth;
        public static int tileHeight;
        public static List<int> obstacles;
        public static List<int> obstacleTypes;
        public static string prefabsname;
        public static List<GameObject> obstaclesLayer;
        public static List<GameObject> staticsLayer;
        public static string PrefabsPath = "Prefabs/";
        public static string BlockName = "Block";
        public static string TexturePath = "Textures/";

        [MenuItem("BombEditor/GenerateBlockWithAABBoxToPrefabsWithScene")]
        static void GeneratedBlockWithAABBox()
        {
            GameObject obj = Selection.activeGameObject;
            if(obj == null)
            {
                Debug.Log("当前未选中任何目标");
                return;
            }
            string name = obj.name.Substring(3);
            string sourceFile = Application.dataPath + "/TileSource/" + name + ".json";
            if (!addJsonFile.IsFileExists(sourceFile))
            {
                Debug.Log("请选中一个Prefab对象");
                return;
            }
            prefabsname = obj.name;
            readJsonFile(sourceFile);

        }

        static void readJsonFile(string path)
        {
            StreamReader sr = new StreamReader(path);
            string json = sr.ReadToEnd();
            JsonData jsonData = JsonMapper.ToObject(json);
            width = (int)jsonData["width"];
            height = (int)jsonData["height"];
            tileWidth = (int)jsonData["tilewidth"];
            tileHeight = (int)jsonData["tileheight"];
            readTilesets(jsonData);
            readObstacle(jsonData);
            resetSortingLayer();
            generateObstacleLayer();
            GenerateStaticLayer();
            generateToHierarchy();
            GenerateStaticAABBox();
            resetToPrefab();
        }

        static void readTilesets(JsonData root)
        {
            blockList = new List<BlockData>();
            JsonData tilesets = root["tilesets"];
            for(int i=0; i<tilesets.Count; i++)
            {
                JsonData tiles = tilesets[i];
                int firstgid = (int)tiles["firstgid"];
                if (!tiles.Keys.Contains("tileproperties")) continue;
                JsonData tileproperties = tiles["tileproperties"];
                foreach(string numstr in tileproperties.Keys)
                {
                    if(tileproperties[numstr].Keys.Contains("block"))
                    {
                        int number = int.Parse(numstr);
                        number += firstgid;
                        BlockData block = new BlockData();
                        block.gid = number;
                        block.index = (int)tileproperties[numstr]["block"];
                        blockList.Add(block);
                    }
                }
            }
        }

        static void readObstacle(JsonData root)
        {
            obstacles = new List<int>();
            obstacleTypes = new List<int>();
            List<int> colliderNumbers = new List<int>();
            for(int i=0; i<blockList.Count; i++)
            {
                colliderNumbers.Add(blockList[i].gid);
            }
            int count = root["layers"].Count;
            for (int i = 0; i < root["layers"].Count; i++)
            {
                JsonData data = root["layers"][i];
                if (data["name"].Equals(StaticConstant.obstacleLayername))
                {
                    for (int j = 0; j < data["data"].Count; j++)
                    {
                        int number = int.Parse(data["data"][j].ToString());
                        if (colliderNumbers.Contains(number))
                        {
                            obstacles.Add(j);
                            obstacleTypes.Add(number);
                        }
                            
                    }
                }
            }
        }

        static void resetToPrefab()
        {
            GameObject instance = Selection.activeGameObject;
            if(instance.GetComponent<SceneInit>() == null)
            {
                instance.AddComponent<SceneInit>();
            }
            PrefabUtility.ReplacePrefab(instance, PrefabUtility.GetPrefabParent(instance), ReplacePrefabOptions.ConnectToPrefab);
            AssetDatabase.Refresh();
        }

        static void generateToHierarchy()
        {
            Transform obj = Selection.activeGameObject.transform;
            for(int i=0; i<obstacles.Count; i++)
            {
                int index = obstacles[i];
                int row = index / width;
                int col = index % width;
                GenerateObstacle(row, col, obstacleTypes[i]);
            }
        }

        static void GenerateObstacle(int row, int col, int type)
        {
            GameObject block = (GameObject)GameObject.Instantiate(Resources.Load(PrefabsPath + BlockName + Convert.ToString(type)));
            GameObject layer = obstaclesLayer[row];
            block.transform.parent = layer.transform;
            block.transform.localPosition = new Vector3(col * tileWidth, -(row + 1) * tileHeight, 0);
            Block blockComponent = block.GetComponent<Block>();
            blockComponent.InitBlockTransform(block.transform.position, tileWidth, tileHeight);
            block.transform.name = BlockName + Convert.ToString(col) + "_" + Convert.ToString(type);
        }

        static void GenerateStaticAABBox()
        {
            int index = 0;
            foreach(GameObject layer in staticsLayer)
            {
                foreach(Transform child in layer.transform)
                {
                    if(child.name == "Collision")
                    {
                        GenerateAABBoxToCollsionLayer(child);
                    }
                }
                index++;
            }
        }

        static void GenerateAABBoxToCollsionLayer(Transform collision)
        {
            PolygonCollider2D[] colliders = collision.GetComponents<PolygonCollider2D>();
            foreach(PolygonCollider2D collider in colliders)
            {
                for(int i=0; i<collider.pathCount; i++)
                {
                    Vector2 []paths = collider.GetPath(i);
                    int count = (int)(paths[2].x - paths[0].x) / tileWidth;
                    for (int n = 0; n < count; n++)
                    {
                        AABBox box = collision.gameObject.AddComponent<AABBox>();
                        Vector2 min = new Vector2(paths[0].x + tileWidth * n, paths[1].y);
                        Vector2 max = new Vector2(paths[0].x + tileWidth * (n + 1), paths[0].y);
                        box.resetAABBox(min, max);
                    }
                }
            }
            for (int i = 0; i < colliders.Length; i++)
            {
                GameObject.DestroyImmediate(colliders[i]);
            }

        }

        static void GenerateStaticLayer()
        {
            staticsLayer = new List<GameObject>();
            Transform obj = Selection.activeGameObject.transform;
            for (int i = 0; i < obj.childCount; i++)
            {
                Transform objChild = obj.GetChild(i);
                string name = objChild.name;
                if (name.StartsWith(StaticConstant.staticPrefName))
                {
                    staticsLayer.Add(objChild.gameObject);
                }
            }
        }

        static void generateObstacleLayer()
        {
            obstaclesLayer = new List<GameObject>();
            Transform obj = Selection.activeGameObject.transform;
            for (int i = 0; i < obj.childCount; i++)
            {
                Transform objChild = obj.GetChild(i);
                string name = objChild.name;
                if (name.StartsWith(StaticConstant.obstaclePrefName))
                {
                    obstaclesLayer.Add(objChild.gameObject);
                }
            }
        }

        //在这里刷新render的Order in layer
        static void resetSortingLayer()
        {
            GameObject obj = Selection.activeGameObject;
            resetObjSortingLayer(obj.transform);
        }

        static void resetObjSortingLayer(Transform obj)
        {
            if(obj.GetComponent<Renderer>() != null)
            {
                obj.GetComponent<Renderer>().sortingOrder = 0;
            }
            
            for(int i=0; i<obj.childCount; i++)
            {
                Transform objChild = obj.GetChild(i);
                resetObjSortingLayer(objChild);
            }
        }
    }
}
