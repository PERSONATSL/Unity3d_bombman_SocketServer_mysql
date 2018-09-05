using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeRoom : MonoBehaviour
{
    public static HomeRoom instance;

    public Text welcomeText;

    private void Start()
    {
        instance = this;
    }
    public void StartShow()
    {
        welcomeText.text = "你好 " + GameMgr.instance.username + " , 欢迎你！";
    }

}
