using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using LitJson;
using System.Xml.Linq;
using System.Xml;
using System;

namespace BombEditor
{
    //修改原来生成的Tmx文件使之导入符合项目要求
    public class GenerateTMx
    {
        public static List<XmlNode> tileList;
        
        
        public static string _n = "\r\n";
        [MenuItem("BombEditor/ResetTMXFile")]
        static void ResetTMXFile()
        {
            string file = EditorUtility.OpenFilePanel("选择一个tmx文件", "", "tmx");
            if (file == "") return;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(file);
            XmlNode root = xmlDoc.SelectSingleNode("map");
            int height = int.Parse(root.Attributes["height"].Value);
            int width = int.Parse(root.Attributes["width"].Value);
            XmlNodeList layerList = xmlDoc.GetElementsByTagName("layer");
            XmlNode obstacleNode = null;
            XmlNode staticBeforeNode = null;
            XmlNode staticAfterNode = null;
            XmlNode backgroundNode = null;
            List<XmlNode> otherLayer = new List<XmlNode>();
            for (int i = 0; i < layerList.Count; i++)
            {
                root.RemoveChild(layerList[i]);
            }
            for (int i = 0; i < layerList.Count; i++)
            {
                XmlNode node = layerList[i];
                if (node.Attributes["name"].Value == StaticConstant.obstacleLayername)
                {
                    obstacleNode = node;

                }
                else if (node.Attributes["name"].Value == StaticConstant.staticLayerName)
                {
                    staticBeforeNode = node;
                }
                else if (node.Attributes["name"].Value == StaticConstant.staticAfterLayerName)
                {
                    staticAfterNode = node;
                }else if (node.Attributes["name"].Value == StaticConstant.backgroundName)
                {
                    backgroundNode = node;
                }
            }
            tileList = GenerateTileNodes(root, width, height);
            XmlNode[] obstacles = GenerateObstacleNodes(obstacleNode, width, height, true);
            XmlNode[] statics = GenerateStaticNodes(staticBeforeNode, staticAfterNode, width, height);
            root.AppendChild(backgroundNode);
            for (int i = 0; i < height; i++)
            {
                root.AppendChild(obstacles[i]);
                root.AppendChild(statics[i]);
            }

            string filename = "new" + System.IO.Path.GetFileNameWithoutExtension(file) + System.IO.Path.GetExtension(file);
            string newFile = System.IO.Path.GetDirectoryName(file) + "/" + filename;
            xmlDoc.Save(newFile);
        }


        static XmlNode[] GenerateObstacleNodes(XmlNode obstacle, int width, int height, bool isNone)
        {
            string data = obstacle["data"].InnerText;
            string[] numbersStrList = data.Split(',');
            int[] numberList = new int[numbersStrList.Length];
            for (int i = 0; i < numbersStrList.Length; i++)
            {
                numberList[i] = int.Parse(numbersStrList[i]);
            }
            XmlNode[] nodes = new XmlNode[height];
            for (int i = 0; i < height; i++)
            {
                nodes[i] = obstacle.Clone();
                if (isNone)
                {
                    nodes[i]["data"].InnerText = GenerateEmptyData(width, height);
                }
                else
                {
                    if (isEmptyStaticLayer(numberList, width, i))
                    {
                        nodes[i]["data"].InnerText = GenerateEmptyData(width, height);
                    }
                    else
                    {
                        nodes[i]["data"].InnerText = GenerateObstacleData(numberList, width, height, i);
                    }
                }
                
                nodes[i].Attributes["name"].Value = StaticConstant.obstaclePrefName + Convert.ToString(i);
            }
            return nodes;
        }

        public static string GenerateObstacleData(int[] numberList, int width, int height, int layerIndex)
        {
            string data = _n;
            for(int i=0; i< height; i++)
            {
                if(i == layerIndex)
                {
                    for(int k=0; k< width; k++)
                    {
                        if(i == height - 1 && k == width - 1)
                        {
                            data += Convert.ToString(numberList[i * width + k]);
                        }
                        else
                        {
                            data += Convert.ToString(numberList[i * width + k]) + ",";
                        }
                    }
                    data += _n;
                }
                else
                {
                    data = GenerateEmptyRow(width, height, i, data);
                }
            }
            return data;
        }

