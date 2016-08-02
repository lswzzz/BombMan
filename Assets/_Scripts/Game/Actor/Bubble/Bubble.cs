using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bubble : MonoBehaviour {

    private Animator animator;
    public float time = 5f;
    public bool trigger = false;

    public int power;
    public int maxPower;
    public int row;
    public int col;
    public Transform Player;
    private bool canWall;
    public class PreWaterNode {
        public WaterColumn.WaterType type;
        public int row;
        public int col;
        public bool isObstacle;
        public bool isBubble;
    }

    //最大的tick时间由服务器决定，当服务器收到的那一刻开始服务器就开始模拟bubble爆炸，那么客户端为了和服务器保持大约一致的爆炸时间
    //就应该以服务器收到的时间为准
    void Start () {
        canWall = false;
        animator = GetComponent<Animator>();
        time -= TimeUtils.UdpSingleDelay/2;
    }

    public void resetBox()
    {
        BubbleOutlineBox box1 = GetComponent<BubbleOutlineBox>();
        BubbleInlineBox box2 = GetComponent<BubbleInlineBox>();
        box1.resetBox();
        box2.resetBox();
    }

	void Update () {
        time -= Time.deltaTime;
        if(time <= 0f && !trigger)
        {
            trigger = true;
            animator.SetTrigger("Boom");
            createWaterColumns();
        }
	}
	
    void ClearPlayerBubble()
    {
        if (Player.GetComponent<PlayerInputController>() != null)
        {
            Player.GetComponent<PlayerInputController>().removeBubble(this);
        }
        if (Player.GetComponent<RobotInputController>() != null)
        {
            Player.GetComponent<RobotInputController>().removeBubble(this);
        }
    }

    public void createWaterColumns()
    {
        List<Bubble> bubbles = new List<Bubble>();
        List<Block> blocks = new List<Block>();
        PreWaterNode[,] vector = new PreWaterNode[GameConst.height, GameConst.width];
        PreWaterNode node = new PreWaterNode();
        node.row = row;
        node.col = col;
        node.type = WaterColumn.WaterType.Center;
        node.isObstacle = false;
        node.isBubble = true;
        vector[row, col] = node;
        find(ref vector);
        for(int i=0; i< GameConst.height; i++)
        {
            for (int k = 0; k < GameConst.width; k++)
            {
                if(vector[i, k] != null)
                {
                    PreWaterNode node_ = vector[i, k];
                    if (node_.isObstacle)
                    {
                        Block block = GameConst.sceneObstacles[i, k].GetComponent<Block>();
                        if(block != null)
                        {
                            blocks.Add(block);
                        }
                    }else if (node_.isBubble)
                    {
                        Bubble bubble = GameConst.bubbles[i, k];
                        bubble.trigger = true;
                        bubbles.Add(GameConst.bubbles[i, k]);
                        CreateWaterColumn(i, k, WaterColumn.WaterType.Center);
                    }
                    else
                    {

                        CreateWaterColumn(i, k, node_.type);
                    }
                }
            }
        }
        foreach(Block block in blocks)
        {
            block.BeDestory();
        }
        foreach(Bubble bubble in bubbles)
        {
            bubble.BeDestory();
        }
    }

    public void CreateWaterColumn(int row, int col, WaterColumn.WaterType type)
    {
        GameObject obj = Instantiate(Resources.Load("Prefabs/waterColumn") as GameObject);
        
        Vector3 position = new Vector3(col * GameConst.tileWidth + GameConst.tileWidth / 2,
                                    -(row * GameConst.tileHeight + GameConst.tileHeight / 2),
                                    transform.position.z);
        obj.transform.position = position;
        WaterColumn waterColumn = obj.gameObject.GetComponent<WaterColumn>();
        waterColumn.setType(type);
        waterColumn.col = col;
        waterColumn.row = row;
        GameConst.waterColumns[row, col] = waterColumn;
    }

    public void find(ref PreWaterNode[,] vector)
    {
        for(int i=0; i<4; i++)
        {
            //上下左右
            switch (i) {
                case 0:
                    findUp(ref vector, row, col);
                    break;
                case 1:
                    findRight(ref vector, row, col);
                    break;
                case 2:
                    findDown(ref vector, row, col);
                    break;
                case 3:
                    findLeft(ref vector, row, col);
                    break;
            }
        }
    }

    public void findUp(ref PreWaterNode[,] vector, int row, int col)
    {
        FindLoop(ref vector, row, col, WaterColumn.WaterType.UpMid, WaterColumn.WaterType.UpTop, 0);
    }

    public void findRight(ref PreWaterNode[,] vector, int row, int col)
    {
        FindLoop(ref vector, row, col, WaterColumn.WaterType.RightMid, WaterColumn.WaterType.RightTop, 1);
    }

    public void findDown(ref PreWaterNode[,] vector, int row, int col)
    {
        FindLoop(ref vector, row, col, WaterColumn.WaterType.DownMid, WaterColumn.WaterType.DownTop, 2);
    }

    public void findLeft(ref PreWaterNode[,] vector, int row, int col)
    {
        FindLoop(ref vector, row, col, WaterColumn.WaterType.LeftMid, WaterColumn.WaterType.LeftTop, 3);
    }

    public void FindLoop(ref PreWaterNode[,] vector, int row, int col, WaterColumn.WaterType type1, WaterColumn.WaterType type2, int type)
    {
        int index = 0;
        int col_ = col;
        int row_ = row;
        while (index < power)
        {
            if(type == 0)
            {
                row_ -= 1;
                if (row_ < 0) break;
            }
            else if(type == 1)
            {
                col_ += 1;
                if (col_ >= GameConst.width) break;
            }
            else if(type == 2)
            {
                row_ += 1;
                if (row_ >= GameConst.height) break;
            }
            else
            {
                col_ -= 1;
                if (col_ < 0) break;
            }
            PreWaterNode node = vector[row_, col_];
            if (node == null)
            {
                node = new PreWaterNode();
                node.row = row_;
                node.col = col_;
                //mid
                if (index < power - 1)
                {
                    node.type = type1;
                }
                //end
                else
                {
                    node.type = type2;
                }
                vector[row_, col_] = node;
                if(GameConst.sceneObstacles[row_, col_] != null)
                {
                    node.isObstacle = true;
                    node.isBubble = false;
                }else if(GameConst.bubbles[row_, col_] != null)
                {
                    node.isObstacle = false;
                    node.isBubble = true;
                    Bubble bubble = GameConst.bubbles[row_, col_];
                    node.type = WaterColumn.WaterType.Center;
                    bubble.find(ref vector);
                }
                else
                {
                    node.isObstacle = false;
                    node.isBubble = false;
                }
            }
            else
            {
                if (index < power - 1)
                {
                    node.type = type1;
                }
            }
            
            if (GameConst.sceneObstacles[row_, col_] != null)
            {
                Block block = GameConst.sceneObstacles[row_, col_].GetComponent<Block>();
                if (block == null) break;
                else if (!canWall) break;
            }
            index++;
        }
    }

    public void BeDestory()
    {
        ClearPlayerBubble();
        GameConst.bubbles[row, col] = null;
        Destroy(gameObject);
    }
}
