using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Signup : MonoBehaviour
{

    public InputField usernameInput, passwordInput, passwordInput2;

    public void OnSignupClick()
    {
        if (usernameInput.text == "" || passwordInput.text == "")
        {
            Debug.Log("用户名密码不能为空！");
            return;
        }
        if (passwordInput.text != passwordInput2.text)
        {
            Debug.Log("两次密码不相同，请重新输入");
            return;
        }
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Register");
        protocol.AddString(usernameInput.text);
        protocol.AddString(passwordInput.text);
        NetMgr.srvConn.Send(protocol, OnSignupBack);
    }
    public void OnSignupBack(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int ret = proto.GetInt(start, ref start);
        if (ret == 0)
        {
            Debug.Log("注册成功");
        }
        else
        {
            Debug.Log("注册失败");
        }
    }
}
