using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Root : MonoBehaviour
{
    void Start()
    {
        Application.runInBackground = true;
        Invoke("StartConnectToTheServer", 3.75f);
    }

    void Update()
    {
        NetMgr.Update();
    }

    private void StartConnectToTheServer()
    {
        ConnectToTheServer.instance.ConnentToTheServer();
    }
}
