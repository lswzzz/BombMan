using UnityEngine;
using System.Collections;

namespace BombEditor
{
    public static class StaticConstant
    {
        public static string staticLayerName = "staticlayerbefore";
        public static string staticAfterLayerName = "staticlayerafter";
        public static string obstacleLayername = "obstacle";
        public static string playerLayer = "playerLayer";
        public static string monsterLayer = "monsterLayer";
        public static string backgroundName = "background";
        public static string staticPrefName = "staticRow";
        public static string obstaclePrefName = "obstacleRow";
        public static string prefabsPath = Application.dataPath + "/Tiled2Unity/Prefabs/";
    }
}
