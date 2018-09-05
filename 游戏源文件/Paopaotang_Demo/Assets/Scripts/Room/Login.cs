using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using MySql.Data;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{

    public InputField usernameInput, passwordInput;

    public GameObject lv1, lv2;

    public void OnLoginClick()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Login");
        protocol.AddString(usernameInput.text);
        protocol.AddString(passwordInput.text);
        Debug.Log("发送" + protocol.GetDesc());
        NetMgr.srvConn.Send(protocol, OnLoginBack);
    }
    public void OnLoginBack(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int ret = proto.GetInt(start, ref start);
        if (ret == 0)
        {
            lv1.SetActive(false);
            lv2.SetActive(true);
            Debug.Log("登录成功");
            GameMgr.instance.username = usernameInput.text;
            RoomListPanel.instance.StartOnshowing();
            RoomPanel.instance.StartOnShowing();
            HomeRoom.instance.StartShow();
            LoadMsg.instance.OnShowing();
            LoadMsg.instance.OnShowingItem();
        }
        else
        {
            Debug.Log("登录失败");
        }
    }
    public void InGameReflash()
    {
        RoomListPanel.instance.StartOnshowing();
        RoomPanel.instance.StartOnShowing();
    }

    public void LoadLevel(int i)
    {
        switch (i)
        {
            case (1):
                SceneManager.LoadScene(1);
                break;
            case (2):
                SceneManager.LoadScene(2);
                break;
            case (3):
                SceneManager.LoadScene(3);
                break;
            case (4):
                SceneManager.LoadScene(4);
                break;
        }
    }

}
