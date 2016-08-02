using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameConst {

    public static List<Transform> obstacleLayers;
    public static List<Transform> staticLayers;
    public static List<Transform> playerStarts;
    public static List<Transform> monsterStarts;
    //行然后列
    public static AABBox[,] sceneObstacles;
    public static Bubble[,] bubbles;
    public static WaterColumn[,] waterColumns;
    public static Items[,] items;
    public static int width;
    public static int height;
    public static int tileWidth;
    public static int tileHeight;

    public static bool hasObstacleOrBubble(int row, int col)
    {
        if (sceneObstacles[row, col] != null || bubbles[row, col] != null) return true;
        return false;
    }

    public static int getRow(float posy)
    {
        return (int)-posy / GameConst.tileHeight;
    }

    public static int getCol(float posx)
    {
        return (int)posx / GameConst.tileWidth;
    }

    public static void OnApplicationQuit()
    {
        if (ConnectSocket.getSocketInstance().isConnect())
        {
            ConnectSocket.getSocketInstance().Close();
        }
        if (UDPSocket.Instance.isInit)
        {
            UDPSocket.Instance.Close();
        }
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
