using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour {

    protected PlayerBoxUpdate boxUpdate;
    public float speed;
    public float maxSpeed;
    protected PlayerColliderBox colliderBox;
    protected PlayerBeHurtBox behurtBox;
    public PlayerState state;
    public PlayerDirection direction;
    private float distance = 5;
    protected PlayerBoxUpdate.PopDirection popDirection = PlayerBoxUpdate.PopDirection.HORIZONTAL;
    protected PlayerAnimationController animatoController;
    
    void Start () {
        
        boxUpdate = new PlayerBoxUpdate();
        boxUpdate.Init(transform, GetComponent<PlayerColliderBox>(), GetComponent<PlayerBeHurtBox>(), GetComponent<PlayerExploreBox>());
        colliderBox = GetComponent<PlayerColliderBox>();
        behurtBox = GetComponent<PlayerBeHurtBox>();
        animatoController = GetComponent<PlayerAnimationController>();
        resetPlayerZ();
    }

    public void removeRelaBubble(int row, int col)
    {
        int key = row * GameConst.width + col;
        boxUpdate.removeKey(key);
    }

    public bool ContainsInline()
    {
        return boxUpdate.ContainsInline();
    }
   
    public void Movement(PlayerState state, PlayerDirection direction)
    {
        this.state = state;
        this.direction = direction;
    }

    //客户端接收到发射bubble的指令就进行检测添加
    public void setInBubble(int row, int col)
    {
        boxUpdate.AddBubbleRelative(row, col);
    }

    public void updateMovement()
    {
        if(this.state == PlayerState.Movement)
        {
            Vector2 center = colliderBox.Center;
            int col = (int)center.x / GameConst.tileWidth;
            int row = (int)-center.y / GameConst.tileHeight;
            Vector2 GridCenter = new Vector2(col * GameConst.tileWidth + GameConst.tileWidth / 2,
                                        -(row * GameConst.tileHeight + GameConst.tileHeight / 2));
            Vector2 offset = center - GridCenter;
            Vector2[] direct = new Vector2[2];
            if (isNormalMovement(offset, ref direct, row, col))
            {
                NormalMovement();
            }
            else
            {
                if (!OutOfEdge(direct))
                {
                    int col1 = (int)direct[0].x + col;
                    int row1 = (int)direct[0].y + row;
                    int col2 = (int)direct[1].x + col;
                    int row2 = (int)direct[1].y + row;
                    AABBox box1 = getGridBox(row1, col1);
                    AABBox box2 = getGridBox(row2, col2);
                    if (box1 != null && box2 == null)
                    {
                        SpecialMovement(direct[1], true);
                    }
                    else if (box1 == null && box2 != null)
                    {
                        SpecialMovement(-direct[1], false);
                    }
                    else
                    {
                        NormalMovement();
                    }
                }
                else
                {
                    //Debug.Log("Player Out Out Out Out Edge");
                    NormalMovement();
                }
            }
        }
        resetPlayerZ();
    }
    
    //如果当前的bubble跟player有关系就不关联进去
    public AABBox getGridBox(int row, int col)
    {
        AABBox box = GameConst.sceneObstacles[row, col];
        if (box == null)
        {
            if(!boxUpdate.ContainsInlineBubbleRelative(row, col) && !boxUpdate.ContainsOutlineBubbleRelative(row, col))
            {
                if(GameConst.bubbles[row, col] != null)
                {
                    box = GameConst.bubbles[row, col].GetComponent<BubbleOutlineBox>();
                }
            }
        }
        return box;
    }

    public bool isNormalMovementLeft(Vector2 offset, ref Vector2[] direct, int row, int col)
    {
        if (offset.x <= 0)
        {
            int col_ = col - 1;
            if (col_ < 0) return true;
            if (getGridBox(row, col_) != null)
            {
                if (offset.y >= distance)
                {
                    direct[0] = new Vector2(-1, 0);
                    direct[1] = new Vector2(-1, -1);
                    return false;
                }
                else if (offset.y <= -distance)
                {
                    direct[0] = new Vector2(-1, 0);
                    direct[1] = new Vector2(-1, 1);
                    return false;
                }
            }
            else
            {
                if (offset.y > 0)
                {
                    direct[0] = new Vector2(-1, 0);
                    direct[1] = new Vector2(-1, -1);
                    return false;
                }
                else if (offset.y < 0)
                {
                    direct[0] = new Vector2(-1, 0);
                    direct[1] = new Vector2(-1, 1);
                    return false;
                }
            }

        }
        return true;
    }

    public bool isNormalMovementRight(Vector2 offset, ref Vector2[] direct, int row, int col)
    {
        if (offset.x >= 0)
        {
            int col_ = col + 1;
            if (col_ >= GameConst.width) return true;
            if (getGridBox(row, col_) != null)
            {
                if (offset.y >= distance)
                {
                    direct[0] = new Vector2(1, 0);
                    direct[1] = new Vector2(1, -1);
                    return false;
                }
                else if (offset.y <= -distance)
                {
                    direct[0] = new Vector2(1, 0);
                    direct[1] = new Vector2(1, 1);
                    return false;
                }
            }
            else
            {
                if (offset.y > 0)
                {
                    direct[0] = new Vector2(1, 0);
                    direct[1] = new Vector2(1, -1);
                    return false;
                }
                else if (offset.y < 0)
                {
                    direct[0] = new Vector2(1, 0);
                    direct[1] = new Vector2(1, 1);
                    return false;
                }
            }
        }
        return true;
    }

    public bool isNormalMovementDown(Vector2 offset, ref Vector2[] direct, int row, int col)
    {
        if (offset.y <= 0)
        {
            int row_ = row + 1;
            if (row_ >= GameConst.height) return true;
            if (getGridBox(row_, col) != null)
            {
                if (offset.x >= distance)
                {
                    direct[0] = new Vector2(0, 1);
                    direct[1] = new Vector2(1, 1);
                    return false;
                }
                else if (offset.x <= -distance)
                {
                    direct[0] = new Vector2(0, 1);
                    direct[1] = new Vector2(-1, 1);
                    return false;
                }
            }
            else
            {
                if (offset.x > 0)
                {
                    direct[0] = new Vector2(0, 1);
                    direct[1] = new Vector2(1, 1);
                    return false;
                }
                else if (offset.x < 0)
                {
                    direct[0] = new Vector2(0, 1);
                    direct[1] = new Vector2(-1, 1);
                    return false;
                }
            }
        }
        return true;
    }

    public bool isNormalMovementUp(Vector2 offset, ref Vector2[] direct, int row, int col)
    {
        if (offset.y >= 0)
        {
            int row_ = row - 1;
            if (row_ < 0) return true;
            if (getGridBox(row_, col) != null)
            {
                if (offset.x >= distance)
                {
                    direct[0] = new Vector2(0, -1);
                    direct[1] = new Vector2(1, -1);
                    return false;
                }
                else if (offset.x <= -distance)
                {
                    direct[0] = new Vector2(0, -1);
                    direct[1] = new Vector2(-1, -1);
                    return false;
                }
            }
            else
            {
                if (offset.x > 0)
                {
                    direct[0] = new Vector2(0, -1);
                    direct[1] = new Vector2(1, -1);
                    return false;
                }
                else if (offset.x < 0)
                {
                    direct[0] = new Vector2(0, -1);
                    direct[1] = new Vector2(-1, -1);
                    return false;

                }
            }
        }
        return true;
    }

    //direct.y -1表示的是上面的格子
    public bool isNormalMovement(Vector2 offset, ref Vector2[] direct, int row, int col)
    {
        switch (direction)
        {
            case PlayerDirection.Left:
                //确保当前格子的中心在当前player的格子中心的右边
                //如果当前格子在左边的话就不需要判断
                return isNormalMovementLeft(offset, ref direct, row, col);
            case PlayerDirection.Right:
                return isNormalMovementRight(offset, ref direct, row, col);
            case PlayerDirection.Down:
                return isNormalMovementDown(offset, ref direct, row, col);
            case PlayerDirection.Up:
                return isNormalMovementUp(offset, ref direct, row, col);
        }
        return true;
    }
    
    public bool OutOfEdge(Vector2[] direct)
    {
        Vector2 center = colliderBox.Center;
        int col = (int)center.x / GameConst.tileWidth;
        int row = (int)-center.y / GameConst.tileHeight;
        int col1 = (int)direct[0].x + col;
        int row1 = (int)direct[0].y + row;
        int col2 = (int)direct[1].x + col;
        int row2 = (int)direct[1].y + row;
        if (col1 >= GameConst.width || col1 < 0)
        {
            return true;
        }
        if (col2 >= GameConst.width || col2 < 0)
        {
            return true;
        }
        if (row1 >= GameConst.height || row1 < 0)
        {
            return true;
        }
        if (row2 >= GameConst.height || row2 < 0)
        {
            return true;
        }
        return false;
    }

    public void NormalMovement()
    {
        switch (direction) {
            case PlayerDirection.Left:
                transform.position = new Vector3(transform.position.x - speed * Time.deltaTime,
                                                transform.position.y, transform.position.z);
                popDirection = PlayerBoxUpdate.PopDirection.HORIZONTAL;
                break;
            case PlayerDirection.Right:
                transform.position = new Vector3(transform.position.x + speed * Time.deltaTime,
                                                transform.position.y, transform.position.z);
                popDirection = PlayerBoxUpdate.PopDirection.HORIZONTAL;
                break;
            case PlayerDirection.Up:
                transform.position = new Vector3(transform.position.x,
                                                transform.position.y + speed * Time.deltaTime,
                                                transform.position.z);
                popDirection = PlayerBoxUpdate.PopDirection.VERTICAL;
                break;
            case PlayerDirection.Down:
                transform.position = new Vector3(transform.position.x,
                                                transform.position.y - speed * Time.deltaTime,
                                                transform.position.z);
                popDirection = PlayerBoxUpdate.PopDirection.VERTICAL;
                break;
        }
    }

    public Vector2 getCenterPos()
    {
        Vector2 center = colliderBox.Center;
        int col = (int)center.x / GameConst.tileWidth;
        int row = (int)-center.y / GameConst.tileHeight;
        Vector2 GridCenter = new Vector2(col * GameConst.tileWidth + GameConst.tileWidth / 2,
                                    -(row * GameConst.tileHeight + GameConst.tileHeight / 2));
        return GridCenter;
    }

    //direct.y为负1表示往下方移动
    //isNormal 表示player正对方向有块 否则表示没有块
    public void SpecialMovement(Vector2 direct, bool isNormal)
    {
        switch (direction) {
            case PlayerDirection.Left:
            case PlayerDirection.Right:
                if (direct.y < 0)
                {
                    float newY = transform.position.y + speed * Time.deltaTime;
                    if (!isNormal)
                    {
                        Vector2 pos = getCenterPos();
                        float positionY = colliderBox.getTransformPosY(pos.y);
                        if (newY > positionY) newY = positionY;
                    }
                    transform.position = new Vector3(transform.position.x,
                                                     newY,
                                                     transform.position.z);
                }
                else
                {
                    float newY = transform.position.y - speed * Time.deltaTime;
                    if (!isNormal)
                    {
                        Vector2 pos = getCenterPos();
                        float positionY = colliderBox.getTransformPosY(pos.y);
                        if (newY < positionY) newY = positionY;
                    }
                    transform.position = new Vector3(transform.position.x,
                                                     newY,
                                                     transform.position.z);
                }
                popDirection = PlayerBoxUpdate.PopDirection.VERTICAL;
                break;
            case PlayerDirection.Up:
            case PlayerDirection.Down:
                if (direct.x < 0)
                {
                    float newX = transform.position.x - speed * Time.deltaTime;
                    if (!isNormal)
                    {
                        Vector2 pos = getCenterPos();
                        float positionX = colliderBox.getTransformPosX(pos.x);
                        if (newX < positionX) newX = positionX;
                    }
                    transform.position = new Vector3(newX,
                                                     transform.position.y,
                                                     transform.position.z);
                }
                else
                {
                    float newX = transform.position.x + speed * Time.deltaTime;
                    if (!isNormal)
                    {
                        Vector2 pos = getCenterPos();
                        float positionX = colliderBox.getTransformPosX(pos.x);
                        if (newX > positionX) newX = positionX;
                    }
                    transform.position = new Vector3(newX,
                                                     transform.position.y,
                                                     transform.position.z);
                }
                popDirection = PlayerBoxUpdate.PopDirection.HORIZONTAL;
                break;
        }

    }

    void Update () {
        boxUpdate.prePosition = transform.position;
        updateMovement();
        boxUpdate.Update(popDirection);
    }

    protected void resetPlayerZ()
    {
        Vector3 center = colliderBox.Center;
        int tileHeight = GameConst.tileHeight;
        int layerIndex = Mathf.Abs(((int)center.y / tileHeight));
        transform.position = new Vector3(transform.position.x, transform.position.y, GameConst.staticLayers[layerIndex].position.z);
    }

    public Vector2 getCurGridPos()
    {
        Vector2 center = colliderBox.Center;
        int row = (int)-center.y / GameConst.tileHeight;
        int col = (int)center.x / GameConst.tileWidth;
        return new Vector2(col, row);
    }
}
