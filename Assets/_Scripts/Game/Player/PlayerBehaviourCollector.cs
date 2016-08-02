using UnityEngine;
using System.Collections;
using proto.clientproto;
using proto.serverproto;

public class PlayerBehaviourCollector : MonoBehaviour {

    private PlayerAnimationController animatorController;
    public GameLogicNetSwap logicSwap;
    private PlayerInputController input;
    private PlayerColliderBox colliderBox;
    void Start () {
        animatorController = GetComponent<PlayerAnimationController>();
        input = GetComponent<PlayerInputController>();
        colliderBox = GetComponent<PlayerColliderBox>();
    }

    //******************Transform
    public SendData getTransformSendData()
    {
        SendData data = new SendData();
        data.cmd = SendDataType.Transform;
        data.fd = GameGlobalData.fd;
        SendTransformData transformData = new SendTransformData();
        transformData.posx = transform.position.x;
        transformData.posy = transform.position.y;
        transformData.direction = (char)animatorController.Direction;
        transformData.state = (char)animatorController.State;
        data.transform = transformData;
        return data;
    }



    //*********************************Action
    public void SendBubbleAction()
    {
        Vector3 center = colliderBox.Center;
        SendData data = new SendData();
        data.fd = GameGlobalData.fd;
        data.cmd = SendDataType.Action;
        SendActionData actionData = new SendActionData();
        int row = (int)-center.y / GameConst.tileHeight;
        int col = (int)center.x / GameConst.tileWidth;
        int power = input.power;
        actionData.action = (char)SendActionData.ActionType.Bubble;
        SendActionData.SendActionBubble bubble = new SendActionData.SendActionBubble();
        bubble.row = (char)row;
        bubble.col = (char)col;
        bubble.power = (char)power;
        actionData.bubble = bubble;
        data.action = actionData;
        logicSwap.ForceSend(data);
    }

    public void SendSaveOneAction(int fd)
    {
        SendData data = new SendData();
        data.fd = GameGlobalData.fd;
        data.cmd = SendDataType.Action;
        SendActionData actionData = new SendActionData();
        actionData.action = (char)SendActionData.ActionType.SaveOne;
        actionData.saveOne = new SendActionData.SaveOne();
        actionData.saveOne.fd = fd;
        data.action = actionData;
        logicSwap.ForceSend(data);
    }

    public void SendKillOneAction(int fd)
    {
        SendData data = new SendData();
        data.fd = GameGlobalData.fd;
        data.cmd = SendDataType.Action;
        SendActionData actionData = new SendActionData();
        actionData.action = (char)SendActionData.ActionType.KillOne;
        actionData.killOne = new SendActionData.KillOne();
        actionData.killOne.fd = fd;
        data.action = actionData;
        logicSwap.ForceSend(data);
    }



//*************State
    public void SendTrapState(TrapLevel level)
    {
        SendData data = new SendData();
        data.fd = GameGlobalData.fd;
        data.cmd = SendDataType.State;
        SendStateData stateData = new SendStateData();
        stateData.state = (char)PlayerState.Trap;
        stateData.trapData = new SendStateData.SendTrapData();
        stateData.trapData.trapLevel = (char)level;
        data.state = stateData;
        logicSwap.ForceSend(data);
    }

    public void SendSaveState()
    {
        SendData data = new SendData();
        data.fd = GameGlobalData.fd;
        data.cmd = SendDataType.State;
        SendStateData stateData = new SendStateData();
        stateData.state = (char)PlayerState.Save;
        data.state = stateData;
        logicSwap.ForceSend(data);
    }

    public void SendDeathState()
    {
        SendData data = new SendData();
        data.fd = GameGlobalData.fd;
        data.cmd = SendDataType.State;
        SendStateData stateData = new SendStateData();
        stateData.state = (char)PlayerState.Death;
        data.state = stateData;
        logicSwap.ForceSend(data);
    }
}
