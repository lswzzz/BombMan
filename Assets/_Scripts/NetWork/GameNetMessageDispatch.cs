using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using proto.serverproto;

public class GameNetMessageDispatch{

    public GameLogicNetSwap logicNetSwap;
    private PlayerInputController playerInputController;

    public GameNetMessageDispatch()
    {
        playerInputController = GameGlobalData.player;
    }

    public void update()
    {
    }

    public void DispatchMessage(ReceiveData data)
    {
        switch ((ReceiveDataType)data.cmd) {
            case ReceiveDataType.Action:
                DispatchAction(data.action);
                break;
            case ReceiveDataType.Transform:
                DispatchRobotTransform(data.transform);
                break;
            case ReceiveDataType.State:
                DispatchState(data.state);
                break;
        }
    }

    //从客户端发送给服务器的回应消息
    public void DispatchMessage(SendData data)
    {
        switch ((SendDataType)data.cmd) {
            case SendDataType.Action:
                SendActionData action = data.action;
                DispatchPlayerAction(data.action);
                break;
            case SendDataType.State:
                SendStateData state = data.state;
                DispatchPlayerState(data.state);
                break;
            case SendDataType.CheckTime:
                break;
        }
    }

    public void DispatchPlayerAction(SendActionData data)
    {
        if((SendActionData.ActionType)data.action == SendActionData.ActionType.Bubble)
        {
            playerInputController.PlayerAttack(data.bubble.row, data.bubble.col, data.bubble.power);
        }else if ((SendActionData.ActionType)data.action == SendActionData.ActionType.SaveOne)
        {
            playerInputController.GetComponent<PlayerExplore>().removeFd(data.saveOne.fd);
        }else if ((SendActionData.ActionType)data.action == SendActionData.ActionType.KillOne)
        {
            playerInputController.GetComponent<PlayerExplore>().removeFd(data.killOne.fd);
        }
    }

    public void DispatchPlayerState(SendStateData data)
    {
        //当玩家进入trap或者save或者death的状态的时候是自动所以在玩家发送以前就已经进入这个状态了所以这里就不需要再处理了
    }

    public void DispatchAction(ReceiveActionData data)
    {
        if(data.fd == GameGlobalData.fd)
        {
            DispatchPlayerAction(data);
        }
        else
        {
            DispatchRobotAction(data);
        }
    }

    public void DispatchState(ReceiveStateData data)
    {
        if (data.fd == GameGlobalData.fd)
        {
            DispatchPlayerState(data);
        }
        else
        {
            DispatchRobotState(data);
        }
    }

    public void DispatchPlayerAction(ReceiveActionData data)
    {
        if ((ReceiveActionData.ActionType)data.action == ReceiveActionData.ActionType.BeSave)
        {
            playerInputController.BeSave();
        }
        else if ((ReceiveActionData.ActionType)data.action == ReceiveActionData.ActionType.BeKill)
        {
            playerInputController.BeDeath();
        }
    }

    public void DispatchPlayerState(ReceiveStateData data)
    {

    }

    public void DispatchRobotTransform(ReceiveTransformData data)
    {
        RobotInputController robot = GameGlobalData.robotList[data.fd];
        robot.ReceiveTransform(data);
    }

    public void DispatchRobotAction(ReceiveActionData data)
    {
        RobotInputController robot = GameGlobalData.robotList[data.fd];
        robot.ReceiveAction(data);
    }

    public void DispatchRobotState(ReceiveStateData data)
    {
        RobotInputController robot = GameGlobalData.robotList[data.fd];
        robot.ReceiveState(data);
    }
}
