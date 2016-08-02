using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using proto.serverproto;
using UnityEngine.SceneManagement;

public class LoginEvent : MonoBehaviour {

    public Text resultText;
    public InputField usernameInputField;
    private bool isConnect;
	// Use this for initialization
	void Start () {
        if (PlayerPrefs.HasKey(PlayerPrefsStringGroup.userName))
        {
            usernameInputField.text = PlayerPrefs.GetString(PlayerPrefsStringGroup.userName);
        }
        isConnect = false;
        StartCoroutine(StartConnect());
    }

    IEnumerator StartConnect()
    {
        yield return new WaitForSeconds(1.0f);
        Debug.Log("连接初始化完成");
        SocketQueue.getInstance().setCallback(getConnectToServerResponse);
        isConnect = true;
    }

    void Update()
    {
        float time = Time.deltaTime;
        if(isConnect) SocketQueue.getInstance().tick();
    }

	public void ButtonClick(){
        if (usernameInputField.text.Equals(""))
        {
            resultText.text = "请输入用户名";
        }
        else
        {
            PlayerPrefs.SetString("UserName", usernameInputField.text);
            resultText.text = "";
            SendDataController.getInstance().ConnectToServerRequest(usernameInputField.text);
        }
    }

    public void getConnectToServerResponse(ProtocolNetResponse resp)
    {
        GameGlobalData.fd = resp.connectToServerResponse.fd;
        SceneManager.LoadScene("MainScene");
    }

}
