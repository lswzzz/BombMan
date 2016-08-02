using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.SceneManagement;
using proto.clientproto;

//http://www.ceeger.com/Manual/LogFiles.html
public class ExceptionControl : MonoBehaviour {

    public Text text;
    public Button reloadButton;

    public List<MessageLine> messageList;
    public static ExceptionControl Instance = null;

    public enum LogMessageType {
        Log,
        Warning,
        Error,
    }

    public struct MessageLine {
        public string message;
        public LogMessageType type;
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        messageList = new List<MessageLine>();
        Instance = this;
        Application.logMessageReceived += HandleLog;
        setShowReload(false);
    }

    void Update()
    {
        //bool attack = Input.GetButtonDown("Jump");
        //if (!attack) attack = Input.GetButtonDown("Fire1");
        //if (attack)
        //{
        //    string message = "发射子弹" + Time.unscaledTime;
        //    AddMessage(message, LogMessageType.Log);
        //    SetText();
        //}
    }

    public void ExitGame()
    {
        GameConst.OnApplicationQuit();
    }

    public void setShowReload(bool isShow)
    {
        reloadButton.gameObject.SetActive(isShow);
    }

    public void ReloadGameScene()
    {
        ProtocolNetRequest req = new ProtocolNetRequest();
        req.cmd = (int)NetRequestType.PLAYERRELOADSCENE;
        PlayerReLoadScene reload = new PlayerReLoadScene();
        reload.fd = GameGlobalData.fd;
        req.playerReloadScene = reload;
        ConnectSocket.getSocketInstance().sendMSG(PBCSerialize.Serialize(req));
    }

    public void AddMessage(string message, LogMessageType type)
    {
        MessageLine line = new MessageLine();
        line.message = message;
        line.type = type;
        messageList.Add(line);
        SetText();
    }

    public void SetText()
    {
        while (messageList.Count > 10)
        {
            messageList.RemoveAt(0);
        }
        text.text = "";
        for (int i = messageList.Count - 1; i >= 0; i--)
        {
            MessageLine line = messageList[i];
            if (line.type == LogMessageType.Log)
            {
                text.text += "<color=\"black\">";
                text.text += line.message;
                text.text += "</color>\n";
            } else if (line.type == LogMessageType.Warning)
            {
                text.text += "<color=\"blue\">";
                text.text += line.message;
                text.text += "</color>\n";
            }
            else
            {
                text.text += "<color=\"red\">";
                text.text += line.message;
                text.text += "</color>\n";
            }
        }
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if(type == LogType.Log)
        {
            AddMessage(logString, LogMessageType.Log);
        }
        else if(type == LogType.Assert)
        {
            AddMessage(logString, LogMessageType.Log);
        }
        else if(type == LogType.Warning)
        {

        }else if(type == LogType.Exception)
        {
            AddMessage(logString + "\n" + stackTrace, LogMessageType.Error);
        }
        else if(type == LogType.Error)
        {
            AddMessage(logString + "\n" + stackTrace, LogMessageType.Error);
        }
    }
}