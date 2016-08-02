using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using proto.serverproto;
using System;

//一切判断以玩家的用户体验为先
public class PlayerInputController : MonoBehaviour {

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
    public PlayerDirection curDirection;
    public PlayerState state;
    private class TouchInfo
    {
        public int touchId;
        public Vector2 point;
        public float touchTime;
        public float triggerTime;
    }

    private List<TouchInfo> touchIds;
    //private ETCJoystick joystickStatic;
    //private float joystickPercent;

    void Awake()
    {
        Application.targetFrameRate = 25;

    }

    void Start () {
        PlayerNetWorkData data = GameGlobalData.playerList[GameGlobalData.fd];
        float posx = data.initPosition.x;
        float posy = data.initPosition.y;
        int row = (int)Mathf.Abs(posy / GameConst.tileHeight);
        float posz = GameConst.staticLayers[row].position.z;
        transform.position = new Vector3(posx, posy, posz);
        behaviorCollector = GetComponent<PlayerBehaviourCollector>();
        animatorController = GetComponent<PlayerAnimationController>();
        GameGlobalData.player = this;
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
        curDirection = PlayerDirection.Down;
        state = PlayerState.Movement;
        touchIds = new List<TouchInfo>();
        
        //switch (Application.platform)
        //{
        //    case RuntimePlatform.Android:
        //    case RuntimePlatform.IPhonePlayer:
        //        GameObject.Find("EasyTouchControlsCanvas/Joystick").SetActive(false);
        //        break;
        //    case RuntimePlatform.WindowsEditor:
        //    case RuntimePlatform.WindowsPlayer:
        //        joystickDynamic = GameObject.Find("EasyTouchControlsCanvas/Joystick").GetComponent<ETCJoystick>();
        //        break;
        //}

        //joystickStatic = GameObject.Find("EasyTouchControlsCanvas/Joystick2").GetComponent<ETCJoystick>();
    }

    bool cantainsIntList(int id)
    {
        foreach (TouchInfo touchInfo in touchIds)
        {
            if (touchInfo.touchId == id)
            {
                return true;
            }
        }
        return false;
    }

    void removeId(int id)
    {
        foreach (TouchInfo touchInfo in touchIds)
        {
            if (touchInfo.touchId == id)
            {
                touchIds.Remove(touchInfo);
                return;
            }
        }
    }

    void addId(Gesture gesture)
    {
        TouchInfo info = new TouchInfo();
        info.touchId = gesture.fingerIndex;
        info.point = gesture.position;
        info.touchTime = 0f;
        info.triggerTime = 0f;
        touchIds.Add(info);
    }

    int touchCount()
    {
        return touchIds.Count;
    }

    void resetAllTouch()
    {
        foreach (TouchInfo touchInfo in touchIds)
        {
            touchInfo.touchTime = 0f;
            touchInfo.triggerTime = 0f;
        }
    }

    void updateId(Gesture gesture)
    {
        foreach (TouchInfo touchInfo in touchIds)
        {
            if (touchInfo.touchId == gesture.fingerIndex)
            {
                touchInfo.point = gesture.position;
                touchInfo.touchTime += Time.deltaTime;
                touchInfo.triggerTime += Time.deltaTime;
            }
        }
    }

    TouchInfo getTouchInfo(int id)
    {
        foreach (TouchInfo touchInfo in touchIds)
        {
            if (touchInfo.touchId == id)
            {
                return touchInfo;
            }
        }
        return null;
    }

    void Update()
    {
        if (!startAccept) return;
        animatorController.AcceptInput(curDirection, state);
        movement.Movement(state, curDirection);
    }

    void resetDirectionRight()
    {
        switch (curDirection) {
            case PlayerDirection.Down:
                curDirection = PlayerDirection.Left;
                break;
            case PlayerDirection.Right:
                curDirection = PlayerDirection.Down;
                break;
            case PlayerDirection.Up:
                curDirection = PlayerDirection.Right;
                break;
            case PlayerDirection.Left:
                curDirection = PlayerDirection.Up;
                break;
        }
    }

    void resetDirectionLeft()
    {
        switch (curDirection)
        {
            case PlayerDirection.Down:
                curDirection = PlayerDirection.Right;
                break;
            case PlayerDirection.Right:
                curDirection = PlayerDirection.Up;
                break;
            case PlayerDirection.Up:
                curDirection = PlayerDirection.Left;
                break;
            case PlayerDirection.Left:
                curDirection = PlayerDirection.Down;
                break;
        }
    }

