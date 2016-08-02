using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using proto.serverproto;
using System;

//一切判断以玩家的用户体验为先
public class PlayerInputController2 : MonoBehaviour {

    private PlayerAnimationController animatorController;
    private PlayerBehaviourCollector behaviorCollector;
    private PlayerMovement movement;
    private List<Bubble> bubbles;
    public int power;
    public int maxPower;
    public int bubbleCount;
    public int maxBubbleCount;
    public float boomTime;
    public bool startAccept;
    private PlayerColliderBox colliderBox;
    private ETCJoystick joystickDynamic;
    private ETCJoystick joystickStatic;
    private float joystickPercent;
  
    void Start () {
        PlayerNetWorkData data = GameGlobalData.playerList[GameGlobalData.fd];
        float posx = data.initPosition.x;
        float posy = data.initPosition.y;
        int row = (int)Mathf.Abs(posy / GameConst.tileHeight);
        float posz = GameConst.staticLayers[row].position.z;
        transform.position = new Vector3(posx, posy, posz);
        behaviorCollector = GetComponent<PlayerBehaviourCollector>();
        animatorController = GetComponent<PlayerAnimationController>();
        //GameGlobalData.player = this;
        movement = GetComponent<PlayerMovement>();
        movement.speed = data.speed;
        movement.maxSpeed = data.maxSpeed;
        bubbleCount = data.bubbleCount;
        maxBubbleCount = data.maxBubbleCount;
        power = data.power;
        maxPower = data.maxPower;
        boomTime = data.boomTime;
        bubbles = new List<Bubble>();
        startAccept = false;
        colliderBox = GetComponent<PlayerColliderBox>();
        //joystickDynamic = GameObject.Find("EasyTouchControlsCanvas/Joystick").GetComponent<ETCJoystick>();
        //joystickStatic = GameObject.Find("EasyTouchControlsCanvas/Joystick2").GetComponent<ETCJoystick>();
    }
	
	void Update () {
        if (!startAccept) return;
        float h = ETCInput.GetAxis("Horizontal");
        float v = ETCInput.GetAxis("Vertical");
        bool attack = Input.GetButtonDown("Jump");
        if (!attack) attack = Input.GetButtonDown("Fire1");
        if (!attack) attack = ETCInput.GetButton("Center");
        bool save = Input.GetButtonDown("Fire2");
        if(save && CanBeSave)
        {
            SaveSelf();
        }
        if (attack && CanBubble)
        {
            behaviorCollector.SendBubbleAction();
        }
        //setPlayerStateWithDirectionAcceleration();
        setPlayerStateWithDirection();
        //setPlayerStateWithDirection(h, v);

    }

    public PlayerDirection getDirection()
    {
        return animatorController.Direction;
    }

    public Vector2 getPosition()
    {
        return transform.position;
    }

    //穿入的h 或者 v其中一个不能为空
    PlayerDirection getDirection(float h, float v)
    {
        Vector2 direct = new Vector2(h, v);
        direct.Normalize();
        float angle = Mathf.Rad2Deg * Mathf.Atan2(direct.y, direct.x);
        if (angle >= -55f && angle < 35f)
        {
            //right
            return PlayerDirection.Right;
        }
        else if (angle >= 35f && angle < 145f)
        {
            //up
            return PlayerDirection.Up;
        }
        else if (angle < -55f && angle >= -145f)
        {
            //down
            return PlayerDirection.Down;
        }
        else
        {
            //left
            return PlayerDirection.Left;
        }
    }

    void setPlayerStateWithDirectionAcceleration()
    {
        Vector2 acceleration = Input.acceleration;
        PlayerDirection direction = animatorController.Direction;
        PlayerState state = animatorController.State;
        if (ETCInput.GetButton("Stop"))
        {
            state = PlayerState.Idle;
        }
        else if (Mathf.Abs(acceleration.x ) >= Mathf.Abs(acceleration.y))
        {
            state = PlayerState.Movement;
            if(acceleration.x < 0f)
            {
                direction = PlayerDirection.Left;
            }
            else
            {
                direction = PlayerDirection.Right;
            }
        }
        else
        {
            state = PlayerState.Movement;
            if (acceleration.y < 0f)
            {
                direction = PlayerDirection.Down;
            }
            else
            {
                direction = PlayerDirection.Up;
            }
        }
        animatorController.AcceptInput(direction, state);
        movement.Movement(state, direction);
    }

    void setPlayerStateWithDirection()
    {
        PlayerDirection direction = animatorController.Direction;
        PlayerState state = animatorController.State;
        if(ETCInput.GetAxis("Vertical") == -1f && ETCInput.GetAxis("Horizontal") == 0f)
        {
            direction = PlayerDirection.Down;
            state = PlayerState.Movement;
        }
        else if (ETCInput.GetAxis("Vertical") == 1f && ETCInput.GetAxis("Horizontal") == 0f)
        {
            direction = PlayerDirection.Up;
            state = PlayerState.Movement;
        }
        else if (ETCInput.GetAxis("Vertical") == 0f && ETCInput.GetAxis("Horizontal") == -1f)
        {
            direction = PlayerDirection.Left;
            state = PlayerState.Movement;
        }
        else if (ETCInput.GetAxis("Vertical") == 0f && ETCInput.GetAxis("Horizontal") == 1f)
        {
            direction = PlayerDirection.Right;
            state = PlayerState.Movement;
        }
        else
        {
            state = PlayerState.Idle;
        }
        animatorController.AcceptInput(direction, state);
        movement.Movement(state, direction);
    }

