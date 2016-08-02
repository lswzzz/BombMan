using UnityEngine;
using System.Collections;

public class RobotMovement : MonoBehaviour {

    private PlayerBoxUpdate boxUpdate;
    [System.NonSerialized]
    public float speed;
    [System.NonSerialized]
    public float maxSpeed;
    private PlayerColliderBox colliderBox;
    [System.NonSerialized]
    public PlayerState state;
    [System.NonSerialized]
    public PlayerDirection targetDirection;
    private float distance = 10;
    private Vector2 targetPosition;
    [System.NonSerialized]
    public float maxDistance = 50;
    private PlayerAnimationController animatorController;

    private PlayerBoxUpdate.PopDirection popDirection = PlayerBoxUpdate.PopDirection.HORIZONTAL;
    void Start () {
        resetPlayerZ();
        animatorController = GetComponent<PlayerAnimationController>();
        boxUpdate = new PlayerBoxUpdate();
        boxUpdate.Init(transform, GetComponent<PlayerColliderBox>(), GetComponent<PlayerBeHurtBox>(), GetComponent<PlayerExploreBox>());
        colliderBox = GetComponent<PlayerColliderBox>();
        //一开始就对玩家的位置进行修正
        target(transform.position, PlayerDirection.Down, PlayerState.Idle);
        boxUpdate.Update(popDirection);
        Debug.Log("New Position " + transform.position.x + "   " + transform.position.y);
    }

    void Update () {
        updateMovement();
        switch (targetDirection) {
            case PlayerDirection.Left:
            case PlayerDirection.Right:
                popDirection = PlayerBoxUpdate.PopDirection.HORIZONTAL;
                break;
            case PlayerDirection.Up:
            case PlayerDirection.Down:
                popDirection = PlayerBoxUpdate.PopDirection.VERTICAL;
                break;
        }
        boxUpdate.RobotUpdate(popDirection);
    }

    public void target(Vector2 position, PlayerDirection direction, PlayerState state)
    {
        targetPosition = position;
        targetDirection = direction;
        animatorController.AcceptInput(animatorController.Direction, state);
    }

    public void setInBubble(int row, int col)
    {
        //boxUpdate.AddBubbleRelative(row, col);
    }

    void updateMovement()
    {
        Vector2 offset = targetPosition - new Vector2(transform.position.x, transform.position.y);
        if(offset.magnitude >= maxDistance)
        {
            BlinkToPosition();
            return;
        }
        else if(offset.magnitude == 0f)
        {
            animatorController.AcceptInput(targetDirection, animatorController.State);
        }

        if(offset.x != 0.0f && offset.y != 0.0f)
        {
            TryMovement(offset);
        }
        else if(offset.x != 0.0f)
        {
            if (offset.x < 0.0f)
            {
                NormalMovement(PlayerDirection.Left, true);
            }
            else
            {
                NormalMovement(PlayerDirection.Right, true);
            }
        }
        else if(offset.y != 0.0f)
        {
            if(offset.y < 0.0f)
            {
                NormalMovement(PlayerDirection.Down, true);
            }
            else
            {
                NormalMovement(PlayerDirection.Up, true);
            }
        }
    }

    public bool Try2Direction(PlayerDirection direction1, PlayerDirection direction2)
    {
        if (IfCanDirection(direction1))
        {
            NormalMovement(direction1, false);
            return true;
        }else if (IfCanDirection(direction2))
        {
            NormalMovement(direction2, true);
            return true;
        }
        else
        {
            BlinkToPosition();
            return false;
        }
    }
    
    public bool IfCanDirection(PlayerDirection direction)
    {
        switch (direction) {
            case PlayerDirection.Left:
                return canLeft();
            case PlayerDirection.Right:
                return canRight();
            case PlayerDirection.Up:
                return canUp();
            case PlayerDirection.Down:
                return canDown();
        }
        return false;
    }

    //简单的移动如果延迟太太或网格情况太复杂直接瞬移到目标点
    public void TryMovement(Vector2 offset)
    {
        if(offset.x < 0.0f && offset.y < 0.0f)
        {
            //左下
            switch (targetDirection) {
                case PlayerDirection.Left:
                    Try2Direction(PlayerDirection.Down, PlayerDirection.Left);
                    break;
                case PlayerDirection.Down:
                    Try2Direction(PlayerDirection.Left, PlayerDirection.Down);
                    break;
                default:
                    Try2Direction(PlayerDirection.Down, PlayerDirection.Left);
                    break;
            }

        }else if(offset.x > 0.0f && offset.y < 0.0f)
        {
            //右下
            switch (targetDirection)
            {
                case PlayerDirection.Right:
                    Try2Direction(PlayerDirection.Down, PlayerDirection.Right);
                    break;
                case PlayerDirection.Down:
                    Try2Direction(PlayerDirection.Right, PlayerDirection.Down);
                    break;
                default:
                    Try2Direction(PlayerDirection.Down, PlayerDirection.Right);
                    break;
            }
        }
        else if(offset.x < 0.0f && offset.y > 0.0f)
        {
            //左上
            switch (targetDirection)
            {
                case PlayerDirection.Left:
                    Try2Direction(PlayerDirection.Up, PlayerDirection.Left);
                    break;
                case PlayerDirection.Up:
                    Try2Direction(PlayerDirection.Left, PlayerDirection.Up);
                    break;
                default:
                    Try2Direction(PlayerDirection.Up, PlayerDirection.Left);
                    break;
            }
        }
        else
        {
            //右上
            switch (targetDirection)
            {
                case PlayerDirection.Right:
                    Try2Direction(PlayerDirection.Up, PlayerDirection.Right);
                    break;
                case PlayerDirection.Up:
                    Try2Direction(PlayerDirection.Right, PlayerDirection.Up);
                    break;
                default:
                    Try2Direction(PlayerDirection.Up, PlayerDirection.Right);
                    break;
            }
        }
    }

