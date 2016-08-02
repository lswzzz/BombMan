using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterAStar : MonoBehaviour {

    public List<AStarNode> pathList;
    public MonsterBehaviorTree behaviorTree;
    public class AStarNode {
        int row;
        int col;
    }

	// Use this for initialization
	void Start () {
        pathList = new List<AStarNode>();
        behaviorTree = GetComponent<MonsterBehaviorTree>();
	}
	
    void resetPathList()
    {
        pathList = new List<AStarNode>();
    }

	// Update is called once per frame
	void Update () {
	    
	}

    bool TargetCanArrive()
    {
        return false;
    }

    public List<AStarNode> getPathList()
    {
        return pathList;
    }
}
