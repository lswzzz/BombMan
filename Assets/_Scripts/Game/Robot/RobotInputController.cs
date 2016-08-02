using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using proto.serverproto;

public class RobotInputController : MonoBehaviour
{
    //在RobotGenerator中生成
    public int fd;
    private List<Bubble> bubbles;
    private PlayerAnimationController animatorController;
    private RobotMovement movement;
    private int bubbleCount;
    private int maxBubbleCount;
    private int power;
    private int maxPower;
    private float boomTime;
    private PlayerColliderBox colliderBox;

    private float bubbleDelayTime;

    public class BubbleData {
        public int row;
        public int col;
        public int power;
        public float delayTime;
    }

    public List<BubbleData> preBubbleList;
    void Start()
    {
        bubbleDelayTime = 0.2f;
        animatorController = GetComponent<PlayerAnimationController>();
        PlayerNetWorkData data = GameGlobalData.playerList[fd];
        float posx = data.initPosition.x;
        float posy = data.initPosition.y;
        int row = (int)Mathf.Abs(posy / GameConst.tileHeight);
        float posz = GameConst.staticLayers[row].position.z;
        transform.position = new Vector3(posx, posy, posz);
        GameGlobalData.robotList.Add(fd, this);
        movement = GetComponent<RobotMovement>();
        movement.speed = data.speed;
        movement.maxSpeed = data.maxSpeed;
        movement.maxDistance = data.maxDistance;
        bubbleCount = data.bubbleCount;
        maxBubbleCount = data.maxBubbleCount;
        power = data.power;
        maxPower = data.maxPower;
        boomTime = data.boomTime;
        bubbles = new List<Bubble>();
        colliderBox = GetComponent<PlayerColliderBox>();
        preBubbleList = new List<BubbleData>();
    }

    //为了放炸弹的时候看起来很平滑，需要根据当前的位置来决定是否放炸弹如果位置不合适但是超时了也可以放炸弹
    void Update()
    {
        bool removeBubble = false;
        if(preBubbleList.Count > 0)
        {
            foreach(BubbleData data in preBubbleList)
            {
                data.delayTime += Time.deltaTime;
            }
            Vector2 center = colliderBox.Center;
            BubbleData bubble = preBubbleList[0];
            float posy = -bubble.row * GameConst.tileHeight;
            float posx = bubble.col * GameConst.tileWidth;
            switch (animatorController.Direction) {
                case PlayerDirection.Left:
                    if (center.x <= posx) removeBubble = true;
                    break;
                case PlayerDirection.Right:
                    if (center.x >= posx) removeBubble = true;
                    break;
                case PlayerDirection.Down:
                    if (center.y < posy) removeBubble = true;
                    break;
                case PlayerDirection.Up:
                    if (center.y > posy) removeBubble = true;
                    break;
            }
            if (bubble.delayTime > bubbleDelayTime) removeBubble = true;
        }
        if (removeBubble)
        {
            BubbleData bubble = preBubbleList[0];
            RobotAttack(bubble.row, bubble.col, bubble.power);
            preBubbleList.RemoveAt(0);
        }
    }

    void FixedUpdate()
    {

    }

    void addPreBubbleData(int row, int col, int power)
    {
        BubbleData data = new BubbleData();
        data.row = row;
        data.col = col;
        data.power = power;
        data.delayTime = 0f;
        preBubbleList.Add(data);
    }

    void RobotAttack(int row, int col, int power)
    {
        GameObject obj = Instantiate(Resources.Load("Prefabs/bubble") as GameObject);
        Bubble bubble = obj.GetComponent<Bubble>();
        Vector3 position = new Vector3(col * GameConst.tileWidth + GameConst.tileWidth / 2,
            -row * GameConst.tileHeight - GameConst.tileHeight / 2, transform.position.z);
        bubble.transform.position = position;
        bubble.power = power;
        bubble.maxPower = maxPower;
        bubble.col = col;
        bubble.row = row;
        bubble.time = boomTime;
        bubble.Player = this.transform;
        bubble.resetBox();
        bubbles.Add(bubble);
        if (GameConst.bubbles[row, col] != null)
        {
            Debug.Log("ERROR");
        }
        else
        {
            GameConst.bubbles[row, col] = bubble;
            AllPlayerFunctions.SetInBubble(row, col);
        }
    }

    public void clearBubble()
    {
        bubbles.Clear();
    }

    public void removeBubble(Bubble bubble)
    {
        bubbles.Remove(bubble);
    }

    public bool CanBubble
    {
        get
        {
            Vector3 center = colliderBox.Center;
            if (bubbles.Count >= maxBubbleCount) return false;
            int col = (int)center.x / GameConst.tileWidth;
            int row = (int)-center.y / GameConst.tileHeight;
            if (GameConst.bubbles[row, col] == null) return true;
            return false;
        }
    }

    public void ReceiveTransform(ReceiveTransformData transformdata)
    {
        movement.target(new Vector2(transformdata.posx, transformdata.posy), (PlayerDirection)transformdata.direction, (PlayerState)transformdata.state);
    }

    public void ReceiveAction(ReceiveActionData actionData)
    {
        switch ((ReceiveActionData.ActionType)actionData.action) {
            case ReceiveActionData.ActionType.Bubble:
                addPreBubbleData(actionData.bubble.row, actionData.bubble.col, actionData.bubble.power);
                break;
        }

    }

    public void ReceiveState(ReceiveStateData stateData)
    {
        switch ((PlayerState)stateData.state) {
            case PlayerState.Trap:
                if(stateData.trapData.trapLevel == (char)TrapLevel.Trap1)
                {
                    animatorController.AcceptSpecialState(PlayerDirection.Down, PlayerState.Trap);
                }
                break;
            case PlayerState.Save:
                animatorController.AcceptSpecialState(PlayerDirection.Down, PlayerState.Save);
                break;
            case PlayerState.Death:
                animatorController.AcceptSpecialState(PlayerDirection.Down, PlayerState.Death);
                break;
        }

    }
}
