using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadMsg : MonoBehaviour {

    public static LoadMsg instance;
    public Text notice1T, notice2T, notice3T;
    public Text t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16, t17, t18, t19, t20, t21, t22, t23, t24, t25, t26, t27, t28, t29, t30, t31, t32;
    private void Start()
    {
        instance = this;
    }
    public void OnShowing()
    {
        NetMgr.srvConn.msgDist.AddListener("GetLoadMsg", GetLoadMsgBack);

        //发送查询
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("GetLoadMsg");
        NetMgr.srvConn.Send(protocol);
    }
    public void GetLoadMsgBack(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);

        notice1T.text = proto.GetString(start, ref start);

        notice2T.text = proto.GetString(start, ref start);

        notice3T.text = proto.GetString(start, ref start);
    }
    public void OnShowingItem()
    {
        NetMgr.srvConn.msgDist.AddListener("GetLoadItemMsg", GetLoadItemMsgBack);

        //发送查询
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("GetLoadItemMsg");
        NetMgr.srvConn.Send(protocol);
    }
    public void GetLoadItemMsgBack(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);

        t1.text = proto.GetString(start, ref start);
        t2.text = proto.GetString(start, ref start);
        t3.text = proto.GetString(start, ref start);
        t4.text = proto.GetString(start, ref start);
        t5.text = proto.GetString(start, ref start);
        t6.text = proto.GetString(start, ref start);
        t7.text = proto.GetString(start, ref start);
        t8.text = proto.GetString(start, ref start);
        //t9.text = proto.GetString(start, ref start);
        //t10.text = proto.GetString(start, ref start);
        //t11.text = proto.GetString(start, ref start);
        //t12.text = proto.GetString(start, ref start);
        //t13.text = proto.GetString(start, ref start);
        //t14.text = proto.GetString(start, ref start);
        //t15.text = proto.GetString(start, ref start);
        //t16.text = proto.GetString(start, ref start);
        //t17.text = proto.GetString(start, ref start);
        //t18.text = proto.GetString(start, ref start);
        //t19.text = proto.GetString(start, ref start);
        //t20.text = proto.GetString(start, ref start);
        //t21.text = proto.GetString(start, ref start);
        //t22.text = proto.GetString(start, ref start);
        //t23.text = proto.GetString(start, ref start);
        //t24.text = proto.GetString(start, ref start);
        //t25.text = proto.GetString(start, ref start);
        //t26.text = proto.GetString(start, ref start);
        //t27.text = proto.GetString(start, ref start);
        //t28.text = proto.GetString(start, ref start);
        //t29.text = proto.GetString(start, ref start);
        //t30.text = proto.GetString(start, ref start);
        //t31.text = proto.GetString(start, ref start);
        //t32.text = proto.GetString(start, ref start);
    }


}