        static XmlNode[] GenerateStaticNodes(XmlNode staticBefore, XmlNode staticAfter, int width, int height)
        {
            string beforedata = staticBefore["data"].InnerText;
            string[] beforenumbersStrList = beforedata.Split(',');
            int[] beforenumberList = new int[beforenumbersStrList.Length];
            for(int i=0; i<beforenumbersStrList.Length; i++)
            {
                beforenumberList[i] = int.Parse(beforenumbersStrList[i]);
            }
            string afterdata = staticAfter["data"].InnerText;
            string[] afternumbersStrList = afterdata.Split(',');
            int[] afternumberList = new int[afternumbersStrList.Length];
            for (int i = 0; i < afternumbersStrList.Length; i++)
            {
                afternumberList[i] = int.Parse(afternumbersStrList[i]);
            }
            XmlNode[] nodes = new XmlNode[height];
            for (int i = 0; i < height; i++)
            {
                nodes[i] = staticBefore.Clone();
                if (isEmptyStaticLayer(beforenumberList, width, i))
                {
                    nodes[i]["data"].InnerText = GenerateEmptyData(width, height);
                }
                else
                {
                    nodes[i]["data"].InnerText = GenerateStaticData(beforenumberList, afternumberList, width, height, i);
                }
                nodes[i].Attributes["name"].Value = StaticConstant.staticPrefName + Convert.ToString(i);
            }
            return nodes;
        }


