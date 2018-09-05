using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ConnectLogUI : MonoBehaviour
{

    public static ConnectLogUI instance;


    public GameObject loadingCircle;
    public Text loadingLoagText;
    [HideInInspector]
    public string str;
    [HideInInspector]
    public bool isTexted;
    [HideInInspector]
    public bool isFailed;
    [HideInInspector]
    public bool isSucced;

    public Transform buttons;
    public Transform loginPanel, signupPanel,logoPanel;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        str = "正在连接服务器......";
    }

    void Update()
    {
        loadingCircle.transform.Rotate(new Vector3(0, 0, -2));
        loadingLoagText.text = str;

        if (isTexted == true)
            loadingCircle.SetActive(false);
        else
            loadingCircle.SetActive(true);

        if (isFailed == true)
            floatButton();
        if (isSucced == true)
            floatButtons();

    }

    public void TryAgain()
    {
        buttons.DOLocalMoveY(-400, 0.5f);
        isTexted = false;
        isFailed = false;
        str = "正在连接服务器......";
        Invoke("TryAgainNow", 4f);
       
    }
    public void TryAgainNow()
    {
        NetMgr.srvConn.ConnentToTheServer();
    }

    public void floatButton()
    {
        isFailed = false;
        buttons.DOLocalMoveY(-233, 0.5f);
    }
    public void floatButtons()
    {
        isSucced = false;
        loginPanel.DOLocalMoveY(0, 0.5f);
        logoPanel.DOLocalMoveY(450, 0.5f);
    }
    public void floatButtonss()
    {
        loginPanel.DOLocalMoveY(-450, 0.5f);
        signupPanel.DOLocalMoveY(0, 0.5f);
    }
    public void floatSignup()
    {
        loginPanel.DOLocalMoveY(0, 0.5f);
        signupPanel.DOLocalMoveY(-450, 0.5f);
    }
    public void ExitTheGame()
    {
        Application.Quit();
    }
}