    public AABBox getGridBox(int row, int col)
    {
        AABBox box = GameConst.sceneObstacles[row, col];
        if (box == null)
        {
            if (GameConst.bubbles[row, col] != null)
            {
                box = GameConst.bubbles[row, col].GetComponent<BubbleOutlineBox>();
            }
        }
        return box;
    }

    public bool canUp()
    {
        Vector2 center = colliderBox.Center;
        int row = GameConst.getRow(center.y);
        int col = GameConst.getCol(center.x);
        if((-center.y % GameConst.tileHeight) == GameConst.tileHeight / 2)
        {
            if (row == 0) return false;
            if(colliderBox.min.x < col * GameConst.tileWidth)
            {
                if (getGridBox(row - 1, col - 1) != null || getGridBox(row - 1, col) != null) return false;
            }else if(colliderBox.max.x > (col + 1) * GameConst.tileWidth)
            {
                if (getGridBox(row - 1, col) != null || getGridBox(row - 1, col + 1) != null) return false;
            }
            else
            {
                if (getGridBox(row - 1, col) != null) return false;
            }
        }
        return true;
    }

    public bool canDown()
    {
        Vector2 center = colliderBox.Center;
        int row = GameConst.getRow(center.y);
        int col = GameConst.getCol(center.x);
        if ((-center.y % GameConst.tileHeight) == GameConst.tileHeight / 2)
        {
            if (row == GameConst.height - 1) return false;
            if (colliderBox.min.x < col * GameConst.tileWidth)
            {
                if (getGridBox(row + 1, col - 1) != null || getGridBox(row + 1, col) != null) return false;
            }
            else if (colliderBox.max.x > (col + 1) * GameConst.tileWidth)
            {
                if (getGridBox(row + 1, col) != null || getGridBox(row + 1, col + 1) != null) return false;
            }
            else
            {
                if (getGridBox(row + 1, col) != null) return false;
            }
        }
        return true;
    }

    public bool canLeft()
    {
        Vector2 center = colliderBox.Center;
        int row = GameConst.getRow(center.y);
        int col = GameConst.getCol(center.x);
        if ((center.x % GameConst.tileWidth) == GameConst.tileWidth / 2)
        {
            if (col == 0) return false;
            if(-colliderBox.min.y > (row + 1)* GameConst.tileHeight)
            {
                if (getGridBox(row + 1, col - 1) != null || getGridBox(row, col - 1) != null) return false;
            }else if(-colliderBox.max.y < row * GameConst.tileHeight)
            {
                if (getGridBox(row - 1, col - 1) != null || getGridBox(row, col - 1) != null) return false;
            }
            else
            {
                if (getGridBox(row, col - 1) != null) return false;
            }
        }
        return true;
    }

    public bool canRight()
    {
        Vector2 center = colliderBox.Center;
        int row = GameConst.getRow(center.y);
        int col = GameConst.getCol(center.x);
        if ((center.x % GameConst.tileWidth) == GameConst.tileWidth / 2)
        {
            if (col == GameConst.width - 1) return false;
            if (-colliderBox.min.y > (row + 1) * GameConst.tileHeight)
            {
                if (getGridBox(row + 1, col + 1) != null || getGridBox(row, col + 1) != null) return false;
            }
            else if (-colliderBox.max.y < row * GameConst.tileHeight)
            {
                if (getGridBox(row - 1, col + 1) != null || getGridBox(row, col + 1) != null) return false;
            }
            else
            {
                if (getGridBox(row, col + 1) != null) return false;
            }
        }
        return true;
    }

    public void NormalMovement(PlayerDirection direction, bool changeDirection)
    {
        Vector2 position = transform.position;
        Vector2 newPosition = Vector2.zero;
        float newPosx = position.x;
        float newPosy = position.y;
        switch (direction) {
            case PlayerDirection.Left:
                newPosx = position.x - speed * Time.deltaTime;
                if(newPosx < targetPosition.x)
                {
                    newPosx = targetPosition.x;
                }
                if(changeDirection) animatorController.AcceptInput(PlayerDirection.Left, animatorController.State); 
                break;
            case PlayerDirection.Right:
                newPosx = position.x + speed * Time.deltaTime;
                if(newPosx > targetPosition.x)
                {
                    newPosx = targetPosition.x;
                }
                if (changeDirection) animatorController.AcceptInput(PlayerDirection.Right, animatorController.State);
                break;
            case PlayerDirection.Down:
                newPosy = position.y - speed * Time.deltaTime;
                if(newPosy < targetPosition.y)
                {
                    newPosy = targetPosition.y;
                }
                if (changeDirection) animatorController.AcceptInput(PlayerDirection.Down, animatorController.State);
                break;
            case PlayerDirection.Up:
                newPosy = position.y + speed * Time.deltaTime;
                if(newPosy > targetPosition.y)
                {
                    newPosy = targetPosition.y;
                }
                if (changeDirection) animatorController.AcceptInput(PlayerDirection.Up, animatorController.State);
                break;
        }
        newPosition = new Vector2(newPosx, newPosy);
        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
        resetPlayerZ();
    }

    public void BlinkToPosition()
    {
        Debug.Log("ERROR position");
        transform.position = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
        animatorController.AcceptInput(animatorController.Direction, animatorController.State);
        resetPlayerZ();
    }

    void resetPlayerZ()
    {
        int tileHeight = GameConst.tileHeight;
        int layerIndex = Mathf.Abs(((int)transform.position.y / tileHeight));
        transform.position = new Vector3(transform.position.x, transform.position.y, GameConst.staticLayers[layerIndex].position.z);
    }
}
