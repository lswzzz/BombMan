using UnityEngine;
using System.Collections;
using proto.clientproto;
using proto.serverproto;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class GameLogicNetSwap : MonoBehaviour {

    private ConnectSocket socket;
    private float perDeltaTime;
    private float curDeltaTime;
    private float reallyTime;
    public bool StartNetSend;
    private GameNetMessageDispatch dispatch;
    private UDPChannel udpSocket;
    private bool UdpTick = true;
    void Start()
    {
        perDeltaTime = 0.05f;
        curDeltaTime = 0f;
        StartNetSend = false;
        UdpTick = true;
        socket = ConnectSocket.getSocketInstance();
        udpSocket = UDPChannel.Instance;
        if(!udpSocket.isInit)udpSocket.init();
        SocketQueue.getInstance().setCallback(ReceiveResponseData);
        dispatch = new GameNetMessageDispatch();
        dispatch.logicNetSwap = this;
        udpSocket.setCallback(dispatch.DispatchMessage);
        udpSocket.setSendCallback(dispatch.DispatchMessage);
        GameGlobalData.player.GetComponent<PlayerBehaviourCollector>().logicSwap = this;
        StartCoroutine(SendStartInfo());
    }

    public void ReceiveResponseData(ProtocolNetResponse resp)
    {
        switch (resp.cmd)
        {
            case (int)NetRequestType.PLAYERGAMEDATA:
                //Debug.Log("GameLogicResponse size:" + resp.playerGameDataResponse.data.Length);
                //GameLogicResponse response = PBCSerialize.Deserialize<GameLogicResponse>(resp.playerGameDataResponse.data);
                //switch ((GameSceneDataType)response.type) {
                //    case GameSceneDataType.OtherOneTransform:
                //    case GameSceneDataType.OtherOneAction:
                //    case GameSceneDataType.OtherOneState:
                //        ForwardList.Add(response);
                //        break;
                //    case GameSceneDataType.Response:
                //         GenerateAverDelayTime();
                //         isReceiveData = true;
                //        break;
                //}
                break;
            case (int)NetRequestType.GAMERESULT:
                break;
            case (int)NetRequestType.PLAYERRELOADSCENE:
                StartNetSend = false;
                UdpTick = false;
                SceneManager.LoadScene("ReloadScene");
                break;
        }
    }

    IEnumerator SendStartInfo()
    {
        yield return null;
        SendData data = new SendData();
        data.cmd = SendDataType.StartInfo;
        data.fd = GameGlobalData.fd;
        udpSocket.Send(data);
    }

    public void GenerateAverDelayTime()
    {
        long time = TimeUtils.GetTimeStampNow();
        float delta = ((float)(time - reallyTime))/1000.0f;
        TimeUtils.GenerateTcpDelayTime(delta);
    }

    public void SendPlayerData()
    {
        SendData data = GameGlobalData.player.GetComponent<PlayerBehaviourCollector>().getTransformSendData();
        udpSocket.Send(data);
    }

    public void ForceSend(SendData data)
    {
        udpSocket.Send(data);
    }

    void Update()
    {
        if (StartNetSend)
        {
            curDeltaTime += Time.deltaTime;
            if (curDeltaTime >= perDeltaTime)
            {
                SendPlayerData();
                curDeltaTime = 0f;
            }
        }
        SocketQueue.getInstance().tick();
        dispatch.update();
        if(UdpTick)udpSocket.tick();
    }
}