    bool isLeft(Vector2 position)
    {
        if(position.x < Screen.width / 2)
        {
            return true;
        }
        return false;
    }

    private void On_TouchStart(Gesture gesture)
    {
        if (!cantainsIntList(gesture.fingerIndex))
        {
            addId(gesture);
        }
        if (touchCount() > 1)
        {
            state = PlayerState.Idle;
        }
        else
        {
            state = PlayerState.Movement;
            if (isLeft(gesture.position))
            {
                resetDirectionLeft();
            }
            else
            {
                resetDirectionRight();
            }
        }
    }

    // During the touch is down
    private void On_TouchDown(Gesture gesture)
    {
        if(touchCount() > 1)
        {
            resetAllTouch();
            state = PlayerState.Idle;
        }
        else
        {
            state = PlayerState.Movement;
            updateId(gesture);
            if (getTouchInfo(gesture.fingerIndex).touchTime >= 0.35f)
            {
                if(getTouchInfo(gesture.fingerIndex).triggerTime >= 0.1f)
                {
                    if (isLeft(gesture.position))
                    {
                        resetDirectionLeft();
                    }
                    else
                    {
                        resetDirectionRight();
                    }
                }
            }
        }
    }

    // At the touch end
    private void On_TouchUp(Gesture gesture)
    {
        removeId(gesture.fingerIndex);
        if(touchCount() > 1)
        {
            state = PlayerState.Idle;
        }
        else
        {
            state = PlayerState.Movement;
        }
    }

    void OnEnable()
    {
        EasyTouch.On_TouchStart += On_TouchStart;
        EasyTouch.On_TouchDown += On_TouchDown;
        EasyTouch.On_TouchUp += On_TouchUp;
    }

    void OnDisable()
    {
        UnsubscribeEvent();
    }

    void OnDestroy()
    {
        UnsubscribeEvent();
    }

    void UnsubscribeEvent()
    {
        EasyTouch.On_TouchStart -= On_TouchStart;
        EasyTouch.On_TouchDown -= On_TouchDown;
        EasyTouch.On_TouchUp -= On_TouchUp;
    }

    //void Update () {
    //       if (!startAccept) return;
    //       bool attack = Input.GetButtonDown("Jump");
    //       if (!attack) attack = Input.GetButtonDown("Fire1");
    //       if (!attack) attack = ETCInput.GetButton("Boom");
    //       bool save = Input.GetButtonDown("Fire2");
    //       if(save && CanBeSave)
    //       {
    //           SaveSelf();
    //       }
    //       if (attack && CanBubble)
    //       {
    //           behaviorCollector.SendBubbleAction();
    //       }
    //       switch (Application.platform)
    //       {
    //           case RuntimePlatform.WindowsEditor:
    //           case RuntimePlatform.WindowsPlayer:
    //               float h = Input.GetAxis("Horizontal");
    //               float v = Input.GetAxis("Vertical");
    //               if(Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f)
    //               {
    //                   if(Mathf.Abs(h) > Mathf.Abs(v))
    //                   {
    //                       if(h > 0f)
    //                       {
    //                           h = 1f;
    //                           v = 0f;
    //                       }
    //                       else
    //                       {
    //                           h = -1f;
    //                           v = 0f;
    //                       }
    //                   }
    //                   else
    //                   {
    //                       if(v > 0f)
    //                       {
    //                           v = 1f;
    //                           h = 0f;
    //                       }
    //                       else
    //                       {
    //                           v = -1f;
    //                           h = 0f;
    //                       }
    //                   }
    //               }
    //               setPlayerStateWithDirection(h, v);
    //               break;
    //           case RuntimePlatform.Android:
    //           case RuntimePlatform.IPhonePlayer:
    //               setPlayerStateWithDirectionAcceleration();
    //               break;
    //       }
    //       //setPlayerStateWithDirection();
    //   }

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
            if(Mathf.Abs(acceleration.x) >= 0.065f)
            {
                state = PlayerState.Movement;
                if (acceleration.x < 0f)
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
                state = PlayerState.Idle;
            }
            
        }
        else
        {
            if(Mathf.Abs(acceleration.y) >= 0.065f)
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
            else
            {
                state = PlayerState.Idle;
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
        //joystickStatic.updateSimulation(h, v);
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

    public void GlobalAttack(int row, int col)
    {
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
        movement.removeRelaBubble(bubble.row, bubble.col);
        GameConst.bubbles[bubble.row, bubble.col] = null;
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
