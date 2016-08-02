using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneInit : MonoBehaviour {

    private Tiled2Unity.TiledMap tileMap;
	// Use this for initialization
	void Start () {
        tileMap = GetComponent<Tiled2Unity.TiledMap>();
        GameConst.obstacleLayers = new List<Transform>();
        GameConst.staticLayers = new List<Transform>();
        GameConst.playerStarts = new List<Transform>();
        GameConst.monsterStarts = new List<Transform>();
        foreach (Transform transChild in transform)
        {
            if (transChild.name.StartsWith("obstacle"))
            {
                GameConst.obstacleLayers.Add(transChild);
            }
            else if (transChild.name.StartsWith("static"))
            {
                GameConst.staticLayers.Add(transChild);
            }
            else if(transChild.name == "playerLayer")
            {
                setPlayerStarts(transChild);
            }
            else if(transChild.name == "monsterLayer")
            {
                setMosnterStarts(transChild);
            }
        }
        GameConst.width = tileMap.NumTilesWide;
        GameConst.height = tileMap.NumTilesHigh;
        GameConst.tileHeight = tileMap.TileHeight;
        GameConst.tileWidth = tileMap.TileWidth;
        initSceneObstacles();
        GameConst.bubbles = new Bubble[GameConst.height, GameConst.width];
        GameConst.waterColumns = new WaterColumn[GameConst.height, GameConst.width];
        GameConst.items = new Items[GameConst.height, GameConst.width];
        RobotGenerator.GeneratedRobot();
    }
	
	void setPlayerStarts(Transform node)
    {
        foreach(Transform child in node)
        {
            GameConst.playerStarts.Add(child);
        }
    }

    void setMosnterStarts(Transform node)
    {
        foreach(Transform  child in node){
            GameConst.monsterStarts.Add(child);
        }
    }

    void initSceneObstacles()
    {
        GameConst.sceneObstacles = new AABBox[GameConst.height, GameConst.width];
        foreach(Transform obstacleLayer in GameConst.obstacleLayers)
        {
            foreach(Transform obstacle in obstacleLayer)
            {
                AABBox box = obstacle.gameObject.GetComponent<AABBox>();
                int col = (int)box.Center.x / GameConst.tileWidth;
                int row = (int)-box.Center.y / GameConst.tileHeight;
                GameConst.sceneObstacles[row, col] = box;
            }
        }

        foreach(Transform staticLayer in GameConst.staticLayers)
        {
            foreach(Transform staticOb in staticLayer)
            {
                if(staticOb.name == "Collision")
                {
                    AABBox[] aabboxs = staticOb.gameObject.GetComponents<AABBox>();
                    foreach(AABBox box in aabboxs)
                    {
                        int col = (int)box.Center.x / GameConst.tileWidth;
                        int row = (int)-box.Center.y / GameConst.tileHeight;
                        GameConst.sceneObstacles[row, col] = box;
                    }
                }
            }
        }
    }

    void Destroy()
    {
        
    }

}