        static bool isEmptyStaticLayer(int[] numbers, int width, int layerIndex)
        {
            bool result = true;
            for (int i = layerIndex * width; i < (layerIndex + 1) * width; i++)
            {
                if (numbers[i] != 0)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        static string GenerateEmptyData(int width, int height)
        {
            string data = "";
            data += _n;
            for (int i = 0; i < height; i++)
            {
                for (int k = 0; k < width; k++)
                {
                    if (i == height - 1 && k == width - 1)
                    {
                        data += "0";
                    }
                    else
                    {
                        data += "0,";
                    }
                    
                }
                data += _n;
            }
            return data;
        }

        //after的位置在before前面
        static string GenerateStaticData(int[] beforeNumbers, int[] afterNumbers, int width, int height, int layerIndex)
        {
            string data = "";
            List<RollbackData> rollList = getContainsRollbackPropertieTiles(tileList);
            data += _n;
            for(int i=0; i< layerIndex; i++)
            {
                if(isEmptyStaticLayer(afterNumbers, width, i))
                {
                    data = GenerateEmptyRow(width, height, i, data);
                }
                else
                {
                    for(int k = 0; k< width; k++)
                    {
                        if(afterNumbers[i*width + k] != 0)
                        {
                            int number = afterNumbers[i * width + k];
                            bool isRoll = false;
                            if(RollbackInCurrentLayer(rollList, number, layerIndex, i))
                            {
                                if (i == height - 1 && k == width - 1)
                                {
                                    data += Convert.ToString(number);
                                }
                                else
                                {
                                    data += Convert.ToString(number) + ",";
                                }
                            }
                            else
                            {
                                if (i == layerIndex - 1 && !inRollbackData(rollList, number))
                                {
                                    data += Convert.ToString(number) + ",";
                                }
                                else
                                {
                                    if (i == height - 1 && k == width - 1)
                                    {
                                        data += "0";
                                    }
                                    else
                                    {
                                        data += "0,";
                                    }
                                }
                            }
                        }
                        else
                        {
                            data += "0,";
                        }
                    }
                    data += _n;
                }
            }
            for (int i = 0; i < width; i++)
            {
                if (beforeNumbers[layerIndex * width + i] != 0)
                {
                    if (layerIndex == height - 1 && i == width - 1)
                    {
                        data += Convert.ToString(beforeNumbers[layerIndex * width + i]);
                    }
                    else
                    {
                        data += Convert.ToString(beforeNumbers[layerIndex * width + i]) + ",";
                    }

                }
                else
                {
                    if (layerIndex == height - 1 && i == width - 1)
                    {
                        data += "0";
                    }
                    else
                    {
                        data += "0,";
                    }
                }
            }
            data += _n;

            data = GenerateEmptyAfter(width, height, layerIndex, data);
            return data;
        }
        static bool RollbackInCurrentLayer(List<RollbackData> rollList, int number, int layerIndex, int curIndex)
        {
            for(int i=0; i<rollList.Count; i++)
            {
                if (rollList[i].gid == number && rollList[i].rollback == layerIndex - curIndex) return true;
            }
            return false;
        }

        static bool inRollbackData(List<RollbackData> rollList, int number)
        {
            for (int i = 0; i < rollList.Count; i++)
            {
                if (rollList[i].gid == number) return true;
            }
            return false;
        }

        static string GenerateEmptyRow(int width, int height, int layerIndex, string data)
        {
            for(int i=0; i<width; i++)
            {
                if(layerIndex == height - 1 && i == width - 1)
                {
                    data += "0";
                }
                else
                {
                    data += "0,";
                }
                
            }
            data += _n;
            return data;
        }

        static string GenerateEmptyBefore(int width, int heigth, int layerIndex, string data)
        {
            if (layerIndex == 0) return data;
            data += _n;
            for (int i = 0; i < layerIndex; i++)
            {
                for (int k = 0; k < width - 1; k++)
                {
                    data += "0,";
                }
                data += _n;
            }
            return data;
        }

        static string GenerateEmptyAfter(int width, int height, int layerIndex, string data)
        {
            if (layerIndex >= height - 1) return data;
            for (int i = layerIndex + 1; i < height; i++)
            {
                for (int k = 0; k < width; k++)
                {
                    if (i == height - 1 && k == width - 1)
                    {
                        data += "0";
                    }
                    else
                    {
                        data += "0,";
                    }
                }
                data += _n;
            }
            return data;
        }

        static List<XmlNode> GenerateTileNodes(XmlNode root, int width, int height)
        {
            List<XmlNode> nodeList = new List<XmlNode>();
            XmlNodeList list = root.ChildNodes;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Name != "tileset") continue;
                if (list[i].ChildNodes.Count <= 1) continue;
                int firstGid = int.Parse(list[i].Attributes["firstgid"].Value);
                XmlNodeList tileList = list[i].ChildNodes;
                for (int k = 0; k < tileList.Count; k++)
                {
                    if (k == 0) continue;
                    XmlNode newNode = tileList[k].Clone();
                    newNode.Attributes["id"].Value = Convert.ToString(firstGid + int.Parse(newNode.Attributes["id"].Value));
                    nodeList.Add(newNode);
                }
            }
            return nodeList;
        }

        public struct RollbackData
        {
            public int gid;
            public int rollback;
        }

        static List<RollbackData> getContainsRollbackPropertieTiles(List<XmlNode> tiles)
        {
            List<RollbackData> list = new List<RollbackData>();
            for (int i = 0; i < tiles.Count; i++)
            {
                XmlNodeList nodeList = tiles[i].ChildNodes;
                for (int k = 0; k < nodeList.Count; k++)
                {
                    if (nodeList[k].Name == "properties")
                    {
                        XmlNodeList properties = nodeList[k].ChildNodes;
                        for (int n = 0; n < properties.Count; n++)
                        {
                            if (properties[n].Name == "property")
                            {
                                RollbackData roll = new RollbackData();
                                roll.gid = int.Parse(tiles[i].Attributes["id"].Value);
                                roll.rollback = int.Parse(properties[n].Attributes["value"].Value);
                                list.Add(roll);
                            }
                        }
                    }
                }
            }
            return list;
        }
    }
}