    void setPlayerStateWithDirection(float h, float v)
    {
        PlayerDirection direction = animatorController.Direction;
        PlayerState state = animatorController.State;
        if(h == -1f && v == 0f)
        {
            state = PlayerState.Movement;
            direction = PlayerDirection.Left;
        }
        else if(h == 1f && v == 0f)
        {
            state = PlayerState.Movement;
            direction = PlayerDirection.Right;
        }
        else if(h == 0f && v == 1f)
        {
            state = PlayerState.Movement;
            direction = PlayerDirection.Up;
        }
        else if(h == 0f && v == -1f)
        {
            state = PlayerState.Movement;
            direction = PlayerDirection.Down;
        }
        else
        {
            state = PlayerState.Idle;
        }
        animatorController.AcceptInput(direction, state);
        movement.Movement(state, direction);
        joystickStatic.updateSimulation(h, v);
    }

    PlayerDirection getTranDirection(PlayerDirection direction)
    {
        switch (direction) {
            case PlayerDirection.Down:
                return PlayerDirection.Up;
            case PlayerDirection.Up:
                return PlayerDirection.Down;
            case PlayerDirection.Left:
                return PlayerDirection.Right;
            case PlayerDirection.Right:
                return PlayerDirection.Left;
        }
        return direction;
    }

    public void PlayerAttack(int row, int col, int power)
    {
		if (GameConst.bubbles[row, col] != null)
        {
            Debug.Log("当前格子已有泡泡");
            return;
        }
        GameObject obj = Instantiate(Resources.Load("Prefabs/bubble") as GameObject);
        Bubble bubble = obj.GetComponent<Bubble>();
        Vector3 position = new Vector3(col * GameConst.tileWidth + GameConst.tileWidth / 2,
            -row * GameConst.tileHeight - GameConst.tileHeight / 2, transform.position.z + 0.001f);
        bubble.transform.position = position;
        bubble.power = power;
        bubble.maxPower = maxPower;
        bubble.col = col;
        bubble.row = row;
        bubble.time = boomTime;
        bubble.Player = this.transform;
        bubble.resetBox();
        bubbles.Add(bubble);
        GameConst.bubbles[row, col] = bubble;
        AllPlayerFunctions.SetInBubble(row, col);
    }

    //这个状态是玩家自身的explore判断的
    public void BeAttack()
    {
        Debug.Log("BeAttack");
        animatorController.AcceptSpecialState(PlayerDirection.Down, PlayerState.Trap);
        behaviorCollector.SendTrapState(TrapLevel.Trap1);
    }

    //save 和death区分为自身的状态变换和被别人操控的状态
    //当一个playerInput执行SaveOne或者KillOne的时候数据发送到对应的客户端，这个客户端
    //的playerInput就执行BeSave或者 beDeath的操作并将自身的save或者death状态发送给服务器
    //服务器接受到后就转发给其他客户端
    public void BeSave()
    {
        Debug.Log("BeSave");
        animatorController.AcceptSpecialState(PlayerDirection.Down, PlayerState.Save);
        behaviorCollector.SendSaveState();
    }

    public void BeDeath()
    {
        Debug.Log("BeDeath");
        animatorController.AcceptSpecialState(PlayerDirection.Down, PlayerState.Death);
        behaviorCollector.SendDeathState();
    }

    public void SaveSelf()
    {
        animatorController.AcceptSpecialState(PlayerDirection.Down, PlayerState.Save);
        behaviorCollector.SendSaveState();
    }

    public void clearBubble()
    {
        bubbles.Clear();
    }

    public void removeBubble(Bubble bubble)
    {
        bubbles.Remove(bubble);
        GameConst.bubbles[bubble.row, bubble.row] = null;
    }

    public bool CanBubble
    {
        get {
            Vector3 center = colliderBox.Center;
            if (bubbles.Count >= maxBubbleCount) return false;
            int col = (int)center.x / GameConst.tileWidth;
            int row = (int)-center.y / GameConst.tileHeight;
            if (movement.ContainsInline()) return false;
            if (GameConst.bubbles[row, col] == null) return true;
            return false;
        }
    }

    public bool CanBeAttack {
        get
        {
            return animatorController.CanTrap;
        }
    }

    public bool CanBeSave {
        get
        {
            return animatorController.CanSave;
        }
    }

    public bool CanExploreItem {
        get
        {
            if (animatorController.State == PlayerState.Idle || animatorController.State == PlayerState.Movement) return true;
            return false;
        }
    }

    public void ExploreItems(int row, int col)
    {

    }

    public void SaveOne(RobotInputController robot)
    {
        behaviorCollector.SendSaveOneAction(robot.fd);
    }

    public void KillOne(RobotInputController robot)
    {
        behaviorCollector.SendKillOneAction(robot.fd);
    }

}
