using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerExplore : MonoBehaviour {

    private PlayerBeHurtBox behurtBox;
    private PlayerInputController inputController;
    public List<int> waitList;
    void Start() {
        behurtBox = GetComponent<PlayerBeHurtBox>();
        inputController = GetComponent<PlayerInputController>();
        waitList = new List<int>();
    }

    public void removeFd(int fd)
    {
        foreach(int fd_ in waitList)
        {
            if(fd_ == fd)
            {
                waitList.Remove(fd);
                return;
            }
        }
    }

    public bool containsFd(int fd)
    {
        foreach(int fd_ in waitList)
        {
            if (fd_ == fd) return true;
        }
        return false;
    }

    //功能检测是否被waterColumn撞到，检测物体的拾取 检测是否碰撞到其他人
    //拯救某人杀死某人都是由玩家控制的角色决定的
    //所以在另一端的玩家会收到被杀死或被拯救的指令
    void Update() {
        Vector2 pos = getCurrentGridPos();
        int row = (int)pos.y;
        int col = (int)pos.x;
        Vector2[] neibor = GenerateListGrid();
        ResetGrid(row, col, ref neibor);
        for (int i = 0; i < 9; i++)
        {
            int row_ = row;
            int col_ = col;
            if (i != 4)
            {
                if (neibor[i] == Vector2.zero) continue;
                row_ += (int)neibor[i].y;
                col_ += (int)neibor[i].x;
            }
            if (inputController.CanBeAttack && JudgeWaterColumn(row_, col_))
            {
                inputController.BeAttack();
            }
            else if(inputController.CanExploreItem && ExploreItems(row_, col_))
            {
                inputController.ExploreItems(row_, col_);
            }
        }
        checkCollisionOtherOne();
    }

    public void checkCollisionOtherOne()
    {
        PlayerExploreBox exploreBox = GetComponent<PlayerExploreBox>();
        foreach (RobotInputController robot in GameGlobalData.robotList.Values)
        {
            PlayerExploreBox robotBox = robot.GetComponent<PlayerExploreBox>();
            if (exploreBox.BoxInline(robotBox))
            {
                if (CanSave(robot))
                {
                    Debug.Log("YES");
                    waitList.Add(robot.fd);
                    inputController.SaveOne(robot);
                }else if (CanKill(robot))
                {
                    Debug.Log("YES");
                    waitList.Add(robot.fd);
                    inputController.KillOne(robot);
                }
            }
        }
    }

    public bool CanSave(RobotInputController robot)
    {
        PlayerAnimationController animator = robot.GetComponent<PlayerAnimationController>();
        if(animator.State == PlayerState.Trap)
        {
            if(animator.ITrapLevel != TrapLevel.Trap3)
            {
                if (containsFd(robot.fd)) return false;
                return true;
            }
        }
        return false;
    }

    public bool CanKill(RobotInputController robot)
    {
        PlayerAnimationController animator = robot.GetComponent<PlayerAnimationController>();
        if (animator.State == PlayerState.Trap)
        {
            if (animator.ITrapLevel == TrapLevel.Trap3)
            {
                if (containsFd(robot.fd)) return false;
                return true;
            }
        }
        return false;
    }

    public bool JudgeWaterColumn(int row, int col)
    {
        WaterColumn waterColumn = GameConst.waterColumns[row, col];
        if(waterColumn != null)
        {
            WaterColumnBox box = waterColumn.GetComponent<WaterColumnBox>();
            if (box.BoxInline(behurtBox))
            {
                return true;
            }
        }
        return false;
    }

    public bool ExploreItems(int row, int col)
    {
        Items item = GameConst.items[row, col];
        if(item != null)
        {
            PlayerExploreBox exploreBox = GetComponent<PlayerExploreBox>();
            if(exploreBox.BoxInline(item.GetComponent<ItemBox>())){
                return true;
            }
        }
        return false;
    }

    //PlayerExplore和PlayerBoxUpdate不同的地方是要探索自身当前所在格子的九个格子
    public Vector2[] GenerateListGrid()
    {
        Vector2[] neiBox = new Vector2[9];
        neiBox[0] = (new Vector2(-1, -1));
        neiBox[1] = (new Vector2(0, -1));
        neiBox[2] = (new Vector2(1, -1));
        neiBox[3] = (new Vector2(-1, 0));
        neiBox[4] = (new Vector2(0, 0));
        neiBox[5] = (new Vector2(1, 0));
        neiBox[6] = (new Vector2(-1, 1));
        neiBox[7] = (new Vector2(0, 1));
        neiBox[8] = (new Vector2(1, 1));
        return neiBox;
    }

    public void ResetGrid(int row, int col, ref Vector2[] neiBox)
    {
        if (row == 0)
        {
            neiBox[0] = Vector2.zero;
            neiBox[1] = Vector2.zero;
            neiBox[2] = Vector2.zero;
        }
        else if (row == GameConst.height - 1)
        {
            neiBox[6] = Vector2.zero;
            neiBox[7] = Vector2.zero;
            neiBox[8] = Vector2.zero;
        }
        if (col == 0)
        {
            neiBox[0] = Vector2.zero;
            neiBox[3] = Vector2.zero;
            neiBox[6] = Vector2.zero;
        }
        else if (col == GameConst.width - 1)
        {
            neiBox[2] = Vector2.zero;
            neiBox[5] = Vector2.zero;
            neiBox[8] = Vector2.zero;
        }
    }

    public Vector2 getCurrentGridPos()
    {
        Vector2 center = behurtBox.Center;
        int row = (int)-center.y / GameConst.tileHeight;
        int col = (int)center.x / GameConst.tileWidth;
        return new Vector2(col, row);
    }
}
